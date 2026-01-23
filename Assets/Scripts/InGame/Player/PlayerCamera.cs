using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    static CinemachineVirtualCamera _virtualCamera;

    private void Start()
    {
        Debug.Log("[PlayerCamera] Start");
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// 로컬 플레이어의 카메라 설정
    /// </summary>
    /// <param name="localplayer">시네머신이 Follow, LookAt 할 대상</param>
    public static void SetPlayerCamera(Transform localplayer)
    {
        if(localplayer == null)
        {
            Debug.Log("[PlayerCamera] 플레이어를 찾을 수 없습니다");
        }
        _virtualCamera.Follow = localplayer;
        _virtualCamera.LookAt = localplayer;
    }
}
