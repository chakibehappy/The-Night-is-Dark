using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG
{
    [System.Serializable]
    public class EventData : BaseData
    {
        public List<EventModifier> Modifiers = new List<EventModifier>();
        public List<EventCardStat> EventCardSet = new List<EventCardStat>();
    }
}
