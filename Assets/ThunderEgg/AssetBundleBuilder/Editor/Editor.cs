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

        //static Regex BandleNameRule = new Regex(@"/AB/(.+?)\.(.+?)/");

        [PostProcessBuild(Int32.MaxValue)]
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, //
            string[] to, string[] from) //
        {
            var set = Settings.Controler.Instance;
            var rule = new Regex(set.BandleNameRule);
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
            //var obj = new GameObject();
            //obj.name = "";
            //obj.AddComponent<Hoge>();
            //AssetDatabase.CreateAsset(obj, "Assets/aa.asset");
            //AssetDatabase.SaveAssets();
            //PrefabUtility.CreatePrefab("Assets/Resources/a.prefab", obj);

            var set = Settings.Controler.Instance;

            //var opt = BuildAssetBundleOptions.None |
            //    BuildAssetBundleOptions.DeterministicAssetBundle |
            //    BuildAssetBundleOptions.ChunkBasedCompression;

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
            var set = Settings.Controler.Instance;
            if (Directory.Exists(set.Output)) {
                Directory.Delete(set.Output, true);
            }
        }

        //     [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Settings", priority = 200)]
        //       static void Settings() {
        //        }

        //
        // Naming
        //

        //[MenuItem("Assets/[ThunderEgg]/AssetBundleName/Auto Naming", priority = 100)]
        //public static void AutoNaming() {
        //    foreach (var asset in SelectedAssets()) {
        //        AutoNaming(asset);
        //    }
        //}

        //[MenuItem("Assets/[ThunderEgg]/AssetBundleName/Auto Naming", true)]
        //public static bool AutoNaming_() {
        //    return false;
        //}

        //[MenuItem("Assets/[ThunderEgg]/AssetBundleName/Clear", priority = 101)]
        //public static void NameClear() {
        //    foreach (var asset in SelectedAssets()) {
        //        SetBundleNameAndVariant(asset, "");
        //    }
        //}

        /// <summary>選択しているアセット</summary>
        static IEnumerable<string> SelectedAssets() {
            var sels = Selection.assetGUIDs
                .Select(_ => AssetDatabase.GUIDToAssetPath(_));
            var alls = AssetDatabase.GetAllAssetPaths();
            return sels.SelectMany(s => alls.Where(a => a.StartsWith(s)))
                .Where(_ => File.Exists(_));
        }

        //
        //
        //

        /// <summary>アセットバンドル名をパス名で決定します</summary>
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

