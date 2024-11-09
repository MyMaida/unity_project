using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayReceiver : IReceiver
{

    public ReplayScript replay;

    public bool isReplaying;

    public bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnFinishReceive()
    {
    }
    public override Vector3[] GetCoord()
    {
        if (isReplaying)
        {
            if (isPaused)
            {
                return replay.Current();
            }
            return replay.Next();
        }
        
        var BaseCoord = GetBaseCoord();
        
        if (BaseCoord == null)
        {
            Debug.LogError("BaseCoord Is Null");
        }
        
        replay.OnFrame(BaseCoord);
        
        return BaseCoord;
    }
}
