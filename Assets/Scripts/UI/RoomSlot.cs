using UnityEngine;

public class RoomSlot : MonoBehaviour
{
    public string roomName;
    public int currentPlayerCount;
    public int masterClientId;

    public RoomSlot(string roomName, int curPlayerCnt, int masterId)
    {
        this.roomName = roomName;
        this.currentPlayerCount = curPlayerCnt;
        this.masterClientId = masterId;
    }
}
