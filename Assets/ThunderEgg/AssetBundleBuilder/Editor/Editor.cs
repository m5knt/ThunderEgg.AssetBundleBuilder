using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ThunderEgg.AssetBundleBuilder {

    public class Editor : AssetPostprocessor {

        //[PostProcessBuild(Int32.MaxValue)]
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, //
            string[] to, string[] from) //
        {
            var set = Settings.Instance;
            var rule = new Regex(set.NameRule);
            var assets = (new[] { imported, to }).SelectMany(_ => _);
            foreach (var asset in assets) {
                AutoNaming(rule, asset);
            }
        }


        //
        // Build
        //

        /// <summary>アセットバンドルを作成する</summary>
        [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Build", priority = 100)]
        static void Build() {

            var set = Settings.Instance;

            var target = EditorUserBuildSettings.activeBuildTarget;
            var roots = new[] {
                AssetBundleBuilder.GetRoot(target),
                AssetBundleBuilder.GetRoot(Application.platform),
                }.Distinct();

            foreach (var root in roots) {
                target = AssetBundleBuilder.RootToBuildTarget(roots.First());
                var output = set.Output + "/" + root;
                if (!Directory.Exists(output)) {
                    Directory.CreateDirectory(output);
                }
                BuildPipeline.BuildAssetBundles(output, set.BuildOption, target);
            }
        }

        /// <summary>アセットバンドルを削除する</summary>
        [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Clean", priority = 101)]
        static void Clean() {
            var set = Settings.Instance;
            if (Directory.Exists(set.Output)) {
                Directory.Delete(set.Output, true);
            }
        }

        //
        //
        //

        /// <summary>アセットバンドル名をパス名から決定します</summary>
        public static void AutoNaming(Regex rule, string asset_path) {
            if (!File.Exists(asset_path)) return;
            var m = rule.Match(asset_path);
            if (m.Success) {
                var bundle = m.Groups[1].Success ? m.Groups[1].Value : "";
                var variant = m.Groups[2].Success ? m.Groups[2].Value : "";
                SetBundleNameAndVariant(asset_path, bundle, variant);
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
            b = b && importer.assetBundleName == bundle;
            b = b && importer.assetBundleVariant == variant;
            if (b) return;
            importer.SetAssetBundleNameAndVariant(bundle, variant);
        }
    }
}

