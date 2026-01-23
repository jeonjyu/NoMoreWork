using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int actorNumber;
    
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 400f);

        Destroy(gameObject, 1.0f);
    }


    // 충돌하면 소멸
}
