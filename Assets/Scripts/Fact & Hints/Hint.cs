using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hint
{
    public string hintText;
    public int id;
    public int relatedFactCount;

    public Hint(string text, int i)
    {
        hintText = text;
        id = i;
    }
}
