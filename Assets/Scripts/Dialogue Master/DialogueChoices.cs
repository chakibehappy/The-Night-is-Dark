using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Aceline.RPG;

[System.Serializable]
public class DialogueChoices
{
    public ChoiceStateType ChoiceState;
    public string Text;
    public UnityAction unityAction;
    public bool conditionCheck = true;
}
