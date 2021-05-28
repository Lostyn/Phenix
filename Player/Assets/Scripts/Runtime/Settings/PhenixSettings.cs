using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix {
    [CreateAssetMenu(fileName = "PhenixSettings", menuName = "Phenix/Settings", order = 1)]
    public class PhenixSettings : ScriptableObject
    {  
        public string appID;
        public string AUTHKey;
    }
}