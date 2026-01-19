using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour, IPunObservable
{
    InputAction _playerInput;
    public static GameObject LocalPlayerInstance;

    private void Start()
    {
        // 카메라워크 설정
    }

    private void Awake()
    {
        //_inputAction = InputSystem.actions[];
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}