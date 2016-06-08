using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ThunderEgg.UnityUtilities.AssetBundleUtilities {

    public class Builder {

        /// <summary>アセットバンドルを作成する</summary>
        [MenuItem("Assets/[ThunderEgg]/Builder/Build", priority = 100)]
        public static void Build() {

            var set = Settings.Instance;

            var target = EditorUserBuildSettings.activeBuildTarget;
            var roots = new[] {
                AssetBundleUtilities.GetRoot(target),
                AssetBundleUtilities.GetRoot(Application.platform),
                }.Distinct();

            foreach (var root in roots) {
                target = AssetBundleUtilities.RootToBuildTarget(roots.First());
                var output = set.Output + "/" + root;
                if (!Directory.Exists(output)) {
                    Directory.CreateDirectory(output);
                }
                BuildPipeline.BuildAssetBundles(output, set.BuildOption, target);
            }
        }

        /// <summary>アセットバンドルを削除する</summary>
        [MenuItem("Assets/[ThunderEgg]/Builder/Clean", priority = 101)]
        public static void Clean() {
            var set = Settings.Instance;
            if (Directory.Exists(set.Output)) {
                Directory.Delete(set.Output, true);
            }
        }

    }
}

