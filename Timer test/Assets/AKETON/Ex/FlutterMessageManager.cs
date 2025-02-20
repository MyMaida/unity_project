using System.Collections;
using System.Collections.Generic;
using FlutterUnityIntegration;
using TMPro;
using UnityEngine;

// 유니티 빌드 순서
// flutter -> export ios debug
public class FlutterMessageManager : MonoBehaviour
{
    public UnityMessageManager manager;
    public BaseReceiver receiver;

    public GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        text.GetComponent<TextMeshProUGUI>().SetText("new");
        manager = gameObject.GetComponent<UnityMessageManager>();
        manager.OnMessage += OnUnityMessage;
    }

    void OnUnityMessage(string message)
    {
        Debug.Log(message);
        
        if (message == "Resume")
        {
            receiver.paused = false;
        } else if (message == "Pause")
        {
            receiver.paused = true;
        }
        
        Debug.LogError("testLogError");
        text.GetComponent<TextMeshProUGUI>().text = message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
