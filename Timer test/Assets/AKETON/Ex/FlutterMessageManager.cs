using System.Collections;
using System.Collections.Generic;
using FlutterUnityIntegration;
using UnityEngine;

// 유니티 빌드 순서
// flutter -> export ios debug
public class FlutterMessageManager : MonoBehaviour
{
    UnityMessageManager manager;
    public BaseReceiver receiver;
    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.AddComponent<UnityMessageManager>();
        manager.OnMessage += OnUnityMessage;
    }

    void OnUnityMessage(string message)
    {
        Debug.Log(message);
        
        if (message == "Resume")
        {
            receiver.paused = true;
        } else if (message == "Stop")
        {
            receiver.paused = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
