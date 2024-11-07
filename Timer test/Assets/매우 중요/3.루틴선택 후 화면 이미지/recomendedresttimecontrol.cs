using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button과 관련된 네임스페이스 추가

public class RecomendedRestTimeControl : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;

    public Button button1; 
    public Button button2; 
    
    // Start is called before the first frame update
    void Start()
    {
        // 첫 번째 오브젝트 활성화
        ActivateImage2();

        // 버튼 클릭 시 이벤트 추가
        button1.onClick.AddListener(ActivateImage1);
        button2.onClick.AddListener(ActivateImage2);
    }

    void ActivateImage1()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
    }

    void ActivateImage2()
    {
        obj1.SetActive(false);
        obj2.SetActive(true);
    }
}
