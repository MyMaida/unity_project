using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button과 관련된 네임스페이스 추가

public class AIguidecontrol : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;

    public Button button1; 
    public Button button2; 
    public Button button3; 
    public Button button4; 
    
    void Start()
    {
        // 첫 번째 오브젝트 활성화
        ActivateImage1();

        // 버튼 클릭 시 이벤트 추가
        button1.onClick.AddListener(ActivateImage1);
        button2.onClick.AddListener(ActivateImage2);
        button3.onClick.AddListener(ActivateImage3);
        button4.onClick.AddListener(ActivateImage4);
    }

    void ActivateImage1()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
        obj3.SetActive(false);
        obj4.SetActive(false);
    }

    void ActivateImage2()
    {
        obj1.SetActive(false);
        obj2.SetActive(true);
        obj3.SetActive(false);
        obj4.SetActive(false);
    }

    void ActivateImage3()
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(true);
        obj4.SetActive(false);
    }

    void ActivateImage4()
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(false);
        obj4.SetActive(true);
    }
}
