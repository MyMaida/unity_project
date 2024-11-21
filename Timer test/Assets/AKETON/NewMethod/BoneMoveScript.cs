using System;
using System.Collections;
using System.Collections.Generic;
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

        var bones = _boneMapping.Bones;
        
        cachedData.Clear();
        
        
        foreach (var bone in bones) //IKRig 생성
        {
            var c = new CachedData();
            
            Debug.Log(bone + "Processing");

            var startBoneName = _boneMapping.GetBoneNameByJointId(bone.StartJointID);
            var endBoneName = _boneMapping.GetBoneNameByJointId(bone.NextJointID);
            
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
            
            if (!cachedData.ContainsKey(startBoneName))
            {
                cachedData.Add(startBoneName, c);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var offset = transform.position;
        
        var coord = _receiver.GetCoord();

        
        _boneMapping.CalculateVirtualPoints(coord);
        


        foreach (var bone in _boneMapping.Bones)
        {
            var boneStartName = _boneMapping.GetBoneNameByJointId(bone.StartJointID);    
            
            var t = _boneReference.GetReferenceByName(boneStartName);

            if (t == null)
            {
                continue;
            }
            
            //Debug.Log(_boneMapping.GetBoneNameById(bone.StartId) + "" + bone.StartId + _boneMapping.GetPositionById(coord, bone.StartId));
            
            t.position = _boneMapping.GetPositionByJointId(coord, bone.StartJointID) + offset;

            var firstBonePos = _boneMapping.GetPositionByJointId(coord, bone.StartJointID);
            var lastBonePos = _boneMapping.GetPositionByJointId(coord, bone.NextJointID);

            
            var front = Vector3.Cross(lastBonePos - firstBonePos, Vector3.right);
            
            var x = Quaternion.LookRotation(front, lastBonePos - firstBonePos);
  
            t.rotation = x;
            
            
            var currentDistance = Vector3.Distance(firstBonePos, lastBonePos);

            if (cachedData.ContainsKey(boneStartName) && cachedData[boneStartName].length > 0.0f)
            {
                var scaleFactor = currentDistance / cachedData[boneStartName ].length;
                
                if (currentDistance != 0.0)
                {
                    t.localScale = new Vector3(1, scaleFactor, 1);
                }
            
            }
            else
            {
                Debug.Log(boneStartName  + "not in cachedData");
                if (cachedData.ContainsKey(boneStartName))
                {
                    Debug.Log(cachedData[boneStartName].length + " " + currentDistance);
                }
            }
        }
    }
}
