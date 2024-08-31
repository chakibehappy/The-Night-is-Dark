using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aceline.RPG;

[CreateAssetMenu(fileName = "Game Data", menuName = "Chaki/Game Data", order = 1)]
[System.Serializable]
public class GameData : DataCardSet
{
    public HintData[] hints;
    public FactData[] facts;
}
