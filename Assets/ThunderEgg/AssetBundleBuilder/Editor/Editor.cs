using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThunderEgg.Editor {

    using ABB = ThunderEgg.AssetBundleBuilder;

    public class AssetBundle {

        [MenuItem("Assets/[ThunderEgg]/Asset Bundle/Build", priority = 100)]
        static void Build() {

            var opt = BuildAssetBundleOptions.None |
                BuildAssetBundleOptions.DeterministicAssetBundle |
                BuildAssetBundleOptions.ChunkBasedCompression;

            var roots = new[] {
                ABB.GetRoot(EditorUserBuildSettings.activeBuildTarget),
                ABB.GetRoot(Application.platform),
                }.Distinct();

            foreach (var root in roots) {
                var target = ABB.RootToBuildTarget(roots.First());
                var output = ABB.Output + "/" + root;
                if (!Directory.Exists(output)) {
                    Directory.CreateDirectory(output);
                }
                BuildPipeline.BuildAssetBundles(output, opt, target);
            }
        }

        [MenuItem("Assets/[ThunderEgg]/Asset Bundle/Clean", priority = 101)]
        static void Clean() {
            if (Directory.Exists(ABB.Output)) {
                Directory.Delete(ABB.Output, true);
            }
        }
    }

    //
    //
    //

    public class AssetBundleName {

        [MenuItem("Assets/[ThunderEgg]/Asset Bundle Name/Auto Naming", priority = 100)]
        public static void AutoNaming() {
            var sels = Selection.assetGUIDs
                .Select(_ => AssetDatabase.GUIDToAssetPath(_));
            var alls = AssetDatabase.GetAllAssetPaths();
            var assets = sels.SelectMany(s => alls.Where(a => a.StartsWith(s)))
                .Where(_ => File.Exists(_));
            foreach (var asset in assets) {
                Set(asset);
            }
        }

        [MenuItem("Assets/[ThunderEgg]/Asset Bundle Name/Clear", priority = 101)]
        public static void Clear() {
            var sels = Selection.assetGUIDs
                .Select(_ => AssetDatabase.GUIDToAssetPath(_));
            var alls = AssetDatabase.GetAllAssetPaths();
            var assets = sels.SelectMany(s => alls.Where(a => a.StartsWith(s)))
                .Where(_ => File.Exists(_));
            foreach (var asset in assets) {
                Set(asset, "");
            }
        }

        [MenuItem("Assets/[ThunderEgg]/Asset Bundle Name/Delete Unused", priority = 102)]
        static void DeleteUnused() {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        //
        //
        //

        static Regex Name = new Regex(@"/(.+?)\.(.+?)/");

        public static void Set(string asset_path) {
            var m = Name.Match(asset_path);
            if (!m.Success) return;
            var bundle = m.Groups[1].Success ? m.Groups[1].Value : "";
            var variant = m.Groups[2].Success ? m.Groups[2].Value : "";
            Set(asset_path, bundle, variant);
        }

        public static void Set(string asset_path, string bundle, //
            string variant = "") //
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

