using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    GameMaster GM;
    private bool isPlayerEnter = false;
    bool playDialog;

    public bool showDialogue = false;
    public string[] dialogLine;

    public bool isGettingItem = false;
    public ItemData item;
    public int count = 1;

    public bool destroyAfterInteraction = true;

    private void Start()
    {
        GM = GameMaster.GM;

    }

    private void Update()
    {
        if (isPlayerEnter)
        {
            GM.DIALOGUE.ShowActionInfo(true, "Check");
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PlayObjectInteractionIE());
            }
        }
    }

    IEnumerator PlayObjectInteractionIE()
    {
        PauseMovement();
        GM.DIALOGUE.ShowActionInfo(false);
        if (!playDialog)
        {
            yield return StartCoroutine(GM.DIALOGUE.ShowDialogueIE(new string[] { "DETECTIVE" }, dialogLine));
        }
        if (isGettingItem)
        {
            GameMaster.GM.INVENTORY.AddItemToInventory(item, count);
            string[] itemInfo = new string[] { "You got " + item.itemName + " x " + count.ToString()};
            yield return StartCoroutine(GM.DIALOGUE.ShowDialogueIE(new string[] { "" }, itemInfo));
        }

        PauseMovement(false);
        if (destroyAfterInteraction)
        {
            Destroy(gameObject);
        }
    }

    void PauseMovement(bool isTalk = true)
    {
        playDialog = isTalk;
        isPlayerEnter = !isTalk;
        GM.canMove = !isTalk;
        //GM.TIME.ResumeTime(!isTalk);
        //GM.STAMINA.ResumeStamina(!isTalk);
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerIsEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerIsEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerEnter)
        {
            if (other.CompareTag("Player"))
            {
                GM.interactWith = null;
                isPlayerEnter = false;
                GM.DIALOGUE.ShowActionInfo(false);
            }
        }
    }

    void PlayerIsEnter(Collider other, bool isEnter = true)
    {
        if (other.CompareTag("Player"))
        {
            if (GM.interactWith == null)
            {
                GM.interactWith = gameObject;
                if (!playDialog)
                {
                    isPlayerEnter = isEnter;
                }
            }
        }
    }
}
