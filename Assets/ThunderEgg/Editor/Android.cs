using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Linq;

namespace ThunderEgg.UnityUtilities {

    [InitializeOnLoad]
    public static class Android {
        static Android() {
            //if (!UnityUtilities.IsBatchMode) {
                //string t = null;
                //var path = Environment.PATHS;
                //t = Environment.ANDROID_HOME;
                //if (t != null) {
                //    path.Insert(0, Path.Combine(t, "tools"));
                //    path.Insert(0, Path.Combine(t, "platform-tools"));
                //}
                //Environment.PATHS = path;
                //t = Environment.GetEnvironmentVariable("ANDROID_HOME");
                //t = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //t = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                //UnityUtilities.ANDROID_HOME = Path.Combine(t, @"Android\android-sdk");
            //}
        }

//        static ShellContextMenu scm = new ShellContextMenu();

        [MenuItem("A/A")]
        public static void Func() {
            //var pos = new System.Drawing.Point(0, 0);
            //var info = new[] { new FileInfo(@"c:\windows\notepad.exe") };
            //scm.ShowContextMenu(info, pos);
        }
        ///    FileInfo[] files = new FileInfo[1];
        ///    files[0] = new FileInfo(@"c:\windows\notepad.exe");
        ///    scm.ShowContextMenu(this.Handle, files, Cursor.Position);
        ///    

    }


}

