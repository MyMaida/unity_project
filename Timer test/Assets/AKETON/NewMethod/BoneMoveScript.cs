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

[RequireComponent(typeof(BoneReference))]
public class BoneMoveScript : MonoBehaviour
{
    public BoneReference _boneReference;
    public IReceiver _receiver;

    public SerializableDictionary<string, CachedData> cachedData;
    
    // Start is called before the first frame update
    void Start()
    {
        _boneReference = GetComponent<BoneReference>();
        _receiver = FindObjectOfType<IReceiver>();
    }

    private void Reset()
    {
        _boneReference = GetComponent<BoneReference>();
        _receiver = FindObjectOfType<IReceiver>();
        
        cachedData = new SerializableDictionary<string, CachedData>();
        
        foreach (Transform obj in _boneReference.root) //IKRig 생성
        {
            
        }

        foreach (var dict in CSVReader.newJointCsv) //IKRig 생성
        {
            string ikName = (string)dict["IKName"];
            
            int jointID = (int)dict["JointID"];
            int targetID = (int)dict["TargetID"];
            
            var c = new CachedData();

            c.length = 1.0f; //TODO
            
            cachedData.Add(ikName, c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var offset = transform.position;
        
        var coord = _receiver.GetCoord();

        var resultList = new List<Vector3>();
        
        if (_boneReference.SpineChain.Count != 0)
        {
            for (var index = 0; index < _boneReference.SpineChain.Count + 1; index++)
            {

                var factor = (index) / (float)_boneReference.SpineChain.Count; // 0.0  ~ 1.0
                factor /= 2.0f;
                factor += 0.3f; // 0.3~ 0.7f
                var inverseFactor = 1.0f - factor;
                
                
                var a = (coord[8] + coord[11]) / 2.0f;
                var b = (coord[2] + coord[5]) / 2.0f;
                
                
                

                

                var res = a * inverseFactor + b * factor  + offset;
                
                //Debug.Log(factor + " " + inverseFactor);
                
                

                resultList.Add(res);
            }

            for (var index = 0; index < _boneReference.SpineChain.Count; index++)
            {
                var spine = _boneReference.SpineChain[index];
                
                //Debug.Log(index + spine.name);

                //Debug.DrawLine(resultList[index], resultList[index + 1], new Color(index * 100, 0, 0));
                
                spine.position = resultList[index];
                spine.rotation = Quaternion.FromToRotation(Vector3.up, resultList[index + 1] - resultList[index]);
            }
        }

        if (_boneReference.Head != null)
        {
            var head = _boneReference.Head;
            
            head.position = coord[32] + offset;
            head.rotation = Quaternion.FromToRotation(Vector3.up, coord[54] - coord[32]);
        }

        {
            var root = _boneReference.root;
            
            root.position = (coord[8] + coord[11]) / 2.0f + offset;
        }


        foreach (var dict in CSVReader.newJointCsv) //IKRig 생성
        {
            string ikName = (string)dict["IKName"];

            string jointType = (string)dict["JointType"];
            
            

            switch (jointType)
            {
                case "Position":
                    int jointID = (int)dict["JointID"];
                    int targetID = (int)dict["TargetID"];
                        
                    var bone = _boneReference.GetReferenceByName(ikName);
                    bone.position = coord[jointID] + offset;

                    var firstBonePos = coord[jointID];
                    var lastBonePos = coord[targetID];

                    Debug.DrawLine(firstBonePos, lastBonePos, Color.red);
                    
                    var front = Vector3.Cross(lastBonePos - firstBonePos, Vector3.right);
                    Debug.DrawRay(firstBonePos, front.normalized, Color.green);
                    
                    var x = Quaternion.LookRotation(front, lastBonePos - firstBonePos);
          
                    bone.rotation = x;
                    
                    bone.localScale = new Vector3(1, 1, 1);
                    break;
                case "Rotation":
                    break;
            }
        }
    }
}
