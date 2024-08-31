using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Aceline.RPG.Editor
{
    public class DialogueSaveAndLoad
    {
        private List<Edge> Edges => graphView.edges.ToList();
        private List<BaseCard> Cards => graphView.nodes.ToList().Where(node => node is BaseCard).Cast<BaseCard>().ToList();

        private BoardGraphView graphView;

        public DialogueSaveAndLoad(BoardGraphView graphView)
        {
            this.graphView = graphView;
        }

        public void Save(GameBoard gameBoard)
        {
            SaveEdges(gameBoard);
            SaveCards(gameBoard);

            EditorUtility.SetDirty(gameBoard);
            AssetDatabase.SaveAssets();
        }

        public void Load(GameBoard gameBoard)
        {
            ClearGraph();
            GenerateCards(gameBoard);
            ConnectCards(gameBoard);
        }

        #region Save
        private void SaveEdges(GameBoard gameBoard)
        {
            gameBoard.CardLinkDatas.Clear();

            Edge[] connectedEdges = Edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                BaseCard outputCard = (BaseCard)connectedEdges[i].output.node;
                BaseCard inputCard = connectedEdges[i].input.node as BaseCard;

                gameBoard.CardLinkDatas.Add(new CardLinkData
                {
                    BaseCardGuid = outputCard.CardGuid,
                    BasePortName = connectedEdges[i].output.portName,
                    TargetCardGuid = inputCard.CardGuid,
                    TargetPortName = connectedEdges[i].input.portName,
                });
            }
        }

        private void SaveCards(GameBoard gameBoard)
        {
            gameBoard.EventDatas.Clear();
            gameBoard.EndDatas.Clear();
            gameBoard.StartDatas.Clear();
            gameBoard.BranchDatas.Clear();
            gameBoard.DialogueDatas.Clear();
            gameBoard.ChoiceDatas.Clear();

            Cards.ForEach(card =>
            {
                switch (card)
                {
                    case DialogueCard dialogueCard:
                        gameBoard.DialogueDatas.Add(SaveCardData(dialogueCard));
                        break;
                    case StartCard startCard:
                        gameBoard.StartDatas.Add(SaveCardData(startCard));
                        break;
                    case EndCard endCard:
                        gameBoard.EndDatas.Add(SaveCardData(endCard));
                        break;
                    case EventCard eventCard:
                        gameBoard.EventDatas.Add(SaveCardData(eventCard));
                        break;
                    case BranchCard branchCard:
                        gameBoard.BranchDatas.Add(SaveCardData(branchCard));
                        break;
                    case ChoiceCard choiceCard:
                        gameBoard.ChoiceDatas.Add(SaveCardData(choiceCard));
                        break;
                    default:
                        break;
                }
            });
        }

        private DialogueData SaveCardData(DialogueCard card)
        {
            DialogueData dialogueData = new DialogueData
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,
            };

            // Set ID
            for (int i = 0; i < card.DialogueData.Containers.Count; i++)
            {
                card.DialogueData.Containers[i].ID.Value = i;
            }

            foreach (StatContainer baseContainer in card.DialogueData.Containers)
            {
                // Name
                if (baseContainer is StatName)
                {
                    StatName tmp = (baseContainer as StatName);
                    StatName tmpData = new StatName();

                    tmpData.ID.Value = tmp.ID.Value;
                    tmpData.CharacterName.Value = tmp.CharacterName.Value;

                    dialogueData.Names.Add(tmpData);
                }

                // Text
                if (baseContainer is StatDialogueText)
                {
                    StatDialogueText tmp = (baseContainer as StatDialogueText);
                    StatDialogueText tmpData = new StatDialogueText();

                    tmpData.ID = tmp.ID;
                    tmpData.GuidID = tmp.GuidID;
                    tmpData.Texts = tmp.Texts;
                    tmpData.AudioClips = tmp.AudioClips;

                    dialogueData.Dialogues.Add(tmpData);
                }

                // Images
                if (baseContainer is StatProfilePic)
                {
                    StatProfilePic tmp = (baseContainer as StatProfilePic);
                    StatProfilePic tmpData = new StatProfilePic();

                    tmpData.ID.Value = tmp.ID.Value;
                    tmpData.LeftSprite.Value = tmp.LeftSprite.Value;
                    tmpData.RightSprite.Value = tmp.RightSprite.Value;

                    dialogueData.Images.Add(tmpData);
                }
            }

            // Port
            foreach (StatPort port in card.DialogueData.Ports)
            {
                StatPort portData = new StatPort();

                portData.OutputGuid = string.Empty;
                portData.InputGuid = string.Empty;
                portData.PortGuid = port.PortGuid;

                foreach (Edge edge in Edges)
                {
                    if (edge.output.portName == port.PortGuid)
                    {
                        portData.OutputGuid = (edge.output.node as BaseCard).CardGuid;
                        portData.InputGuid = (edge.input.node as BaseCard).CardGuid;
                    }
                }

                dialogueData.Ports.Add(portData);
            }

            return dialogueData;
        }

        private StartData SaveCardData(StartCard card)
        {
            StartData cardData = new StartData()
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,
            };

            return cardData;
        }

        private EndData SaveCardData(EndCard card)
        {
            EndData cardData = new EndData()
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,
            };
            cardData.EndState.Value = card.EndData.EndState.Value;

            return cardData;
        }

        private EventData SaveCardData(EventCard card)
        {
            EventData cardData = new EventData()
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,
            };

            // Save Card Event
            foreach (EventCardStat dialogueEvent in card.EventData.EventCardSet)
            {
                cardData.EventCardSet.Add(dialogueEvent);
            }

            // Save String Event
            foreach (EventModifier stringEvents in card.EventData.Modifiers)
            {
                EventModifier tmp = new EventModifier();
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.Text.Value = stringEvents.Text.Value;
                tmp.Modifier.Value = stringEvents.Modifier.Value;

                cardData.Modifiers.Add(tmp);
            }

            return cardData;
        }

        private BranchData SaveCardData(BranchCard card)
        {
            List<Edge> tmpEdges = Edges.Where(x => x.output.node == card).Cast<Edge>().ToList();

            Edge trueOutput = Edges.FirstOrDefault(x => x.output.node == card && x.output.portName == "True");
            Edge falseOutput = Edges.FirstOrDefault(x => x.output.node == card && x.output.portName == "False");

            BranchData cardData = new BranchData()
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,
                OptionTrueGuid = (trueOutput != null ? (trueOutput.input.node as BaseCard).CardGuid : string.Empty),
                OptionFalseGuid = (falseOutput != null ? (falseOutput.input.node as BaseCard).CardGuid : string.Empty),
            };

            foreach (BranchCondition stringEvents in card.BranchData.Conditions)
            {
                BranchCondition tmp = new BranchCondition();
                tmp.DataCardSet = stringEvents.DataCardSet;
                tmp.SelectedStat = stringEvents.SelectedStat;
                tmp.Value.Value = stringEvents.Value.Value;
                tmp.Condition.Value = stringEvents.Condition.Value;

                cardData.Conditions.Add(tmp);
            }

            return cardData;
        }

        private ChoiceData SaveCardData(ChoiceCard card)
        {
            ChoiceData cardData = new ChoiceData()
            {
                CardGuid = card.CardGuid,
                Position = card.GetPosition().position,

                Texts = card.ChoiceData.Texts,
                VoiceOvers = card.ChoiceData.VoiceOvers,
            };
            cardData.Choice.Value = card.ChoiceData.Choice.Value;

            foreach (BranchCondition condition in card.ChoiceData.Conditions)
            {
                BranchCondition tmp = new BranchCondition();
                tmp.DataCardSet = condition.DataCardSet;
                tmp.SelectedStat = condition.SelectedStat;
                tmp.Value.Value = condition.Value.Value;
                tmp.Condition.Value = condition.Condition.Value;

                cardData.Conditions.Add(tmp);
            }

            return cardData;
        }
        #endregion

        #region Load

        private void ClearGraph()
        {
            Edges.ForEach(edge => graphView.RemoveElement(edge));

            foreach (BaseCard card in Cards)
            {
                graphView.RemoveElement(card);
            }
        }

        private void GenerateCards(GameBoard gameBoard)
        {
            // Start
            foreach (StartData card in gameBoard.StartDatas)
            {
                StartCard newCard = graphView.CreateStartCard(card.Position);
                newCard.CardGuid = card.CardGuid;

                graphView.AddElement(newCard);
            }

            // End Card 
            foreach (EndData card in gameBoard.EndDatas)
            {
                EndCard newCard = graphView.CreateEndCard(card.Position);
                newCard.CardGuid = card.CardGuid;
                newCard.EndData.EndState.Value = card.EndState.Value;

                newCard.LoadValueInToField();
                graphView.AddElement(newCard);
            }

            // Event Card
            foreach (EventData card in gameBoard.EventDatas)
            {
                EventCard newCard = graphView.CreateEventCard(card.Position);
                newCard.CardGuid = card.CardGuid;

                foreach (EventCardStat item in card.EventCardSet)
                {
                    newCard.AddEventDataModifier(item);
                }
                foreach (EventModifier item in card.Modifiers)
                {
                    newCard.AddModifier(item);
                }

                newCard.LoadValueInToField();
                graphView.AddElement(newCard);
            }

            // Branch Card
            foreach (BranchData card in gameBoard.BranchDatas)
            {
                BranchCard newCard = graphView.CreateBranchCard(card.Position, card.Conditions.Count == 0);
                newCard.CardGuid = card.CardGuid;

                foreach (BranchCondition item in card.Conditions)
                {
                    newCard.AddCondition(item);
                }

                newCard.LoadValueInToField();
                newCard.ReloadLanguage();
                graphView.AddElement(newCard);
            }

            // Choice Card
            foreach (ChoiceData card in gameBoard.ChoiceDatas)
            {
                ChoiceCard newCard = graphView.CreateChoiceCard(card.Position);
                newCard.CardGuid = card.CardGuid;

                newCard.ChoiceData.Choice.Value = card.Choice.Value;

                foreach (Languages<string> dataText in card.Texts)
                {
                    foreach (Languages<string> editorText in newCard.ChoiceData.Texts)
                    {
                        if (editorText.Name == dataText.Name)
                        {
                            editorText.Value = dataText.Value;
                        }
                    }
                }
                foreach (Languages<AudioClip> dataAudioClip in card.VoiceOvers)
                {
                    foreach (Languages<AudioClip> editorAudioClip in newCard.ChoiceData.VoiceOvers)
                    {
                        if (editorAudioClip.Name == dataAudioClip.Name)
                        {
                            editorAudioClip.Value = dataAudioClip.Value;
                        }
                    }
                }

                foreach (BranchCondition item in card.Conditions)
                {
                    newCard.AddCondition(item);
                }

                newCard.LoadValueInToField();
                newCard.ReloadLanguage();
                graphView.AddElement(newCard);
            }

            // Dialogue Card
            foreach (DialogueData card in gameBoard.DialogueDatas)
            {
                DialogueCard newCard = graphView.CreateDialogueCard(card.Position);
                newCard.CardGuid = card.CardGuid;

                List<StatContainer> data_BaseContainer = new List<StatContainer>();

                data_BaseContainer.AddRange(card.Images);
                data_BaseContainer.AddRange(card.Dialogues);
                data_BaseContainer.AddRange(card.Names);

                data_BaseContainer.Sort(delegate (StatContainer x, StatContainer y)
                {
                    return x.ID.Value.CompareTo(y.ID.Value);
                });

                foreach (StatContainer data in data_BaseContainer)
                {
                    switch (data)
                    {
                        case StatName Name:
                            newCard.CharacterName(Name);
                            break;
                        case StatDialogueText Text:
                            newCard.TextLine(Text);
                            break;
                        case StatProfilePic image:
                            newCard.ImagePic(image);
                            break;
                        default:
                            break;
                    }
                }

                foreach (StatPort port in card.Ports)
                {
                    newCard.AddChoicePort(newCard, port);
                }

                newCard.LoadValueInToField();
                newCard.ReloadLanguage();
                graphView.AddElement(newCard);
            }
        }

        private void ConnectCards(GameBoard dialogueContainer)
        {
            // Make connection for all card.
            for (int i = 0; i < Cards.Count; i++)
            {
                List<CardLinkData> connections = dialogueContainer.CardLinkDatas.Where(edge => edge.BaseCardGuid == Cards[i].CardGuid).ToList();

                List<Port> allOutputPorts = Cards[i].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    string targetCardGuid = connections[j].TargetCardGuid;
                    BaseCard targetCard = Cards.First(card => card.CardGuid == targetCardGuid);

                    if (targetCard == null)
                        continue;

                    foreach (Port item in allOutputPorts)
                    {
                        if (item.portName == connections[j].BasePortName)
                        {
                            LinkCardsTogether(item, (Port)targetCard.inputContainer[0]);
                        }
                    }
                }
            }
        }

        private void LinkCardsTogether(Port outputPort, Port inputPort)
        {
            Edge tempEdge = new Edge()
            {
                output = outputPort,
                input = inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        #endregion

    }
}