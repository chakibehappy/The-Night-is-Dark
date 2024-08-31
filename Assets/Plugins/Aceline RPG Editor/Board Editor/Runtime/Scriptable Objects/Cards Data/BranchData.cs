using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG
{

    [System.Serializable]
    public class BranchData : BaseData
    {
        public string OptionTrueGuid;
        public string OptionFalseGuid;
        public List<BranchCondition> Conditions = new List<BranchCondition>();
    }
}
