using System.IO;
using System;
using System.Threading;
using System.Net;
using System.Net.Mime;
using UnityEngine;

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
                    var path = set.Output;
                    TestServer = new TestServer(path, "*", set.TestServerPort);
                    TestServer.Start();
                }
            }
        }

        public int MaxConnection = 20;
        static int ResponseBufferSize = 64 * 1024;

        public string ContentsPath { get; private set; }
        public string ServerBaseUri { get; private set; }

        HttpListener HttpListener;
        int Connection_;

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
            if (HttpListener != null) {
                Stop();
            }
            Connection_ = 0;
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(ServerBaseUri);
            HttpListener.Start();
            for (var i = 0; i < MaxConnection; ++i) {
                HttpListener.BeginGetContext(CallBack, HttpListener);
            }
        }

        /// <summary>サービス停止</summary>
        public void Stop() {
            if (HttpListener == null) return;
            //            Debug.Log("TestServer Stop");
            HttpListener.Stop();
            while (Interlocked.Decrement(ref Connection_) >= 0) {
                Interlocked.Increment(ref Connection_);
                Thread.Sleep(1);
            }
            using (var t = HttpListener) { HttpListener = null; }
            Connection_ = 0;
        }

        /// <summary>非同期コールバック処理</summary>
        public void CallBack(IAsyncResult result) {

            Interlocked.Increment(ref Connection_);

            var listener = (HttpListener)result.AsyncState;
            HttpListenerResponse res = null;

            try {
                // コンテクストの取得を試みる
                var ctx = listener.EndGetContext(result);
                var req = ctx.Request;
                var path = ContentsPath + req.RawUrl;

                // メソッド確認
                res = ctx.Response;
                if (string.Compare(req.HttpMethod, "GET", true) != 0) {
                    res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    res.Close();
                    res = null;
                }

                // ファイル確認
                if (!File.Exists(path)) {
                    // ファイルが見つからなかった
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Close();
                    res = null;
                }

                // ファイル送信を試みる
                res.StatusCode = (int)HttpStatusCode.OK;
                Responser(res, path);
                res.Close();
                res = null;
            }
            catch (Exception e) {
                // 停止要求例外なら続けないようにする
                if (e is HttpListenerException || e is ThreadAbortException ||
                    e is ObjectDisposedException) //
                {
                    listener = null;
                } else {
                    if (res != null) {
                        res.Abort();
                    }
                }
            }
            finally {
                if (listener != null) {
                    listener.BeginGetContext(CallBack, listener);
                }
                Interlocked.Decrement(ref Connection_);
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
                from.Close();
            }
        }

        /// <summary>ストリームのコピー</summary>
        static void CopyTo(Stream from, Stream to) {
            var buffer = new byte[ResponseBufferSize];
            var total = 0;
            int count;
            using (var wr = new BinaryWriter(to)) {
                while ((count = from.Read(buffer, 0, buffer.Length)) > 0) {
                    wr.Write(buffer, 0, count);
                    wr.Flush();
                    total += count;
                }
                wr.Close();
            }
        }
    }
}

