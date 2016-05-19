using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ThunderEgg.AssetBundleUtilities {

    public class Builder {

        /// <summary>アセットバンドルを作成する</summary>
        [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Build", priority = 100)]
        static void Build() {

            var set = Settings.Instance;

            var target = EditorUserBuildSettings.activeBuildTarget;
            var roots = new[] {
                Utilities.GetRoot(target),
                Utilities.GetRoot(Application.platform),
                }.Distinct();

            foreach (var root in roots) {
                target = Utilities.RootToBuildTarget(roots.First());
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

    }
}

