using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ThunderEgg {

    using ABB = AssetBundleBuilder;

    public class AssetBundleBuilderEditor {

        [MenuItem("Assets/[ThunderEgg]/Build Asset Bundle", priority = 100)]
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

        [MenuItem("Assets/[ThunderEgg]/Clean Asset Bundle", priority = 101)]
        static void Clean() {
            if (Directory.Exists(ABB.Output)) {
                Directory.Delete(ABB.Output, true);
            }
        }
        public static void SetBundleName(string asset_path, string bundle, //
            string variant = "") //
        {
            var importer = AssetImporter.GetAtPath(asset_path);
            var b = true;
            b = b && importer.assetBundleName == bundle;
            b = b && importer.assetBundleVariant == variant;
            if (b) return;
            importer.SetAssetBundleNameAndVariant(bundle, variant);
        }

        //
        //
        //

        [MenuItem("Assets/[ThunderEgg]/Set Asset Bundle Name", priority = 200)]
        public static void SetBundleNameAndVariant() {
            var sels = Selection.assetGUIDs
                .Select(_ => AssetDatabase.GUIDToAssetPath(_));
            var alls = AssetDatabase.GetAllAssetPaths();
            var assets = sels.SelectMany(s => alls.Where(a => a.StartsWith(s)))
                .Where(_ => File.Exists(_));
            foreach (var asset in assets) {
                //SetBundleName(asset, "");
            }
        }

        [MenuItem("Assets/[ThunderEgg]/Clear Asset Bundle Name", priority = 201)]
        public static void ClearBundleNameAndVariant() {
            var sels = Selection.assetGUIDs
                .Select(_ => AssetDatabase.GUIDToAssetPath(_));
            var alls = AssetDatabase.GetAllAssetPaths();
            var assets = sels.SelectMany(s => alls.Where(a => a.StartsWith(s)))
                .Where(_ => File.Exists(_));
            foreach (var asset in assets) {
                SetBundleName(asset, "");
            }
        }

        [MenuItem("Assets/[ThunderEgg]/Delete Unused Asset Bundle Name", priority = 202)]
        static void DeleteUnusedBandleName() {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

    }
}

