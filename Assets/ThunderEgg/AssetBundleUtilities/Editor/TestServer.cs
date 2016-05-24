using System.IO;
using System;
using System.Threading;
using System.Net;
using System.Net.Mime;
using UnityEngine;

namespace ThunderEgg.AssetBundleUtilities {

    public class TestServer : IDisposable {

        public class Control : IDisposable {

            TestServer TestServer;

            ~Control() {
                Dispose();
            }

            public void Dispose() {
                Set(false);
            }

            public void Set(bool run) {
                var has_server = TestServer != null;
                if (run == has_server) return;
                if (run) {
                    var set = Settings.Instance;
                    TestServer = new TestServer(set.Output, "*", set.TestServerPort);
                    TestServer.Start();
                } else {
                    using (var t = TestServer) {
                        TestServer.Stop();
                        TestServer = null;
                    }
                }
            }
        }

        public int MaxConnection = 20;
        int ResponseBufferSize = 64 * 1024;

        public string ContentsPath { get; private set; }
        public string ServerBaseUri { get; private set; }

        volatile HttpListener HttpListener;
        int Connection_;

        /// <summary>ディスポーズされているか</summary>
        public bool IsDisposed { get; private set; }

        /// <summary>ディスポーズされていたら例外を投げる</summary>
        void ThrowIfDisposed() {
            if (IsDisposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        ~TestServer() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
        }

        void Dispose(bool b) {
            if (IsDisposed) {
                return;
            }
            Stop();
            IsDisposed = true;
        }

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

        public bool IsStart {
            get {
                ThrowIfDisposed();
                return HttpListener != null;
            }
        }

        /// <summary>サービス開始</summary>
        public void Start() {
            //            Debug.Log(string.Format("TestServer Start {0} {1}", ContentsPath, ServerBaseUri));
            if (IsStart) Stop();
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
            if (!IsStart) return;
            //            Debug.Log("TestServer Stop");
            HttpListener.Stop();
            using (var listerner = HttpListener) {
                // すべてが停止になるまで監視
                HttpListener = null;
                while (Interlocked.Decrement(ref Connection_) >= 0) {
                    Interlocked.Increment(ref Connection_);
                    Thread.Sleep(1);
                }
            }
            Connection_ = 0;
        }

        /// <summary>非同期コールバック処理</summary>
        void CallBack(IAsyncResult result) {
            if (HttpListener == null) {
                return;
            }
            var listener = (HttpListener)result.AsyncState;
            HttpListenerResponse res = null;

            try {
                Interlocked.Increment(ref Connection_);
                // コンテクストの取得を試みる
                var ctx = listener.EndGetContext(result);
                var req = ctx.Request;
                var path = ContentsPath + req.RawUrl;

                // メソッド確認
                res = ctx.Response;
                if (string.Compare(req.HttpMethod, "GET", true) != 0) {
                    res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    res.Close();
                    return;
                }

                // ファイル確認
                if (!File.Exists(path)) {
                    // ファイルが見つからなかった
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Close();
                    return;
                }

                // ファイル送信を試みる
                res.StatusCode = (int)HttpStatusCode.OK;
                if (!Responser(res, path)) {
                    res.OutputStream.Dispose();
                    res.Abort();
                    listener = null;
                    return;
                }
                res.Close();
            }
            catch (Exception e) {
                // 停止要求例外系なら続けないようにする
                if (e is HttpListenerException || //
                    e is ThreadAbortException ||
                    e is ObjectDisposedException) //
                {
                    listener = null;
                }
                else {
                    // その他の例外はレスポンスをアボートさせリスンを継続
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
        bool Responser(HttpListenerResponse res, string path) {
            using (var from = File.OpenRead(path)) {
                var fname = Path.GetFileName(path);
                res.ContentLength64 = from.Length;
                res.SendChunked = false;
                res.ContentType = MediaTypeNames.Application.Octet;
                res.KeepAlive = true;
                res.AddHeader("Content-disposition", "attachment; filename=" + fname);
                return CopyTo(from, res.OutputStream);
            }
        }

        /// <summary>ストリームのコピー</summary>
        bool CopyTo(Stream from, Stream to) {
            var buffer = new byte[ResponseBufferSize];
            var total = 0;
            int count;
            using (var wr = new BinaryWriter(to)) {
                while ((count = from.Read(buffer, 0, buffer.Length)) > 0) {
                    if (HttpListener == null) {
                        return false;
                    }
                    wr.Write(buffer, 0, count);
                    wr.Flush();
                    total += count;
                }
                wr.Close();
            }
            return true;
        }
    }
}

