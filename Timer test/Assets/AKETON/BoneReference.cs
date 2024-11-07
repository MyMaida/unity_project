using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class BoneReference : MonoBehaviour
{
    public Transform root;
    
    public Transform Head;
    
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
    
    
    // Start is called before the first frame update
    void Start()
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
    void AutoUpdateReferences()
    {
        if (root == null)
        {
            return;
        }
        
        List<string> boneNames = new List<string> { "Head" ,"LeftHand", "LeftForeArm", "LeftArm",
            "LeftShoulder", "RightHand", "RightForeArm" ,"RightArm", "RightShoulder", "LeftUpLeg","LeftLeg", "LeftFoot","RightUpLeg","RightLeg","RightFoot"};
        
        List<string> fbbikBoneNames = new List<string> { "head" , "leftHand", "leftForearm", "leftUpperArm",
            "", "rightHand", "rightForearm" ,"rightUpperArm", "", "leftThigh","leftCalf", "leftFoot","rightThigh","rightCalf","rightFoot"};

        CustomizeScript t;
        bool with_scalable_bone = TryGetComponent<CustomizeScript>(out t);
        
        if (with_scalable_bone)
        {
            foreach (Transform child in root)
            {
                foreach (var boneName in boneNames)
                {
                    if (child.name.Contains(boneName))
                    {
                        Debug.Log("Find boneName : " + boneName);

                        SetReferenceByName(boneName, child);

                        //GetType().GetField(boneName).SetValue(this, child);
                    }
                }
            }
        }
        else
        {
            var ikscript = GetComponent<FullBodyBipedIK>();
            for (int i = 0 ; i < boneNames.Count ; i++)
            {

                if (fbbikBoneNames[i] == "")
                {
                    continue;
                }
                
                Debug.Log("find by name"+ fbbikBoneNames[i]);
                
                var bone = GetReferenceByName(ikscript.references, fbbikBoneNames[i]);
                
                Debug.Log("find bone " + bone);
                
                SetReferenceByName(boneNames[i], bone);
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
