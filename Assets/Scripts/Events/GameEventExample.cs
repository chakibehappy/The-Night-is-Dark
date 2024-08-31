using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aceline.RPG.Test;
using Aceline.RPG;

public class GameEventExample : GameEvents
{
    private GameMaster GM;

    private void Start()
    {
        GM = GameMaster.GM;
    }

    public override bool EventCondition(string varName, StatConditionType condition, float value = 0)
    {
        switch (varName.ToLower())
        {
            case "money":
                return useConditionCard.ConditionFloatCheck(GM.money, value, condition);

            //case "energy":
            //    return useConditionCard.ConditionFloatCheck(GM.STAMINA.energy, value, condition);

            case "canmove":
                return useConditionCard.ConditionBoolCheck(GM.canMove,  condition);
                    
            default:
                return false;
        }
    }

    public override void EventModifier(string varName, StatModifierType modifier, float value = 0)
    {
        switch (varName.ToLower())
        {
            case "energy":
                //GM.ModifyEnergy(useEventCard.ModifierFloatCheck(GM.STAMINA.energy, value, modifier), true);
                break;

            default:
                break;
        }
    }
}
