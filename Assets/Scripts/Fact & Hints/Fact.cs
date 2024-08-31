using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fact
{
    public string factText;
    public int id;
    public bool isCommand;
    public List<int> hints;
    public bool isUnlocked;

    public Fact(string text, int i, bool iscommand, List<int> hint, bool unlocked = false)
    {
        factText = text;
        id = i;
        isCommand = iscommand;
        hints = hint;
        isUnlocked = unlocked;
    }
}
