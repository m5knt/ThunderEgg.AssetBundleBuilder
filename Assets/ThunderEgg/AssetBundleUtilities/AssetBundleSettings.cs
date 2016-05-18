using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ThunderEgg.AssetBundleUtilities  {

    public class AssetBundleSettings : ScriptableObject {

        /// <summary>アセットバンドルの自動命名規則</summary>
        public string NameRule = @"/AB/([^\..]+?)\.([^\..]+?)/";

        /// <summary>アセットバンドル出力位置</summary>
        public string Output = "AssetBundles";

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

        public static AssetBundleSettings Instance {
            get { 
                return Instance_ ?? (Instance_ =
                    AssetBundleUtilities.CreateScriptableObject<AssetBundleSettings>(Base));
            }
        }

        static AssetBundleSettings Instance_;

        static string Base = "Assets/ThunderEgg/AssetBundleUtilities/";

    }
}
