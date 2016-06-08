using System.IO;
using System;
using System.Threading;
using System.Net;
using System.Net.Mime;
using UnityEngine;
using ThunderEgg.UnityUtilities.AssetBundleUtilities;

namespace ThunderEgg.UnityUtilities {

    public class TestServer : IDisposable {

        public class Control : IDisposable {

            TestServer TestServer;

            ~Control() {
                Dispose();
            }

            public void Dispose() {
                if (TestServer == null) {
                    return;
                }
                using (var t = TestServer) {
                    TestServer.Stop();
                    TestServer = null;
                }
            }

            public void Set(bool order) {
                var server_stat = TestServer != null;
                if (order == server_stat) return;
                if (order) {
                    var set = AssetBundleUtilities.Settings.Instance;
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

        const int MaxConnection = 20;

        const int ResponseBufferSize = 64 * 1024;

        /// <summary>公開ファイルの位置</summary>
        public string ContentsPath { get; private set; }

        /// <summary>公開URI位置</summary>
        public string ServerBaseUri { get; private set; }

        /// <summary>接続数</summary>       
        public int ConnectionCount {
            get { return Interlocked.Add(ref ConnectionCount_, 0); }
        }

        /// <summary>接続数</summary>       
        int ConnectionCount_;

        volatile HttpListener HttpListener;


        /// <summary>ディスポーズされているか</summary>
        public bool IsDisposed { get; private set; }

        /// <summary>ディスポーズされていたら例外を投げる</summary>
        void ThrowIfDisposed() {
            if (IsDisposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        ~TestServer() {
            Dispose();
        }

        public void Dispose() {
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
            if (IsStart) Stop();
            //            Debug.Log(string.Format("TestServer Start {0} {1}", ContentsPath, ServerBaseUri));
            ConnectionCount_ = 0;
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(ServerBaseUri);
            HttpListener.Start();
            for (var i = 0; i < MaxConnection; ++i) {
                HttpListener.BeginGetContext(CallBack, HttpListener);
            }
        }

        /// <summary>サービス停止</summary>
        public void Stop() {//
            if (!IsStart) return;
            //            Debug.Log("TestServer Stop");
            HttpListener.Stop();
            HttpListener.Close();
            // HttpListener.Abort();
            using (var listerner = HttpListener) {
                // すべてが停止になるまで監視
                HttpListener = null;
            }
            ConnectionCount_ = 0;
        }

        /// <summary>非同期コールバック処理</summary>
        void CallBack(IAsyncResult result) {

            if (HttpListener == null) {
                return;
            }

            var listener = (HttpListener)result.AsyncState;
            HttpListenerResponse res = null;

            try {
                Interlocked.Increment(ref ConnectionCount_);

                // コンテクストの取得を試みる
                var ctx = listener.EndGetContext(result);
                var req = ctx.Request;
                var path = ContentsPath + req.RawUrl;

                // GETメソッドでなければクローズ
                res = ctx.Response;
                if (string.Compare(req.HttpMethod, "GET", true) != 0) {
                    res.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    res.Close();
                    return;
                }

                // ファイルが無い場合はクローズ
                if (!File.Exists(path)) {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Close();
                    return;
                }

                // ファイル送信を試みる
                res.StatusCode = (int)HttpStatusCode.OK;
                if (!Responser(res, path)) {
                    // 終了要求が来ているのでアボート
                    var t = res;
                    res = null;
                    t.Abort();
                    listener = null;
                    return;
                }
                res.Close();
            }
            catch (Exception e) {
                // 停止要求例外系はリスンを続けないようにする
                // その他の例外はアボートさせリスンを継続
                if (e is HttpListenerException || //
                    e is ThreadAbortException ||
                    e is ObjectDisposedException) //
                {
                    listener = null;
                }
                else {
                    if (res != null) {
                        res.Abort();
                    }
                }
            }
            finally {

                // リスターナーが生きていればリスンを継続
                if (listener != null) {
                    listener.BeginGetContext(CallBack, listener);
                }

                Interlocked.Decrement(ref ConnectionCount_);
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

