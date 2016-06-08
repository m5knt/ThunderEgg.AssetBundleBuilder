#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderEgg {

    /// <summary>雑多なユーティリティーの入れ物</summary>
    public static class Environment {

        /// <summary>環境変数を取得します</summary>
        /// <param name="key">変数名</param>
        /// <returns>存在しない場合はnull</returns>
        public static string Get(string key) {
            return System.Environment.GetEnvironmentVariable(key);
        }

        /// <summary>環境変数を設定します</summary>
        /// <param name="key">変数名</param>
        /// <param name="val">値</param>
        /// <returns>以前の値を返します</returns>
        public static string Set(string key, string val) {
            var prev = System.Environment.GetEnvironmentVariable(key);
            if (prev == val) {
                return prev;
            }
            System.Environment.SetEnvironmentVariable(key, val);
            return prev;
        }

        /// <summary>変更前の環境変数 PATH</summary>
        public static string ORIGINAL_PATH {
            get { 
                return ORIGINAL_PATH_ ?? (ORIGINAL_PATH_ = Get("PATH"));
            }
        }

        static string ORIGINAL_PATH_;

        /// <summary>環境変数 PATH</summary>
        public static string PATH {
            get {
                var path = Get("PATH");
                ORIGINAL_PATH_ = ORIGINAL_PATH_ ?? path;
                return path;
            }
            set {
                ORIGINAL_PATH_ = ORIGINAL_PATH_ ?? Get("PATH");
                Set("PATH", value);
            }
        }

        /// <summary>環境変数 JDK_HOME</summary>
        public static string JDK_HOME {
            get { return Get("JDK_HOME"); }
            set { Set("JDK_HOME", value); }
        }

        /// <summary>環境変数 ANDROID_HOME</summary>
        public static string ANDROID_HOME {
            get { return Get("ANDROID_HOME"); }
            set { Set("ANDROID_HOME", value); }
        }

        /// <summary>環境変数 ANDROID_NDK_ROOT</summary>
        public static string ANDROID_NDK_ROOT {
            get { return Get("ANDROID_NDK_ROOT"); }
            set { Set("ANDROID_NDK_ROOT", value); }
        }

    }
}
