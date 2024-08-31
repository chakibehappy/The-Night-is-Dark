using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG.Editor
{
    public class BaseCard : Node
    {
        private string label;
        protected string cardGuid;
        protected BoardGraphView graphView;
        protected BoardEditorWindow editorWindow;
        protected Vector2 defaultCardSize = new Vector2(200, 250);

        public string CardGuid { get => cardGuid; set => cardGuid = value; }
        public string Label { get => label; set => label = value; }

        private List<LanguageText> languageTexts = new List<LanguageText>();
        private List<LanguageAudioClip> languageClips = new List<LanguageAudioClip>();

        public BaseCard()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/_CardStyleSheet");
            styleSheets.Add(styleSheet);
        }


        #region Methods
        public virtual void LoadValueInToField()
        {

        }

        public virtual void ReloadLanguage()
        {
            foreach (LanguageText item in languageTexts)
            {
                ReloadTextLanguage(item.inputText, item.textField, item.placeHolderText);
            }
            foreach (LanguageAudioClip item in languageClips)
            {
                ReloadAudioLanguage(item.inputClip, item.objectField);
            }
        }

        protected void ReloadTextLanguage(List<Languages<string>> inputText, TextField textField, string placeHolderText = "")
        {
            textField.RegisterValueChangedCallback(value =>
            {
                Helper.AssignTextOfLanguage(inputText, editorWindow.SelectedLanguage, value.newValue);
            });
            textField.SetValueWithoutNotify(Helper.GetTextByLanguage(inputText, editorWindow.SelectedLanguage));

            SetPlaceHolderText(textField, placeHolderText);
        }

        protected void SetPlaceHolderText(TextField textField, string placeHolderText)
        {
            string placeHolderClass = TextField.ussClassName + "__placeholder";

            CheckForText();
            onFocusOut();
            textField.RegisterCallback<FocusInEvent>(evt => onFocusIn());
            textField.RegisterCallback<FocusOutEvent>(evt => onFocusOut());

            void onFocusIn()
            {
                if (textField.ClassListContains(placeHolderClass))
                {
                    textField.value = string.Empty;
                    textField.RemoveFromClassList(placeHolderClass);
                }
            }

            void onFocusOut()
            {
                if (string.IsNullOrEmpty(textField.text))
                {
                    textField.SetValueWithoutNotify(placeHolderText);
                }

            }

            void CheckForText()
            {
                if (!string.IsNullOrEmpty(textField.text))
                {
                    textField.RemoveFromClassList(placeHolderClass);
                }
            }
        }

        protected void ReloadAudioLanguage(List<Languages<AudioClip>> inputClip, ObjectField objectField)
        {
            objectField.RegisterValueChangedCallback(value =>
            {
                Helper.AssignAudioOfLanguage(inputClip, editorWindow.SelectedLanguage, value.newValue as AudioClip);
            });
            objectField.SetValueWithoutNotify(Helper.GetAudioByLanguage(inputClip, editorWindow.SelectedLanguage));
        }

        protected void AddEventModifier(List<EventModifier> eventModifiers, EventModifier modifier = null)
        {
            EventModifier newModifier = new EventModifier();

            if(modifier != null)
            {
                newModifier.Text.Value = modifier.Text.Value;
                newModifier.Value.Value = modifier.Value.Value;
                newModifier.Modifier.Value = modifier.Modifier.Value;
            }

            eventModifiers.Add(newModifier);

            Box boxContainer = new Box();
            Box boxFloatField = new Box();
            boxContainer.AddToClassList("EventBox");
            boxContainer.AddToClassList("EventModifierFloatField");

            TextField textField = GetNewTextField(newModifier.Text, "Event Name", "EventModifierText");
            FloatField floatField = GetNewFloatField(newModifier.Value, "EventModifierFloat");
            
            Action action = () => ShowHideEventModifier(newModifier.Modifier.Value, boxFloatField);
            EnumField enumField = GetNewModifierEnumField(newModifier.Modifier, action, "EventModifierEnum");

            ShowHideEventModifier(newModifier.Modifier.Value, boxFloatField);

            Button btn = GetNewButton("X", "DeleteButton");
            btn.clicked += () => {
                eventModifiers.Remove(newModifier);
                DeleteBox(boxContainer);
            };

            boxContainer.Add(textField);
            boxContainer.Add(enumField);
            boxFloatField.Add(floatField);
            boxContainer.Add(boxFloatField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        protected void AddBranchCondition(List<BranchCondition> eventConditions, BranchCondition condition = null)
        {
            BranchCondition newCondition = new BranchCondition();

            if (condition != null)
            {
                newCondition.DataCardSet = condition.DataCardSet;
                newCondition.SelectedStat = condition.SelectedStat;
                newCondition.Value.Value = condition.Value.Value;
                newCondition.Condition.Value = condition.Condition.Value;
            }

            eventConditions.Add(newCondition);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("ConditionBox");


            ObjectField dataField = new ObjectField()
            {
                objectType = typeof(DataCardSet),
                allowSceneObjects = false,
                value = newCondition.DataCardSet
            };
            dataField.SetValueWithoutNotify(newCondition.DataCardSet);
            dataField.ElementAt(0).ElementAt(0).style.marginTop = 3;
            dataField.AddToClassList("DataField");

            ToolbarMenu menuDataType = new ToolbarMenu();
            menuDataType.AddToClassList("DataTypeOption");

            ToolbarMenu menuCondition = new ToolbarMenu();
            menuCondition.AddToClassList("DataTypeOption");

            Box boxFloatField = new Box();
            FloatField inputFloat = new FloatField("");
            inputFloat.RegisterValueChangedCallback(value => { newCondition.SelectedStat.FloatValue = value.newValue; });
            inputFloat.SetValueWithoutNotify(newCondition.SelectedStat == null ? 0 : newCondition.SelectedStat.FloatValue);
            inputFloat.AddToClassList("InputNumber");
            boxFloatField.Add(inputFloat);

            Box boxIntField = new Box();
            IntegerField inputInt = new IntegerField("");
            inputInt.RegisterValueChangedCallback(value => { newCondition.SelectedStat.IntValue = value.newValue; });
            inputInt.SetValueWithoutNotify(newCondition.SelectedStat == null ? 0 : newCondition.SelectedStat.IntValue);
            inputInt.AddToClassList("InputNumber");
            boxIntField.Add(inputInt);

            Box boxStringField = new Box();
            TextField inputString = new TextField("");
            inputString.RegisterValueChangedCallback(value => { newCondition.SelectedStat.StringValue = value.newValue; });
            inputString.SetValueWithoutNotify(newCondition.SelectedStat == null ? "" : newCondition.SelectedStat.StringValue);
            inputString.AddToClassList("InputText");
            boxStringField.Add(inputString);

            Box boxInputDataField = new Box();
            ObjectField inputData = new ObjectField()
            {
                objectType = typeof(DataCardSet),
                allowSceneObjects = false,
                value = newCondition.DataCardSet
            };
            inputData.SetValueWithoutNotify(newCondition.SelectedStat == null ? null : newCondition.SelectedStat.DataValue);
            inputData.ElementAt(0).ElementAt(0).style.marginTop = 3;
            inputData.AddToClassList("DataField");
            boxInputDataField.Add(inputData);

            List<Box> AllInputBox = new List<Box>
            {
                boxInputDataField,
                boxIntField,
                boxFloatField,
                boxStringField,
            };

            AddDataTypeCollections(menuDataType, menuCondition, newCondition, AllInputBox);
            AddConditionSelection(menuCondition, newCondition);
            CheckDataType(newCondition, AllInputBox);

            dataField.RegisterValueChangedCallback(value =>
            {
                newCondition.DataCardSet = value.newValue as DataCardSet;
                AddDataTypeCollections(menuDataType, menuCondition, newCondition, AllInputBox);
                AddConditionSelection(menuCondition, newCondition);
            });
            dataField.SetValueWithoutNotify(newCondition.DataCardSet);


            Button btn = GetNewButton("X", "DeleteButton");
            btn.clicked += () => {
                eventConditions.Remove(newCondition);
                DeleteBox(boxContainer);
            };


            boxContainer.Add(dataField);
            boxContainer.Add(menuDataType);
            boxContainer.Add(boxInputDataField);
            boxContainer.Add(menuCondition);

            boxContainer.Add(boxIntField);
            boxContainer.Add(boxFloatField);
            boxContainer.Add(boxStringField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

        void AddDataTypeCollections(ToolbarMenu menu, ToolbarMenu menuCondition, BranchCondition condition, List<Box> boxs)
        {
            if (condition.DataCardSet != null)
            {
                menu.style.display = DisplayStyle.Flex;
                if (condition.DataCardSet.Stats.Count > 0)
                {
                    menu.text = condition.SelectedStat == null ? condition.DataCardSet.Stats[0].Name : condition.SelectedStat.Name;

                    foreach (Stat item in condition.DataCardSet.Stats)
                    {
                        menu.menu.AppendAction(item.Name, new Action<DropdownMenuAction>(x => {
                            menu.text = item.Name;
                            condition.SelectedStat = item;
                            AddConditionSelection(menuCondition, condition);
                            CheckDataType(condition, boxs);
                        }));
                    }
                }

                if(condition.SelectedStat == null)
                {
                    condition.SelectedStat = condition.DataCardSet.Stats[0];
                }
            }
            else
            {
                menu.menu.MenuItems().Clear();
                menu.text = "";
                menu.style.display = DisplayStyle.None;
                condition.SelectedStat = null;
            }
        }

        void AddConditionSelection(ToolbarMenu menu, BranchCondition condition)
        {
            if (condition.SelectedStat != null)
            {
                menu.style.display = DisplayStyle.Flex;
                menu.menu.MenuItems().Clear();

                string[] dataSet;
                string initialValue = "";

                if (condition.SelectedStat.Type == ValueType.Bool)
                {
                    dataSet = condition.DataCardSet.BoolCondition;
                    if (condition.SelectedStat.BoolValue == true)
                    {
                        initialValue = dataSet[0];
                    }
                    else
                    {
                        initialValue = dataSet[1];
                    }
                }
                else if (condition.SelectedStat.Type == ValueType.Int || condition.SelectedStat.Type == ValueType.Float)
                {
                    dataSet = condition.DataCardSet.IntCondition;
                    initialValue = dataSet[0];
                }
                else if (condition.SelectedStat.Type == ValueType.String)
                {
                    dataSet = condition.DataCardSet.StringCondition;
                    initialValue = dataSet[0];
                }
                else
                {
                    dataSet = condition.DataCardSet.AllCondition;
                    initialValue = dataSet[0];
                }
                
                menu.text = initialValue;
                
                foreach (string item in dataSet)
                {
                    menu.menu.AppendAction(item, new Action<DropdownMenuAction>(x => 
                    {
                        if (item == "True")
                            condition.SelectedStat.BoolValue = true;
                        else
                            condition.SelectedStat.BoolValue = false;

                        menu.text = item;
                    }));
                }
            }
            else
            {
                menu.menu.MenuItems().Clear();
                menu.text = "";
                menu.style.display = DisplayStyle.None;
            }
        }

        protected void CheckDataType(BranchCondition condition, List<Box> boxs)
        {
            if(condition.SelectedStat != null)
            {
                if (condition.SelectedStat.Type == ValueType.Bool)
                {
                    ShowInputBox(boxs, -1);
                }
                else if (condition.SelectedStat.Type == ValueType.GameData)
                {
                    ShowInputBox(boxs, 0);
                }
                else if (condition.SelectedStat.Type == ValueType.Int)
                {
                    ShowInputBox(boxs, 1);
                }
                else if (condition.SelectedStat.Type == ValueType.Float)
                {
                    ShowInputBox(boxs, 2);
                }
                else if (condition.SelectedStat.Type == ValueType.String)
                {
                    ShowInputBox(boxs, 3);
                }
            }
            else
            {
                ShowInputBox(boxs, -1);
            }
        }

        protected void ShowInputBox(List<Box> boxs, int index)
        {
            for (int i = 0; i < boxs.Count; i++)
            {
                boxs[i].style.display = DisplayStyle.None;
            }

            if (index >= 0)
            {
                boxs[index].style.display = DisplayStyle.Flex;
                if(index == 0)
                {
                    boxs[1].style.display = DisplayStyle.Flex;
                }
            }
        }


        private void ShowHideEventCondition(StatConditionType value, Box boxContainer)
        {
            if (value == StatConditionType.True || value == StatConditionType.False)
            {
                ShowHide(false, boxContainer);
            }
            else
            {
                ShowHide(true, boxContainer);
            }
        }

        private void ShowHideEventModifier(StatModifierType value, Box boxContainer)
        {
            if(value == StatModifierType.SetTrue || value == StatModifierType.SetFalse)
            {
                ShowHide(false, boxContainer);
            }
            else
            {
                ShowHide(true, boxContainer);
            }
        }

        public void ShowHide(bool show, Box boxContainer)
        {
            string hideUssClass = "Hide";
            if (show == true)
            {
                boxContainer.RemoveFromClassList(hideUssClass);
            }
            else
            {
                boxContainer.AddToClassList(hideUssClass);
            }
        }

        protected virtual void DeleteBox(Box boxContainer)
        {
            mainContainer.Remove(boxContainer);
            RefreshExpandedState();
        }

        #endregion


        #region Get new Field

        protected Label GetNewLabel(string labelName, string styleSheet1 = "", string styleSheet2 = "")
        {
            Label label = new Label(labelName);
            label.AddToClassList(styleSheet1);
            label.AddToClassList(styleSheet2);
            return label;
        }

        protected Button GetNewButton(string buttonName, string styleSheet1 = "", string styleSheet2 = "")
        {
            Button button = new Button() { text = buttonName };
            button.AddToClassList(styleSheet1);
            button.AddToClassList(styleSheet2);
            return button;
        }

        protected IntegerField GetNewIntField(IntStat input, string styleSheet1 = "", string styleSheet2 = "")
        {
            IntegerField field= new IntegerField();
            field.RegisterValueChangedCallback(value => { input.Value = value.newValue; });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            return field;
        }

        protected FloatField GetNewFloatField(FloatStat input, string styleSheet1 = "", string styleSheet2 = "")
        {
            FloatField field = new FloatField();
            field.RegisterValueChangedCallback(value => { input.Value = value.newValue; });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            return field;
        }

        protected TextField GetNewTextField(TextStat input, string placeHolder, string styleSheet1 = "", string styleSheet2 = "")
        {
            TextField field = new TextField();
            field.RegisterValueChangedCallback(value => { input.Value = value.newValue; });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            SetPlaceHolderText(field, placeHolder);
            return field;
        }

        protected Image GetNewImage(string styleSheet1 = "", string styleSheet2 = "")
        {
            Image image = new Image();
            image.AddToClassList(styleSheet1);
            image.AddToClassList(styleSheet2);
            return image;
        }

        protected ObjectField GetNewSpriteField(ImageStat input, Image imagePreview, string styleSheet1 = "", string styleSheet2 = "")
        {
            ObjectField field = new ObjectField()
            {
                objectType = typeof(Sprite),
                allowSceneObjects = false,
                value = input.Value
            };
            field.RegisterValueChangedCallback(value =>
            {
                input.Value = value.newValue as Sprite;
                imagePreview.image = input.Value != null ? input.Value.texture : null;
            });
            imagePreview.image = input.Value != null ? input.Value.texture : null;
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            return field;
        }

        protected ObjectField GetNewCardSetField(EventCardStat input, string styleSheet1 = "", string styleSheet2 = "")
        {
            ObjectField field = new ObjectField()
            {
                objectType = typeof(EventCardSet),
                allowSceneObjects = false,
                value = input.CardEvent
            };
            field.RegisterValueChangedCallback(value =>
            {
                input.CardEvent = value.newValue as EventCardSet;
            });
            field.SetValueWithoutNotify(input.CardEvent);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            return field;
        }

        protected EnumField GetNewChoiceEnumField(ChoiceStat input, string styleSheet1 = "", string styleSheet2 = "")
        {
            EnumField field = new EnumField()
            {
                value = input.Value
            };
            field.Init(input.Value);
            field.RegisterValueChangedCallback(value =>
            {
                input.Value = (ChoiceStateType)value.newValue;
            });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            input.EnumField = field;
            return field;
        }

        protected EnumField GetNewEndTypeEnumField(EndStat input, string styleSheet1 = "", string styleSheet2 = "")
        {
            EnumField field = new EnumField()
            {
                value = input.Value
            };
            field.Init(input.Value);
            field.RegisterValueChangedCallback(value =>
            {
                input.Value = (EndCardType)value.newValue;
            });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            input.EnumField = field;
            return field;
        }

        protected EnumField GetNewModifierEnumField(ModifierStat input, Action action, string styleSheet1 = "", string styleSheet2 = "")
        {
            EnumField field = new EnumField()
            {
                value = input.Value
            };
            field.Init(input.Value);
            field.RegisterValueChangedCallback(value =>
            {
                input.Value = (StatModifierType)value.newValue;
                action?.Invoke();
            });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            input.EnumField = field;
            return field;
        }

        protected EnumField GetNewConditionEnumField(ConditionType input, Action action, string styleSheet1 = "", string styleSheet2 = "")
        {
            EnumField field = new EnumField()
            {
                value = input.Value
            };
            field.Init(input.Value);
            field.RegisterValueChangedCallback(value =>
            {
                input.Value = (StatConditionType)value.newValue;
                action?.Invoke();
            });
            field.SetValueWithoutNotify(input.Value);
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);
            input.EnumField = field;
            return field;
        }

        protected TextField GetNewLanguageTextField(List<Languages<string>> Text, string placeHolder, string styleSheet1 = "", string styleSheet2 = "")
        {
            foreach (Language item in (Language[])Enum.GetValues(typeof(Language)))
            {
                Text.Add(new Languages<string>
                {
                    Name = item,
                    Value = ""
                });
            }
            TextField field = new TextField("");
            languageTexts.Add(new LanguageText(Text, field, placeHolder));

            field.RegisterValueChangedCallback(value => {
                Helper.AssignTextOfLanguage(Text, editorWindow.SelectedLanguage, value.newValue);
            });
            field.SetValueWithoutNotify(Helper.GetTextByLanguage(Text, editorWindow.SelectedLanguage));
            field.multiline = true;
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);

            return field;
        }

        protected ObjectField GetNewLanguageAudioField(List<Languages<AudioClip>> Clips, string styleSheet1 = "", string styleSheet2 = "")
        {
            foreach (Language item in (Language[])Enum.GetValues(typeof(Language)))
            {
                Clips.Add(new Languages<AudioClip>
                {
                    Name = item,
                    Value = null
                });
            }
            ObjectField field = new ObjectField() 
            { 
                objectType = typeof(AudioClip),
                allowSceneObjects = false,
                value = Helper.GetAudioByLanguage(Clips, editorWindow.SelectedLanguage)
            };
            languageClips.Add(new LanguageAudioClip(Clips, field));

            field.RegisterValueChangedCallback(value => {
                Helper.AssignAudioOfLanguage(Clips, editorWindow.SelectedLanguage, value.newValue as AudioClip);
            });
            field.SetValueWithoutNotify(Helper.GetAudioByLanguage(Clips, editorWindow.SelectedLanguage));
            field.AddToClassList(styleSheet1);
            field.AddToClassList(styleSheet2);

            return field;
        }

        #endregion


        #region Adding Port
        public Port AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capacity);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
            return outputPort;
        }


        public Port AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi)
        {
            Port inputPort = GetPortInstance(Direction.Input, capacity);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
            return inputPort;
        }


        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }

        #endregion


        #region Changing Language Class
        class LanguageText
        {
            public LanguageText(List<Languages<string>> inputText, TextField textField, string placeHolderText = "")
            {
                this.inputText = inputText;
                this.textField = textField;
                this.placeHolderText = placeHolderText;
            }
            public List<Languages<string>> inputText;
            public TextField textField;
            public string placeHolderText;
        }

        class LanguageAudioClip
        {
            public LanguageAudioClip(List<Languages<AudioClip>> inputClip, ObjectField objectField)
            {
                this.inputClip = inputClip;
                this.objectField = objectField;
            }
            public List<Languages<AudioClip>> inputClip;
            public ObjectField objectField;
        }
        #endregion
    }
}
