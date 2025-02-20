using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public enum ReceiverMode
{
    Raw,
    Transformed,
    Scaled,
    TransformAndScaled
}

public class BaseReceiver : MonoBehaviour
{
    private UdpClient m_Receiver;
    public int m_Port = 12345;
    public string m_ReceiveMessage;
    public Vector3[] baseCoord;

    public Action OnReceive;
    public Action OnEndReceive;
    
    public ReceiverMode mode = ReceiverMode.TransformAndScaled;
    
    
    
    
    
    [FormerlySerializedAs("floorTransform")] [ReadOnly]
    public Vector3 centerOfFoot;
    public Vector3 veticalVector;
    
    public bool isFloorTransformSet;
    public bool useVerticalVector = true;
    
    public float receivedScale = 20.0f;
    
    private bool _isFinished;

    public bool paused = false;

    void Awake()
    {
        baseCoord = new Vector3[Helpers.CoordVectorSize];
        Receive();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application Quit");
        _isFinished = true;
        baseCoord = new Vector3[Helpers.CoordVectorSize];
        
        OnEndReceive?.Invoke();
    }
    
    
    public Vector3 TransformReceivedPosition(Vector3 V) //원래 있던 함수입니다.
    {
        var PointB = Helpers.PointB;
        
        var Position = (V - baseCoord[10]) / 50 + PointB;
        
        
        
        
        Vector3 pointA = (baseCoord[134] - baseCoord[135]) / 50 + PointB;
        Vector3 pointC = new Vector3(0, 0, 0);
        
        Debug.DrawLine(Vector3.zero, pointA, Color.red, duration: 0.5f);
        Debug.DrawLine(Vector3.zero, PointB, Color.green, duration: 0.5f);
        Debug.DrawLine(Vector3.zero, pointC, Color.blue, duration: 0.5f);
        
        // 두 벡터 계산
        Vector3 vector1 = pointA - PointB;
        Vector3 vector2 = pointC - PointB;

        // 두 벡터 사이의 각도 계산
        float angle = Vector3.Angle(vector1, vector2);

        // 평면 위의 회전 축 계산 (pointA, pointB, pointC가 이루는 평면의 법선 벡터)
        Vector3 rotationAxis = Vector3.Cross(vector1, vector2).normalized;

        // 기준점에서 평면 위의 각도만큼 회전 이동
        Vector3 position = Position;
        Vector3 vector3 = Quaternion.AngleAxis(angle, rotationAxis) * (position - PointB);
        Position = PointB + vector3;
        Position = Quaternion.AngleAxis(angle * ((float)Math.PI / 180f), rotationAxis) * Position;

        Position.z *= -1;
            
        return Position;
    }

    public Vector3[] GetBaseCoord()
    {
        return baseCoord;
    }

    async Task Receive()
    {
        
        _isFinished = false;
        
        try
        {
            using var udpClient = new UdpClient(m_Port);
            //using var udpClient = new UdpClient("localhost", m_Port);
            while (!_isFinished)
            {
                if (paused)
                {
                    await Task.Delay(10);
                    continue;
                }
                
                
                var receivedResult = await udpClient.ReceiveAsync();

                
                
                m_ReceiveMessage = Encoding.Default.GetString(receivedResult.Buffer).Trim();


                if (m_ReceiveMessage.Equals("End"))
                {
                    continue;
                }
                
                m_ReceiveMessage = m_ReceiveMessage.Replace("[", "").Replace("]", "");


                string[] str = m_ReceiveMessage.Split(',');
                

                for (int i = 0; i < Helpers.CoordVectorSize; i++)
                {
                    try
                    {
                        baseCoord[i] = new Vector3(float.Parse(str[i * 3]), float.Parse(str[i * 3 + 1]),
                            float.Parse(str[i * 3 + 2]));
                    }catch (Exception e)
                    {
                        Debug.Log(e);
                        Debug.Log("error index is"  + i);
                    }
                }
                
                //Debug.Log(baseCoord[0]);
                
                var newBaseCoord = new Vector3[Helpers.CoordVectorSize];
                
                if (mode == ReceiverMode.TransformAndScaled || mode == ReceiverMode.Transformed)
                {

                    for (int i = 0; i < Helpers.CoordVectorSize; i++)
                    {
                        newBaseCoord[i] = TransformReceivedPosition(baseCoord[i]);
                    }

                    baseCoord = newBaseCoord;
                }
                
                //Debug.Log("->" + baseCoord[0]);
                
                if (mode == ReceiverMode.TransformAndScaled || mode == ReceiverMode.Scaled)
                {
                    var footCenter = (baseCoord[10] + baseCoord[13]) / 2.0f;
                    var resizedFootCenter = footCenter / receivedScale;
                    
                    var resizedVerticalVector = baseCoord[135] / receivedScale;
                        
                    if (isFloorTransformSet == false)
                    {
                        

                        
                        
                        centerOfFoot = resizedFootCenter;
                        veticalVector = resizedVerticalVector;
                        
                        
                        isFloorTransformSet = true;
                    }
                    

                    //divideFactor = thighDistance / boneDistance / 1.1f;

                    for (int i = 0; i < Helpers.CoordVectorSize; i++)
                    {
                        var saved = baseCoord[i];
                        
                        
                        baseCoord[i] = baseCoord[i] / receivedScale - centerOfFoot +
                                       (useVerticalVector ? veticalVector - resizedVerticalVector : Vector3.zero);
                        
                        
                            
                    }
                }
                
                //Debug.Log("->" + baseCoord[0]);

                OnReceive?.Invoke();
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }

    }

    
}