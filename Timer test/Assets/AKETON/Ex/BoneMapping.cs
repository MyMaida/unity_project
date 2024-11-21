using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public struct VirtualJoint
{
    public string Name;
    [FormerlySerializedAs("ID")] public int[] rawIndexs;
    [FormerlySerializedAs("Factor")] public float[] factors;

    

    public VirtualJoint(string name, int[] rawIndexs, float[] factors)
    {
        Name = name;
        this.rawIndexs = rawIndexs;
        this.factors = factors;
    }
}

[System.Serializable]
public struct Bone
{
    [FormerlySerializedAs("StartId")] public int StartJointID;
    [FormerlySerializedAs("EndId")] public int NextJointID;

    public bool ApplyScale;

    public Bone(int startJointID, int nextJointID, bool applyScale = true)
    {
        this.StartJointID = startJointID;
        this.NextJointID = nextJointID;
        this.ApplyScale = applyScale;
    }
}

public static class BoneManager
{
    public static List<Joint> Joints;
    public static List<VirtualJoint> VirtualJoints;
    public static int GetJointIDFromName(string name)
    {
        var defaultID = Joints.FindIndex((x) => x.Name == name);
        var virtualID = (VirtualJoints.FindIndex((joint) => joint.Name == name) + 1) * -1;
            
        return defaultID == -1 ? virtualID : defaultID;
    }
    
    public static Bone NewBoneFromName(string start, string end, bool applyScale = true)
    {
        var bone = new Bone(GetJointIDFromName(start), GetJointIDFromName(end), applyScale);
        return bone;
    }
}
[System.Serializable]
public struct Joint
{
    public string Name;
    [FormerlySerializedAs("ID")] public int rawIndex;

    public Joint(string name, int rawIndex)
    {
        this.Name = name;
        this.rawIndex = rawIndex;
    }
}

[System.Serializable]
public class BoneMapping  : MonoBehaviour
{
    public Bone[] Bones;
    public Joint[] Joints;
    
    public VirtualJoint[] VirtualJoints;
    
    public Vector3[] CalculatedVirtualPoints;

    public string GetBoneNameByJointId(int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;
            
            return VirtualJoints[virtualID].Name;
        }
        else
        {
            return Joints[id].Name;
        }
    }
    
    public Vector3 GetPositionByJointId(Vector3[] received, int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;

            return CalculatedVirtualPoints[virtualID];
        }
        else
        {
            var rawIndex = Joints[id].rawIndex;
            return received[rawIndex];
        }
    }
    
    public void CalculateVirtualPoints(Vector3[] received)
    {
        for (int i = 0; i < VirtualJoints.Length; i++)
        {
            var currentVirtualPoint = VirtualJoints[i];
            
            Vector3 resultVector = Vector3.zero;

            for (int j = 0; j < currentVirtualPoint.rawIndexs.Length; j++)
            {
                var id = currentVirtualPoint.rawIndexs[j];
                
                var virtualID = Math.Abs(id) - 1;
                
                if (id < 0 && virtualID >= i) // i번째 vbone 작업시 i번째 이상의 값이 나올시 에러
                {
                    Debug.LogError(
                        "버추얼 본 포인트 계산 중에 에러 발생. 초기화 부분에서 의존성 문제 해결 바람." +
                        $"({i}번째 작업 중 {virtualID}참조 발생.)"
                        );
                }

                Vector3 point;
                
                if (id < 0)
                {
                    point = CalculatedVirtualPoints[virtualID];
                }
                else
                {
                    point = received[id];
                }

                
                
                resultVector += point * currentVirtualPoint.factors[j];
            }
            
            CalculatedVirtualPoints[i] = resultVector;
        }
    }

    
    
    [ContextMenu("Initialize")]
    public void Initialize()
    {
        Joints = new[]
        {
            new Joint("LeftHand", 4), 
            new Joint("LeftForeArm", 3), 
            new Joint("RightHand", 7), 
            new Joint("RightForeArm", 6), 
            new Joint("LeftFoot", 10), 
            new Joint("LeftLeg", 9), 
            new Joint("RightFoot", 13), 
            new Joint("RightLeg", 12),
            new Joint("LeftArm", 2),
            new Joint("RightArm", 5),
            new Joint("LeftUpLeg", 8),
            new Joint("RightUpLeg", 11),
            
            new Joint("LeftHand_End", 125),
            new Joint("RightHand_End", 103),
            new Joint("LeftFoot_End", 21),
            new Joint("RightFoot_End", 19),
            new Joint("HeadCenter", 54),
        };
        
        VirtualJoints = new VirtualJoint[]
        {
            new VirtualJoint("Head",new[] {32}, new[] {1.0f}), // -1
            new VirtualJoint("Neck",new[] {1}, new[] {1.0f}), // -2
            new VirtualJoint("LeftShoulder",new[] {1, 2}, new[] {0.5f, 0.5f}), // -3
            new VirtualJoint("RightShoulder",new[] {1, 5}, new[] {0.5f, 0.5f}), // -4
            new VirtualJoint("Hips",new[] {8, 11}, new[] {0.5f, 0.5f}), // -5
            
            new VirtualJoint("Spine",new[] {1, -5}, new[] {0.2f, 0.8f}), // -6
            new VirtualJoint("Spine1",new[] {1, -5}, new[] {0.5f, 0.5f}), // -7
            new VirtualJoint("Spine2",new[] {1, -5}, new[] {0.8f, 0.2f}), // -8
        };

        BoneManager.Joints = Joints.ToList();
        BoneManager.VirtualJoints = VirtualJoints.ToList();

        Bones = new[]
        {
            // 팔 부분
            BoneManager.NewBoneFromName("LeftHand", "LeftHand_End", false),
            BoneManager.NewBoneFromName("LeftForeArm", "LeftHand"),
            BoneManager.NewBoneFromName("LeftArm", "LeftForeArm"),
            BoneManager.NewBoneFromName("RightHand", "RightHand_End", false),
            BoneManager.NewBoneFromName("RightForeArm", "RightHand"),
            BoneManager.NewBoneFromName("RightArm", "RightForeArm"),
    
            // 다리 부분
            BoneManager.NewBoneFromName("LeftFoot", "LeftFoot_End", false),
            BoneManager.NewBoneFromName("LeftLeg", "LeftFoot"),
            BoneManager.NewBoneFromName("LeftUpLeg", "LeftLeg"),
            BoneManager.NewBoneFromName("RightFoot", "RightFoot_End", false),
            BoneManager.NewBoneFromName("RightLeg", "RightFoot"),
            BoneManager.NewBoneFromName("RightUpLeg", "RightLeg"),
    
            // 상체 부분
            BoneManager.NewBoneFromName("Head", "HeadCenter", false),
            BoneManager.NewBoneFromName("Neck", "Head"),
            BoneManager.NewBoneFromName("LeftShoulder", "LeftArm"),
            BoneManager.NewBoneFromName("RightShoulder", "RightArm"),
            BoneManager.NewBoneFromName("Hips", "Spine"),
            BoneManager.NewBoneFromName("Spine", "Spine1"),
            BoneManager.NewBoneFromName("Spine1", "Spine2"),
            BoneManager.NewBoneFromName("Spine2", "Neck"),
        };

        

        CalculatedVirtualPoints = new Vector3[VirtualJoints.Length];
    }
}
