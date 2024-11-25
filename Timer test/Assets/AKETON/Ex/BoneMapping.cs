
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public struct VirtualJoint
{
    public string name;
    public int[] rawIndexs;
    public float[] factors;

    

    public VirtualJoint(string name, int[] rawIndexs, float[] factors)
    {
        this.name = name;
        this.rawIndexs = rawIndexs;
        this.factors = factors;
    }
}

public enum ScaleApplyMode
{
    None,
    Length,
    Size
}


[System.Serializable]
public struct Bone
{
    public int startJointID;
    public int nextJointID;

    public bool applyDirection;
    public int hintJointID; // nullable
    
    
    public ScaleApplyMode scaleApplyMode;

    public Bone(int startJointID, int nextJointID, ScaleApplyMode scaleApplyMode = ScaleApplyMode.Length, bool applyDirection = false, int hintJointID = 0)
    {
        this.startJointID = startJointID;
        this.nextJointID = nextJointID;
        this.scaleApplyMode = scaleApplyMode;
        this.applyDirection = applyDirection;
        this.hintJointID = hintJointID;
        
    }
}

public static class BoneManager
{
    public static List<Joint> joints;
    public static List<VirtualJoint> virtualJoints;
    public static int GetJointIDFromName(string name)
    {
        var defaultID = joints.FindIndex((x) => x.name == name);
        var virtualID = (virtualJoints.FindIndex((joint) => joint.name == name) + 1) * -1;
        
        return defaultID == -1 ? virtualID : defaultID;
    }
    
    public static Bone NewBoneFromName(string start, string end, ScaleApplyMode scaleApplyMode = ScaleApplyMode.Length, string hint = null)
    {
        
        var bone = new Bone(GetJointIDFromName(start), GetJointIDFromName(end), scaleApplyMode, hint != null, hint == null ? 0 : GetJointIDFromName(hint)
        );
        return bone;
    }
}
[System.Serializable]
public struct Joint
{
    public string name;
     public int rawIndex;

    public Joint(string name, int rawIndex)
    {
        this.name = name;
        this.rawIndex = rawIndex;
    }
}

[System.Serializable]
public class BoneMapping  : MonoBehaviour
{
    public Bone[] bones;
    public Joint[] joints;
    
    public VirtualJoint[] virtualJoints;
    
    public Vector3[] calculatedVirtualPoints;

    public string GetBoneNameByJointId(int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;
            
            return virtualJoints[virtualID].name;
        }
        else
        {
            return joints[id].name;
        }
    }
    
    public Vector3 GetPositionByJointId(Vector3[] received, int id)
    {
        if (id < 0)
        {
            var virtualID = Math.Abs(id) - 1;

            return calculatedVirtualPoints[virtualID];
        }
        else
        {
            var rawIndex = joints[id].rawIndex;
            return received[rawIndex];
        }
    }
    
    public void CalculateVirtualPoints(Vector3[] received)
    {
        for (int i = 0; i < virtualJoints.Length; i++)
        {
            var currentVirtualPoint = virtualJoints[i];
            
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
                    point = calculatedVirtualPoints[virtualID];
                }
                else
                {
                    point = received[id];
                }

                
                
                resultVector += point * currentVirtualPoint.factors[j];
            }
            
            calculatedVirtualPoints[i] = resultVector;
        }
    }

    
    
    [ContextMenu("Initialize")]
    public void Initialize()
    {
        joints = new[]
        {
            new Joint("LeftHand", 7), 
            new Joint("LeftForeArm", 6), 
            
            new Joint("RightHand", 4), 
            new Joint("RightForeArm", 3), 
            
            new Joint("LeftFoot", 13), 
            new Joint("LeftLeg", 12), 
            
            new Joint("RightFoot", 10), 
            new Joint("RightLeg", 9),
            
            new Joint("LeftArm", 5),
            new Joint("RightArm", 2),
            
            new Joint("LeftUpLeg", 11),
            new Joint("RightUpLeg", 8),
            
            new Joint("LeftHand_End", 101),
            new Joint("RightHand_End", 122),
            
            new Joint("LeftFoot_End", 18),
            new Joint("RightFoot_End", 21),
            
            new Joint("HeadCenter", 54),
            
            new Joint("LeftHand_Hint", 94),
            new Joint("RightHand_Hint", 115),
            
            new Joint("LeftFoot_Hint", 19),
            new Joint("RightFoot_Hint", 22),
        };
        
        virtualJoints = new VirtualJoint[]
        {
            new VirtualJoint("Head",new[] {32}, new[] {1.0f}), // -1
            new VirtualJoint("Neck",new[] {1}, new[] {1.0f}), // -2
            new VirtualJoint("LeftShoulder",new[] {1, 5}, new[] {0.5f, 0.5f}), // -3
            new VirtualJoint("RightShoulder",new[] {1, 2}, new[] {0.5f, 0.5f}), // -4
            new VirtualJoint("Hips",new[] {8, 11}, new[] {0.5f, 0.5f}), // -5
            
            new VirtualJoint("Spine",new[] {1, -5}, new[] {0.3f, 0.7f}), // -6
            new VirtualJoint("Spine1",new[] {1, -5}, new[] {0.6f, 0.4f}), // -7
            new VirtualJoint("Spine2",new[] {1, -5}, new[] {0.8f, 0.2f}), // -8
        };

        BoneManager.joints = joints.ToList();
        BoneManager.virtualJoints = virtualJoints.ToList();

        bones = new[]
        {
            // 팔 부분
            BoneManager.NewBoneFromName("LeftHand", "LeftHand_End", ScaleApplyMode.None, "LeftHand_Hint"),
            BoneManager.NewBoneFromName("LeftForeArm", "LeftHand"),
            BoneManager.NewBoneFromName("LeftArm", "LeftForeArm"),
            
            BoneManager.NewBoneFromName("RightHand", "RightHand_End", ScaleApplyMode.None, "RightHand_Hint"),
            BoneManager.NewBoneFromName("RightForeArm", "RightHand"),
            BoneManager.NewBoneFromName("RightArm", "RightForeArm"),
    
            // 다리 부분
            BoneManager.NewBoneFromName("LeftFoot", "LeftFoot_End", ScaleApplyMode.None, "LeftFoot_Hint"),
            BoneManager.NewBoneFromName("LeftLeg", "LeftFoot"),
            BoneManager.NewBoneFromName("LeftUpLeg", "LeftLeg"),
            
            BoneManager.NewBoneFromName("RightFoot", "RightFoot_End", ScaleApplyMode.None, "RightFoot_Hint"),
            BoneManager.NewBoneFromName("RightLeg", "RightFoot"),
            BoneManager.NewBoneFromName("RightUpLeg", "RightLeg"),
    
            // 상체 부분
            BoneManager.NewBoneFromName("Head", "HeadCenter", ScaleApplyMode.None),
            BoneManager.NewBoneFromName("Neck", "Head"),
            BoneManager.NewBoneFromName("LeftShoulder", "LeftArm"),
            BoneManager.NewBoneFromName("RightShoulder", "RightArm"),
            BoneManager.NewBoneFromName("Hips", "Spine", ScaleApplyMode.None), // Hips 는 스케일되면 안됨 !!
            BoneManager.NewBoneFromName("Spine", "Spine1", ScaleApplyMode.Size),
            BoneManager.NewBoneFromName("Spine1", "Spine2", ScaleApplyMode.Size),
            BoneManager.NewBoneFromName("Spine2", "Neck", ScaleApplyMode.Size),
        };

        

        calculatedVirtualPoints = new Vector3[virtualJoints.Length];
    }
}
