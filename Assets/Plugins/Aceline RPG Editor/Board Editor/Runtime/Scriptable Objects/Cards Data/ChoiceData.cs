using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG
{
    [System.Serializable]
    public class ChoiceData : BaseData
    {
        
#if UNITY_EDITOR
        public TextField TextField { get; set; }
        public ObjectField ObjectField { get; set; }
#endif

        public ChoiceStat Choice = new ChoiceStat();
        public List<Languages<string>> Texts = new List<Languages<string>>();
        public List<Languages<AudioClip>> VoiceOvers = new List<Languages<AudioClip>>();
        public List<BranchCondition> Conditions = new List<BranchCondition>();
    }
}
