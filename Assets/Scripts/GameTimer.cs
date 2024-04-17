using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] TextMeshProUGUI timerTexT;

    [SerializeField] int maxTime;

    [SerializeField] Animator textAnimator;

    int currentTime;

    void Start() 
    {
        currentTime = maxTime;

        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(1.5f);
        while(currentTime > 0)
        {
            textAnimator.SetTrigger("Idle");
            yield return new WaitForSeconds(1);
            PV.RPC(nameof(RPC_UpdateTimerValue),RpcTarget.All);
        }
        if(currentTime <= 0)
        {
            textAnimator.SetTrigger("Idle");
            Manager.Instance.PatrickWinGame();
            StopAllCoroutines();
        }
    }

    [PunRPC] void RPC_UpdateTimerValue()
    {
        currentTime--;
        timerTexT.text = currentTime.ToString();
        textAnimator.SetTrigger("Anim");
        timerTexT.color = Color.Lerp(timerTexT.color,Color.red,Time.deltaTime);
    }

}
