using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace ThunderEgg.AssetBundleUtilities {

    public class AssetBundleSettings : ScriptableObject {

        /// <summary>アセットバンドルの自動命名規則</summary>
        public string NameRule = @"/AB/([^\..]+?)\.([^\..]+?)/";

        /// <summary>アセットバンドル出力位置</summary>
        public string Output = "AssetBundles";

        /// <summary>アセットバンドル作成時のオプション</summary>
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

        public static AssetBundleSettings Instance {
            get {
                return Instance_ ?? (Instance_ = Create());
            }
        }

        static AssetBundleSettings Instance_;

        static string SettingsAsset = "Assets/ThunderEgg/AssetBundleUtilities/AssetBundleSettings.asset";

        static AssetBundleSettings Create() {
            // var t = AssetDatabase.FindAssets("Settings");
            // var path = (t.Length > 0) ? AssetDatabase.GUIDToAssetPath(t[0]) : SettingsAsset;
            var path = SettingsAsset;
            var o = AssetDatabase.LoadAssetAtPath<AssetBundleSettings>(path);
            if (o == null) {
                o = CreateInstance<AssetBundleSettings>();
                AssetDatabase.CreateAsset(o, SettingsAsset);
            }
            return o;
        }
    }
}
