using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    GameMaster GM;
    public GameObject inventoryUI;
    public GameObject[] itemSlot;
    public List<ItemData> itemData;
    public List<ItemData> itemOnBag;
    public int[] itemCountOnBag;

    public Color activeItemColor = Color.yellow;
    public Color inactiveItemColor = Color.white;
    public GameObject descriptionBox;
    public TextMeshProUGUI descriptionNameText;
    public TextMeshProUGUI descriptionText;

    private void Start()
    {
        GM = GameMaster.GM;
        FillItemCountOnBag();
        DisplayInventory(false);
    }

    void FillItemCountOnBag()
    {
        itemCountOnBag = new int[itemOnBag.Count];
        for (int i = 0; i < itemOnBag.Count; i++)
        {
            itemCountOnBag[i] = itemOnBag[i].count;
        }
    }

    public void DisplayInventory(bool isDisplay = true)
    {
        for(int i = 0; i < itemSlot.Length; i++)
        {
            if(i < itemOnBag.Count)
            {
                itemSlot[i].SetActive(true);
                itemSlot[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemOnBag[i].itemName;
                itemSlot[i].transform.GetChild(2).GetComponent<Image>().sprite = itemOnBag[i].icon;
                if (itemCountOnBag[i] > 1)
                {
                    itemSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = itemCountOnBag[i].ToString();
                }
                else
                {
                    itemSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "";
                }

                EventTrigger trigger = itemSlot[i].GetComponent<EventTrigger>();

                int index = i;
                EventTrigger.Entry entryDown = new EventTrigger.Entry();
                entryDown.eventID = EventTriggerType.PointerDown;
                entryDown.callback.AddListener((eventData) => {
                    UseItem(itemOnBag[index]);
                });

                EventTrigger.Entry entryEnter = new EventTrigger.Entry();
                entryEnter.eventID = EventTriggerType.PointerEnter;
                entryEnter.callback.AddListener((eventData) => { ShowItemDescription(true, index); });

                EventTrigger.Entry entryExit = new EventTrigger.Entry();
                entryExit.eventID = EventTriggerType.PointerExit;
                entryExit.callback.AddListener((eventData) => { ShowItemDescription(false, index); });

                trigger.triggers.Clear();
                trigger.triggers.Add(entryDown);
                trigger.triggers.Add(entryEnter);
                trigger.triggers.Add(entryExit);

            }
            else
            {
                itemSlot[i].SetActive(false);
            }
        }
        inventoryUI.SetActive(isDisplay);
        
        if(!isDisplay)
            ShowItemDescription(false);
    }

    void ShowItemDescription(bool isShow = true, int index = 0)
    {
        Color col = isShow ? activeItemColor : inactiveItemColor;
        itemSlot[index].transform.GetChild(0).GetComponent<Image>().color = col;
        descriptionNameText.text = itemOnBag[index].itemName;
        descriptionText.text = itemOnBag[index].description;
        descriptionBox.SetActive(isShow);
    }

    public void AddItemToInventory(ItemData item, int itemCount = 1)
    {
        if(itemOnBag.Contains(item))
        {
            int itemindex = itemOnBag.IndexOf(item);
            itemCountOnBag[itemindex] += itemCount;
        }
        else
        {
            itemOnBag.Add(item);
            FillItemCountOnBag();
            itemCountOnBag[itemCountOnBag.Length - 1] = itemCount;
        }
        DisplayInventory(false);
    }

    public void ReduceItemFromInventory(ItemData item, int itemCount = 1)
    {
        if (itemOnBag.Contains(item))
        {
            int itemindex = itemOnBag.IndexOf(item);
            itemCountOnBag[itemindex] -= itemCount;
            if (itemCountOnBag[itemindex] <= 0)
            {
                itemOnBag.Remove(item);
                FillItemCountOnBag();
                ShowItemDescription(false);
            }
        }
        DisplayInventory();
    }

    public void UseItem(ItemData item)
    {
        if(item.isConsumable)
        {
            int i = itemOnBag.IndexOf(item);
            if (itemCountOnBag[i] > 0)
            {
                ReduceItemFromInventory(item);
                GM.ModifyEnergy(item.energy, true);
            }
        }
    }

}
