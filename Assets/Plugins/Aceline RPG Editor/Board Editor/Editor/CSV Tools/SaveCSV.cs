using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Aceline.RPG.Editor
{
    public class SaveCSV
    {
        private string csvDirectoryName = "Resources/Dialogue Editor/CSV File";
        private string csvFileName = "DialogueCSV_Save.csv";
        private string csvSeparator = ",";
        private List<string> csvHeader;
        private string node_ID = "Node Guid ID";
        private string text_ID = "Text Guid ID";
        private string dialogueName = "Dialogue Name";

        public void Save()
        {
            List<GameBoard> dialogueContainers = Helper.FindAllGameBoard();

            CreateFile();

            foreach (GameBoard dialogueContainer in dialogueContainers)
            {
                foreach (DialogueData cardData in dialogueContainer.DialogueDatas)
                {
                    foreach (StatDialogueText textData in cardData.Dialogues)
                    {
                        List<string> texts = new List<string>();

                        texts.Add(dialogueContainer.name);
                        texts.Add(cardData.CardGuid);
                        texts.Add(textData.GuidID.Value);

                        foreach (Language languageType in (Language[])Enum.GetValues(typeof(Language)))
                        {
                            string tmp = textData.Texts.Find(language => language.Name == languageType).Value.Replace("\"", "\"\"");
                            texts.Add($"\"{tmp}\"");
                        }

                        AppendToFile(texts);
                    }
                }

                foreach (ChoiceData cardData in dialogueContainer.ChoiceDatas)
                {
                    List<string> texts = new List<string>();

                    texts.Add(dialogueContainer.name);
                    texts.Add(cardData.CardGuid);
                    texts.Add("Choice Dont have Text ID");

                    foreach (Language languageType in (Language[])Enum.GetValues(typeof(Language)))
                    {
                        string tmp = cardData.Texts.Find(language => language.Name == languageType).Value.Replace("\"", "\"\"");
                        texts.Add($"\"{tmp}\"");
                    }

                    AppendToFile(texts);
                }
            }
        }

        private void AppendToFile(List<string> strings)
        {
            using (StreamWriter sw = File.AppendText(GetFilePath()))
            {
                string finalString = "";
                foreach (string text in strings)
                {
                    if (finalString != "")
                    {
                        finalString += csvSeparator;
                    }
                    finalString += text;
                }

                sw.WriteLine(finalString);
            }
        }

        private void CreateFile()
        {
            VerifyDirectory();
            MakeHeader();
            using (StreamWriter sw = File.CreateText(GetFilePath()))
            {
                string finalString = "";
                foreach (string header in csvHeader)
                {
                    if (finalString != "")
                    {
                        finalString += csvSeparator;
                    }
                    finalString += header;
                }

                sw.WriteLine(finalString);
            }
        }

        private void MakeHeader()
        {
            List<string> headerText = new List<string>();
            headerText.Add(dialogueName);
            headerText.Add(node_ID);
            headerText.Add(text_ID);

            foreach (Language language in (Language[])Enum.GetValues(typeof(Language)))
            {
                headerText.Add(language.ToString());
            }

            csvHeader = headerText;
        }

        private void VerifyDirectory()
        {
            string directory = GetDirectoryPath();

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string GetDirectoryPath()
        {
            return $"{Application.dataPath}/{csvDirectoryName}";
        }

        private string GetFilePath()
        {
            return $"{GetDirectoryPath()}/{csvFileName}";
        }
    }
}