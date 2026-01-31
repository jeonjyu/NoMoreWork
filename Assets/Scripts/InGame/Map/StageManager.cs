using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Tilemaps;
using Cinemachine;
using TMPro;

// 스테이지 클리어 
// 다음 스테이지 생성
// 현재 스테이지 문 열기
// 다음 스테이지로 이동하면 스테이지 요소들 초기화
// 스테이지 요소 : 클리어 UI, 퍼즐들 리스트, 클리어 퍼즐 딕셔너리, 스테이지 클리어 여부, 


public enum StageState
{
    Ready, Playing, Clear
}

// 맵 교체 및 오브젝트 생성

// PuzzleButton의 버튼만 모아서 클리어 여부 확인
// 스테이지 클리어 판정
public class StageManager : SingletonPun<StageManager>, IPunObservable
{
    [Tooltip("현재 오브젝트들을 스폰시킬 스테이지")]
    public Map currentMap;
    [SerializeField] GameObject objSpawner;
    [SerializeField] GameObject clearAlertUI;
    [SerializeField] GameObject puzzles;
    public CinemachineVirtualCamera virtualCamera;

    public int _currentStage = 1;
    bool _isStageClear = false;
    public List<PuzzleButton> puzzleButtons = new List<PuzzleButton>();
    public static Dictionary<int, bool> clearedPuzzle = new Dictionary<int, bool>();

    GameObject _gate;
    Animator _animator;

    public bool IsStageClear => _isStageClear;

    public event EventHandler OnClearStage;

    void Start()
    {
        // 호스트가 아닌 플레이어도 문으로 이동하면 맵 이동하기 위해 
        // 이벤트 구독/해제 Init과 별개로 수행하기

        //if (currentMap != null && currentMap.outTrigger != null)
        //{
        //    Debug.Log($"[StageManger] outTrigger 이벤트 구독");
        //    currentMap.outTrigger.PlayerEnteredNextStage += InitStage;
        //}

        Debug.Log($"[StageManager] 플레이어 수 : {playerCount}");
        
        // 맵 오브젝트는 모든 클라이언트가 로컬에 생성
        SetMapObj();
        
        // 호스트가 현재 맵의 설정을 진행 후 각 클라이언트에게 전달
        if (PhotonNetwork.IsMasterClient)
        {
            InitStage();
            //photonView.RPC(nameof(InitStage), RpcTarget.All);
        }

    }
    private void FixedUpdate()
    {
        // 호스트만 처리하고 RPC로 클리어 여부 쏴주기
        // 클라이언트는 RPC로 클리어 여부 받아서 클리어 출력 처리

        // activatedPuzzles에서 value가 true인 애들 갯수 비교
        // 클리어 이벤트 받아서 클리어 처리하는 게 좋을듯
        //if (!_isStageClear && puzzles.Count == _clearedPuzzleCount)
        //{
        //    ClearStage();
        //}
    }

    // todo : OutTrigger에 트리거 설정해서 실행시키기

    // 플레이어 제외 오브젝트들 존재할 경우 모두 풀로 집어넣기
    // 맵 피벗 지정
    // 맵 피벗따라 맵, 퍼즐 오브젝트 변경
    [PunRPC]
    public void InitStage()
    {
        Debug.Log($"[StageManager] 스테이지 초기화");

        // 두번째 스테이지 변경시부터 이벤트 구독 취소
        if (_currentStage != 1)
        {
            ClearStage();
        }

        // 맵 설정
        currentMap = maps[_currentStage % 2];
        Debug.Log($"[StageManager] 현재 맵 : {currentMap}({_currentStage} 층)");
        floorUIText.text = _currentStage + "층";

        ChangeMap(currentMap);
    }

    // 피벗 따라 맵, 퍼즐 기준 변경
    private void ChangeMap(Map map)
    {
        Debug.Log($"[StageManager] 맵 변경");
        this.puzzleObjects = map.puzzleObjects;
        this.mapObjects = map.mapObjects;
        this._gate = map.gate;
        this.tilemap = map.tilemap;
        this.virtualCamera = map.virtualCamera;
        virtualCamera.gameObject.SetActive(true);

        if (currentMap != null && currentMap.outTrigger != null)
        {
            Debug.Log($"[StageManger] outTrigger 이벤트 구독");
            currentMap.outTrigger.PlayerEnteredNextStage += InitStage;
        }

        puzzleButtons.Clear();
        puzzleButtons = puzzles.GetComponentsInChildren<PuzzleButton>().ToList();
        _gate = GameObject.Find("OutGate");
        _animator = _gate.GetComponent<Animator>();
        _animator.SetBool("IsOpened", false);
    }

    // 랜덤 위치에 퍼즐 생성하고 퍼즐 이름 변경, 번호 부여
    public void CreatePuzzle()
    {

    }

    public void UpdateClearedPuzzle(int puzzleNumber)
    {
        if (!clearedPuzzle.ContainsKey(puzzleNumber))
            clearedPuzzle.Add(puzzleNumber, true);
        else 
            Debug.LogWarning($"[StageManager] 버튼 {puzzleNumber}를 클리어한 퍼즐에 넣을 수 없음");
    }

    public void SetLocalPlayerCamera(Transform localPlayer)
    {
        if(virtualCamera == null)
        {
            Debug.LogError("[StageManager] 가상 카메라가 할당되어 있지 않음");
            return;
        }
        
        virtualCamera.Follow = localPlayer;
        virtualCamera.LookAt = localPlayer;
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
