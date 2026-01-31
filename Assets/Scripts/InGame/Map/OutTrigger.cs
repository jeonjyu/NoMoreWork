using UnityEngine;
using System;

public class OutTrigger : MonoBehaviour
{
    public event Action PlayerEnteredNextStage;

    // Trigger 영역에 들어가면 다음 스테이지로 넘어감
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerEnteredNextStage != null)
        {
            // InitStage 이벤트 발생
            PlayerEnteredNextStage?.Invoke();
        }
    }
}
