using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Linq;

namespace ThunderEgg.UnityUtilities {

    /// <summary>雑多なユニティーのユーティリティー</summary>
    [InitializeOnLoad]
    public static class UnityUtilities {

        static UnityUtilities() {
            // エディタ実行では初回起動時に2回来ている
            if (int.TryParse(Environment.Get("UNITYINITILIZECOUNT") ?? "-1", //
                out InitilizeCount_)) //
            {
                var val = (++InitilizeCount_).ToString();
                Environment.Set("UNITYINITILIZECOUNT", val);
            }
        }

        /// <summary>初期化カウント</summary>
        public static int InitilizeCount {
            get { return InitilizeCount_; }
            private set { InitilizeCount_ = value; }

        }
        static int InitilizeCount_ = 0;

        /// <summary>現在実行しているユニティーのパス</summary>
        public static string Unity {
            get {
                return System.Environment.GetCommandLineArgs()[0];
            }
        }

        /// <summary>バッチモード起動であるか返します</summary>
        public static bool IsBatchMode {
            get {
                return (IsBatchMode_ ?? (IsBatchMode_ =
                    System.Environment.GetCommandLineArgs()
                    .Any(n => n.StartsWith("-batchmode")))).Value;
            }
        }

        static bool? IsBatchMode_;

        /// <summary>グラフィックなしであるか返す</summary>
        public static bool IsNoGraphics {
            get {
                return (IsNoGraphics_ ?? (IsNoGraphics_ =
                    System.Environment.GetCommandLineArgs()
                    .Any(n => n.StartsWith("-nographics")))).Value;
            }
        }

        static bool? IsNoGraphics_;

#if UNITY_EDITOR

        /// <summary>ファイルからスクリプタブルオブジェクト作成します</summary>
        public static T CreateScriptableObject<T>(string path) //
            where T : ScriptableObject //
        {
            var o = AssetDatabase.LoadAssetAtPath<T>(path);
            if (o == null) {
                o = ScriptableObject.CreateInstance<T>();
                if (!File.Exists(path)) {
                    AssetDatabase.CreateAsset(o, path);
                    AssetDatabase.SaveAssets();
                }
            }
            return o;
        }

        /// <summary>エディタに設定されているAndroidJDK</summary>
        public static string AndroidJDK {
            get { return EditorPrefs.GetString("JdkPath"); }
            set { EditorPrefs.SetString("JdkPath", value); }
        }

        /// <summary>エディタに設定されているAndroidSDK</summary>
        public static string AndroidSDK {
            get { return EditorPrefs.GetString("AndroidSdkRoot"); }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }

        /// <summary>エディタに設定されているAndroidNDK</summary>
        public static string AndroidNDK {
            get { return EditorPrefs.GetString("AndroidNdkRoot"); }
            set { EditorPrefs.SetString("AndroidNdkRoot", value); }
        }

        /// <summary>ウィンドウの型名からEditorWindowオブジェクトを返す</summary>
        public static EditorWindow FindWindow(string name) {
            return Resources.FindObjectsOfTypeAll<EditorWindow>()
                .FirstOrDefault(_ => _.GetType().ToString() == name);
        }

        /// <summary>プロジェクトウィンドウを返す</summary>
        public static EditorWindow ProjectWindow {
            get {
                return ProjectWindow_ ?? (ProjectWindow_ = //
                  FindWindow("UnityEditor.ProjectWindow") ?? //
                  FindWindow("UnityEditor.ObjectBrowser") ?? //
                  FindWindow("UnityEditor.ProjectBrowser"));
            }
        }

        static EditorWindow ProjectWindow_;

        /// <summary>ヒエラルキーウィンドウを返す</summary>
        public static EditorWindow HierarchyWindow {
            get {
                return HierarchyWindow_ ?? (HierarchyWindow_ = 
                  FindWindow("UnityEditor.HierarchyWindow"));
            }
        }

        static EditorWindow HierarchyWindow_;

#endif

    }
}
