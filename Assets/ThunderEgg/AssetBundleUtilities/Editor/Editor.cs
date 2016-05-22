using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ThunderEgg.AssetBundleUtilities {

    public class Editor {

        static TestServer TestServer;

        [MenuItem("Assets/[ThunderEgg]/TestServer/Start", priority = 100)]
        static void Start() {
            if (TestServer != null) Stop();
            var set = Settings.Instance;
            var path = set.Output + "/" + Utilities.Root;
            TestServer = new TestServer(path, "*", set.TestServerPort);
            TestServer.Start();
        }

        [MenuItem("Assets/[ThunderEgg]/TestServer/Stop", priority = 100)]
        static void Stop() {
            if (TestServer == null) return;
            TestServer.Stop();
            TestServer = null;
        }

        [MenuItem("Assets/[ThunderEgg]/Builder/Build", priority = 100)]
        static void Build() {
            Builder.Build();
        }

        [MenuItem("Assets/[ThunderEgg]/Builder/Clean", priority = 101)]
        static void Clean() {
            Builder.Clean();
        }
    }
}

