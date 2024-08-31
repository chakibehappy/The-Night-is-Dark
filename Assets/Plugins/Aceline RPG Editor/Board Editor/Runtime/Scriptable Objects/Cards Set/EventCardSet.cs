using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG
{
    [System.Serializable]
    public class EventCardSet : ScriptableObject
    {
        public virtual void RunEvent()
        {
            //Debug.Log("Event was Call");
        }
    }
}
