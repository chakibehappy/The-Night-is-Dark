using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Aceline.RPG
{
    [ScriptedImporter(1, "aceline")]
    public class AcelineScript : ScriptedImporter
    {
        GameBoard asset;
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // check and register extension if not exist :
            List<string> defaultExtensionList = EditorSettings.projectGenerationUserExtensions.ToList();
            if (!defaultExtensionList.Contains("aceline"))
            {
                string[] ext = new string[defaultExtensionList.Count + 1];
                ext[defaultExtensionList.Count] = "aceline";
                for (int i = 0; i < defaultExtensionList.Count; i++)
                {
                    ext[i] = defaultExtensionList[i];
                }
                EditorSettings.projectGenerationUserExtensions = ext;
            }

            string filePath = ctx.assetPath.Replace(".aceline", ".asset");
            string[] lines = File.ReadAllLines(ctx.assetPath);

            if (File.Exists(filePath))
            {
                // reload scriptable object :
                asset = AssetDatabase.LoadAssetAtPath<GameBoard>(filePath);
                asset.GetDataFromScript(lines);
                //AssetDatabase.SaveAssets();
            }
            else
            {
                // saving onto new scriptable object :
                asset = ScriptableObject.CreateInstance<GameBoard>();
                asset.GetDataFromScript(lines);
                AssetDatabase.CreateAsset(asset, filePath);
                //AssetDatabase.SaveAssets();
            }

            //AssetDatabase.Refresh();
            EditorUtility.SetDirty(asset);

            // not only adding icon, but will combine scriptable object onto script! :
            Texture2D icon = Resources.Load<Texture2D>("Icon/AcelineScript");
            ctx.AddObjectToAsset(filePath, asset, icon);
            ctx.SetMainObject(asset);
            
        }

    }
}
