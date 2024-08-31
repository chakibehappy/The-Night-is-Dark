using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class DialogueCard : BaseCard
    {
        private DialogueData dialogueData = new DialogueData();

        public DialogueData DialogueData { get => dialogueData; set => dialogueData = value; }

        private Sprite topArrowIcon, downArrowIcon;

        private List<Box> Boxs = new List<Box>();

        public DialogueCard() { }

        public DialogueCard(Vector2 position, BoardEditorWindow editorWindow, BoardGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/DialogueCardStyleSheet");
            styleSheets.Add(styleSheet);

            topArrowIcon = Resources.Load<Sprite>("Icon/top_arrow");
            downArrowIcon = Resources.Load<Sprite>("Icon/bottom_arrow");

            title = "Dialogue";
            SetPosition(new Rect(position, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Continue");
            TopContainer();
        }

        private void TopContainer()
        {
            AddDropdownMenu();
            AddPortButton();
        }

        private void AddPortButton()
        {
            Button btn = new Button() { text = "Add Choice" };
            btn.AddToClassList("ChoiceButton");

            btn.clicked += () => {  AddChoicePort(this); };
            titleButtonContainer.Add(btn);
        }

        private void AddDropdownMenu()
        {
            ToolbarMenu Menu = new ToolbarMenu();
            Menu.text = "Add Content";
            Menu.AddToClassList("MenuToolbarButton");

            Menu.menu.AppendAction("Text", new Action<DropdownMenuAction>(x => TextLine()));
            Menu.menu.AppendAction("Image", new Action<DropdownMenuAction>(x => ImagePic()));
            Menu.menu.AppendAction("Name", new Action<DropdownMenuAction>(x => CharacterName()));

            titleButtonContainer.Add(Menu);
        }

        public Port AddChoicePort(BaseCard baseCard, StatPort statPort = null)
        {
            Port port = GetPortInstance(Direction.Output);
            StatPort newPort = new StatPort();

            if (statPort != null)
            {
                newPort.InputGuid = statPort.InputGuid;
                newPort.OutputGuid = statPort.OutputGuid;
                newPort.PortGuid = statPort.PortGuid;
            }
            else
            {
                newPort.PortGuid = Guid.NewGuid().ToString();
            }

            Button deleteButton = new Button(() => DeletePort(baseCard, port)) { text = "X" };
            port.contentContainer.Add(deleteButton);

            port.portName = newPort.PortGuid; // We use portName as port ID
            
            // Get Label in port that is used to contain the port name.
            Label portNameLabel = port.contentContainer.Q<Label>("type");   
            portNameLabel.AddToClassList("PortName");                       

            port.portColor = Color.yellow;
            DialogueData.Ports.Add(newPort);

            baseCard.outputContainer.Add(port);

            baseCard.RefreshPorts();
            baseCard.RefreshExpandedState();

            return port;
        }

        private void DeletePort(BaseCard node, Port port)
        {
            StatPort tmp = DialogueData.Ports.Find(findPort => findPort.PortGuid == port.portName);
            DialogueData.Ports.Remove(tmp);

            IEnumerable<Edge> portEdge = graphView.edges.ToList().Where(edge => edge.output == port);

            if (portEdge.Any())
            {
                Edge edge = portEdge.First();
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                graphView.RemoveElement(edge);
            }

            node.outputContainer.Remove(port);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }


        public void TextLine(StatDialogueText texts = null)
        {
            StatDialogueText dialogueText = new StatDialogueText();
            DialogueData.Containers.Add(dialogueText);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("DialogueBox");

            AddLabelAndButton(dialogueText, boxContainer, "Dialogue " + (Boxs.Count + 1), "DialogueText");
            AddTextField(dialogueText, boxContainer);
            AddAudioClips(dialogueText, boxContainer);

            if (texts != null)
            {
                dialogueText.GuidID = texts.GuidID;

                foreach (Languages<string> item in texts.Texts)
                {
                    foreach (Languages<string> line in dialogueText.Texts)
                    {
                        if (line.Name == item.Name)
                            line.Value = item.Value;
                    }
                }

                foreach (Languages<AudioClip> data_audioclip in texts.AudioClips)
                {
                    foreach (Languages<AudioClip> audioclip in dialogueText.AudioClips)
                    {
                        if (audioclip.Name == data_audioclip.Name)
                            audioclip.Value = data_audioclip.Value;
                    }
                }
            }
            else
            {
                // Make New Guid ID, if its empty
                dialogueText.GuidID.Value = Guid.NewGuid().ToString();
            }

            ReloadLanguage();
            mainContainer.Add(boxContainer);
        }

        public void ImagePic(StatProfilePic data_Images = null)
        {
            StatProfilePic dialogue_Images = new StatProfilePic();
            if (data_Images != null)
            {
                dialogue_Images.LeftSprite.Value = data_Images.LeftSprite.Value;
                dialogue_Images.RightSprite.Value = data_Images.RightSprite.Value;
            }
            DialogueData.Containers.Add(dialogue_Images);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("DialogueBox");

            AddLabelAndButton(dialogue_Images, boxContainer, "Picture", "ImageColor");
            AddImages(dialogue_Images, boxContainer);

            mainContainer.Add(boxContainer);
        }

        public void CharacterName(StatName data_Name = null)
        {
            StatName dialogue_Name = new StatName();
            if (data_Name != null)
            {
                dialogue_Name.CharacterName.Value = data_Name.CharacterName.Value;
            }
            DialogueData.Containers.Add(dialogue_Name);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("CharacterNameBox");

            AddLabelAndButton(dialogue_Name, boxContainer, "Character Name", "NameColor");
            AddTextField_CharacterName(dialogue_Name, boxContainer);

            mainContainer.Add(boxContainer);
        }

        private void AddLabelAndButton(StatContainer container, Box boxContainer, string labelName, string uniqueUSS = "")
        {
            Box topBoxContainer = new Box();
            topBoxContainer.AddToClassList("TopBox");

            Label label_texts = GetNewLabel(labelName, "LabelText", uniqueUSS);

            Box arrowBox = new Box();
            Button topArrow = GetNewButton("", "ArrowButtonUp");
            topArrow.style.backgroundImage = topArrowIcon.texture;
            topArrow.clicked += () =>
            {
                MoveBox(container, true);
            };

            
            Button downArrow = GetNewButton("", "ArrowButtonDown");
            downArrow.name = "ArrowButtonDown" + Boxs.Count;
            downArrow.style.backgroundImage = downArrowIcon.texture;
            downArrow.clicked += () =>
            {
                MoveBox(container, false);
            };

            topArrow.style.unityBackgroundImageTintColor = Boxs.Count == 0 ? Color.black : Color.white;
            downArrow.style.unityBackgroundImageTintColor = Color.white;


            Button btn = GetNewButton("X", "TextBtn");
            btn.AddToClassList("DeleteButton");
            btn.clicked += () =>
            {
                DeleteBox(boxContainer);
                Boxs.Remove(boxContainer);
                DialogueData.Containers.Remove(container);
            };

            Boxs.Add(boxContainer);

            arrowBox.Add(topArrow);
            arrowBox.Add(downArrow);
            topBoxContainer.Add(arrowBox);
            topBoxContainer.Add(label_texts);
            topBoxContainer.Add(btn);

            boxContainer.Add(topBoxContainer);
            CheckDownArrowColors();
        }

        private void CheckDownArrowColors()
        {
            for (int i = 0; i < Boxs.Count; i++)
            {
                Button arrowButton = Boxs[i].Q<Button>("ArrowButtonDown" + i);
                arrowButton.style.unityBackgroundImageTintColor = i == Boxs.Count-1 ? Color.black : Color.white;
            }
        }

        private void MoveBox(StatContainer container, bool moveUp)
        {
            List<StatContainer> tempContainers = new List<StatContainer>();
            tempContainers.AddRange(dialogueData.Containers);

            foreach (Box item in Boxs)
            {
                mainContainer.Remove(item);
            }
            Boxs.Clear();

            for (int i = 0; i < tempContainers.Count; i++)
            {
                tempContainers[i].ID.Value = i;
            }

            if(container.ID.Value > 0 && moveUp)
            {
                StatContainer tmp01 = tempContainers[container.ID.Value];
                StatContainer tmp02 = tempContainers[container.ID.Value - 1];

                tempContainers[container.ID.Value] = tmp02;
                tempContainers[container.ID.Value - 1] = tmp01;
            }
            else if (container.ID.Value < tempContainers.Count - 1 && !moveUp)
            {
                StatContainer tmp01 = tempContainers[container.ID.Value];
                StatContainer tmp02 = tempContainers[container.ID.Value + 1];

                tempContainers[container.ID.Value] = tmp02;
                tempContainers[container.ID.Value + 1] = tmp01;
            }

            dialogueData.Containers.Clear();

            foreach (StatContainer item in tempContainers)
            {
                switch (item)
                {
                    case StatName Name:
                        CharacterName(Name);
                        break;
                    case StatProfilePic Picture:
                        ImagePic(Picture);
                        break;
                    case StatDialogueText Dialogue:
                        TextLine(Dialogue);
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddTextField_CharacterName(StatName container, Box boxContainer)
        {
            TextField textField = GetNewTextField(container.CharacterName, "Input Name...", "CharacterName");
            boxContainer.Add(textField);
        }

        private void AddTextField(StatDialogueText container, Box boxContainer)
        {
            TextField textField = GetNewLanguageTextField(container.Texts, "Input Text...", "TextBox");
            container.TextField = textField;
            boxContainer.Add(textField);
        }

        private void AddAudioClips(StatDialogueText container, Box boxContainer)
        {
            ObjectField objectField = GetNewLanguageAudioField(container.AudioClips, "AudioClip");

            container.ObjectField = objectField;

            boxContainer.Add(objectField);
        }
        private void AddImages(StatProfilePic container, Box boxContainer)
        {
            Box ImagePreviewBox = new Box();
            Box ImagesBox = new Box();

            ImagePreviewBox.AddToClassList("BoxRow");
            ImagesBox.AddToClassList("BoxRow");

            Image leftImage = GetNewImage("ImagePreview", "ImagePreviewLeft");
            Image rightImage = GetNewImage("ImagePreview", "ImagePreviewRight");

            ImagePreviewBox.Add(leftImage);
            ImagePreviewBox.Add(rightImage);

            ObjectField objectField_Left = GetNewSpriteField(container.LeftSprite, leftImage, "SpriteLeft");
            ObjectField objectField_Right = GetNewSpriteField(container.RightSprite, rightImage, "SpriteRight");

            ImagesBox.Add(objectField_Left);
            ImagesBox.Add(objectField_Right);

            boxContainer.Add(ImagePreviewBox);
            boxContainer.Add(ImagesBox);
        }

        public override void ReloadLanguage()
        {
            base.ReloadLanguage();
        }

        public override void LoadValueInToField()
        {

        }
    }
}