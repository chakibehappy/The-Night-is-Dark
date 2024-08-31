using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG
{
    [System.Serializable]
    public class Stat 
    {
        public string Name;
        public ValueType Type;

        [HideInInspector] public DataCardSet DataValue;
        [HideInInspector] public bool BoolValue;
        [HideInInspector] public int IntValue;
        [HideInInspector] public float FloatValue;
        [HideInInspector] public string StringValue;
        [HideInInspector] public Vector3 Vector3Value;
        [HideInInspector] public Color ColorValue;
        [HideInInspector] public AudioClip AudioClipValue;
        [HideInInspector] public Sprite SpriteValue;
    }

}
