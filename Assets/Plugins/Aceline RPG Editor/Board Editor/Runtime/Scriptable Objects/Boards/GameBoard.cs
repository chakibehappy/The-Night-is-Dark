using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Aceline.RPG
{
    [CreateAssetMenu(menuName = "AcelineRPG/Board/New Board")]
    [System.Serializable]
    public class GameBoard : ScriptableObject
    {
        public List<CardLinkData> CardLinkDatas = new List<CardLinkData>();

        public List<StartData> StartDatas = new List<StartData>();
        public List<DialogueData> DialogueDatas = new List<DialogueData>();
        public List<ChoiceData> ChoiceDatas = new List<ChoiceData>();
        public List<BranchData> BranchDatas = new List<BranchData>();
        public List<EventData> EventDatas = new List<EventData>();
        public List<EndData> EndDatas = new List<EndData>();

        public List<BaseData> AllDatas
        {
            get
            {
                List<BaseData> tmp = new List<BaseData>();
                tmp.AddRange(StartDatas);
                tmp.AddRange(DialogueDatas);
                tmp.AddRange(ChoiceDatas);
                tmp.AddRange(BranchDatas);
                tmp.AddRange(EventDatas);
                tmp.AddRange(EndDatas);

                return tmp;
            }
        }


        #region Importer Handler Variable
        private Vector2 startPos = new Vector2(100, 350);
        #endregion


        #region Importer Handler Method
        public void GetDataFromScript(string[] lines)
        {
            // Clear all current Data;
            CardLinkDatas = new List<CardLinkData>();
            StartDatas = new List<StartData>();
            DialogueDatas = new List<DialogueData>();
            ChoiceDatas = new List<ChoiceData>();
            BranchDatas = new List<BranchData>();
            EventDatas = new List<EventData>();
            EndDatas = new List<EndData>();


            // create start data :
            string startCardGuid = Guid.NewGuid().ToString();
            StartData startData = new StartData
            {
                CardGuid = startCardGuid,
                Position = startPos
            };
            StartDatas.Add(startData);

            // Get dialogue from script :
            List<StatContainer> DialogueStatContainer = new List<StatContainer>();
            List<StatDialogueText> DialogueTexts = new List<StatDialogueText>();
            List<StatName> DialogueNames = new List<StatName>();
            List<StatProfilePic> DialoguePics = new List<StatProfilePic>();
            List<StatPort> DialoguePorts = new List<StatPort>();

            // variable to detect choice is begin, end, or being connected to another dialogue :
            int choiceCount = 0;
            int currentChoiceCount = 0;
            List<ChoiceData> currentChoicesGroup = new List<ChoiceData>();
            List<int> currentChoicesIndex = new List<int>();

            bool registerChoice = false;
            bool isAddingChoice = false;
            bool isConnectingChoice = false;
            List<int> choiceConnection = new List<int>();

            string startChoiceCommand = "--open choice";
            string endChoiceCommand = "--close choice";
            string continueChoiceCommand = "--continue choice";

            int horizontalCount = 1;
            int verticalCount = 0;
            string currentCardGuid = startCardGuid;
            string currentDialogueGuid = startCardGuid;
            DialogueData currentDialogueData;

            int dialogueindex = 0;

            EndData unconnectedEndData = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string lineDialogue = lines[i].Trim();

                if (lineDialogue.Contains("#"))
                    continue;

                if (lineDialogue.Contains(startChoiceCommand))
                {
                    registerChoice = isAddingChoice = true;
                    verticalCount = 0;
                    continue;
                }

                if (lineDialogue.Contains(endChoiceCommand))
                {
                    verticalCount = 0;
                    isAddingChoice = false;
                    continue;
                }

                if (lineDialogue.Contains(continueChoiceCommand))
                {
                    isConnectingChoice = true;
                    string[] splitText = lines[i].Split(new string[] { continueChoiceCommand }, StringSplitOptions.None);
                    choiceConnection.Add(int.Parse(splitText[1].Trim()));
                    continue;
                }

                if (lineDialogue.Contains("--goto"))
                {
                    string endDataGuid = Guid.NewGuid().ToString();
                    string[] splitText = lineDialogue.Split(new string[] { "goto" }, StringSplitOptions.None);

                    EndStat endType = new EndStat();

                    if (splitText[1].Trim() == "back")
                        endType.Value = EndCardType.GoBack;
                    else if (splitText[1].Trim() == "start")
                        endType.Value = EndCardType.ReturnToStart;
                    else
                        endType.Value = EndCardType.End;

                    EndData endData = new EndData
                    {
                        CardGuid = endDataGuid,
                        Position = new Vector2(startPos.x + ((horizontalCount + 1) * 350), -    100 + startPos.y + (verticalCount * 300)),
                        EndState = endType
                    };
                    EndDatas.Add(endData);
                    unconnectedEndData = endData;
                    continue;
                }


                if (!registerChoice)
                {
                    // connecting existing choices check
                    if (isConnectingChoice)
                    {
                        if (choiceConnection.Count > 1) // find another continue connection of choice
                        {
                            // mean this current line is the end of dialogue
                            if(DialogueTexts.Count > 0)
                            {
                                currentDialogueGuid = Guid.NewGuid().ToString();
                                currentDialogueData = new DialogueData
                                {
                                    CardGuid = currentDialogueGuid,
                                    Position = new Vector2(startPos.x + (horizontalCount * 350), startPos.y + (verticalCount * 300)),
                                    Containers = DialogueStatContainer,
                                    Names = DialogueNames,
                                    Dialogues = DialogueTexts,
                                    Images = DialoguePics,
                                    Ports = DialoguePorts
                                };
                                DialogueDatas.Add(currentDialogueData);
                                verticalCount++;
                                horizontalCount++;
                            }
                            

                            int selectedIndex = currentChoicesIndex.IndexOf(choiceConnection[0]);
                            
                            CardLinkData link = new CardLinkData
                            {
                                BaseCardGuid = currentChoicesGroup[selectedIndex].CardGuid,
                                BasePortName = "Output",
                                TargetCardGuid = currentDialogueGuid,
                                TargetPortName = "Input"
                            };

                            choiceConnection.RemoveAt(0);
                            CardLinkDatas.Add(link);

                            currentCardGuid = currentDialogueGuid;

                            // if there's end data havent been connected
                            if (unconnectedEndData != null)
                            {
                                link = new CardLinkData
                                {
                                    BaseCardGuid = currentDialogueGuid,
                                    BasePortName = "Continue",
                                    TargetCardGuid = unconnectedEndData.CardGuid,
                                    TargetPortName = "Input"
                                };
                                unconnectedEndData = null;
                                CardLinkDatas.Add(link);
                            }

                            DialogueStatContainer = new List<StatContainer>();
                            DialogueTexts = new List<StatDialogueText>();
                            DialogueNames = new List<StatName>();
                            DialoguePics = new List<StatProfilePic>();
                            DialoguePorts = new List<StatPort>();

                        }
                    }


                    bool havingName = false;
                    string charName = "";

                    // checking right image :
                    if (lines[i].Contains("img-l") || lines[i].Contains("img-r"))
                    {
                        ImageStat leftImage = new ImageStat { Value = null };
                        ImageStat rightImage = new ImageStat { Value = null };
                        if (lines[i].Contains("img-l"))
                        {
                            string[] splitText = lines[i].Split(new string[] { "img-l." }, StringSplitOptions.None);
                            leftImage.Value = Resources.Load<Sprite>("Characters/" + splitText[1]);
                        }

                        if (lines[i].Contains("img-r"))
                        {
                            string[] splitText = lines[i].Split(new string[] { "img-r." }, StringSplitOptions.None);
                            rightImage.Value = Resources.Load<Sprite>("Characters/" + splitText[1]);

                        }
                        StatProfilePic newDialoguePic = new StatProfilePic
                        {
                            LeftSprite = leftImage,
                            RightSprite = rightImage,
                            ID = new IntStat { Value = dialogueindex },
                        };
                        DialoguePics.Add(newDialoguePic);
                        DialogueStatContainer.Add(newDialoguePic);
                        dialogueindex++;
                        // go to next line
                        continue;
                    }


                    if (lines[i].Contains(":"))
                    {
                        havingName = true;
                        string[] splitText = lines[i].Split(':');
                        charName = splitText[0].Trim();
                        lineDialogue = splitText[1].Trim();
                    }


                    if (havingName)
                    {
                        StatName name = new StatName
                        {
                            ID = new IntStat { Value = dialogueindex },
                            CharacterName = new TextStat { Value = charName }
                        };
                        DialogueNames.Add(name);
                        DialogueStatContainer.Add(name);
                        dialogueindex++;
                    }

                    if (!string.IsNullOrEmpty(lineDialogue))
                    {
                        List<Languages<string>> languageTexts = new List<Languages<string>>();
                        foreach (Language languageType in Enum.GetValues(typeof(Language)))
                        {
                            Languages<string> txtLanguage = new Languages<string>
                            {
                                Name = languageType,
                                Value = lineDialogue
                            };
                            languageTexts.Add(txtLanguage);
                        }


                        StatDialogueText text = new StatDialogueText
                        {
                            ID = new IntStat { Value = dialogueindex },
                            GuidID = new TextStat { Value = Guid.NewGuid().ToString() },
                            Texts = languageTexts,
                            AudioClips = null
                        };
                        DialogueTexts.Add(text);
                        DialogueStatContainer.Add(text);
                        dialogueindex++;
                    }
                }
                else
                {
                    // start register choice of current dialogue,
                    if (isAddingChoice)
                    {
                        string choiceText = "";
                        int choiceIndex = 0;
                        if (lines[i].Contains("--"))
                        {
                            string[] splitText = lines[i].Split(new string[] { "--" }, StringSplitOptions.None);
                            choiceIndex = int.Parse(splitText[0].Trim());
                            choiceText = splitText[1].Trim();
                        }

                        if (!string.IsNullOrEmpty(choiceText))
                        {
                            List<Languages<string>> languageTexts = new List<Languages<string>>();
                            foreach (Language languageType in Enum.GetValues(typeof(Language)))
                            {
                                Languages<string> txtLanguage = new Languages<string>
                                {
                                    Name = languageType,
                                    Value = choiceText
                                };
                                languageTexts.Add(txtLanguage);
                            }


                            string choiceGuid = Guid.NewGuid().ToString();
                            ChoiceData choiceData = new ChoiceData
                            {
                                CardGuid = choiceGuid,
                                Position = new Vector2(startPos.x + ((horizontalCount + 1) * 350), startPos.y + (verticalCount * 150)),
                                Texts = languageTexts
                            };

                            currentChoicesGroup.Add(choiceData);
                            currentChoicesIndex.Add(choiceIndex);
                            ChoiceDatas.Add(choiceData);

                            verticalCount++;
                            choiceCount++;
                        }

                    }
                    else
                    {
                        // mean that dialogue is stop here and continue to another dialogue

                        // add output port
                        for (int j = 0; j < choiceCount - currentChoiceCount; j++)
                        {
                            string portGuid = Guid.NewGuid().ToString();
                            StatPort newStatPort = new StatPort
                            {
                                PortGuid = portGuid,
                            };
                            DialoguePorts.Add(newStatPort);
                        }

                        currentDialogueGuid = Guid.NewGuid().ToString();
                        currentDialogueData = new DialogueData
                        {
                            CardGuid = currentDialogueGuid,
                            Position = new Vector2(startPos.x + (horizontalCount * 350), startPos.y),
                            Containers = DialogueStatContainer,
                            Names = DialogueNames,
                            Dialogues = DialogueTexts,
                            Images = DialoguePics,
                            Ports = DialoguePorts
                        };

                        for (int j = currentChoiceCount; j < choiceCount; j++)
                        {
                            DialoguePorts[j - currentChoiceCount].OutputGuid = currentDialogueGuid;
                            DialoguePorts[j - currentChoiceCount].InputGuid = currentChoicesGroup[j].CardGuid;

                            CardLinkData portlink = new CardLinkData
                            {
                                BaseCardGuid = currentDialogueGuid,
                                BasePortName = DialoguePorts[j - currentChoiceCount].PortGuid,
                                TargetCardGuid = currentChoicesGroup[j].CardGuid,
                                TargetPortName = "Input"
                            };
                            CardLinkDatas.Add(portlink);
                        }
                        currentChoiceCount = choiceCount;

                        DialogueDatas.Add(currentDialogueData);

                        CardLinkData link = new CardLinkData
                        {
                            BaseCardGuid = currentCardGuid,
                            BasePortName = "Output",
                            TargetCardGuid = currentDialogueGuid,
                            TargetPortName = "Input"
                        };
                        CardLinkDatas.Add(link);

                        currentCardGuid = currentDialogueGuid;

                        DialogueStatContainer = new List<StatContainer>();
                        DialogueTexts = new List<StatDialogueText>();
                        DialogueNames = new List<StatName>();
                        DialoguePics = new List<StatProfilePic>();
                        DialoguePorts = new List<StatPort>();


                        horizontalCount += 2;

                        registerChoice = false;
                    }
                }
            }

            bool isSingleDialogue = DialogueDatas.Count == 0;

            if (choiceConnection.Count > 0 || isSingleDialogue) // check if there's choice connection havent been connected or just single dialogue
            {
                // mean this current line is the end of dialogue
                currentDialogueGuid = Guid.NewGuid().ToString();
                currentDialogueData = new DialogueData
                {
                    CardGuid = currentDialogueGuid,
                    Position = new Vector2(startPos.x + (horizontalCount * 350), startPos.y + (verticalCount * 300)),
                    Containers = DialogueStatContainer,
                    Names = DialogueNames,
                    Dialogues = DialogueTexts,
                    Images = DialoguePics,
                    Ports = DialoguePorts
                };
                verticalCount++;
                horizontalCount++;

                if(choiceConnection.Count > 0)
                {
                    int selectedIndex = currentChoicesIndex.IndexOf(choiceConnection[0]);

                    CardLinkData link = new CardLinkData
                    {
                        BaseCardGuid = currentChoicesGroup[selectedIndex].CardGuid,
                        BasePortName = "Output",
                        TargetCardGuid = currentDialogueGuid,
                        TargetPortName = "Input"
                    };

                    choiceConnection.RemoveAt(0);
                    CardLinkDatas.Add(link);
                }

                if (isSingleDialogue)
                {
                    CardLinkData link = new CardLinkData
                    {
                        BaseCardGuid = startCardGuid,
                        BasePortName = "Output",
                        TargetCardGuid = currentDialogueGuid,
                        TargetPortName = "Input"
                    };
                    CardLinkDatas.Add(link);
                }

                DialogueDatas.Add(currentDialogueData);

                // if there's end data havent been connected
                if (unconnectedEndData != null)
                {
                    CardLinkData link = new CardLinkData
                    {
                        BaseCardGuid = currentDialogueGuid,
                        BasePortName = "Continue",
                        TargetCardGuid = unconnectedEndData.CardGuid,
                        TargetPortName = "Input"
                    };
                    unconnectedEndData = null;
                    CardLinkDatas.Add(link);
                }

                currentCardGuid = currentDialogueGuid;

                DialogueStatContainer = new List<StatContainer>();
                DialogueTexts = new List<StatDialogueText>();
                DialogueNames = new List<StatName>();
                DialoguePics = new List<StatProfilePic>();
                DialoguePorts = new List<StatPort>();

            }
        }
        #endregion
    }


    [System.Serializable]
    public class CardLinkData
    {
        public string BaseCardGuid;
        public string BasePortName;
        public string TargetCardGuid;
        public string TargetPortName;
    }
}