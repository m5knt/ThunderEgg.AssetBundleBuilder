using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading;

namespace ThunderEgg.AssetBundleUtilities {

    public class TestServer {

        const int ResponseBufferSize = 128 * 1024;

        string ContentsPath;
        string ServerBaseUri;

        HttpListener HttpListener;
        byte[] ResponseBuffer = new byte[ResponseBufferSize];

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
            Debug.Log(string.Format("TestServer Start {0} {1}", ContentsPath, ServerBaseUri));
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(ServerBaseUri);
            HttpListener.Start();
            HttpListener.BeginGetContext(CallBack, HttpListener);
        }

        /// <summary>サービス停止</summary>
        public void Stop() {
            if (HttpListener == null) return;
            Debug.Log("TestServer Stop");
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
            Debug.Log("TestServer Request " + path);
            using (var res = ctx.Response) {
                Responser(res, path);
            }
        }

        /// <summary>レスポンス返却</summary>
        void Responser(HttpListenerResponse res, string path) {
            using (var @in = File.OpenRead(path)) {
                var fname = Path.GetFileName(path);
                res.ContentLength64 = @in.Length;
                res.SendChunked = false;
                res.ContentType = MediaTypeNames.Application.Octet;
                res.AddHeader("Content-disposition", "attachment; filename=" + fname);
                int read;
                using (var @out = new BinaryWriter(res.OutputStream)) {
                    while ((read = @in.Read(ResponseBuffer, 0, ResponseBuffer.Length)) > 0) {
                        @out.Write(ResponseBuffer, 0, read);
                    }
                }
            }
        }
    }
}

