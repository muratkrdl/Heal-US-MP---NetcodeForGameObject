using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameText;

    public RoomInfo info;

    public void SetRoom(RoomInfo _info)
    {
        info = _info;
        roomNameText.text = info.Name;
    }

    public void JoinRoomEvent()
    {
        Launcher.Instance.JoinRoom(info);
    }

}
