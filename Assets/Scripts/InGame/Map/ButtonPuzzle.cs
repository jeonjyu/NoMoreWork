using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPuzzle : PuzzleBase, IPunObservable
{
    //기능

    //퍼즐 클리어 시간 측정
    //페달 영역 안에 들어가면 버튼이 아래로 살짝 이동하도록 설정
    //페달 영역 안에 들어가면 시간 게이지가 뜨고, 시간에 따라 게이지가 증가하도록

    //네트워크

    // 퍼즐에서 측정중인 시간, 클리어 여부가 다른 플레이어에게 동기화
    // 스테이지에서 클리어 여부 체크

    public int puzzleNumber;

    [SerializeField] float _clearValue = 10;
    float _startTime;
    float _endTime;
    public float _savedValue;

    public bool _pressed = false;
    public bool _isClear = false;

    [SerializeField] Slider slider;

    [SerializeField] GameObject ButtonObject;

    Animator _animator;

    void Start()
    {
        slider.maxValue = _clearValue;
        _animator = ButtonObject.GetComponent<Animator>();
        slider.value = 0;
        _savedValue = 0;
        _isClear = false;
    }

    void Update()
    {
        // _isPressed면 게이지 갱신
        if (_pressed && _isClear == false)
        {
            // 기존 게이지 중간 저장 값에 추가 
            slider.value = _savedValue + Time.time - _startTime; // 현재 시간 - 시작 시간 -> 마지막 startTime이 시작 시간이 됨, 중간 시작점을 저장해야 하는가?

            if (slider.value == slider.maxValue)
            {
                _isClear = true;
                Debug.Log($"[PuzzleButton] 버튼 {puzzleNumber} 클리어");
                StageManager.Instance.ClearedPuzzleCount++;
                return;
            }
        }
    }


    public void InitPuzzle()
    {
        slider.value = 0;
        _savedValue = 0;
        _isClear = false;
    }

    // 영역 안에 들어오면 토글 온 애니메이터
    // 시간 측정 시작
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isClear == false)
        {
            Debug.Log("[PuzzleButton] 버튼으로 진입");
            // 아래로 내리는 animator
            _animator.SetBool("IsPressed", true);
            _pressed = true;

            _startTime = Time.time;
        }
    }

    // 영역 밖으로 나오면 토글 오프 애니메이터
    // 시간 측정 중단 
    // 중간 값 저장
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _pressed == true)
        {
            Debug.Log("[PuzzleButton] 버튼에서 벗어남");
            // 위로 올리는 animator
            _animator.SetBool("IsPressed", false);
            _pressed = false;

            // 진척도 계산
            _endTime = Time.time;
            _savedValue += _endTime - _startTime;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isClear);
            stream.SendNext(_savedValue);
        }
        else
        {
            this._isClear = (bool)stream.ReceiveNext();
            this._savedValue = (float)stream.ReceiveNext();
        }
    }
}
