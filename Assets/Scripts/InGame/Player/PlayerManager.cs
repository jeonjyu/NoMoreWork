using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;


public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject Weapon;
    public static GameObject PlayerInstance;

    [SerializeField] float _health = 100.0f;
    string _name;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerNameDisplay;

    float testDamage = 40f;

    private void Awake()
    {
        playerNameDisplay = GameObject.Find("PlayerName").GetComponent<TMP_Text>();
        playerNameDisplay.text = "이름";
        // 현재 플레이어의 이름을 받아와 저장
        photonView.Owner.CustomProperties.TryGetValue("FirebaseName", out object name);
        playerName.text = (string)name;
        
        if (photonView.IsMine)
        {
            playerNameDisplay.text = (string)name;
        }
    }

    private IEnumerator Start()
    {
        // 생성한 로컬 플레이어를 카메라의 주시 대상으로 설정
        Debug.Log($"[PlayerManager] PlayerInstance : {PlayerInstance}를 주시 대상으로 설정");

        if (photonView.IsMine)
        {
            yield return null;

            playerNameDisplay.text = PlayerInstance.name + "의 뷰" + StageManager.Instance.virtualCamera.name;
            StageManager.Instance.SetLocalPlayerCamera(this.transform);
        }


    }

    // 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if(other.CompareTag("Enemy"))
        {
            // 체력 감소 처리
            TakeDamage(testDamage);
            // 상호작용 취소

            // 체력이 다 떨어지면 행동 불능 처리
        }
    }

    // 체력이 다 떨어지면 행동 불능 처리하는 메서드
    // 퍼즐 상호작용 중에 행동 불능 되면 상호작용 취소


    // 포톤뷰로 동기화할 것들
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}