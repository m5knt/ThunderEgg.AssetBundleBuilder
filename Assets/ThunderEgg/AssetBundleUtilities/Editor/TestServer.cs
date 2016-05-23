using System.IO;
using System;
using System.Net;
using System.Net.Mime;

namespace ThunderEgg.AssetBundleUtilities {

    public class TestServer {

        public class Control {

            TestServer TestServer;

            public void Set(bool start) {
                if (!start || TestServer != null) {
                    if (TestServer != null) TestServer.Stop();
                    TestServer = null;
                }

                if (start) {
                    var set = Settings.Instance;
                    var path = set.Output + "/" + Utilities.Root;
                    TestServer = new TestServer(path, "*", set.TestServerPort);
                    TestServer.Start();
                }
            }
        }

        const int ResponseBufferSize = 64 * 1024;

        string ContentsPath;
        string ServerBaseUri;

        HttpListener HttpListener;

        /// <summary>テストサーバー</summary>
        /// <param name="contents_path">公開コンテンツのパス</param>
        /// <param name="server_name">サーバー名</param>
        /// <param name="port">ポート</param>
        public TestServer(string contents_path, string server_name, int port) :
            this(contents_path, string.Format("http://{0}:{1}/", server_name, port)) {
        }

        /// <summary>テストサーバー</summary>
        /// <param name="contents_path">公開コンテンツのパス</param>
        /// <param name="server_base_uri">公開サーバーのベースURI</param>
        public TestServer(string contents_path, string server_base_uri) {
            contents_path = contents_path.Replace('\\', '/');
            if (contents_path.EndsWith("/")) {
                contents_path = contents_path.Substring(0, contents_path.Length - 1);
            }
            ContentsPath = contents_path;
            ServerBaseUri = server_base_uri;
        }

        /// <summary>サービス開始</summary>
        public void Start() {
            //            Debug.Log(string.Format("TestServer Start {0} {1}", ContentsPath, ServerBaseUri));
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(ServerBaseUri);
            HttpListener.Start();
            HttpListener.BeginGetContext(CallBack, HttpListener);
        }

        /// <summary>サービス停止</summary>
        public void Stop() {
            if (HttpListener == null) return;
            //            Debug.Log("TestServer Stop");
            HttpListener.Stop();
            HttpListener = null;
        }

        /// <summary>非同期コールバック処理</summary>
        public void CallBack(IAsyncResult result) {
            // コンテクストの取得を試みる
            var listener = (HttpListener)result.AsyncState;
            var ctx = listener.EndGetContext(result);
            listener.BeginGetContext(CallBack, listener);

            // レスポンス返却を試みる
            var path = ContentsPath + ctx.Request.RawUrl;
            //            Debug.Log("TestServer Request " + path);
            using (var res = ctx.Response) {
                try {
                    if (!File.Exists(path)) {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }
                    Responser(res, path);
                }
                catch (Exception e) {
                    res.Abort();
                    throw e;
                }
                finally {
                    res.Close();
                }
            }
        }

        /// <summary>レスポンス返却</summary>
        void Responser(HttpListenerResponse res, string path) {
            using (var from = File.OpenRead(path)) {
                var fname = Path.GetFileName(path);
                res.ContentLength64 = from.Length;
                res.SendChunked = false;
                res.ContentType = MediaTypeNames.Application.Octet;
                res.AddHeader("Content-disposition", "attachment; filename=" + fname);
                CopyTo(from, res.OutputStream);
                // from.CopyTo(res.OutputStream);
                res.StatusCode = (int)HttpStatusCode.OK;
                from.Close();
            }
        }

        /// <summary>ストリームのコピー</summary>
        static void CopyTo(Stream from, Stream to) {
            var buffer = new byte[ResponseBufferSize];
            int count;
            using (var wr = new BinaryWriter(to)) {
                while ((count = from.Read(buffer, 0, buffer.Length)) > 0) {
                    wr.Write(buffer, 0, count);
                    wr.Flush();
                }
                wr.Close();
            }
        }
    }
}

