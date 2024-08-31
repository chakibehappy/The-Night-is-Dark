using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    GameMaster GM;
    [SerializeField] LayerMask clickableLayers;

    void Start()
    {
        GM = GameMaster.GM;    
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            GM.ShowHoverActionInfo(Input.mousePosition, layerName == "NPC", "Talk");
        }
        else
        {
            GM.ShowHoverActionInfo(Vector2.zero, false);
        }
    }
}
