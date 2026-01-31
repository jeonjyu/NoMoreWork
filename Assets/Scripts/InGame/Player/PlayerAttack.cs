using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviourPun
{
    public InputActionAsset _inputActions;

    [SerializeField] Transform firePos;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] GameObject bulletSpawner;

    InputAction _attackAction;

    Animator _animator;

    PhotonView pv;

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
        ObjectPoolManager.Instance.Init(bulletPrefab, 10, bulletSpawner.transform);
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (!pv.IsMine) return;

        Debug.Log("[PlayerAttack] Awake");

        _attackAction = InputSystem.actions.FindAction("Attack");
        _animator = GetComponent<Animator>();

    }

    void Update()
    {
        if(!pv.IsMine) return;

        if (_attackAction.WasPressedThisFrame())
        {
            Debug.Log("[PlayerAttack] 공격키 눌림");
            //Attack();
            // RPC 전송
            // 몹한테만 뎀이 들어가면 되면 Master를 해야 하는지 ALl을 해야 하는지
            pv.RPC(nameof(Attack), RpcTarget.All);
        }
    }

    // actorNum으로 오브젝트풀링?? 할수도 있다고 한다 
    // 오브젝트 풀링이 필요하면 넣고 아니면 그냥 actorNum 없이
    //[PunRPC]
    //void FireBullet(int actornum)
    //{
    //    GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    //    bullet.GetComponent<Bullet>().actorNumber = actornum;
    //}

    

    [PunRPC]
    public void Attack()
    {
        // 공격 호출
        Debug.Log("공격");
        _animator?.SetTrigger("Attack");
        // 오브젝트 풀링으로 변경
        Bullet bullet = ObjectPoolManager.Instance.CreateObjWithUsePool(bulletPrefab, bulletSpawner.transform);
        //bullet.transform.parent = firePos.transform;
        //bullet.transform.position = firePos.position;
        //bullet.transform.rotation = firePos.rotation;
    }
}
