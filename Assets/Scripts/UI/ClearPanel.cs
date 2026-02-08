using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Realtime;

public class ClearPanel : MonoBehaviour
{

    [SerializeField] Button exitButton;
    [SerializeField] GameObject playersPanel;
    [SerializeField] GameObject menuCanvas;
    TMP_Text[] playerTexts;

    private void Awake()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (menuCanvas.activeInHierarchy)
            menuCanvas.SetActive(false);

        playerTexts = playersPanel.GetComponentsInChildren<TMP_Text>();
        Player[] players = PhotonNetwork.PlayerList;
        
        for(int i = 0; i < players.Length; i++)
        {
            players[i].CustomProperties.TryGetValue("FirebaseName", out object name);
            playerTexts[i].text = (string)name;
        }
    }
}
