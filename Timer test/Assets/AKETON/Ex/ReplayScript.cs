using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ReplayScript : MonoBehaviour
{
    private List<Vector3[]> replayList;
    public int index;
    public int currentIndex;
    
    
    private bool isRecording;
    public IReceiver receiver;
      
    // Start is called before the first frame update
    void Start()
    {
        replayList = new List<Vector3[]>();
        
        if (receiver == null)
        {
            Debug.LogError("Receiver is null");
        }
    }
    
    public void OnFrame(Vector3[] t)
    {
        if (isRecording)
        {
            replayList.Add(t);
        } 
    }

    public Vector3[] Next()
    {
        return replayList[index++];
    }

    public Vector3[] Current()
    {
        return replayList[index];
    }

    [ContextMenu("StartRecord")]
    public void StartRecord()
    {
        replayList.Clear();
        isRecording = true;
    }

    [ContextMenu("EndRecord")]
    public void EndRecord()
    {
        isRecording = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentIndex = replayList.Count;
    }
}
