using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Aceline.RPG.Test
{

    public class DialoguePlayer : GetGameBoardData
    {
        private GameMaster GM;
        private LanguageController LC;

        private DialogueData currentDialogueCardData;
        private DialogueData lastDialogueCardData;
        List<DialogueData> allDialogueData = new List<DialogueData>();

        MovementController player;

        private List<StatContainer> baseContainer;
        int currentIndex = 0;

        private void Start()
        {
            GM = GameMaster.GM;
            LC = GM.LANGUAGE;
        }

        public void StartDialogue()
        {
            CheckCardType(GetNextCard(gameBoard.StartDatas[0]));
        }


        public void StartDialogue(GameBoard board)
        {
            gameBoard = board;
            CheckCardType(GetNextCard(gameBoard.StartDatas[0]));
        }

        public void StartDialogue(MovementController pc, GameBoard board)
        {
            gameBoard = board;
            player = pc;
            CheckCardType(GetNextCard(gameBoard.StartDatas[0]));
        }

        private void CheckCardType(BaseData baseCard)
        {
            switch (baseCard)
            {
                case StartData cardData:
                    PlayCard(cardData);
                    break;

                case DialogueData cardData:
                    PlayCard(cardData);
                    break;

                case EventData cardData:
                    PlayCard(cardData);
                    break;

                case BranchData cardData:
                    PlayCard(cardData);
                    break;

                case EndData cardData:
                    PlayCard(cardData);
                    break;

                default:
                    break;
            }
        }

        private void PlayCard(StartData data)
        {
            CheckCardType(GetNextCard(gameBoard.StartDatas[0]));
        }

        private void PlayCard(DialogueData data)
        {
            currentDialogueCardData = data;
            allDialogueData.Add(data);

            baseContainer = new List<StatContainer>();
            baseContainer.AddRange(data.Names);
            baseContainer.AddRange(data.Images);
            baseContainer.AddRange(data.Dialogues);


            baseContainer.Sort(delegate (StatContainer x, StatContainer y)
            {
                return x.ID.Value.CompareTo(y.ID.Value);
            });

            DialogueToDo();
        }

        void DialogueToDo()
        {
            int totalDialogue = baseContainer.Where(x => x is StatDialogueText).ToList().Count;
            string[] names = new string[totalDialogue];
            string[] texts = new string[totalDialogue];
            Sprite[] images1 = new Sprite[totalDialogue];
            Sprite[] images2 = new Sprite[totalDialogue];

            int dialogueCount = 0;
            Sprite currentLeftImage = null;
            Sprite currentRightImage = null;
            string currentName = null;

            // Fill all dialogue data :
            for (int i = 0; i < baseContainer.Count; i++)
            {
                if (baseContainer[i] is StatName)
                {
                    currentName = (baseContainer[i] as StatName).CharacterName.Value;
                }

                if (baseContainer[i] is StatProfilePic)
                {
                    currentLeftImage = (baseContainer[i] as StatProfilePic).LeftSprite.Value;
                    currentRightImage = (baseContainer[i] as StatProfilePic).RightSprite.Value;
                }

                if (baseContainer[i] is StatDialogueText)
                {
                    StatDialogueText dialogueTexts = (baseContainer[i] as StatDialogueText);
                    texts[dialogueCount] = GetTextByLanguage(dialogueTexts.Texts, GM.LANGUAGE.Language);
                    names[dialogueCount] = currentName;
                    images1[dialogueCount] = currentLeftImage;
                    images2[dialogueCount] = currentRightImage;
                    dialogueCount++;
                }
            }

            // Check and fill the choices :
            List<DialogueChoices> dialogueChoices = new List<DialogueChoices>();
            if(currentDialogueCardData.Ports.Count > 0)
            {
                foreach (StatPort item in currentDialogueCardData.Ports)
                {
                    ChoiceCheck(item.InputGuid, dialogueChoices);
                }
            }

            
            if (currentIndex != 0)
            {
                string[] newNames = new string[] { names[names.Length - 1] };
                string[] newTexts = new string[] { texts[texts.Length - 1] };
                Sprite[] newImages1 = new Sprite[] { images1[images1.Length - 1] };
                Sprite[] newImages2 = new Sprite[] { images2[images2.Length - 1] };

                if (dialogueChoices.Count == 0)
                    StartCoroutine(ShowDialogueIE(newNames, newTexts, newImages1, newImages2, dialogueChoices));
                else
                    StartCoroutine(ShowDialogueIE(null, null, null, null, dialogueChoices));
            }
            else
            {

                StartCoroutine(ShowDialogueIE(names, texts, images1, images2, dialogueChoices));
            }
            
        }

        IEnumerator ShowDialogueIE(string[] names, string[] texts, Sprite[] images1 = null, Sprite[] images2 = null, List<DialogueChoices> choices = null)
        {
            currentIndex = 0;
            yield return StartCoroutine(GM.DIALOGUE.ShowDialogueIE(names, texts, images1, images2, 0, 0, true, choices));
            
            if (choices.Count == 0)
            {
                CheckCardType(GetNextCard(currentDialogueCardData));
            }
        }


        void ChoiceCheck(string cardID, List<DialogueChoices> dialogueChoices)
        {
            BaseData baseData = GetCardByGuid(cardID);
            ChoiceData choiceData = GetCardByGuid(cardID) as ChoiceData;

            bool checkBranch = true;
            
            if(choiceData.Conditions.Count > 0)
            {
                foreach (BranchCondition item in choiceData.Conditions)
                {
                    if (!GameEvents.Instance.EventCondition(item.SelectedStat.Name, item.Condition.Value, item.Value.Value))
                    {
                        checkBranch = false;
                        break;
                    }
                }
            }

            UnityAction unityAction = null;
            unityAction += () =>
            {
                GM.playerIsChosing = true;
                CheckCardType(GetNextCard(choiceData));
            };


            DialogueChoices choice = new DialogueChoices
            {
                ChoiceState = choiceData.Choice.Value,
                Text = GetTextByLanguage(choiceData.Texts, GM.LANGUAGE.Language),
                unityAction = unityAction,
                conditionCheck = checkBranch
            };
            dialogueChoices.Add(choice);
        }

        private void PlayCard(EventData data)
        {
            foreach (EventCardStat item in data.EventCardSet)
            {
                if(item.CardEvent != null)
                {
                    item.CardEvent.RunEvent();
                }
            }

            foreach (EventModifier item in data.Modifiers)
            {
                GameEvents.Instance.EventModifier(item.Text.Value, item.Modifier.Value, item.Value.Value);
            }
            CheckCardType(GetNextCard(data));
        }

        private void PlayCard(BranchData data)
        {
            bool checkBranch = true;
            foreach (BranchCondition item in data.Conditions)
            {
                if (!GameEvents.Instance.EventCondition(item.SelectedStat.Name, item.Condition.Value, item.Value.Value))
                {
                    checkBranch = false;
                    break;
                }
            }

            string nextCard = checkBranch ? data.OptionTrueGuid : data.OptionFalseGuid;
            CheckCardType(GetCardByGuid(nextCard));
        }

        private void PlayCard(EndData data)
        {
            switch (data.EndState.Value)
            {
                case EndCardType.End:
                    player.ShowDialogue(false);
                    GM.DIALOGUE.ShowDialogueUI(false);
                    break;

                case EndCardType.Repeat:
                    currentIndex = 0;
                    CheckCardType(GetCardByGuid(currentDialogueCardData.CardGuid));
                    break;

                case EndCardType.GoBack:
                    currentIndex = allDialogueData[allDialogueData.Count - 2].Dialogues.Count - 1;
                    CheckCardType(GetCardByGuid(allDialogueData[allDialogueData.Count-2].CardGuid));
                    
                    break;

                case EndCardType.ReturnToStart:
                    currentIndex = 0;
                    CheckCardType(GetNextCard(gameBoard.StartDatas[0]));
                    break;

                default:
                    break;
            }
        }

        string GetTextByLanguage(List<Languages<string>> textList, Language language)
        {
            return textList.Find(text => text.Name == language).Value;
        }
    }

}
