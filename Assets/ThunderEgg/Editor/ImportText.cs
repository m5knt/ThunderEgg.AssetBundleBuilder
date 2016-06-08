using UnityEditor;
using UnityEngine;

using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Re = System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

/*
 *
 */

public class ImportText : AssetPostprocessor {

    enum TextFileExtentions {
        cs,
        txt,
        xml,
        lua,
    }

	static Re.Regex SourcesPattern = new Re.Regex(@"\.(cs|txt|xml|lua)");
	// static byte[] Bom = new byte[]{0xef, 0xbb, 0xbf};

	static void OnPostprocessAllAssets(
		string[] imported,
		string[] deleted,
		string[] moved,
		string[] movedFromAssetPaths)
	{
		var decoder = Encoding.GetEncoding("UTF-8", 
			EncoderFallback.ReplacementFallback,
			new DecoderExceptionFallback());

		foreach (var file in imported) {
			// ファイル名確認
			if (!SourcesPattern.IsMatch(file)) {
				continue;
			}

			// エンコード確認
			var bytes = File.ReadAllBytes(file);

			// ASCIIチェック
			var b = true;
			for (var i = 0; b && i < bytes.Length; ++i) {
				if (bytes[i] >= 0x80) {
					b = false;
					break;
				}
			}

			// UTF8チェック
			try {
				decoder.GetString(bytes);
			}
			catch (DecoderFallbackException) {
				Debug.LogError("UTF8にデコード出来ません : " + file + ":1");
				continue;
			}

			// // BOMチェック
			// var head = bytes.Take(Bom.Length).ToArray();
			// if (!Bom.SequenceEqual(head)) {
			// 	Information.LogWarning("[xest] BOMが有りません : " + file + ":1");
			// }
		}
	}
}
