using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
// view
// 클릭으로 model 값 바뀌도록 
public class PlayerSlotView : MonoBehaviour
{
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private TMP_Text _nickname;
    [SerializeField] private Image _readyIcon;
    //[SerializeField] private Button _readyButton;

    PlayerSlotPresenter _presenter;

    private void Start()
    {
        _presenter = new PlayerSlotPresenter(this, new PlayerSlotModel(PhotonNetwork.LocalPlayer));
    }

    public void Init(string nickname)
    {
        _presenter = new PlayerSlotPresenter(this, new PlayerSlotModel(PhotonNetwork.LocalPlayer));
        Debug.Log($"[PlayerSlotView] {nickname}");
        _nickname.text = nickname;
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
