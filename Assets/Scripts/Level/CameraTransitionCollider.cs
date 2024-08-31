using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionCollider : MonoBehaviour
{
    public LevelManager levelManager;
    public int cameraIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameMaster.GM.activeCameraIndex != cameraIndex)
            {
                GameMaster.GM.activeCameraIndex = cameraIndex;
                levelManager.ActivateCamera();
            }
        }
    }
}
