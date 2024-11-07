using UnityEngine;
using UnityEngine.UI;

public class ObjectController : MonoBehaviour
{
    public GameObject object1; // 첫 번째 오브젝트
    public GameObject object2; // 두 번째 오브젝트

    public Button button1; // 첫 번째 버튼
    public Button button2; // 두 번째 버튼

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        button1.onClick.AddListener(ActivateObject2);
        button2.onClick.AddListener(ActivateObject1);

        // 초기 설정: 2번 오브젝트만 활성화
        ActivateObject2();
    }

    private void ActivateObject1()
    {
        object1.SetActive(true);
        object2.SetActive(false);
    }

    private void ActivateObject2()
    {
        object1.SetActive(false);
        object2.SetActive(true);
    }
}
