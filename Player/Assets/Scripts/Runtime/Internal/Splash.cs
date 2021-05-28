using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix
{    
    public class Splash : MonoBehaviour
    {
        void Start()
        {
            App.Initialize(OnAppInitialized);
        }

        private void OnAppInitialized(bool succes, string error)
        {
            throw new NotImplementedException();
        }
    }
}
