using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG
{
    [System.Serializable]
    public class DataCardSet : ScriptableObject
    {
        public List<Stat> Stats;
        public string[] AllCondition = new string[] { "==", ">", ">=", "<", "<=", "!=" };
        [HideInInspector] public string[] BoolCondition = new string[] { "True", "False" };
        [HideInInspector] public string[] IntCondition = new string[] { "==", ">", ">=", "<", "<=", "!=" };
        [HideInInspector] public string[] StringCondition = new string[] { "==", "!=" };
    }
}
