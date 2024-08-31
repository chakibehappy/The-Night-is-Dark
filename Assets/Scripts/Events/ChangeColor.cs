using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aceline.RPG.Test;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private int myNumber; // work like Id
    private List<Material> materials = new List<Material>();

    private void Awake()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
        {
            foreach (Material mat in smr.materials)
            {
                materials.Add(mat);
            }
        }
    }

    private void Start()
    {
        GameEvents.Instance.ChangeColorModel += DoRandomColorModel;
    }

    private void OnDisable()
    {
        GameEvents.Instance.ChangeColorModel -= DoRandomColorModel;
    }

    private void DoRandomColorModel(int number)
    {
        if (myNumber == number)
        {
            foreach (Material mat in materials)
            {
                mat.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            }
        }    
    }
}
