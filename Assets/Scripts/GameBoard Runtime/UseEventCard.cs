using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aceline.RPG.Test
{
    public class UseEventCard
    {
        public bool ModifierBoolCheck(StatModifierType modifier)
        {
            switch (modifier)
            {
                case StatModifierType.SetTrue:
                    return true;

                case StatModifierType.SetFalse:
                    return false;

                default:
                    return false;
            }
        }

        public float ModifierFloatCheck(float currentValue, float inputValue, StatModifierType modifier)
        {
            switch (modifier)
            {
                case StatModifierType.Add:
                    return currentValue + inputValue;
                
                case StatModifierType.Subtract:
                    return currentValue - inputValue;

                case StatModifierType.Multiply:
                    return currentValue + inputValue;

                case StatModifierType.Divide:
                    return currentValue + inputValue;

                case StatModifierType.Change:
                    return inputValue;
                
                default:
                    return currentValue;
            }
        }
    }
}
