using UnityEngine;

// 타일 위에 스테이지 오브젝트들 지우고 생성
public class StageObjectManager : MonoBehaviour
{
    GameObject map;
    GameObject puzzle;
    GameObject mapObjects;
    [SerializeField] GameObject[] Grids;

    private void Start()
    {
        map = GameObject.Find("Map");
        puzzle = GameObject.Find("Map/Puzzle");
        mapObjects = GameObject.Find("Map/MapObjects");

        //SetStage();
    }

    // 스테이지에 있던 퍼즐 오브젝트들 모두 삭제(풀에 반납)
    public void SetStage()
    {
        //Puzzle[] puzzles = puzzle.GetComponentsInChildren<Puzzle>();
        //foreach (Puzzle puzzle in puzzles) ObjectPoolManager.Instance.SetObjInPool(puzzle);
            //Destroy(puzzle.gameObject);

        Instantiate(Grids[Random.Range(0, Grids.Length)], mapObjects.transform);

        // 사람 수만큼 같은 유형의 퍼즐 생성하기

    }

    // 퍼즐 세팅
    // 타일 오브젝트 딕셔너리 업데이트
    // puzzle을 타일 오브젝트 딕셔너리가 없는 곳에 무작위 생성
    // 오브젝트들 설정 > SetStage?
    // 플레이어 스폰
    // 플레이 재개
}
