using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class ZenModeManager : MonoBehaviour
{
    GameMaster GM;
    public RandomToughtMovement PLAYER;

    public Color activeColor;
    public Color inactiveHintsColor;
    public Color inactiveFactsColor;
    public Color inactiveActionColor;
    public Color inactiveButtonColor;
    public Color inactiveTextButtonColor;

    public Transform hintPanel, factPanel;
    public Transform[] hintPanelPos;
    public Transform[] factPanelPos;

    public Button[] hintArrowButton;
    public Button[] factArrowButton;

    public Image[] ballonBoxHints;
    public TextMeshProUGUI[] txtHints;
    List<Vector3> originalHintPos = new List<Vector3>();
    public Image[] ballonBoxFacts;
    public TextMeshProUGUI[] txtFacts;

    bool isSelectingHints;
    int overLapHints, overLapFacts;

    int selectedHints;
    bool isDraggingHints;
    bool isSelectingFacts;

    public Image dimScreen;
    public Image ideaBoxBg;
    public Image ideaBoxIcon;
    public Image ideaBoxOutlineIcon;
    public TextMeshProUGUI ideaBoxLabel;
    public Button btnCancel;
    public Button btnThink;
    public GameObject floatingBallonAction;
    public TextMeshProUGUI txtFloatingAction;

    public List<Hint> hintsOnUsed = new List<Hint>();
    public Transform[] activeHintsBox;
    public TextMeshProUGUI[] txtActiveHints;
    public Transform topLeftIdeaBox, bottomRightIdeaBox;
    public Transform topLeftDropPos, bottomRightDropPos;
    public Transform centerScreen;

    float dimScreenFade, ideaBoxBgFade, ideaBoxIconFade, ideaBoxOutlineIconFade, ideaBoxLabelFade;
    IEnumerator idleCoroutine;
    public float noInteractionTime = 5f;

    private void Start()
    {
        hintPanel.position = hintPanelPos[1].position;
        factPanel.position = factPanelPos[0].position;
        GM = GameMaster.GM;
        isSelectingHints = true;

        for (int i = 0; i < ballonBoxHints.Length; i++)
        {
            originalHintPos.Add(ballonBoxHints[i].transform.position);
        }
        hintArrowButton[0].onClick.RemoveAllListeners();
        hintArrowButton[1].onClick.RemoveAllListeners();
        hintArrowButton[0].onClick.AddListener(() => HintArrowButtonPress());
        hintArrowButton[1].onClick.AddListener(() => HintArrowButtonPress(false));

        AssignIdeaBoxButtons();
        DisplayHintsAndFacts();

        dimScreenFade = dimScreen.color.a;
        ideaBoxBgFade = ideaBoxBg.color.a;
        ideaBoxIconFade = ideaBoxIcon.color.a;
        ideaBoxOutlineIconFade = ideaBoxOutlineIcon.color.a;
        ideaBoxLabelFade = ideaBoxLabel.color.a;
        ShowIdeaBox(false);
    }

    void AssignIdeaBoxButtons()
    {
        btnCancel.onClick.RemoveAllListeners();
        btnCancel.onClick.AddListener(() => { ClearActiveHints(); });

        btnThink.onClick.RemoveAllListeners();
        btnThink.onClick.AddListener(() => { CombineHints(); });
    }

    private void Update()
    {
        HandleDragHints();
    }

    void HandleDragHints()
    {
        if (isDraggingHints)
        {
            ballonBoxHints[selectedHints].transform.position = Input.mousePosition;
        }
    }

    void OnDragHints(int index, bool isDrag = true)
    {
        selectedHints = index;
        ballonBoxHints[selectedHints].transform.SetAsLastSibling();
        isDraggingHints = isDrag;
    }

    void OnDropHints(int id, int index)
    {
        if (hintsOnUsed.Count < 4)
        {
            if (isDraggingHints)
            {
                if (ballonBoxHints[index].transform.position.x >= topLeftIdeaBox.position.x &&
                    ballonBoxHints[index].transform.position.x <= bottomRightIdeaBox.position.x &&
                    ballonBoxHints[index].transform.position.y >= bottomRightIdeaBox.position.y &&
                    ballonBoxHints[index].transform.position.y <= topLeftIdeaBox.position.y)
                {
                    SelectHint(id, index);
                }
                ballonBoxHints[index].transform.position = originalHintPos[index];
                OnDragHints(index, false);
            }
        }
        else
        {
            ballonBoxHints[index].transform.position = originalHintPos[index];
            OnDragHints(index, false);
        }
    }

    void DisplayHintsAndFacts()
    {
        hintArrowButton[0].gameObject.SetActive(overLapHints > 0);
        hintArrowButton[1].gameObject.SetActive(GM.playerHints.Count > ballonBoxHints.Length + overLapHints);

        for (int i = 0; i < ballonBoxHints.Length; i++)
        {
            if(i < GM.playerHints.Count + overLapHints)
            {
                int id = i + overLapHints;
                ballonBoxHints[i].color = inactiveHintsColor;
                txtHints[i].text = GM.playerHints[id].hintText;
                int index = i;
                AssignHintClick(ballonBoxHints[index].gameObject, GM.playerHints[id].id, index);
            }
            ballonBoxHints[i].gameObject.SetActive(i < GM.playerHints.Count);
        }

        factArrowButton[0].gameObject.SetActive(overLapFacts > 0);
        factArrowButton[1].gameObject.SetActive(GM.playerFact.Count > ballonBoxFacts.Length + overLapFacts);

        for (int i = 0; i < ballonBoxFacts.Length; i++)
        {
            if (i < GM.playerFact.Count + overLapFacts)
            {
                int id = i + overLapFacts;
                ballonBoxFacts[i].color = GM.playerFact[id].isCommand? inactiveActionColor : inactiveFactsColor;
                txtFacts[i].color = GM.playerFact[id].isCommand ? Color.black : Color.white;
                txtFacts[i].text = GM.playerFact[id].isCommand ?  GM.playerFact[id].factText.ToUpper() : GM.playerFact[id].factText;
                int index = i;
                AssignFactClick(ballonBoxFacts[index].gameObject, GM.playerFact[id], index);
            }
            ballonBoxFacts[i].gameObject.SetActive(i < GM.playerFact.Count);
        }

        btnCancel.gameObject.SetActive(hintsOnUsed.Count > 0);
        btnThink.gameObject.SetActive(hintsOnUsed.Count > 1);
    }

    void HintArrowButtonPress(bool isUp = true)
    {
        if (isUp)
            overLapHints--;
        else
            overLapHints++;
        DisplayHintsAndFacts();
    }

    void AssignHintClick(GameObject obj, int id, int index)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();

        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { OnDragHints(index); });

        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { OnDropHints(id, index); });

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { HighlightHint(obj, activeColor, true); });

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { HighlightHint(obj, inactiveHintsColor, false); });

        trigger.triggers.Clear();
        trigger.triggers.Add(entryDown);
        trigger.triggers.Add(entryUp);
        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    void AssignFactClick(GameObject obj, Fact fact, int index)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        Color inactiveColor = fact.isCommand ? inactiveActionColor : inactiveFactsColor;
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { HighlightFact(obj, activeColor); });

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { HighlightFact(obj, inactiveColor); });

        trigger.triggers.Clear();
        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    void HighlightHint(GameObject obj, Color col, bool isActive = true)
    {
        if(isActive)
        {
            ShowIdeaBox(true, 0, 1f);
        }
        else
        {
            ShowIdeaBox(false);
        }

        if (!isSelectingHints)
        {
            isSelectingHints = true;
            isSelectingFacts = false;
            MovePanel(hintPanel, hintPanelPos[1]);
            MovePanel(factPanel, factPanelPos[0], 1.5f);
        }
        if (!isDraggingHints)
        {
            obj.GetComponent<Image>().color = col;
        }
    }

    void HighlightFact(GameObject obj, Color col, bool isActive = true)
    {
        if (isActive)
        {
            ShowIdeaBox(true, 0, 1f);
        }
        else
        {
            ShowIdeaBox(false);
        }

        ShowIdeaBox(true, 0, 1f);
        OpenFactPanel();
        obj.GetComponent<Image>().color = col;
    }
    private void OpenFactPanel()
    {
        //PLAYER.PlayerIsThinking(false);
        if (!isSelectingFacts)
        {
            isSelectingHints = false;
            isSelectingFacts = true;
            MovePanel(factPanel, factPanelPos[1]);
            MovePanel(hintPanel, hintPanelPos[0], 1.5f);
        }
    }

    void SelectHint(int id, int index)
    {
        PLAYER.PlayerIsThinking();
        if (overLapHints > 0)
            overLapHints--;

        GM.playerHints.Remove(GM.hints[id]);
        hintsOnUsed.Add(GM.hints[id]);
        
        DisplayHintsAndFacts();
        ShowHintOnBox(index);
    }

    void ShowHintOnBox(int index, bool autoSort = false)
    {
        float gap = 120;
        if (autoSort)
        {
            activeHintsBox[0].transform.localPosition = new Vector3(0, (hintsOnUsed.Count - 1) * 0.5f * gap, 0);
        }
        else
        {
            Vector3 hintPos = ballonBoxHints[index].transform.position;
            if (ballonBoxHints[index].transform.position.x < topLeftDropPos.position.x)
                hintPos.x = topLeftDropPos.position.x;
            if (ballonBoxHints[index].transform.position.y > topLeftDropPos.position.y)
                hintPos.y = topLeftDropPos.position.y;
            if (ballonBoxHints[index].transform.position.x > bottomRightDropPos.position.x)
                hintPos.x = bottomRightDropPos.position.x;
            if (ballonBoxHints[index].transform.position.y < bottomRightDropPos.position.y)
                hintPos.y = bottomRightDropPos.position.y;

            activeHintsBox[hintsOnUsed.Count - 1].transform.position = hintPos;
        }
        for (int i = 0; i < hintsOnUsed.Count; i++)
        {
            activeHintsBox[i].gameObject.SetActive(i < hintsOnUsed.Count);
            if (autoSort)
            {
                activeHintsBox[i].transform.localPosition = new Vector3(0, activeHintsBox[0].transform.localPosition.y - gap * i, 0);
            }
            txtActiveHints[i].text = hintsOnUsed[i].hintText;
        }
        btnCancel.gameObject.SetActive(hintsOnUsed.Count > 0);
        btnThink.gameObject.SetActive(hintsOnUsed.Count > 1);
    }

    void MovePanel(Transform panel, Transform pos, float delay = 0.5f)
    {
        panel.DOMove(pos.position, delay).SetEase(Ease.Linear);
    }

    void CombineHints()
    {
        StartCoroutine(CombineHintsIE());
    }
    IEnumerator CombineHintsIE()
    {
        bool isRightAnswer = false;
        Fact newFact = null;

        for(int i = 0; i < GM.facts.Count; i++)
        {
            int hintCount = 0;
            hintsOnUsed.ForEach((hint) => {
                if (GM.facts[i].hints.Contains(hint.id))
                {
                    hintCount++;
                }
            });
            if(hintCount == GM.facts[i].hints.Count)
            {
                newFact = GM.facts[i];
                GM.playerFact.Insert(0, newFact);
                isRightAnswer = true;
                PLAYER.PlayerGotFacts();
                break;
            }
        }

        List<Vector3> oriPos = new List<Vector3>();
        for (int i = 0; i < activeHintsBox.Length; i++)
        {
            oriPos.Add(activeHintsBox[i].transform.position);
            activeHintsBox[i].transform.DOMove(centerScreen.position, 0.5f);
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < activeHintsBox.Length; i++)
        {
            if (!isRightAnswer)
                activeHintsBox[i].transform.DOMove(oriPos[i], 0.5f);
            else
                activeHintsBox[i].gameObject.SetActive(false);
        }

        if (!isRightAnswer)
        {
            PLAYER.PlayerIsThinking(false);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(1.75f);
            if (newFact.isCommand)
            {
                txtFloatingAction.text = newFact.factText;
                floatingBallonAction.SetActive(true);
            }
            hintsOnUsed.Clear();
            ShowIdeaBox(false, 0, 1f);
            DisplayHintsAndFacts();
            OpenFactPanel();
        }
    }

    void ClearActiveHints()
    {
        PLAYER.PlayerIsThinking(false);
        for (int i = 0; i < activeHintsBox.Length; i++)
        {
            activeHintsBox[i].gameObject.SetActive(false);
        }
        hintsOnUsed.ForEach((hint) => GM.playerHints.Add(hint));
        hintsOnUsed.Clear();
        DisplayHintsAndFacts();
        ShowIdeaBox(false);
    }

    void ShowIdeaBox(bool isShow = true, float delay = -1, float delayFade = 2f)
    {
        if (delay < 0)
            delay = noInteractionTime;
        StopIdeaBoxIdle();
        idleCoroutine = ShowIdeaBoxIE(isShow, delay, delayFade);
        StartCoroutine(idleCoroutine);
    }
    IEnumerator ShowIdeaBoxIE(bool isShow, float delay, float delayFade = 2f)
    {
        float fade0, fade1, fade2, fade3, fade4;
        fade0 = fade1 = fade2 = fade3 = fade4 = 0;
        if(isShow)
        {
            fade0 = ideaBoxBgFade;
            fade1 = ideaBoxIconFade;
            fade2 = ideaBoxOutlineIconFade;
            fade3 = ideaBoxLabelFade;
            fade4 = dimScreenFade;
        }

        yield return new WaitForSeconds(delay);

        if (hintsOnUsed.Count == 0)
        {
            ideaBoxBg.DOFade(fade0, delayFade).SetEase(Ease.Linear);
            ideaBoxIcon.DOFade(fade1, delayFade).SetEase(Ease.Linear);
            ideaBoxOutlineIcon.DOFade(fade2, delayFade).SetEase(Ease.Linear);
            ideaBoxLabel.DOFade(fade3, delayFade).SetEase(Ease.Linear);
            dimScreen.DOFade(fade4, delayFade).SetEase(Ease.Linear);
        }
    }

    void StopIdeaBoxIdle()
    {
        if(idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }
    }
}
