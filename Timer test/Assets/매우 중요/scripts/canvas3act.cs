using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button을 사용하기 위해 필요한 네임스페이스

public class canvas3act : MonoBehaviour
{
    public Canvas canvas1;
    public Canvas canvas2;
    public Canvas canvas3;
    public Canvas canvas4;
    public Canvas canvas5;

    public Button button1;
    
    // Start is called before the first frame update
    void Start()
    {
        button1.onClick.AddListener(actcanvas2); // AddListener로 수정
    }

    // Update is called once per frame
    private void actcanvas2()
    {
        canvas1.gameObject.SetActive(false);
        canvas2.gameObject.SetActive(false);
        canvas3.gameObject.SetActive(true);
        canvas4.gameObject.SetActive(false);
        canvas5.gameObject.SetActive(false);
    }
}
