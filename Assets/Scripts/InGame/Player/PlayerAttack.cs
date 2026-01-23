using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviourPun
{
    public InputActionAsset _inputActions;

    [SerializeField] Transform firePos;
    [SerializeField] GameObject bulletPrefab;

    InputAction _attackAction;

    Animator _animator;

    //PhotonView photonView;

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        if (!photonView.IsMine) return;

        Debug.Log("[PlayerAttack] Awake");

        _attackAction = InputSystem.actions.FindAction("Attack");
        _animator = GetComponent<Animator>();

        //photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(!photonView.IsMine) return;

        if (_attackAction.WasPressedThisFrame())
        {
            Debug.Log("[PlayerAttack] 공격키 눌림");
            Attack();
            // RPC 전송
            // 몹한테만 뎀이 들어가면 되면 Master를 해야 하는지 ALl을 해야 하는지
            photonView.RPC(nameof(Attack), RpcTarget.Others);
        }
    }

    // actorNum으로 오브젝트풀링?? 할수도 있다고 한다 
    // 오브젝트 풀링이 필요하면 넣고 아니면 그냥 actorNum 없이
    [PunRPC]
    void FireBullet(int actornum)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actornum;
    }

    [PunRPC]
    public void Attack()
    {
        // 공격 호출
        Debug.Log("공격");
        _animator.SetTrigger("Attack");
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
