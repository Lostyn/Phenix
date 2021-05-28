using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix {
    public class App : MonoBehaviour
    {
        internal static string AppId;
        internal static string Authkey;
        internal static AppData Data;               /// <summary>json data</summary>
        internal static GameObject DataObject;
        internal static string Host;                /// <summary>Url to server base</summary>
        internal static string BasePath;            /// <summary>Filepath to local folders</summary>
        internal static Downloader Downloader;

        internal static string GetManifestPath(string appId) => appId;
        internal static string GetManifestUrl(string Id, bool cloudbuild = false) => App.Host + "apps/" + Id;



        /// <summary>
        /// Initialize the app and download necessary data
        /// </summary>
        /// <param name="OnReady">Will be called when done</param>
        public static void Initialize(OnEnd OnReady) {
            PhenixSettings phenixSettings = Resources.Load<PhenixSettings>("PhenixSettings");
            if (phenixSettings == null) {
                Debug.LogError("No App information found");
                OnReady(false, "No App information found");
            } else {
                App.Initialize(OnReady, phenixSettings.appID, phenixSettings.AUTHKey);
            }
        }

        public static void Initialize(OnEnd onReady, string appID, string authKey)
        {
            App.AppId = appID;
            App.Authkey = authKey;

            if (DataObject == null) {
                GameObject go = new GameObject("DATA");
                App.DataObject = go;
                App.DataObject.AddComponent<Startup>().Initialize(onReady, appID, authKey);
            }
        }
    }
}