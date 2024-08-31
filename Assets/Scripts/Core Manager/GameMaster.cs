using Aceline.RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameMaster : MonoBehaviour
{
    public static GameMaster GM;

    public GameData DATA;
    public DialogueManager DIALOGUE;
    public InventoryManager INVENTORY;

    //public GameTimeManager TIME;
    //public StaminaManager STAMINA;
    public LanguageController LANGUAGE;
    
    public bool canMove = true;
    public GameObject interactWith;
    public bool isOpenMenu;
    public float waitingToTalkDelayNPC = 2.5f;
    public float money = 100;
    public bool playerIsChosing = false;

    // to copy game data :
    public List<Fact> facts;
    public List<Hint> hints;

    // storing playerFact & Hint
    public List<Fact> playerFact;
    public List<Hint> playerHints;

    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioSource VO;


    public UnityAction OnChangeMoney;
    public UnityAction OnChangeHealth;

    // Level and position
    public int activeScene = 0;
    public int activeCameraIndex = 0;
    public Vector3 playerPos;
    public Vector3 playerRot;
    public bool cameraIsChange;
    public float cameraChangeDelay = 0.5f;

    public Transform currentCamera;

    public bool canHover = true;

    public void ChangeCamera(int cameraIndex, bool addDelay = false)
    {
        StartCoroutine(ChangeCameraIE(cameraIndex, addDelay));
    }
    IEnumerator ChangeCameraIE(int cameraIndex, bool addDelay)
    {
        activeCameraIndex = cameraIndex;
        if (addDelay)
        {
            cameraIsChange = true;
            yield return new WaitForSeconds(cameraChangeDelay);
            cameraIsChange = false;
        }
    }

    public void ShowActionInfo(bool isShow = true, string actionText = "TALK", Sprite icon = null)
    {
        DIALOGUE.ShowActionInfo(isShow, actionText);
    }
    public void ShowHoverActionInfo(Vector2 pos, bool isShow = true, string actionText = "TALK", Sprite icon = null)
    {
        DIALOGUE.ShowHoverActionInfo(pos, isShow, actionText);
    }

    public void StartDialogue(MovementController player, GameBoard board)
    {
        DIALOGUE.DialoguePlay.StartDialogue(player, board);
    }


    public void ModifyMoney(float value)
    {
        GM.money += value;
        OnChangeMoney?.Invoke();
    }

    public void ModifyEnergy(float value, bool pauseStamina = false)
    {
    //    GM.STAMINA.AddStamina(value, pauseStamina);
        OnChangeHealth?.Invoke();
    }


    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
            CopyGameData();
            OnChangeMoney?.Invoke();
            OnChangeHealth?.Invoke();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CopyGameData()
    {
        for (int i = 0; i < DATA.facts.Length; i++)
        {
            List<int> H = new List<int>();
            DATA.facts[i].hints.ForEach((hint) => { H.Add(hint.id); });
            Fact F = new Fact(DATA.facts[i].factText, DATA.facts[i].id, DATA.facts[i].isCommand, H);
            facts.Add(F);
        }

        for (int i = 0; i < DATA.hints.Length; i++)
        {
            Hint H = new Hint(DATA.hints[i].hintText, DATA.hints[i].id);
            hints.Add(H);
            playerHints.Add(H);
        }
    }

    public IEnumerator WaitForPlayerPress()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }

    public IEnumerator WaitForPlayerChoose()
    {
        while (!playerIsChosing)
        {
            yield return null;
        }
    }

    public void PlayVO(AudioClip voClip)
    {
        VO.PlayOneShot(voClip);
    }
    public void StopVO()
    {
        VO.Stop();
    }
}
