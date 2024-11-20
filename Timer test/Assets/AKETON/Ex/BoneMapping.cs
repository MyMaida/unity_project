using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct VirtualPoint
{
    public string Name;
    public int[] ID;
    public float[] Factor;

    public VirtualPoint(string name, int[] id, float[] factor)
    {
        Name = name;
        this.ID = id;
        this.Factor = factor;
    }
}
[System.Serializable]
public struct Bone
{
    public string Name;
    public int StartId;
    public int EndId;

    public Bone(string name, int startId, int endId)
    {
        this.Name = name;
        this.StartId = startId;
        this.EndId = endId;
    }
}

[System.Serializable]
public class BoneMapping  : MonoBehaviour
{
    public Bone[] Bones;
    public VirtualPoint[] VirtualPoints;
    
    private Vector3[] CalculatedVirtualPoints;

    public string GetBoneNameById(int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;
            
            return VirtualPoints[virtualID].Name;
        }
        else
        {
            return Bones[id].Name;
        }
    }

    public void CalculateVirtualPoints(Vector3[] received)
    {
        for (int i = 0; i < VirtualPoints.Length; i++)
        {
            var currentVirtualPoint = VirtualPoints[i];
            
            Vector3 resultVector = Vector3.zero;

            for (int j = 0; j < currentVirtualPoint.ID.Length; j++)
            {
                var id = currentVirtualPoint.ID[j];
                var virtualID = Math.Abs(id) - 1;
                
                if (id < 0 && virtualID >= i) // i번째 vbone 작업시 i번째 이상의 값이 나올시 에러
                {
                    Debug.LogError(
                        "버추얼 본 포인트 계산 중에 에러 발생. 초기화 부분에서 의존성 문제 해결 바람." +
                        $"({i}번째 작업 중 {virtualID}참조 발생.)"
                        );
                }

                var point = GetPositionById(received, id);
                
                resultVector += point * currentVirtualPoint.Factor[j];
            }
            
            CalculatedVirtualPoints[i] = resultVector;
        }
    }

    public Vector3 GetPositionById(Vector3[] received, int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;

            return CalculatedVirtualPoints[virtualID];
        }
        else
        {
            return received[id];
        }
    }
    
    public void Reset()
    {
        
        Bones = new Bone[]
        {
            new Bone("LeftHand", 4, 125),
            new Bone("LeftForeArm", 3, 4), 
            new Bone("RightHand", 7, 103),
            new Bone("RightForeArm", 6, 7),
            new Bone("LeftFoot", 10, 21),
            new Bone("LeftLeg", 9, 10),
            new Bone("RightFoot", 13, 19), 
            new Bone("RightLeg", 12, 13),
            new Bone("LeftArm", 2, 3),
            new Bone("RightArm", 5, 6),
            new Bone("LeftUpLeg", 8, 9),
            new Bone("RightUpLeg", 11, 12),
            
            new Bone("Head", -1, 54),
            new Bone("Neck", -2, -1),
            new Bone("LeftShoulder", -3, 5),
            new Bone("RightShoulder", -4, 2),
            new Bone("Hips", -5, -6),
            new Bone("Spine", -6, -7),
            new Bone("Spine1", -7, -8),
            new Bone("Spine2", -8, -2),
        };

        VirtualPoints = new VirtualPoint[]
        {
            new VirtualPoint("Head",new[] {32}, new[] {1.0f}), // -1
            new VirtualPoint("Neck",new[] {1}, new[] {1.0f}), // -2
            new VirtualPoint("LeftShoulder",new[] {1, 5}, new[] {0.5f, 0.5f}), // -3
            new VirtualPoint("RightShoulder",new[] {1, 2}, new[] {0.5f, 0.5f}), // -4
            new VirtualPoint("Hips",new[] {8, 11}, new[] {0.5f, 0.5f}), // -5
            
            new VirtualPoint("Spine",new[] {1, -5}, new[] {0.2f, 0.8f}), // -6
            new VirtualPoint("Spine1",new[] {1, -5}, new[] {0.5f, 0.5f}), // -7
            new VirtualPoint("Spine2",new[] {1, -5}, new[] {0.8f, 0.2f}), // -8
        };

        CalculatedVirtualPoints = new Vector3[VirtualPoints.Length];
    }

    public void Start()
    {
        Reset();
    }
}
