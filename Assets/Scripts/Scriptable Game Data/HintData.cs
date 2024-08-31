using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hint_Data", menuName = "Chaki/Hint", order = 1)]
public class HintData : ScriptableObject
{
    public int id;
    public string hintText;
}
