using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThunderEgg.UnityUtilities.AssetBundleUtilities {

    /// <summary>アセットバンドル名をパス名から自動命名します</summary>
    public class AutoNaming : AssetPostprocessor {

        //[PostProcessBuild(Int32.MaxValue)]
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, 
            string[] to, string[] from) //
        {
            var set = Settings.Instance;
            var rule = new Regex(set.NameRule);
            var assets = (new[] { imported, to }).SelectMany(_ => _);
            foreach (var asset in assets) {
                Set(rule, asset);
            }
        }
        
        /// <summary>アセットバンドル名をパス名から決定します</summary>
        public static void Set(Regex rule, string asset_path) {
            if (!File.Exists(asset_path)) return;
            var m = rule.Match(asset_path);
            if (m.Success) {
                var name = m.Groups["name"].Success ? m.Groups["name"].Value : "";
                var variant = m.Groups["variant"].Success ? m.Groups["variant"].Value : "";
                SetBundleNameAndVariant(asset_path, name, variant);
            }
            else {
                SetBundleNameAndVariant(asset_path, "");
            }
        }

        /// <summary>アセットバンドル名を設定します</summary>
        public static void SetBundleNameAndVariant(string asset_path, //
            string bundle, string variant = "") //
        {
            var importer = AssetImporter.GetAtPath(asset_path);
            var b = true;
            b = b && (importer.assetBundleName == bundle);
            b = b && (importer.assetBundleVariant == variant);
            if (b) return;
            importer.SetAssetBundleNameAndVariant(bundle, variant);
        }
    }
}

