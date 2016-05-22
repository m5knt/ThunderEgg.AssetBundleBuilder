using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ThunderEgg.AssetBundleUtilities  {

    public sealed class Settings : ScriptableObject {

        /// <summary>アセットバンドルの自動命名規則</summary>
        public string NameRule = @"/AB/([^\..]+?)\.([^\..]+?)/";

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

        public static Settings Instance {
            get { 
                return Instance_ ?? (Instance_ =
                    Utilities.CreateScriptableObject<Settings>(Base));
            }
        }

        static Settings Instance_;

        static string Base = "Assets/ThunderEgg/AssetBundleUtilities/";

    }
}
