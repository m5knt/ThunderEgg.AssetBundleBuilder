using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ThunderEgg.AssetBundleUtilities {

    public class AssetBundleBuilder {

        /// <summary>アセットバンドルを作成する</summary>
        [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Build", priority = 100)]
        static void Build() {

            var set = AssetBundleSettings.Instance;

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
        [MenuItem("Assets/[ThunderEgg]/AssetBundleBuilder/Clean", priority = 101)]
        static void Clean() {
            var set = AssetBundleSettings.Instance;
            if (Directory.Exists(set.Output)) {
                Directory.Delete(set.Output, true);
            }
        }

    }
}

