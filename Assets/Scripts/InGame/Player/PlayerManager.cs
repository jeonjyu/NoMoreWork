using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject Weapon;
    public static GameObject LocalPlayerInstance;

    [SerializeField] float _health = 100.0f;
    bool isInteractioning;

    private void Awake()
    {

    }

    //private void OnEnable()
    //{
    //    base.OnEnable();

    //    if (!photonView.IsMine) return;

    //    // 포톤뷰가 내거라면 inputAction 활성화

        
    //}

    //private void OnDisable()
    //{
        
    //}

    // 공격 입력 이벤트 처리

    // 공격 시 애니메이션 처리

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