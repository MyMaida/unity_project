
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


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

    public bool isApplyDirection;
    public int hintJointID; // nullable
    
    public float originalLength;
    
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    
    public ScaleApplyMode scaleApplyMode;
}

public class BoneBuilder
{
    public List<Joint> joints;
    public List<VirtualJoint> virtualJoints;
    public ExBoneReference exBoneReference;

    public BoneBuilder(List<Joint> joints, List<VirtualJoint> virtualJoints, ExBoneReference exBoneReference  )
    {
        this.joints = joints;
        this.virtualJoints = virtualJoints;
        this.exBoneReference = exBoneReference;
    }
    
    public int GetJointIDFromName(string name)
    {
        var defaultID = joints.FindIndex((x) => x.name == name);
        var virtualID = (virtualJoints.FindIndex((joint) => joint.name == name) + 1) * -1;
        
        return defaultID == -1 ? virtualID : defaultID;
    }
    
    public Bone NewBoneFromName(string start, string end, ScaleApplyMode scaleApplyMode = ScaleApplyMode.Length, string hint = null)
    {
        if (joints == null || virtualJoints == null || exBoneReference == null)
        {
            Debug.LogError("null");
        }
        

        var bone = new Bone();
        bone.startJointID = GetJointIDFromName(start);
        bone.nextJointID = GetJointIDFromName(end);
        bone.scaleApplyMode = scaleApplyMode;
        bone.isApplyDirection = hint != null;
        bone.hintJointID = hint == null ? 0 : GetJointIDFromName(hint);
        
        var a =exBoneReference.GetReferenceByName(
            start
        );
        
        var b = exBoneReference.GetReferenceByName(
            end );
        
        if (a == null || b == null)
        {
            Debug.Log("bone length null " + start + end);
            
        }
        else
        {
            bone.originalLength = Vector3.Distance(a.position, b.position);
            
            bone.originalPosition = a.position;
            bone.originalRotation = a.rotation;
            
        }
        
        

        
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
            
            new VirtualJoint("Spine",new[] {1, -5}, new[] {0.4f, 0.6f}), // -6
            new VirtualJoint("Spine1",new[] {1, -5}, new[] {0.6f, 0.4f}), // -7
            new VirtualJoint("Spine2",new[] {1, -5}, new[] {0.8f, 0.2f}), // -8
        };

        BoneBuilder boneBuilder = new BoneBuilder(joints.ToList(), virtualJoints.ToList(), GetComponent<ExBoneReference>());
        

        bones = new[]
        {
            // 팔 부분
            boneBuilder.NewBoneFromName("LeftHand", "LeftHand_End", ScaleApplyMode.None, "LeftHand_Hint"),
            boneBuilder.NewBoneFromName("LeftForeArm", "LeftHand"),
            boneBuilder.NewBoneFromName("LeftArm", "LeftForeArm"),
            
            boneBuilder.NewBoneFromName("RightHand", "RightHand_End", ScaleApplyMode.None, "RightHand_Hint"),
            boneBuilder.NewBoneFromName("RightForeArm", "RightHand"),
            boneBuilder.NewBoneFromName("RightArm", "RightForeArm"),
    
            // 다리 부분
            boneBuilder.NewBoneFromName("LeftFoot", "LeftFoot_End", ScaleApplyMode.None, "LeftFoot_Hint"),
            boneBuilder.NewBoneFromName("LeftLeg", "LeftFoot"),
            boneBuilder.NewBoneFromName("LeftUpLeg", "LeftLeg"),
            
            boneBuilder.NewBoneFromName("RightFoot", "RightFoot_End", ScaleApplyMode.None, "RightFoot_Hint"),
            boneBuilder.NewBoneFromName("RightLeg", "RightFoot"),
            boneBuilder.NewBoneFromName("RightUpLeg", "RightLeg"),
    
            // 상체 부분
            boneBuilder.NewBoneFromName("Head", "HeadCenter", ScaleApplyMode.None),
            boneBuilder.NewBoneFromName("Neck", "Head"),
            boneBuilder.NewBoneFromName("LeftShoulder", "LeftArm"),
            boneBuilder.NewBoneFromName("RightShoulder", "RightArm"),
            boneBuilder.NewBoneFromName("Hips", "Spine", ScaleApplyMode.None), // Hips 는 스케일되면 안됨 !!
            boneBuilder.NewBoneFromName("Spine", "Spine1", ScaleApplyMode.Size),
            boneBuilder.NewBoneFromName("Spine1", "Spine2", ScaleApplyMode.Size),
            boneBuilder.NewBoneFromName("Spine2", "Neck", ScaleApplyMode.Size),
        };

        

        calculatedVirtualPoints = new Vector3[virtualJoints.Length];
    }
}
