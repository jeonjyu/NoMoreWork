using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// PuzzleButton의 버튼만 모아서 클리어 여부 확인
public class StageManager : MonoBehaviour, IPunObservable
{
    [SerializeField] GameObject clearAlertCanvas;
    bool isStageClear = false;
    public List<PuzzleButton> puzzleButtons = new List<PuzzleButton>();
    public static Dictionary<int, bool> clearedPuzzle = new Dictionary<int, bool>();

    void Start()
    {
        // 퍼즐 생성하고 번호 부여
        // 테스트할 때는 인스펙터에서 임의로 지정

        //puzzleButtons.Clear();
        puzzleButtons = GameObject.Find("Map").GetComponentsInChildren<PuzzleButton>().ToList();
    }

    private void FixedUpdate()
    {
        // 딕셔너리에서 clear == true인 애들 갯수랑 전체 갯수 비교
        // 갯수 동일하면 스테이지 클리어 
        // FixedUpdate 말고 이벤트로 해도 될 것 같은데
        if(!isStageClear && clearedPuzzle.Count == puzzleButtons.Count)
        {
            isStageClear = true;
            Debug.Log("[StageManager] 스테이지 클리어!");
            clearAlertCanvas.SetActive(true);
        }
    }

    public static void UpdateClearedPuzzle(int puzzleNumber)
    {
        if (!clearedPuzzle.ContainsKey(puzzleNumber))
            clearedPuzzle.Add(puzzleNumber, true);
        else 
            Debug.LogWarning($"[StageManager] 버튼 {puzzleNumber}를 클리어한 퍼즐에 넣을 수 없음");
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isStageClear);
        }
        else
        {
            this.isStageClear = (bool)stream.ReceiveNext();
        }
    }
}
