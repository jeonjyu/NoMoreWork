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
    [SerializeField] PuzzleBase puzzlePrefab;

    public GameObject puzzleObjects;
    public GameObject mapObjects;
    public Tilemap tilemap;
    public CinemachineVirtualCamera virtualCamera;

    public int _currentStage = 1;
    bool _isStageClear = false;
    int playerCount = RoomManager.playerCount;
    int _clearedPuzzleCount = 0;

    [SerializeField] TMP_Text floorUIText;

    [SerializeField] public List<Map> maps = new List<Map>();
    // 클리어한 퍼즐 갯수를 체크하고 퍼즐들 리스트화 하는 자료구조로 변경하기
    public List<PuzzleBase> puzzles = new List<PuzzleBase>();
    //public static Dictionary<int, bool> activatedPuzzles = new Dictionary<int, bool>();
    
    [SerializeField] List<mapStructure> mapStructures = new List<mapStructure>();


    GameObject _gate;
    Animator _animator;

// properties
    public bool IsStageClear => _isStageClear;
    public int CurrentStage => _currentStage;

    public int ClearedPuzzleCount { get => _clearedPuzzleCount; set => _clearedPuzzleCount = value; }

    public delegate void StageChange(Map map);

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
            Debug.Log($"[StageManager] 1 스테이지 아님~");
            //virtualCamera.gameObject.SetActive(false);

            // 퍼즐 디스폰
            foreach (PuzzleBase puzzle in puzzles)
            {
                // 퍼즐이 있을 경우 풀로 반납
                if (puzzle != null)
                {
                    PhotonView photonView = puzzle.GetComponent<PhotonView>();
                    if (photonView != null) photonView.ViewID = 0;
                    ObjectPoolManager.Instance.SetObjInPool(puzzle);
                }
            }
            puzzles.RemoveAll(p => p == null);

            Debug.Log($"[StageManager] 퍼즐 풀에 반납");
            if (currentMap != null && currentMap.outTrigger != null)
            {
                Debug.Log($"[StageManger] outTrigger 이벤트 구독해제");
                //currentMap.outTrigger.PlayerEnteredNextStage -= InitStage;
            }
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

        _animator = _gate.GetComponent<Animator>();
        _animator.SetBool("IsOpened", false);

        // 플레이어 스폰 > map만 있으면 모두 같은 위치로 생성되니까 동기화 필요 없음

        // 호스트가 다른 클라이언트들에게 RPC로 오브젝트 스폰시킬 오브젝트 알림
        if (PhotonNetwork.IsMasterClient)
        {
            int currentStructure = UnityEngine.Random.Range(0, mapStructures.Count);
            SpawnMapObj(currentStructure);
            //photonView.RPC(nameof(SpawnMapObj), RpcTarget.All, currentStructure);
        }
        //SpawnMapObj(); 
    }

    // 게임씬 초기화 시 맵 오브젝트들 미리 생성해 풀에 넣음
    private void SetMapObj()
    {
        Debug.Log($"[StageManager] 맵 오브젝트 미리 생성");

        ObjectPoolManager.Instance.Init(puzzlePrefab, 4, objSpawner.transform);
        // 그리드 두개 만들고 오브젝트 풀에 넣고, 
        foreach(var mapObj in mapStructures)
        {
            ObjectPoolManager.Instance.Init(mapObj, 2, objSpawner.transform);
        }
    }

    // 플레이어들을 변경된 맵에 스폰
    private void SpawnPlayerToNextStage()
    {
        Player[] players = PhotonNetwork.PlayerList;
    }

    // 풀에 있는 오브젝트들 가져와서 스폰시키기
    // 위치도 지정
    [PunRPC]
    private void SpawnMapObj(int currStructure)
    {
        Debug.Log($"[StageManager] 맵 오브젝트 스폰");

        // 현재 맵에 맵 구조 스폰
        mapStructure structure = ObjectPoolManager.Instance.CreateObjWithUsePool(mapStructures[currStructure]);
        structure.transform.parent = mapObjects.transform;

        //TileManager.Init();

        // 플레이어 수만큼 퍼즐 스폰
        for (int i = 0; i < playerCount; i++)
        {
            Debug.Log($"[StageManager] 퍼즐 오브젝트 스폰 {i}");

            //Puzzle puzzle = ObjectPoolManager.Instance.CreateObjWithUsePool(puzzlePrefab);
            //puzzles.Add(puzzle);
            //// 랜덤 위치 찾아서 오브젝트 있는지 확인하고 스폰 위치로 설정
            //puzzle.transform.parent = puzzleObjects.transform;


            //if (PhotonNetwork.IsMasterClient)
            //{
            //    puzzle.transform.position = GetSpawnPos();
            //}

            Vector3 spawnpos = GetSpawnPos();
            int photonViewId = PhotonNetwork.AllocateViewID(false);
            SpawnPuzzle(spawnpos, photonViewId);
        }

        // 플레이어 수만큼 반복
        // 호스트에서 랜덤 위치 뽑아서 각 클라이언트에게 위치 알려줌
        // 뷰 아이디 할당해서 넣어줌
        // 

        //while (puzzles.Count == playerCount)
        //{
        //    int photonViewId = PhotonNetwork.AllocateViewID(false);
        //    Vector3 spawnId = GetSpawnPos();

        //}

        // 맵에 스폰된 퍼즐들 리스트에 저장
        //puzzles = puzzleObjects.GetComponentsInChildren<Puzzle>().ToList();
    }

    // 맵에서 오브젝트가 없는 랜덤 위치를 반환
    public Vector3 GetSpawnPos()
    {
        bool isAvailable = false;
        LayerMask maplayerMask = 7; // Map
        Vector3 spawnPos = currentMap.transform.position;
        while (!isAvailable)
        {
            spawnPos = new Vector3(UnityEngine.Random.Range(-8, 8), 0, UnityEngine.Random.Range(-8, 8));
            if (Physics.OverlapBox(spawnPos, new Vector3(0.5f, 0, 0.5f), Quaternion.identity, maplayerMask) != null)
                isAvailable = true;
        }
        return spawnPos;
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

    /// <summary>
    /// 호스트에서 정한 위치, 뷰 ID로 풀에서 가져오는 메서드
    /// </summary>
    [PunRPC]
    public void SpawnPuzzle(Vector3 pos, int id)
    {
        PuzzleBase puzzle = ObjectPoolManager.Instance.CreateObjWithUsePool(puzzlePrefab);
        puzzle.transform.parent = puzzleObjects.transform;

        puzzle.transform.position = pos;
        PhotonView photonView = puzzle.gameObject.GetComponent<PhotonView>();
        photonView.ViewID = id;

        // 스폰시킨 퍼즐 리스트에 저장
        puzzles.Add(puzzle);
    } 

    public void ClearStage()
    {
        _isStageClear = true;
        Debug.Log("[StageManager] 스테이지 클리어");
        clearAlertUI.SetActive(true);
        _animator.SetBool("IsOpened", true);
        _currentStage++;
    }


    // 실시간으로 동기화해야 하는 것만 보낼 것
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isStageClear);
            //stream.SendNext(_currentStage);
        }
        else
        {
            this._isStageClear = (bool)stream.ReceiveNext();
            //this._currentStage = (int)stream.ReceiveNext();
        }
    }
}
