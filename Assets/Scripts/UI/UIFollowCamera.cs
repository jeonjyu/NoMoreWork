using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    // UI의 rotation을 버추얼 카메라를 향하도록 한다
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
