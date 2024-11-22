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

    public SerializableDictionary<string, CachedData> cachedData= new SerializableDictionary<string, CachedData>();
    
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

        var bones = _boneMapping.bones;
        
        cachedData.Clear();
        
        
        foreach (var bone in bones) //IKRig 생성
        {
            var c = new CachedData();
            
            Debug.Log(bone + "Processing");

            var startBoneName = _boneMapping.GetBoneNameByJointId(bone.startJointID);
            var endBoneName = _boneMapping.GetBoneNameByJointId(bone.nextJointID);
            
            Debug.Log($"{startBoneName} and {endBoneName}");
                
            
            var a = _boneReference.GetReferenceByName(
                startBoneName 
                );
           
            
            
            var b = _boneReference.GetReferenceByName(
                endBoneName );

            if (a == null || b == null)
            {
                continue;
            }
            
            c.length = Vector3.Distance(a.position, b.position);
            
            cachedData.Add(startBoneName, c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var offset = transform.position;
        
        var coord = _receiver.GetCoord();

        
        _boneMapping.CalculateVirtualPoints(coord);

        foreach (var bone in _boneMapping.bones)
        {
            var boneStartName = _boneMapping.GetBoneNameByJointId(bone.startJointID);    
            
            var firstBonePos = _boneMapping.GetPositionByJointId(coord, bone.startJointID);
            var lastBonePos = _boneMapping.GetPositionByJointId(coord, bone.nextJointID);
            
            var boneTransform = _boneReference.GetReferenceByName(boneStartName);
            
            if (boneTransform == null)
            {
                continue;
            }
            
            //Debug.Log(_boneMapping.GetBoneNameById(bone.StartId) + "" + bone.StartId + _boneMapping.GetPositionById(coord, bone.StartId));
            
            boneTransform.position = _boneMapping.GetPositionByJointId(coord, bone.startJointID) + offset;

            

            Debug.DrawLine(firstBonePos, lastBonePos, Color.white);
            
            Vector3 boneDirection = (lastBonePos - firstBonePos).normalized;
            
            var front = Vector3.Cross(boneDirection, Vector3.left);
            
            var x = Quaternion.LookRotation(front, boneDirection);
  
            Debug.DrawRay(firstBonePos, x * Vector3.up * 0.1f, Color.red);
            
            boneTransform.rotation = x;


            if (bone.scaleApplyMode == ScaleApplyMode.None)
            {
                continue;
            }
            
            var currentDistance = Vector3.Distance(firstBonePos, lastBonePos);

            if (cachedData.ContainsKey(boneStartName) && cachedData[boneStartName].length > 0.0f)
            {
                
                
                var scaleFactor = currentDistance / cachedData[boneStartName].length;

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
