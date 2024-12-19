using System;
using System.Collections;
using System.Collections.Generic;
using OnlyNew.BodyProportions;

using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor.ShaderGraph.Drawing;
#endif

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

    public float debugRotate;
    
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
                Quaternion OmniLookRotation(
                    Vector3 exactAxis,       Vector3 exactTarget,
                    Vector3 approximateAxis, Vector3 approximateTarget
                ) {
                    // Compute a rotation that takes the z+ and y+ axes to our custom axes.
                    var zyToCustom = Quaternion.LookRotation(exactAxis, approximateAxis);
                    // Invert this, to map our custom axes to z+ and y+.
                    var customToZY = Quaternion.Inverse(zyToCustom);

                    // Compute a rotation that takes the z+ and y+ axes to our target directions.
                    var zyToTarget = Quaternion.LookRotation(exactTarget, approximateTarget);
    
                    // Chain these two rotations so that exactAxis maps to exactTarget,
                    // and approximateAxis maps as closely as it can to approximateTarget.
                    var customToTarget = zyToTarget * customToZY;

                    return customToTarget;
                }
                
                Vector3 boneDirection = (lastJointPos - firstJointPos).normalized;


                var localRight = bone.originalRotation * Vector3.right;
                
                Debug.DrawRay(boneTransform.position, localRight, Color.red);
                            
                var up = Vector3.Cross(boneDirection, localRight);


                var viewrot = OmniLookRotation(Vector3.up, boneDirection,
                                                        Vector3.right, localRight);
                
                var viewEuler= viewrot.eulerAngles;
                

                var angleDiff = bone.originalRotation.y;

                var test = Quaternion.Euler(Vector3.up * angleDiff);

                boneTransform.rotation = viewrot;// * Quaternion.Euler(Vector3.up * angleDiff);
                
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
