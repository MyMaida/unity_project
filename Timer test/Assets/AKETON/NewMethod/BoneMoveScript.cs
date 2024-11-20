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

    public SerializableDictionary<string, CachedData> cachedData;
    
    // Start is called before the first frame update
    void Start()
    {
        _boneReference = GetComponent<ExBoneReference>();
        _boneMapping = GetComponent<BoneMapping>();
        _receiver = FindObjectOfType<IReceiver>();
    }

    private void Reset()
    {
        CSVReader.ResetCachedCsv();
        
        _boneReference = GetComponent<ExBoneReference>();
        _boneMapping = GetComponent<BoneMapping>();
        _receiver = FindObjectOfType<IReceiver>();
        
        cachedData = new SerializableDictionary<string, CachedData>();

        var bones = _boneMapping.Bones;
        
        
        
        foreach (var bone in bones) //IKRig 생성
        {
            var c = new CachedData();
            
            var a = _boneReference.GetReferenceByName(bone.Name);
           
            
            
            var b = _boneReference.GetReferenceByName(_boneMapping.GetBoneNameById(bone.EndId));
            c.length = Vector3.Distance(a.position, b.position);
            
            cachedData.Add(bone.Name, c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var offset = transform.position;
        
        var coord = _receiver.GetCoord();

        
        _boneMapping.CalculateVirtualPoints(coord);
        


        foreach (var bone in _boneMapping.Bones) //IKRig 생성
        {
                
            var t = _boneReference.GetReferenceByName(bone.Name);

            if (t == null)
            {
                continue;
            }
            
            t.position = _boneMapping.GetPositionById(coord, bone.StartId) + offset;

            var firstBonePos = _boneMapping.GetPositionById(coord, bone.StartId);
            var lastBonePos = _boneMapping.GetPositionById(coord, bone.EndId);

            Debug.DrawLine(firstBonePos, lastBonePos, Color.red);
            
            var front = Vector3.Cross(lastBonePos - firstBonePos, Vector3.right);
            Debug.DrawRay(firstBonePos, front.normalized, Color.green);
            
            var x = Quaternion.LookRotation(front, lastBonePos - firstBonePos);
  
            t.rotation = x;
            
            var currentDistance = Vector3.Distance(firstBonePos, lastBonePos);

            if (cachedData.ContainsKey(bone.Name) && cachedData[bone.Name].length > 0.0f)
            {
                var scaleFactor = currentDistance / cachedData[bone.Name].length;
                
                if (currentDistance != 0.0)
                {
                    t.localScale = new Vector3(1, scaleFactor, 1);
                }
            
                Debug.Log(cachedData[bone.Name].length + " " + currentDistance);
            }
            else
            {
                Debug.Log(bone.Name + "not in cachedData");
            }
        }
    }
}
