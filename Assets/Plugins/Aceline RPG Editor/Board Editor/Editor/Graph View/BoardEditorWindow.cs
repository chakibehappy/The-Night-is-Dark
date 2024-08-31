using UnityEngine;
using UnityEngine.UIElements;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG.Editor
{
    public class BoardEditorWindow : EditorWindow
    {
        private GameBoard currentGameBoard;
        private BoardGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        private Language selectedLanguage = Language.English;
        private ToolbarMenu languageDropDownMenu;
        private Label nameOfDialogueContainer;
        private string styleSheetName = "USS/BoardEditor/BoardEditorStyleSheet";

        public Language SelectedLanguage { get => selectedLanguage; set => selectedLanguage = value; }


        [OnOpenAsset(0)]
        public static bool ShowWindow(int instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceId);

            if (item is GameBoard)
            {
                BoardEditorWindow window = (BoardEditorWindow)GetWindow(typeof(BoardEditorWindow));
                window.titleContent = new GUIContent("Game Board Editor");
                window.currentGameBoard = item as GameBoard;
                window.minSize = new Vector2(500, 250);
                window.Load();
            }

            return false;
        }


        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            Load();
        }


        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }


        private void ConstructGraphView()
        {
            graphView = new BoardGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }


        #region Creating Toolbar
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(styleSheetName);
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            AddSaveAndLoadButton(toolbar);
            AddDropDownMenuLanguage(toolbar);
            AddDialogueContainerTitle(toolbar);


            rootVisualElement.Add(toolbar);
        }

        private void AddSaveAndLoadButton(Toolbar toolbar)
        {
            Button saveBtn = new Button() { text = "Save" };
            saveBtn.clicked += () => { Save(); };
            toolbar.Add(saveBtn);

            Button loadBtn = new Button() { text = "Load" };
            loadBtn.clicked += () => { Load(); };
            toolbar.Add(loadBtn);
        }

        private void AddDropDownMenuLanguage(Toolbar toolbar)
        {
            languageDropDownMenu = new ToolbarMenu();
            foreach (Language language in (Language[])Enum.GetValues(typeof(Language)))
            {
                languageDropDownMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(
                    x => SetLanguage(language))
                );
            }
            toolbar.Add(languageDropDownMenu);
        }

        private void AddDialogueContainerTitle(Toolbar toolbar)
        {
            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfBoard");
        }
        #endregion


        #region Save & Load
        private void Load()
        {
            if (currentGameBoard != null)
            {
                SetLanguage(Language.English);
                nameOfDialogueContainer.text = currentGameBoard.name;
                saveAndLoad.Load(currentGameBoard);
            }
        }

        private void Save()
        {
            if (currentGameBoard != null)
            {
                saveAndLoad.Save(currentGameBoard);
            }
        }
        #endregion


        private void SetLanguage(Language language)
        {
            languageDropDownMenu.text = "Language : " + language.ToString();
            selectedLanguage = language;
            graphView.ReloadLanguage();
        }

    }
}
