using System;
using System.Collections;
using System.Collections.Generic;
using OnlyNew.BodyProportions;
using RootMotion.FinalIK;
using UnityEngine;
// ReSharper disable InconsistentNaming


public class BoneReference : MonoBehaviour
{
    // bone들의 레퍼런스. ikRig의 레퍼런스가 아님.
    
    public Transform root;

    public List<Transform> SpineChain;
    
    public Transform Head;
    public Transform Body;
    
    public Transform LeftHand;
    public Transform LeftForeArm;
    public Transform LeftArm;
    public Transform LeftShoulder;
    
    public Transform RightHand;
    public Transform RightForeArm;
    public Transform RightArm;
    public Transform RightShoulder;
    
    public Transform LeftUpLeg;
    public Transform LeftLeg;
    public Transform LeftFoot;
    
    public Transform RightUpLeg;
    public Transform RightLeg;
    public Transform RightFoot;

    public bool dontBind;
    
    
    // Start is called before the first frame update
    void Awake()
    {
    }

    public Transform GetReferenceByName(string boneName)
    {
        return (Transform)GetType().GetField(boneName).GetValue(this);
    }

    public void SetReferenceByName(string boneName, Transform bone)
    {
        GetType().GetField(boneName).SetValue(this, bone);
    }
    
    public Transform GetReferenceByName(object target, string boneName)
    {
        return (Transform)target.GetType().GetField(boneName).GetValue(target);
    }

    
    [ContextMenu("AutoUpdateReferences")]
    public void AutoUpdateReferences()
    {
        SpineChain.Clear();
        
        if (root == null)
        {
            return;
        }
        
        List<string> boneNames = new List<string> { "LeftHand", "LeftForeArm", "LeftArm",
            "LeftShoulder", "RightHand", "RightForeArm" ,"RightArm", "RightShoulder", "LeftUpLeg","LeftLeg", "LeftFoot","RightUpLeg","RightLeg","RightFoot"};

        foreach (Transform child in root)
        {
            foreach (var boneName in boneNames)
            {
                if (child.name.Contains(boneName))
                {
                    Debug.Log("AutoUpdateReferences: Find boneName : " + boneName);

                    SetReferenceByName(boneName, child);

                    if (dontBind)
                    {
                        var scalebone = GetReferenceByName(boneName).GetComponent<ScalableBone>();
                        scalebone.bindPosition = false;
                        scalebone.bindRotation = false;
                    }

                    //GetType().GetField(boneName).SetValue(this, child);
                }
            }
            
            if (child.name.Contains("Spine"))
            {
                SpineChain.Add(child);
                
                var scaleBone = child.GetComponent<ScalableBone>();
                
                scaleBone.bindPosition = false;
                scaleBone.bindRotation = false;
            } else

            if (child.name.EndsWith("Head"))
            {
                Head = child;
                
                var scaleBone = child.GetComponent<ScalableBone>();
                
                scaleBone.bindPosition = false;
                scaleBone.bindRotation = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
