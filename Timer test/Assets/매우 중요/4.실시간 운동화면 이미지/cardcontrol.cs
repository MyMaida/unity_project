using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class cardcontrol : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    public Button button1; 
    public Button button2; 
    
    // Start is called before the first frame update
    void Start()
    {
        ActivateImage1();

        button1.onClick.AddListener(ActivateImage2);
        button2.onClick.AddListener(ActivateImage1);
    

    void ActivateImage1()
    {
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(true);

    }

    void ActivateImage2()
    {
        obj1.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(false);
    }
    }
 
}
