using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEditor.Callbacks;
using System.Diagnostics;

namespace ThunderEgg.UnityUtilities.AssetBundleUtilities {

    [InitializeOnLoad]
    public class Editor {

        class Setting {
            public bool UseServer;
        }

        static HttpServer.Control LocalServerControl = new HttpServer.Control();
/////
        static Editor() {
            //var json = EditorPrefs.GetString(TestServerMenuString, "");
            //EditorPrefs.DeleteKey(TestServerMenuString);
            //var set = JsonUtility.FromJson<Setting>(json);
            //if (set == null) {
            //    set = new Setting();
            //    json = JsonUtility.ToJson(set);
            //    EditorPrefs.SetString(TestServerMenuString, json);
            //}

            //if (!ThunderEgg.UnityUtilities.UnityUtilities.IsBatchMode) {
            //    TestServerControl.Set(set.UseServer);
            //}
            //EditorApplication.playmodeStateChanged += OnStateChanged;
        }

        static void OnStateChanged() {
     //       if (EditorApplication.isCompiling) {
   //  //           TestServerControl.Set(false);
 //               //
//            }
            //
            //else {
            //    var b = EditorPrefs.GetBool(TestServerMenuString, false);
            //    TestServerControl.Set(b);
            //}
            // EditorApplication.isPaused;
            // EditorApplication.isPlaying;
        }

        const string LocalServerMenuString = "Assets/[ThunderEgg]/LocalServer";

        static bool server_ = false;
        
        [MenuItem(LocalServerMenuString, false, 100)]
        static void LocalServerMenu() {
            //var json = EditorPrefs.GetString(TestServerMenuString, "");
            //var set = JsonUtility.FromJson<Setting>(json);
            //set.UseServer = !set.UseServer;
            //json = JsonUtility.ToJson(set);
            //EditorPrefs.SetString(TestServerMenuString, json);
            server_ = !server_;
            Menu.SetChecked(LocalServerMenuString, server_);
            LocalServerControl.Set(server_);
        }

        [MenuItem(LocalServerMenuString, true)]
        static bool LocalServerMenu_() {
            //var json = EditorPrefs.GetString(TestServerMenuString, "");
            //UnityEngine.Debug.Log(json);
            //var set = JsonUtility.FromJson<Setting>(json) ?? new Setting();
            //Menu.SetChecked(TestServerMenuString, set.UseServer);
            return true;
        }

        [MenuItem("Assets/[ThunderEgg]/Builder/Build", false, 101)]
        static void Build() {
            Builder.Build();
        }

        [MenuItem("Assets/[ThunderEgg]/Builder/Clean", false, 102)]
        static void Clean() {
            Builder.Clean();
        }

    }
}

