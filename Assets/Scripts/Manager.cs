using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    [SerializeField] PhotonView PV;

    [SerializeField] Camera cam;
    [SerializeField] Image blackImage;

    [SerializeField] GameObject floatingTextPrefab;

    PlayerManager playerManager;

    public PlayerManager SetPlayerManager
    {
        set
        {
            playerManager = value;
        }
    }

    public void SetCamera(bool value)
    {
        cam.gameObject.SetActive(value);
        blackImage.gameObject.SetActive(false);
    }

    void Awake() 
    {
        Instance = this;
    }

    void Start() 
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Invoke(nameof(DisableBlackImage),2);
    }

    void DisableBlackImage()
    {
        if(blackImage.gameObject.activeSelf)
        {
            blackImage.GetComponent<Animator>().SetTrigger("UnFade");
        }
    }

    public void SpawnFloatingText(Vector3 pos,string str,Color color)
    {
        if(PV.IsMine) { return; }
        var text = Instantiate(floatingTextPrefab,pos,Quaternion.identity);
        text.GetComponent<FloatingText>().SetTextValues(str,color);
    }

    public void PatrickLoseGame()
    {
        PV.RPC(nameof(RPC_PatrickLoseGame), RpcTarget.All);
    }

    [PunRPC] void RPC_PatrickLoseGame()
    {
        playerManager.GetFPSPlayer.GetComponentInChildren<EscMenu>().SetFade();
        Invoke(nameof(LoadPatrickLoseLevel), 1.5f);
    }

    void LoadPatrickLoseLevel()
    {
        PhotonNetwork.LoadLevel(3);
    }

    public void PatrickWinGame()
    {
        PV.RPC(nameof(RPC_PatrickWinGame), RpcTarget.All);
    }

    [PunRPC] void RPC_PatrickWinGame()
    {
        playerManager.GetFPSPlayer.GetComponentInChildren<EscMenu>().SetFade();
        Invoke(nameof(LoadPatrickWinLevel), 1.5f);
    }

    void LoadPatrickWinLevel()
    {
        PhotonNetwork.LoadLevel(4);
    }

    public void CheckPatrickLose()
    {
        int deadCounter = 0;
        PV.RPC(nameof(SendToMaster),RpcTarget.MasterClient, "checking");
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if(player.CustomProperties.ContainsKey("Dead"))
            {
                deadCounter++;
            }
        }
        if(deadCounter >= (PhotonNetwork.PlayerList.Length -2) / 2)
        {
            //PatrickLoseGame();
            Debug.Log("patrick lose");
        }
        PV.RPC(nameof(SendToMaster),RpcTarget.MasterClient, deadCounter.ToString());
    }

    //sil 
    [PunRPC] void SendToMaster(string str)
    {
        Debug.Log(str);
    }

}
