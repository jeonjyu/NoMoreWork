using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// 스테이지 클리어 
// 다음 스테이지 생성
// 현재 스테이지 문 열기
// 다음 스테이지로 이동하면 스테이지 요소들 초기화
// 스테이지 요소 : 클리어 UI, 퍼즐들 리스트, 클리어 퍼즐 딕셔너리, 스테이지 클리어 여부, 

// PuzzleButton의 버튼만 모아서 클리어 여부 확인
// 스테이지 클리어 판정
public class StageManager : Singleton<StageManager>, IPunObservable
{
    [SerializeField] GameObject clearAlertUI;
    [SerializeField] GameObject puzzles;

    static int _currentStage;
    bool _isStageClear = false;
    public List<PuzzleButton> puzzleButtons = new List<PuzzleButton>();
    public static Dictionary<int, bool> clearedPuzzle = new Dictionary<int, bool>();

    GameObject _gate;
    Animator _animator;

    public bool IsStageClear => _isStageClear;

    public event EventHandler OnClearStage;

    void Start()
    {
        // 퍼즐 생성하고 번호 부여
        // 테스트할 때는 인스펙터에서 임의로 지정

        //puzzleButtons.Clear();
        puzzleButtons = puzzles.GetComponentsInChildren<PuzzleButton>().ToList();
        _gate = GameObject.Find("OutGate");
        _animator = _gate.GetComponent<Animator>();
        _animator.SetBool("IsOpened", false);
    }

    private void FixedUpdate()
    {
        // 딕셔너리에서 clear == true인 애들 갯수랑 전체 갯수 비교
        // 갯수 동일하면 스테이지 클리어 
        // FixedUpdate 말고 이벤트로 해도 될 것 같은데
        
        if(!_isStageClear && clearedPuzzle.Count == puzzleButtons.Count)
        {
            ClearStage();
        }
    }

    public void UpdateClearedPuzzle(int puzzleNumber)
    {
        if (!clearedPuzzle.ContainsKey(puzzleNumber))
            clearedPuzzle.Add(puzzleNumber, true);
        else 
            Debug.LogWarning($"[StageManager] 버튼 {puzzleNumber}를 클리어한 퍼즐에 넣을 수 없음");
    }

    public void ClearStage()
    {
        _isStageClear = true;
        Debug.Log("[StageManager] 스테이지 클리어!");
        clearAlertUI.SetActive(true);
        _animator.SetBool("IsOpened", true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isStageClear);
        }
        else
        {
            this._isStageClear = (bool)stream.ReceiveNext();
        }
    }
}
