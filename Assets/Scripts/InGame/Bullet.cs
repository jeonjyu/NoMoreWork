using UnityEngine;

public class Bullet : ObjectBase
{
    Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        _rigidbody.AddRelativeForce(this.transform.forward * 400f);
        //_rigidbody.AddForce(this.transform.forward * 400f, ForceMode.Impulse);
    }
    
    // 소멸 조건 추가

    // 일정 거리 이상 가면 디스폰, 그 전에 충돌하면 디스폰
    public void Despawn()
    {
        ObjectPoolManager.Instance.SetObjInPool(this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // other에 데미지 
            //other.TakeDamage();
        }

        ObjectPoolManager.Instance.SetObjInPool(this);
        //transform.position = transform.parent.position;
        //transform.rotation = transform.parent.rotation;
    }
}
