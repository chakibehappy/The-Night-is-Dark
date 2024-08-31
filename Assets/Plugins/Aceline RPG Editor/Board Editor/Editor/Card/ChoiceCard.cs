using UnityEngine;
using System;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG.Editor
{
    public class ChoiceCard : BaseCard
    {
        private ChoiceData choiceData = new ChoiceData();
        
        public ChoiceData ChoiceData { get => choiceData; set => choiceData = value; }

        private Box choiceStateEnumBox;

        public ChoiceCard() { }

        public ChoiceCard(Vector2 pos, BoardEditorWindow editorWindow, BoardGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/ChoiceCardStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Choice";
            SetPosition(new Rect(pos, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            Port inputPort = AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            inputPort.portColor = Color.yellow;

            AddTopMenuButton();
            AddTextBox();
            AddChoiceStateEnum();
        }

        private void AddTopMenuButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Condition";

            menu.menu.AppendAction("Condition", new Action<DropdownMenuAction>(x => AddCondition()));
            menu.AddToClassList("branchTopMenuButton");

            titleButtonContainer.Add(menu);
        }

        public void AddCondition(BranchCondition condition = null)
        {
            AddBranchCondition(ChoiceData.Conditions, condition);
            ShowHideChoiceEnum();
        }

        private void AddTextBox()
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("ChoiceTextBox");

            TextField textField = GetNewLanguageTextField(ChoiceData.Texts, "Text", "TextBox");
            ChoiceData.TextField = textField;
            boxContainer.Add(textField);

            ObjectField audioField = GetNewLanguageAudioField(ChoiceData.VoiceOvers, "AudioClip");
            ChoiceData.ObjectField = audioField;
            boxContainer.Add(audioField);

            ReloadLanguage();
            mainContainer.Add(boxContainer);
        }

        private void AddChoiceStateEnum()
        {
            choiceStateEnumBox = new Box();
            choiceStateEnumBox.AddToClassList("BoxRow");
            ShowHideChoiceEnum();

            Label enumLabel = GetNewLabel("If the condition is not met", "ChoiceLabel");
            EnumField choicesStateEnumField = GetNewChoiceEnumField(ChoiceData.Choice, "enumHide");

            choiceStateEnumBox.Add(choicesStateEnumField);
            choiceStateEnumBox.Add(enumLabel);
            mainContainer.Add(choiceStateEnumBox);
        }

        protected override void DeleteBox(Box boxContainer)
        {
            base.DeleteBox(boxContainer);
            ShowHideChoiceEnum();
        }

        private void ShowHideChoiceEnum()
        {
            ShowHide(ChoiceData.Conditions.Count > 0, choiceStateEnumBox);
        }

        public override void ReloadLanguage()
        {
            base.ReloadLanguage();
        }

        public override void LoadValueInToField()
        {
            if (ChoiceData.Choice.EnumField != null)
                ChoiceData.Choice.EnumField.SetValueWithoutNotify(ChoiceData.Choice.Value);
        }
    }
}
