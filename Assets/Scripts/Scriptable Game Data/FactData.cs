using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fact_Data", menuName = "Chaki/Fact", order = 1)]
public class FactData : ScriptableObject
{
    public int id;
    public string factText;
    public bool isCommand;
    public List<HintData> hints;
}
