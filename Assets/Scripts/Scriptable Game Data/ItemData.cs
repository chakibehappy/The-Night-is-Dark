using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aceline.RPG;

[CreateAssetMenu(fileName = "Item_Data", menuName = "Chaki/Item", order = 1)]
public class ItemData : DataCardSet
{
    public string itemName;
    public int count;
    public bool isConsumable;
    public float energy;
    public Sprite icon;
    public string description;

    private void OnEnable()
    {
        Stat stat = new Stat
        {
            Name = "Count",
            Type = ValueType.Int,
        };
        Stats = new List<Stat>();
        Stats.Add(stat);
    }
}
