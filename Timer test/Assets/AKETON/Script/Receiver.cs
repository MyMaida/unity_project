using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Serialization;

public abstract class IReceiverMiddleWare: MonoBehaviour
{
    public abstract Vector3[] Transform(Vector3[] input);
}

public abstract class IReceiver: MonoBehaviour
{
    [FormerlySerializedAs("BaseReceiver")] public BaseReceiver baseReceiver;

    public IReceiverMiddleWare[] baseReceiverMiddleWare;

    private Vector3[] received = new Vector3[Helpers.CoordVectorSize];
    
    protected void OnReceive()
    {
        var coord = baseReceiver.GetBaseCoord();
        
        foreach (IReceiverMiddleWare middleWare in baseReceiverMiddleWare)
        {
            coord = middleWare.Transform(coord);
        }

        received = (Vector3[])coord.Clone() ;
    }
    
    private void Awake()
    {
        baseReceiver.OnEndReceive += OnEndReceive;
        baseReceiver.OnReceive += OnReceive;
    }

    protected Vector3[] GetBaseCoord()
    {
        return received;
    }

    protected abstract void OnEndReceive();

    
    private void OnDestroy()
    {
        baseReceiver.OnEndReceive -= OnEndReceive;
    }

    public abstract Vector3[] GetCoord();
}

public class Receiver : IReceiver
{
    protected override void OnEndReceive()
    {
        
    }

    public override Vector3[] GetCoord()
    {
        var BaseCoord = GetBaseCoord();
        
        
        
        if (BaseCoord == null)
        {
            Debug.LogError("BaseCoord Is Null");
        }
        
        return BaseCoord;
    }
}
