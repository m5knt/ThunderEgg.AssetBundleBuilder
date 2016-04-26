using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ThunderEgg.AssetBundleBuilder {

    public class Settings : ScriptableObject {

        public string BandleNameRule = @"/AB/(.+?)\.(.+?)/";

        public string Output = "AssetBundles";

        public BuildAssetBundleOptions[] BuildOptions = new[] {
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildAssetBundleOptions.None};

        public BuildAssetBundleOptions BuildOption {
            get {
                return (BuildAssetBundleOptions)BuildOptions.Cast<int>().Sum();
            }
        }

        public class Controler {

            static string nn = "Assets/ThunderEgg/AssetBundleBuilderSettings.asset";

            public static void Reset() {
                var t = AssetDatabase.FindAssets("AssetBundleBuilderSettings");
                var path = t.Length > 0 ? AssetDatabase.GUIDToAssetPath(t[0]) : nn;
                var o = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(o, path);
            }

            static Settings Instance_;

            public static Settings Instance {
                get {
                    if (Instance_ == null) {
                        //var t = AssetDatabase.FindAssets("AssetBundleBuilderSettings");
                        //var path = t.Length > 0 ? AssetDatabase.GUIDToAssetPath(t[0]) : nn;
                        var o = AssetDatabase.LoadAssetAtPath<Settings>(nn);
                        if (o == null) {
                            o = CreateInstance<Settings>();
                            AssetDatabase.CreateAsset(o, nn);
                        }
                        Instance_ = o;
                    }
                    return Instance_;
                }
            }
        }
    }
}
