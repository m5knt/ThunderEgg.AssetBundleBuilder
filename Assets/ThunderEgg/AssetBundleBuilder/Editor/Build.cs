using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NUnit.Framework;

namespace ThunderEgg.AssetBundleBuilder {

    public class Builder {

        [MenuItem("ThunderEgg/AssetBundleNamer/Clean")]
        static void Clean() {
            if (Directory.Exists(Tool.Output)) {
                Directory.Delete(Tool.Output, true);
            }
        }

        [MenuItem("ThunderEgg/AssetBundleNamer/Build")]
        static void Build() {

            var opt = BuildAssetBundleOptions.None |
                BuildAssetBundleOptions.DeterministicAssetBundle |
                BuildAssetBundleOptions.ChunkBasedCompression;

            var build = EditorUserBuildSettings.activeBuildTarget.ToString();
            var runtime = Application.platform.ToString();

            var roots = new[] {
                Tool.PlatformToRoot(EditorUserBuildSettings.activeBuildTarget),
                Tool.PlatformToRoot(Application.platform),
                }.Distinct();

            foreach (var root in roots) {
                var target = Tool.RootToBuildTarget(roots.First());
                var output = Tool.Output + "/" + root;
                if (!Directory.Exists(output)) {
                    Directory.CreateDirectory(output);
                }
                BuildPipeline.BuildAssetBundles(output, opt, target);
            }
        }

#if false
        // BUNDLE.Foo.Bar.Baz/a.png
        // BUNDLE.Foo.Bar/a.png
        // BUNDLE.Foo/VARIANT.Bar/a.png
        // VARIANT.Bar/BUNDLE.Foo/a.png

        static Regex SearchRule = new Regex(@"Assets/(.+?)/?(.+?)\.?(.+?)?/.+");

        static void OnPostprocessAllAssets(string[] imported, string[] deleted,
            string[] movedto, string[] movedfrom) //
        {
            new[] { imported, movedto }
                .SelectMany(_ => _)
                .Select(_ => SearchRule.Match(_))
                .Where(_ => _.Success)
                .ToList()
                .ForEach(_ => {
                    var dirs = _.Groups[1].Value;
                    var bundle = _.Groups[2].Value;
                    var variant = _.Groups[3].Success ? _.Groups[3].Value : string.Empty;
                    SetBundleName(_.Value, bundle, variant);
                });
        }

        static void SetBundleName(string path, string bundle, string variant) {
            var importer = AssetImporter.GetAtPath(path);
            var b = true;
            b = b && importer.assetBundleName == bundle;
            b = b && importer.assetBundleVariant == variant;
            if (b) return;
            importer.SetAssetBundleNameAndVariant(bundle, variant);
        }
#endif
    }
}

