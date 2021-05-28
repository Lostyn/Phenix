using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Phenix {
    public class Downloader : MonoBehaviour
    {
        protected Dictionary<string, string> response;
        protected List<string> queue;
        
        Dictionary<string, JobInfo> _jobs;

        internal void Initialize()
        {
            queue = new List<string>();
            _jobs = new Dictionary<string, JobInfo>();

            this.response = new Dictionary<string, string>();
            this.response.Add("200", "OK");
            this.response.Add("202", "Accepted");
            this.response.Add("204", "No Content");
            this.response.Add("400", "Bad Request");
            this.response.Add("401", "Unauthorized");
            this.response.Add("403", "Forbidden");
            this.response.Add("404", "Not Found");
            this.response.Add("405", "Method Not Allowed");
            this.response.Add("406", "Not Acceptable");
            this.response.Add("408", "Request Timeout");
            StartCoroutine(DownloadManager(queue));
        }

        internal string GetAuthSign(UnityWebRequest www) {
            string apiKey = App.AppId;
            string requestSignatureBase64String = "";
            var method = www.method.ToLower();
            var uri = www.uri.AbsolutePath;

            var signatureRawData = $"(request-target): {method} {uri}";
            var secretKeyBytes = Encoding.UTF8.GetBytes(App.Authkey);
            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);
            using( HMACSHA256 hmac = new HMACSHA256(secretKeyBytes)) {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
            }
            Debug.Log($"Signature keyId=\"{apiKey}\",algorithm=\"hmac-sha256\",headers=\"(request-target)\",signature=\"{requestSignatureBase64String}\"");
            return $"Signature keyId=\"{apiKey}\",algorithm=\"hmac-sha256\",headers=\"(request-target)\",signature=\"{requestSignatureBase64String}\"";
        }

        internal void DownloadManifest(string appId, OnEnd onEnd)
        {
            queue.Add(appId);
            if (_jobs.ContainsKey(appId))
                _jobs[appId].onEnd += onEnd;
            else {
                _jobs.Add(appId, new JobInfo() {
                    url = App.GetManifestUrl(appId),
                    destination = App.BasePath + App.GetManifestPath(appId),
                    IsDone = false,
                    onEnd = onEnd
                });
            }
        }

        private IEnumerator DownloadManager(List<string> dlQueue) {
            string currentDownload = "";
            UnityWebRequest www;

            while(true) {
                while (dlQueue.Count == 0) yield return (object) new WaitForSeconds(1f);
                currentDownload = dlQueue[0];
                dlQueue.RemoveAt(0);
                if (!_jobs.ContainsKey(currentDownload))
                {
                    Debug.LogError((object) ("[Downloader] Current download in queue does not have associated Job: " + currentDownload));
                }
                else
                {
                    www = UnityWebRequest.Get(_jobs[currentDownload].url);
                    www.SetRequestHeader("Authorization", GetAuthSign(www));
                    www.disposeDownloadHandlerOnDispose = true;
                    Debug.Log("Current download: " + currentDownload + " [" + _jobs[currentDownload].url + "]");
                    DataStream handler = new DataStream(_jobs[currentDownload].destination);
                    www.downloadHandler = handler;
                    www.SendWebRequest();

                    while(!www.isDone){
                        yield return null;
                        if (!(www.result != UnityWebRequest.Result.Success) && _jobs.ContainsKey(currentDownload)) {
                            _jobs[currentDownload].Progress = handler.downloadedBytes;
                        } else break;
                    }

                    string error = www.error;
                    bool isError = www.result != UnityWebRequest.Result.Success;
                    long code = www.responseCode;

                    if (isError || code == 404L) {
                        handler.FlushAndClose();
                        www.Dispose();

                        try {
                            if (_jobs.ContainsKey(currentDownload));
                                File.Delete(_jobs[currentDownload].destination);
                        } catch( Exception ex) {
                            Debug.LogError("Error while deleting falied download file: " + ex?.ToString());
                        }

                        string responseMeaning = "";
                        if (response.ContainsKey(code.ToString()))
                            responseMeaning = response[code.ToString()];
                        _jobs[currentDownload].onEnd?.Invoke(false, "Given Error: " + error + ". Response code: " + code.ToString() + " " + responseMeaning);
                        _jobs.Remove(currentDownload);
                    } else {
                        handler.FlushAndClose();
                        www.disposeDownloadHandlerOnDispose = true;
                        www.Dispose();

                        if (code == 200L || _jobs[currentDownload].url.Contains("http")) {
                            if (App.Data != null) {
                                Debug.Log((object) ("Trying to set local version of " + currentDownload));
                                // Todo:
                            }

                            _jobs[currentDownload].onEnd?.Invoke(true, null);
                            _jobs.Remove(currentDownload);
                        } else {
                            if (File.Exists(_jobs[currentDownload].destination)) {
                                File.Delete(_jobs[currentDownload].destination);
                                Debug.LogWarning((object) ("Incomplete file deleted. ID: " + currentDownload));
                            }

                            string responseMeaning = "";
                            if (response.ContainsKey(code.ToString()))
                                responseMeaning = response[code.ToString()];
                            _jobs[currentDownload].onEnd?.Invoke(false, "Given Error: " + error + ". Response code: " + code.ToString() + " " + responseMeaning);
                            _jobs.Remove(currentDownload);
                        }

                        handler = null;
                    }
                }
            }
        }


        internal class JobInfo {
            internal string url;
            internal string destination;
            internal long Size;
            internal long Progress;
            internal bool IsDone;
            internal OnEnd onEnd;
        }

        internal class DataStream : DownloadHandlerScript {
            private string path;
            private FileStream stream;
            internal ulong contentLength = 1;
            internal long downloadedBytes = 0;

            internal DataStream(string path) {
                Directory.CreateDirectory(new FileInfo(path).Directory.FullName);
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, 512, true);
            }

            internal void FlushAndClose() {
                stream.Flush();
                stream.Close();
            }

            internal DataStream(byte[] buffer) : base(buffer) { }

            protected override byte[] GetData() => (byte[]) null;

            protected override bool ReceiveData(byte[] data, int dataLength) {
                if (data == null || data.Length < 1) {
                    Debug.Log("Received a null/empty buffer");
                    return false;
                }

                stream.Write(data, 0, dataLength);
                downloadedBytes += (long) dataLength;
                return true;
            }

            protected override void ReceiveContentLengthHeader(ulong contentLength) => this.contentLength = contentLength;
        }
    }
}