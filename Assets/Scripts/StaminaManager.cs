using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaManager : MonoBehaviour
{
    GameMaster GM;
    public float energy = 3f;
    public Image[] energybar;
    int activeStamina = 2;
    public float hourDecreaseStamina = 4;

    public Sequence staminaSequence;
    public bool decreaseStamina = true;
    
    private void Start()
    {
        activeStamina = GetActiveStaminaBar();
        GM = GameMaster.GM;
        DecreaseStamina();

        GM.OnChangeHealth += () =>
        {
            DisplayStamina();
        };
    }

    public void DecreaseStamina()
    {
        decreaseStamina = true;
        for(int i = 0; i < energybar.Length; i++)
        {
            if (i == activeStamina)
                energybar[i].fillAmount = energy - activeStamina;
            else if (i < activeStamina)
                energybar[i].fillAmount = 1;
            else
                energybar[i].fillAmount = 0;
        }
    
        staminaSequence = DOTween.Sequence();
        staminaSequence.Append(
            energybar[activeStamina].DOFillAmount(0, GetStaminaTime()).SetEase(Ease.Linear).OnComplete(() =>
            {
                ChangeStaminaBar();
            })
        );
    }

    public IEnumerator IncreaseStaminaIE()
    {
        for (int i = 0; i <= activeStamina; i++)
        {
            if(energybar[i].fillAmount < 1)
            {
                float endValue = energy - i;
                if(activeStamina != i)
                {
                    endValue = 1;
                }
                energybar[i].DOFillAmount(endValue, 0.5f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    void ChangeStaminaBar()
    {
        activeStamina--;
        if (activeStamina >= 0)
        {
            DecreaseStamina();
        }
        else
        {
            //Debug.Log("DEATH!");
        }
    }

    public void ResumeStamina(bool isPlay = true)
    {
        decreaseStamina = isPlay;
        if (staminaSequence != null)
        {
            if (isPlay)
            {
                DecreaseStamina();
            }
            else
                staminaSequence.Pause();
        }
    }

    private void Update()
    {
        DisplayStamina();
    }

    public void DisplayStamina()
    {
        if (decreaseStamina && activeStamina >= 0)
        {
            energy = activeStamina + energybar[activeStamina].fillAmount;
        }
    }

    public float GetStaminaTime()
    {
        //return (energy - activeStamina) * hourDecreaseStamina * GM.TIME.tenMinuteDelay * 6;
        return 0;
    }

    int GetActiveStaminaBar()
    {
        int staminaBar = Mathf.CeilToInt(energy);
        return staminaBar - 1;
    }

    public void AddStamina(float value, bool pausingStamina = false)
    {
        StartCoroutine(AddStaminaIE(value, pausingStamina));
    }
    IEnumerator AddStaminaIE(float value, bool pausingStamina)
    {
        ResumeStamina(false);
        staminaSequence.Kill();
        decreaseStamina = false;
        energy += value;
        if (energy >= 3f)
        {
            energy = 3f;
        }
        activeStamina = GetActiveStaminaBar();
        yield return StartCoroutine(IncreaseStaminaIE());
        
        if (!pausingStamina)
        {
            DecreaseStamina();
        }
    }
}
