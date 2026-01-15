using UnityEngine;
// presenter
// 뷰 - 모델 데이터 동기화
// 
// 방장이 시작 누르면 
public class PlayerSlotPresenter : MonoBehaviour, IPlayerPresenter
{
    [SerializeField] PlayerSlotView _view;
    [SerializeField] PlayerSlotModel _model;

    public PlayerSlotPresenter(PlayerSlotView view, PlayerSlotModel model)
    {
        _view = view;
        _model = model;
    }

    public void OnEnable()
    {
        if (_model != null)
        {
            _model.PlayerSlotChanged += OnPlayerChanged;
            UpdateUI();
        }
    }

    public void ChangeReadyStatus()
    {
        _model.IsReady = !_model.IsReady;
    }

    public void OnDisable()
    {
        if (_model != null) _model.PlayerSlotChanged -= OnPlayerChanged;
    }

    private void OnPlayerChanged() => UpdateUI();

    public void UpdateUI()
    {
        _view.ChangeReadyStatus(_model.IsReady);
    }
}
