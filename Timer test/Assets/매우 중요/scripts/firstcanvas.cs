using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject[] canvases; // 5개의 캔버스를 배열로 추가

    // 게임이 시작될 때 1번 캔버스만 활성화
    void Start()
    {
        // 모든 캔버스를 비활성화
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].SetActive(false);
        }

        // 1번 캔버스만 활성화 (배열 인덱스는 0부터 시작하므로 canvases[0]이 1번 캔버스에 해당)
        canvases[0].SetActive(true);
    }
}
