using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

#endif

namespace Aceline.RPG
{
    public class GameBoardData { }


    #region Language
    [System.Serializable]
    public class Languages<T>
    {
        public Language Name;
        public T Value;
    }
    #endregion


    #region Scriptable Objects
    [System.Serializable]
    public class EventCardStat
    {
        public EventCardSet CardEvent;
    }
    #endregion



    #region Singular Stat
    [System.Serializable]
    public class TextStat
    {
        public string Value;
    }

    [System.Serializable]
    public class IntStat
    {
        public int Value;
    }

    [System.Serializable]
    public class FloatStat
    {
        public float Value;
    }

    [System.Serializable]
    public class ImageStat
    {
        public Sprite Value;
    }

    [System.Serializable]
    public class AudioStat
    {
        public AudioClip Value;
    }

    [System.Serializable]
    public class ChoiceStat
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public ChoiceStateType Value = ChoiceStateType.GrayOut;
    }

    [System.Serializable]
    public class ModifierStat
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public StatModifierType Value = StatModifierType.SetTrue;
    }

    [System.Serializable]
    public class ConditionType
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public StatConditionType Value = StatConditionType.True;
    }

    [System.Serializable]
    public class EndStat
    {
#if UNITY_EDITOR
        public UnityEngine.UIElements.EnumField EnumField;
#endif
        public EndCardType Value = EndCardType.End;
    }
    #endregion



    #region Event
    [System.Serializable]
    public class EventModifier
    {
        public TextStat Text = new TextStat();
        public FloatStat Value = new FloatStat();
        public ModifierStat Modifier = new ModifierStat();
    }

    [System.Serializable]
    public class BranchCondition
    {
        public DataCardSet DataCardSet;
        public Stat SelectedStat;

        // delete this both later :
        public FloatStat Value = new FloatStat(); 
        public ConditionType Condition = new ConditionType();
    }
    #endregion

}
