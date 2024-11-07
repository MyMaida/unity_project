using UnityEngine;
using UnityEngine.UI;

public class ObjectToggleController : MonoBehaviour
{
    public GameObject targetObject;  // 활성화/비활성화할 오브젝트
    public Button deactivateButton;  // 비활성화 버튼 (버튼 A)
    public Button activateButton;    // 활성화 버튼 (버튼 B)

    private void Start()
    {
        // 오브젝트 초기 상태를 비활성화로 설정
        targetObject.SetActive(false);

        // 버튼 클릭 이벤트 추가
        deactivateButton.onClick.AddListener(DeactivateObject);
        activateButton.onClick.AddListener(ActivateObject);
    }

    // 오브젝트를 비활성화하는 메서드
    private void DeactivateObject()
    {
        targetObject.SetActive(false);
    }

    // 오브젝트를 활성화하는 메서드
    private void ActivateObject()
    {
        targetObject.SetActive(true);
    }
}
