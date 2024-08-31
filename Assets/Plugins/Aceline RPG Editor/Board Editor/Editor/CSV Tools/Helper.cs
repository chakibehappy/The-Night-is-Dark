using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Aceline.RPG.Editor
{
    public static class Helper
    {
        // Find generic asset type on spesific folder, ex : on Resources
        public static List<T> FindAllObjectFromResources<T>()
        {
            List<T> tmp = new List<T>();
            string resourcesPath = Application.dataPath + "/Resources";
            string[] directories = Directory.GetDirectories(resourcesPath, "*", SearchOption.AllDirectories);

            foreach (string item in directories)
            {
                string directoryPath = item.Substring(resourcesPath.Length + 1);
                T[] results = Resources.LoadAll(directoryPath, typeof(T)).Cast<T>().ToArray();

                foreach (T result in results)
                {
                    if (!tmp.Contains(result))
                    {
                        tmp.Add(result);
                    }
                }
            }

            return tmp;
        }

        public static List<GameBoard> FindAllGameBoard()
        {
            // get the guid ID
            string[] guids = AssetDatabase.FindAssets("t:GameBoard");

            GameBoard[] items = new GameBoard[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                items[i] = AssetDatabase.LoadAssetAtPath<GameBoard>(path);
            }
            return items.ToList();
        }


        public static string GetTextByLanguage(List<Languages<string>> textList, Language language)
        {
            return textList.Find(text => text.Name == language).Value;
        }


        public static AudioClip GetAudioByLanguage(List<Languages<AudioClip>> audioList, Language language)
        {
            return audioList.Find(clip => clip.Name == language).Value;
        }


        public static void AssignTextOfLanguage(List<Languages<string>> textList, Language language, string newValue)
        {
            textList.Find(text => text.Name == language).Value = newValue;
        }

        public static void AssignAudioOfLanguage(List<Languages<AudioClip>> audioList, Language language, AudioClip newValue)
        {
            audioList.Find(clip => clip.Name == language).Value = newValue;
        }
    }
}

