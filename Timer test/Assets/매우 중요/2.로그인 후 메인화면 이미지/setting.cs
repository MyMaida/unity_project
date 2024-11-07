using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class setting : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;
    public GameObject obj5;
    public GameObject obj6;

    public Button button1; 
    public Button button2; 
    public Button button3; 
    public Button button4; 
    public Button button5; 
    public Button button6; 
    
    // Start is called before the first frame update
    void Start()
    {
        // 첫 번째 오브젝트 활성화
        ActivateImage1();

        // 버튼 클릭 시 이벤트 추가
        button1.onClick.AddListener(ActivateImage2);
        button2.onClick.AddListener(ActivateImage1);
        button3.onClick.AddListener(ActivateImage1);
        button4.onClick.AddListener(ActivateImage3);
        button5.onClick.AddListener(ActivateImage1);
        button6.onClick.AddListener(ActivateImage1);
    }

    void ActivateImage1()
    {
        obj1.SetActive(true);
        obj2.SetActive(false);
        obj3.SetActive(true);
        obj4.SetActive(false);
        obj5.SetActive(true);
        obj6.SetActive(true);
    }

    void ActivateImage2()
    {
        obj1.SetActive(false);
        obj2.SetActive(true);
        obj3.SetActive(false);
        obj4.SetActive(false);
        obj5.SetActive(false);
        obj6.SetActive(false);
    }
    void ActivateImage3()
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(false);
        obj4.SetActive(true);
        obj5.SetActive(false);
        obj6.SetActive(false);
    }
    
}
