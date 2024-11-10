using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ReplayScript : IReceiverMiddleWare
{
    private List<Vector3[]> replayList;
    private Vector3[] replayBuffer;
    public int index;

    public int replayListLength = 0;
    
    public bool isRecording;


    public bool isReplaying;

    public bool isPaused;

    public float replaySpeed = 1f;
    float baseReplaySpeed = 0.1f;
    
    

    // Start is called before the first frame update
    void Start()
    {
        replayList = new List<Vector3[]>();
    }

    private IEnumerator Replay()
    {
        while (isReplaying)
        {
            replayBuffer = replayList[index];

            if (!isPaused)
            {
                index++;
            
                if (index >= replayList.Count - 1)
                {
                    index = 0;
                }
            }
        
            yield return new WaitForSeconds(baseReplaySpeed / replaySpeed);
        }
    }

    public override Vector3[] Transform(Vector3[] input)
    {
        if (isRecording)
        {
            replayList.Add(input.Clone() as Vector3[]);
        }
        
        if (isReplaying)
        {
            return replayBuffer;
        }
        
        return input;
    }

    [ContextMenu("StartRecord")]
    public void StartRecord()
    {
        replayList.Clear();
        index = 0;
        isRecording = true;
    }

    [ContextMenu("EndRecord")]
    public void EndRecord()
    {
        isRecording = false;
    }
    
    [ContextMenu("StartReplay")]
    public void StartReplay()
    {
        if (replayList.Count == 0)
        {
            return;
        }
        
        isReplaying = true;
        isRecording = false;
        StartCoroutine(Replay());
    }
    
    [ContextMenu("EndReplay")]
    public void EndReplay()
    {
        isReplaying = false;
    }

    private void Update()
    {
        replayListLength = replayList.Count;
    }
}
