using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerTexT;

    [SerializeField] int maxTime;

    [SerializeField] Animator textAnimator;

    int currentTime;

    void Start() 
    {
        currentTime = maxTime;
    }

    public void StartGameTimer()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(4);
        while(currentTime > 0)
        {
            textAnimator.SetTrigger("Idle");
            yield return new WaitForSeconds(1);
            UpdateTimerValue();
        }
        if(currentTime <= 0)
        {
            textAnimator.SetTrigger("Idle");
            Manager.Instance.PatrickWinGame();
            StopAllCoroutines();
        }
    }

    void UpdateTimerValue()
    {
        currentTime--;
        UpdateTimerValueServerRpc(currentTime);
    }

    [ServerRpc(RequireOwnership = false)] void UpdateTimerValueServerRpc(int _time)
    {
        textAnimator.SetTrigger("Anim");
        UpdateTimerValueClientRpc(_time);
    } 

    [ClientRpc] void UpdateTimerValueClientRpc(int _time)
    {
        timerTexT.text = _time.ToString();
        timerTexT.color = Color.Lerp(timerTexT.color,Color.red,Time.deltaTime);
    }

}
