using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNearestNPC : MonoBehaviour
{
    public List<GameObject> nearestNPC = new List<GameObject>(); 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if (!nearestNPC.Contains(other.gameObject))
            {
                nearestNPC.Add(other.gameObject);
                if (nearestNPC.Count > 1)
                {
                    GameMaster.GM.waitingToTalkDelayNPC = 0.5f;
                }
                else
                {
                    GameMaster.GM.waitingToTalkDelayNPC = 2.5f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            if (nearestNPC.Contains(other.gameObject))
            {
                nearestNPC.Remove(other.gameObject);
                if (nearestNPC.Count > 1)
                {
                    GameMaster.GM.waitingToTalkDelayNPC = 0.5f;
                }
                else
                {
                    GameMaster.GM.waitingToTalkDelayNPC = 2.5f;
                }
            }
        }
    }
}
