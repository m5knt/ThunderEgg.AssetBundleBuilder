using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace ThunderEgg.AssetBundleBuilder {

    public sealed class Settings {

        public string NameRule = @"/AB/([^\..]+?)\.([^\..]+?)/";

        public string Output = "AssetBundles";

        public BuildAssetBundleOptions[] BuildOptions = new[] {
            BuildAssetBundleOptions.ChunkBasedCompression,
        };

        //
        //
        //

        BuildAssetBundleOptions? BuildOption_;

        public BuildAssetBundleOptions BuildOption {
            get {
                return (BuildOption_ ??
                    (BuildOption_ = (BuildAssetBundleOptions)BuildOptions.Cast<int>().Sum())).Value;
            }
        }

        public static readonly Settings Instance = new Settings(); // Create();

        //static string SettingsAsset = "Assets/ThunderEgg/AssetBundleBuilderSettings.asset";

        //static Settings Create() {
        //    var t = AssetDatabase.FindAssets("AssetBundleBuilderSettings");
        //    var path = t.Length > 0 ? AssetDatabase.GUIDToAssetPath(t[0]) : SettingsAsset;
        //    var o = AssetDatabase.LoadAssetAtPath<Settings>(path);
        //    if (o == null) {
        //        o = CreateInstance<Settings>();
        //        AssetDatabase.CreateAsset(o, SettingsAsset);
        //    }
        //    return o;
        //}
    }
}
