using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Phenix {
    internal class Startup : MonoBehaviour
    {
        public bool GotJson { get; private set; }
        public bool GotJsonSucess { get; private set; }

        internal void Initialize(OnEnd OnReady, string appId, string authKey) {
            App.Host = "http://localhost:8080/api/";
            App.BasePath = Application.persistentDataPath + "/";
            StartCoroutine(Init(OnReady));
        }

        IEnumerator Init(OnEnd onReady)
        {
            string DataFilePath = App.BasePath + App.GetManifestPath(App.AppId);
            App.Downloader = App.DataObject.AddComponent<Downloader>();
            App.Downloader.Initialize();
            DontDestroyOnLoad(App.DataObject);

            if (File.Exists(DataFilePath))
                File.Delete(DataFilePath);

            App.Downloader.DownloadManifest(App.AppId, new OnEnd(OnJsonDownloaded));
            while (!GotJson) yield return null;

            AppData appData = (AppData) App.DataObject.AddComponent<AppData>();
            Debug.Log("GotJson");

            App.Data = appData;
        }

        private void OnJsonDownloaded(bool succes, string error)
        {
            if (!succes) Debug.Log("No server data: " + error);
            GotJson = true;
            GotJsonSucess = succes;
        }
    }
}