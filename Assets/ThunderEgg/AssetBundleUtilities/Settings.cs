using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ThunderEgg.UnityUtilities.AssetBundleUtilities {

    /// <summary>AssetBundleUtilities向け設定の型</summary>
    public sealed class Settings : ScriptableObject {

        /// <summary>アセットバンドルの自動命名規則</summary>
        //public string NameRule = @"/AB([^\..]+)@([^\..]+)?/";
        public string NameRule = @"/(?'name'@[^\..]*?@)(?'variant'[^\..]+)?/";

        /// <summary>アセットバンドル出力位置</summary>
        public string Output = "AssetBundles";

        /// <summary>テストサーバのポート</summary>
        public int TestServerPort = 7888;

        /// <summary>アセットバンドル作成時のオプション</summary>
        [SerializeField]
        BuildAssetBundleOptions[] BuildOptions = new[] {
            BuildAssetBundleOptions.ChunkBasedCompression,
        };

        /// <summary>アセットバンドル作成時のオプション</summary>
        public BuildAssetBundleOptions BuildOption {
            get {
                return (BuildOption_ ?? (BuildOption_ =
                    (BuildAssetBundleOptions)BuildOptions.Cast<int>().Sum())).Value;
            }
        }

        BuildAssetBundleOptions? BuildOption_;

        //
        //
        //

        /// <summary>インスタンスを返す</summary>
        public static Settings Instance {
            get { 
                return Instance_ ?? (Instance_ =
                    UnityUtilities.CreateScriptableObject<Settings>(Path));
            }
        }

        static Settings Instance_;

        static string Path = "Assets/ThunderEgg/Settings/AssetBundleUtilities.asset";

    }
}
