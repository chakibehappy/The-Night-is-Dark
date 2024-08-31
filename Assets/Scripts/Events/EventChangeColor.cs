using Aceline.RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aceline.RPG.Test;


[CreateAssetMenu(menuName = "Event/New Change Color Event")]
[System.Serializable]
public class EventChangeColor : EventCardSet
{
    [SerializeField] private int number;
    public override void RunEvent()
    {
        GameEvents.Instance.CallChangeColorModel(number);
        base.RunEvent();
    }
}