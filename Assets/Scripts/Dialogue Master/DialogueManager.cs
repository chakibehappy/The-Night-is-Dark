using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Aceline.RPG.Test;

public class DialogueManager : MonoBehaviour
{
    GameMaster GM;
    [SerializeField] private DialoguePlayer dialoguePlay;

    public GameObject dialogueUI;
    public GameObject dialogueBox;
    public GameObject choicesUI;

    [Header("Setting")]
    [SerializeField] private bool showDialoguePicture;
    [SerializeField] private bool show3DCharacterFace;
    [SerializeField] private bool playDialogueAudioClip;

    [Header("Action Help")]
    public GameObject ActionInfo;
    public TextMeshProUGUI txtActionInfo;
    
    [Header("Action Hover Help")]
    public GameObject ActionHoverInfo;
    public TextMeshProUGUI txtActionHoverInfo;

    [Header("Text")]
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDialogue;

    [Header("Image")]
    [SerializeField] private Image leftImage;
    [SerializeField] private GameObject leftImageObject;
    [SerializeField] private Image rightImage;
    [SerializeField] private GameObject rightImageObject;

    [Header("Choice")]
    [SerializeField] private List<Button> choiceButton;
    [SerializeField] private List<TextMeshProUGUI> choiceText;

    TMP_Text m_textMeshPro;

    public float textSpeed = 0.025f;
    public float fullTextDelay = 0.5f;

    bool isSkippingText = false;

    public DialoguePlayer DialoguePlay { get => dialoguePlay; set => dialoguePlay = value; }

    private void Start()
    {
        GM = GameMaster.GM;
    }

    public void ShowActionInfo(bool isShow, string actionText = "")
    {
        if (dialogueBox.activeInHierarchy)
            isShow = false;
        txtActionInfo.text = actionText;
        ActionInfo.SetActive(isShow);
    }

    public void ShowHoverActionInfo(Vector2 pos, bool isShow, string actionText = "")
    {
        if (dialogueBox.activeInHierarchy)
            isShow = false;
        txtActionHoverInfo.text = actionText;
        ActionHoverInfo.transform.position = pos;
        ActionHoverInfo.SetActive(isShow);
    }

    public void SetImageFace1(Sprite image)
    {
        SetImageFace(leftImageObject, leftImage, image);
    }
    public void SetImageFace2(Sprite image)
    {
        SetImageFace(rightImageObject, rightImage, image);
    }

    public void SetImageFace(GameObject imageBox, Image renderer, Sprite image )
    {
        imageBox.SetActive(false);
        if (image != null)
        {
            renderer.sprite = image;
            imageBox.SetActive(true);
        }
    }

    public void SetChoicesButton(List<DialogueChoices> choices)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        choiceButton.ForEach(button => { button.gameObject.SetActive(false); });

        for (int i = 0; i < choices.Count; i++)
        {
            choiceText[i].text = choices[i].Text;
            if(choices[i].conditionCheck)
            {
                choiceButton[i].gameObject.SetActive(true);
            }
            choiceButton[i].onClick = new Button.ButtonClickedEvent();
            choiceButton[i].onClick.AddListener(choices[i].unityAction);
        }
    }

    public void ShowChoices(bool isShow)
    {
        choicesUI.SetActive(isShow);
    }

    public void ShowDialogueUI(bool isShow)
    {
        dialogueUI.SetActive(isShow);
    }

    public void ShowDialogue(bool isShow, string[] names = null, string[] texts = null, Sprite[] images1 = null, Sprite[] images2 = null, float speed = 0, float delay = 0, bool waitingInput = true, List<DialogueChoices> choices = null)
    {
        dialogueBox.SetActive(isShow);
        if (isShow)
        {
            DisplayText(names, texts, images1, images2, speed, delay, waitingInput, choices);
        }
        else
        {
            txtDialogue.text = string.Empty;
        }
    }


    #region Play Dialogue
    public IEnumerator ShowDialogueIE(string[] names, string[] texts, Sprite[] images1 = null, Sprite[] images2 = null, float speed = 0, float delay = 0, bool waitingInput = true, List<DialogueChoices> choices = null)
    {
        dialogueUI.SetActive(true);
        dialogueBox.SetActive(true);
        float newSpeed = speed == 0 ? textSpeed : speed;
        float newTextDelay = delay == 0 ? fullTextDelay : delay;
        yield return StartCoroutine(RevealAndDisplayTextIE(names, texts, images1, images2, newSpeed, newTextDelay, waitingInput, choices));
    }



    public void DisplayText(string[] names, string[] texts, Sprite[] images1 = null, Sprite[] images2 = null, float speed = 0, float textDelay = 0, bool waitingInput = true, List<DialogueChoices> choices = null)
    {
        float newSpeed = speed == 0 ? textSpeed : speed;
        float newTextDelay = textDelay == 0 ? fullTextDelay : textDelay;
        StartCoroutine(RevealAndDisplayTextIE(names, texts, images1, images2, newSpeed, newTextDelay, waitingInput, choices));
    }


    IEnumerator RevealAndDisplayTextIE(string[] names, string[] texts, Sprite[] images1, Sprite[] images2, float speed, float textDelay, bool waitingInput, List<DialogueChoices> choices)
    {
        if (texts != null)
        {
            m_textMeshPro = txtDialogue.GetComponent<TMP_Text>();


            txtName.alignment = TextAlignmentOptions.Left;


            for (int i = 0; i < texts.Length; i++)
            {
                int counter = 0;
                int visibleCount;

                if (names[i] != null)
                {
                    txtName.text = names[i].ToUpper();
                }

                m_textMeshPro.text = texts[i];
                m_textMeshPro.ForceMeshUpdate();


                if (images1 != null)
                {
                    SetImageFace1(images1[i]);
                }


                if (images2 != null)
                {
                    SetImageFace2(images2[i]);
                }

                int totalVisibleCharacters = m_textMeshPro.textInfo.characterCount;
                bool playDialogue = true;
                while (playDialogue)
                {
                    visibleCount = counter % (totalVisibleCharacters + 1);

                    m_textMeshPro.maxVisibleCharacters = visibleCount;

                    if (isSkippingText)
                    {
                        visibleCount = totalVisibleCharacters;
                        m_textMeshPro.maxVisibleCharacters = visibleCount;
                        yield return new WaitForSeconds(speed);
                        yield return StartCoroutine(GM.WaitForPlayerPress());
                        yield return new WaitForSeconds(0.1f);
                        m_textMeshPro.text = "";
                        break;
                    }

                    if (visibleCount >= totalVisibleCharacters)
                    {
                        playDialogue = false;
                        if (waitingInput)
                        {
                            yield return StartCoroutine(GM.WaitForPlayerPress());
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            yield return new WaitForSeconds(textDelay);
                        }
                        m_textMeshPro.text = "";
                    }

                    counter += 1;
                    yield return new WaitForSeconds(speed);
                }
            }
        }
        

        if(choices != null && choices.Count > 0)
        {
            ShowDialogue(false);
            txtName.text = "";

            GM.playerIsChosing = false;
            SetChoicesButton(choices);
            ShowChoices(true);
            yield return StartCoroutine(GM.WaitForPlayerChoose());
            yield return new WaitForSeconds(0.1f);
            ShowChoices(false);
        }
    }

    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSkippingText)
        {
            StartCoroutine(StartSkippingMessageIE());
        }
    }

    IEnumerator StartSkippingMessageIE()
    {
        isSkippingText = true;
        yield return new WaitForSeconds(textSpeed);
        isSkippingText = false;
    }
}
