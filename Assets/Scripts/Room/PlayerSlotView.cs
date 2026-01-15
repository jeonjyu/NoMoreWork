using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
// view
// 클릭으로 model 값 바뀌도록 
[RequireComponent(typeof(PlayerSlotPresenter))]
public class PlayerSlotView : MonoBehaviour
{
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private TMP_Text _nickname;
    [SerializeField] private Image _readyIcon;
    //[SerializeField] private Button _readyButton;

    PlayerSlotPresenter _presenter;

    private void Start()
    {
        //_presenter = new PlayerSlotPresenter(this, new PlayerSlotModel(PhotonNetwork.LocalPlayer));
    }

    public void Init(Player player)
    {
        // 프레젠터 생성
        _presenter = GetComponent<PlayerSlotPresenter>();
        _presenter.Init(player);

        // 슬롯에 플레이어 닉네임 설정
        Debug.Log($"[PlayerSlotView] {player.UserId}");
        _nickname.text = player.UserId;
        
        ChangeReadyStatus(false);
        //_readyButton.interactable = true;
    }

    public void ChangeReadyStatus(bool isReady)
    {
        _readyIcon.gameObject.SetActive(isReady);
    }

    public void OnClickButton()
    {
        _presenter.ChangeReadyStatus();
        //_readyButton.interactable = false;
    }
}
