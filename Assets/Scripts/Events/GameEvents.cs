using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Aceline.RPG.Test
{
    public class GameEvents : MonoBehaviour
    {
        private event Action<int> changeColorModel;

        protected UseConditionCard useConditionCard = new UseConditionCard();
        protected UseEventCard useEventCard = new UseEventCard();

        public static GameEvents Instance { get; private set; }

        public Action<int> ChangeColorModel { get => changeColorModel; set => changeColorModel = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CallChangeColorModel(int number)
        {
            changeColorModel?.Invoke(number);
        }

        public virtual void EventModifier(string varName, StatModifierType modifier, float value = 0 )
        {

        }

        public virtual bool EventCondition(string varName, StatConditionType condition, float value = 0)
        {
            return false;
        }
    }
}
