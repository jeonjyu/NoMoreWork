using UnityEngine;
using System;

public class OutTrigger : MonoBehaviour
{
    public event Action PlayerEnteredNextStage;

    // Trigger 영역에 들어가면 다음 스테이지로 넘어감
    private void OnTriggerEnter(Collider other)
    {
        if (StageManager.Instance.IsStageClear && StageManager.Instance.CurrentStage < 10)
        {
            if (other.CompareTag("Player") && PlayerEnteredNextStage != null)
            {
                // InitStage 이벤트 발생
                Debug.Log($"[OutTrigger] 다음 스테이지로 가자");
                PlayerEnteredNextStage?.Invoke();
            }
        }
    }
}
