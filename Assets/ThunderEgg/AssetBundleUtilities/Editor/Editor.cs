using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEditor.Callbacks;

namespace ThunderEgg.AssetBundleUtilities {

    [InitializeOnLoad]
    public class Editor {

        static TestServer.Control TestServerControl = new TestServer.Control();

        static Editor() {           
            if (!Utilities.IsBatchMode) {
                var b = EditorPrefs.GetBool(TestServerMenuString, false);
                TestServerControl.Set(b);
            }
        }

        const string TestServerMenuString = "Assets/[ThunderEgg]/TestServer";

        [MenuItem(TestServerMenuString, false, 100)]
        static void TestServerMenu() {
            var b = !EditorPrefs.GetBool(TestServerMenuString, false);
            EditorPrefs.SetBool(TestServerMenuString, b);
            TestServerControl.Set(b);
        }

        [MenuItem(TestServerMenuString, true)]
        static bool TestServerMenu_() {
            var b = EditorPrefs.GetBool(TestServerMenuString, false);
            Menu.SetChecked(TestServerMenuString, b);
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

