using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aceline.RPG.Test
{
    public class UseConditionCard 
    {
        public bool ConditionFloatCheck(float currentValue, float checkValue, StatConditionType condition)
        {
            switch (condition)
            {
                case StatConditionType.Equal:
                    return currentValue == checkValue;

                case StatConditionType.EqualOrBigger:
                    return currentValue >= checkValue;

                case StatConditionType.Bigger:
                    return currentValue > checkValue;

                case StatConditionType.EqualOrSmaller:
                    return currentValue <= checkValue;

                case StatConditionType.Smaller:
                    return currentValue < checkValue;
                    
                case StatConditionType.NotEqual:
                    return currentValue != checkValue;
                    
                default:
                    Debug.Log("Game Event didnt find a event");
                    return false;
            }
        }

        public bool ConditionBoolCheck(bool currentValue, StatConditionType condition)
        {
            switch (condition)
            {
                case StatConditionType.True:
                    return currentValue == true;

                case StatConditionType.False:
                    return currentValue == false;

                default:
                    Debug.Log("Game Event didnt find a event");
                    return false;
            }
        }
    }
}