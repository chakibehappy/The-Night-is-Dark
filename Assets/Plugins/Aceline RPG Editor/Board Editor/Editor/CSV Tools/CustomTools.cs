using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aceline.RPG.Editor
{
    public class CustomTools
    {
        [MenuItem("Custom Tools/Dialogue/Save Dialogue to CSV")]
        public static void SaveToCSV()
        {
            SaveCSV saveCSV = new SaveCSV();
            saveCSV.Save();

            EditorApplication.Beep();
            Debug.Log("<color=#90EE90> Save CSV File Successfully! </color>");
        }


        [MenuItem("Custom Tools/Dialogue/Load CSV to Dialogue")]
        public static void LoadCSVToDialogue()
        {
            LoadCSV loadCSV = new LoadCSV();
            loadCSV.Load();

            EditorApplication.Beep();
            Debug.Log("<color=#A865C9> CSV File Successfully Loaded! </color>");
        }


    }
}
