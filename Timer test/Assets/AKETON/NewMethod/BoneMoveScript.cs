using System;
using System.Collections;
using System.Collections.Generic;
using OnlyNew.BodyProportions;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class CachedData
{
    public float length;
}

[RequireComponent(typeof(ExBoneReference))]
[RequireComponent(typeof(BoneMapping))]
public class BoneMoveScript : MonoBehaviour
{
    public ExBoneReference _boneReference;
    public BoneMapping _boneMapping;
    public IReceiver _receiver;
    
    // Start is called before the first frame update
    void Start()
    {
        _boneReference = GetComponent<ExBoneReference>();
        _boneMapping = GetComponent<BoneMapping>();
        _receiver = FindObjectOfType<IReceiver>();
    }

    [ContextMenu("Initialize")]
    private void Initialize()
    {
        CSVReader.ResetCachedCsv();
        
        _boneReference = GetComponent<ExBoneReference>();
        _boneMapping = GetComponent<BoneMapping>();
        _receiver = FindObjectOfType<IReceiver>();
    }

    // Update is called once per frame
    void Update()
    {
        var offset = transform.position;
        
        var coord = _receiver.GetCoord();

        
        _boneMapping.CalculateVirtualPoints(coord);

        var middle = (coord[8] + coord[11]) / 2.0f;

        var gup = (coord[135] - coord[134]).normalized;
        
        var gfront = Vector3.Cross(coord[8] - coord[11], gup).normalized;
        
        var gright = Vector3.Cross(gup, gfront).normalized;
        
        Debug.DrawRay(coord[11], gright, Color.red);

        foreach (var bone in _boneMapping.bones)
        {
            var boneStartName = _boneMapping.GetBoneNameByJointId(bone.startJointID);    
            
            var firstJointPos = _boneMapping.GetPositionByJointId(coord, bone.startJointID);
            var lastJointPos = _boneMapping.GetPositionByJointId(coord, bone.nextJointID);
            
            var boneTransform = _boneReference.GetReferenceByName(boneStartName);
            
            if (boneTransform == null)
            {
                continue;
            }
            
            //Debug.Log(_boneMapping.GetBoneNameById(bone.StartId) + "" + bone.StartId + _boneMapping.GetPositionById(coord, bone.StartId));
            
            boneTransform.position = _boneMapping.GetPositionByJointId(coord, bone.startJointID) + offset;

            if (bone.isApplyDirection)
            {
                var hintBonePos = _boneMapping.GetPositionByJointId(coord, bone.hintJointID);
                
                Vector3 boneDirection = (lastJointPos - firstJointPos).normalized;
            
                var front = Vector3.Cross(boneDirection, hintBonePos);

                if (boneStartName.Contains("Left"))
                {
                    front = -front;
                }
                
                var x = Quaternion.LookRotation(front, boneDirection);
            
                boneTransform.rotation = x;
            }
            else
            {
                Vector3 boneDirection = (lastJointPos - firstJointPos).normalized;
                            
                var front = Vector3.Cross(boneDirection, -gright);
                
                var x = Quaternion.LookRotation(front, boneDirection);
                
                
                boneTransform.rotation = x;
                
               // Debug.DrawRay(firstJointPos, front.normalized * 0.5f, Color.cyan);

            }
            
            
            
            
            
            if (bone.scaleApplyMode == ScaleApplyMode.None)
            {
                continue;
            }
            
            var currentDistance = Vector3.Distance(firstJointPos, lastJointPos);

            if (bone.scaleApplyMode != ScaleApplyMode.None && bone.originalLength > 0.0f)
            {
                var scaleFactor = currentDistance / bone.originalLength;

                if (bone.scaleApplyMode == ScaleApplyMode.Length)
                {
                    boneTransform.localScale = new Vector3(1, scaleFactor, 1);
                } else if (bone.scaleApplyMode == ScaleApplyMode.Size)
                {
                    boneTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
            }
        }
    }
}
