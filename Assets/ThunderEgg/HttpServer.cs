using System.IO;
using System;
using System.Threading;
using System.Net;
using System.Net.Mime;
using UnityEngine;
using ThunderEgg.UnityUtilities.AssetBundleUtilities;

namespace ThunderEgg.UnityUtilities {

    // curl --connect-timeout 5 --speed-limit 0 --speed-time 5 

    public class HttpServer : IDisposable {

        public class Control : IDisposable {

            HttpServer HttpServer;

            ~Control() {
                Dispose();
            }

            public void Dispose() {
                if (HttpServer == null) {
                    return;
                }
                using (var t = HttpServer) {
                    HttpServer.Stop();
                    HttpServer = null;
                }
            }

            public void Set(bool order) {
                var server_stat = HttpServer != null;
                if (order == server_stat) return;
                if (order) {
                    var set = Settings.Instance;
                    HttpServer = new HttpServer(set.Output, "*", set.TestServerPort);
                    HttpServer.Start();
                } else {
                    HttpServer.Stop();
                    HttpServer = null;
                }
            }
        }

        const int MaxConnection = 1;

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
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        ~HttpServer() {
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
        public HttpServer(string contents_path, string server_name, int port) :
            this(contents_path, string.Format("http://{0}:{1}/", server_name, port)) {
        }

        /// <summary>テストサーバー</summary>
        /// <param name="contents_path">公開コンテンツのパス</param>
        /// <param name="server_base_uri">公開サーバーのベースURI</param>
        public HttpServer(string contents_path, string server_base_uri) {
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
            HttpListener.Stop();
            using (var listerner = HttpListener) {
                HttpListener = null;
                listerner.Close();
                listerner.Abort();
                // すべてが停止になるまで監視
                while (Interlocked.Add(ref ConnectionCount_, 0) > 0) {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>非同期コールバック処理</summary>
        void CallBack(IAsyncResult result) {

            var listener = (HttpListener)result.AsyncState;
            if (listener != HttpListener) {
                return;
            }
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
                if (!Responser(listener, res, path)) {
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
        bool Responser(HttpListener listener, HttpListenerResponse res, string path) {
            using (var from = File.OpenRead(path)) {
                var fname = Path.GetFileName(path);
                res.ContentLength64 = from.Length;
                res.SendChunked = false;
                res.ContentType = MediaTypeNames.Application.Octet;
                res.KeepAlive = true;
                res.AddHeader("Content-disposition", "attachment; filename=" + fname);
                return CopyTo(listener, from, res.OutputStream);
            }
        }

        /// <summary>ストリームのコピー</summary>
        bool CopyTo(HttpListener listener, Stream from, Stream to) {
            var buffer = new byte[ResponseBufferSize];
            var total = 0;
            int count;
            using (var wr = new BinaryWriter(to)) {
                while ((count = from.Read(buffer, 0, buffer.Length)) > 0) {
                    if (listener != HttpListener) {
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

