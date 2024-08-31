using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    GameMaster GM;

    public string levelName = "Testing Room";
    public GameObject[] cameras;

    private void Start()
    {
        GM = GameMaster.GM;
        GM.currentCamera = cameras[GM.activeCameraIndex].transform;
        ActivateCamera();
    }

    public void ActivateCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if(i == GM.activeCameraIndex)
            {
                GameMaster.GM.ChangeCamera(i, GM.currentCamera.transform.eulerAngles.y != cameras[i].transform.eulerAngles.y);
                GM.currentCamera = cameras[i].transform;
            }
            cameras[i].SetActive(i == GM.activeCameraIndex);
        }
    }
}
