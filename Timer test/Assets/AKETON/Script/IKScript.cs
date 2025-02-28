using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using RootMotion;
using RootMotion.FinalIK;

using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations.Rigging;
#endif

[Serializable]
public struct InitRotation
{
    public Quaternion rotation;
    public Vector3 upvector;
    public Vector3 customRotation;
    public InitRotation(Quaternion diff, Vector3 upvector)
    {
        this.rotation = diff;
        this.upvector = upvector;
        customRotation = Vector3.zero;
    }
}

[RequireComponent(typeof(FullBodyBipedIK))]
[RequireComponent(typeof(BoneReference))]
public class IKScript : MonoBehaviour
{
    public IReceiver receiver;
    public FullBodyBipedIK ik;
    public bool isStopReceving = false;
    public bool isMirroredReceving = false;
    //public FullBodyBipedIK ik;
    
    // Start is called before the first frame update

    public Transform rootBone;
    public bool debug;

    public SerializableDictionary<string, InitRotation> baseRotations;

    private BoneReference _boneReference;
    
    [ContextMenu("AutoInit")]
    private void AutoInit()                                                                                                                                                                                  
    {
        /* ik 스크립트 초기화 방법.
         *  1. scaleable bone manager & ik스크립트를 추가한다. 이때 fbbik reference는 수동으로 채워야 함 (TODO: 자동 설정으로!)
         *  2. (option) customize script 같이 추가한다. - customize script 을 한 경우 set original length를 실행한다. + ResetScript
         *  3. ikScript 의 FBBIK, rootBone, BoneReference의 RootBone 을 채운 후에 IK스크립트의 AutoInit을 누른다
         *  4. (option) Grip 등을 위해 Hand Poser과 BoneLink를 추가한다.
         */
        string error = "";
        
        ik = GetComponent<FullBodyBipedIK>();
        _boneReference = GetComponent<BoneReference>();
        
        if (ik == null)
        {
            Debug.LogWarning("IK script is null");
            return;
        }
        
        if (ik.ReferencesError(ref error) && rootBone != null)
        {
            var bipedRef = new BipedReferences();
            BipedReferences.AutoDetectReferences(ref bipedRef, rootBone, BipedReferences.AutoDetectParams.Default);
            
            ik.SetReferences(bipedRef, null);
            
            
        }
        
        if (ik.ReferencesError(ref error))
        {
            Debug.LogWarning("IK script is not initiated");
            return;
        }

        
        
        ik.solver.bodyEffector.positionWeight = 0.5f;
        ik.solver.leftHandEffector.positionWeight = 1.0f;
        ik.solver.leftHandEffector.rotationWeight = 1.0f;
        ik.solver.leftShoulderEffector.positionWeight = 1.0f;
        ik.solver.leftArmChain.bendConstraint.weight = 0.8f;

        ik.solver.rightHandEffector.positionWeight = 1.0f;
        ik.solver.rightHandEffector.rotationWeight = 1.0f;

        ik.solver.rightShoulderEffector.positionWeight = 1.0f;

        ik.solver.rightArmChain.bendConstraint.weight = 0.8f;

        ik.solver.leftFootEffector.positionWeight = 1.0f;
        ik.solver.leftFootEffector.rotationWeight = 1.0f;

        ik.solver.leftThighEffector.positionWeight = 1.0f;

        ik.solver.leftLegChain.bendConstraint.weight = 0.8f;

        ik.solver.rightFootEffector.positionWeight = 1.0f;
        ik.solver.rightFootEffector.rotationWeight = 1.0f;

        ik.solver.rightThighEffector.positionWeight = 1.0f;
        ik.solver.rightLegChain.bendConstraint.weight = 0.8f;

        _boneReference.root = rootBone;
        _boneReference.AutoUpdateReferences();
        
        baseRotations.Clear();

        var ikrig = transform.Find("IKRig");
        
        if (ikrig != null)
        {
            DestroyImmediate(ikrig.gameObject);
        }
            
        var obj = new GameObject("IKRig");
        obj.transform.parent = transform;
        obj.AddComponent<Rig>();
    
        foreach (var dict in CSVReader.jointCsv) //IKRig 생성
        {
            string ikName = (string)dict["IKName"];
            
            Debug.Log("IKRig Creation Phase: " + ikName + "setting");
        
            var ikObject = new GameObject(ikName);
            ikObject.transform.parent = obj.transform;
            
            if (ikName.Contains("Body"))
            {
                continue;
            }
            
            ikObject.transform.position = _boneReference.GetReferenceByName(ikName).position;
        }

        var rig = GetComponent<RigBuilder>();

        if (rig != null) // Rig Visaul Update Phase
        {
            if (rig.layers.Count >= 1)
            {
                rig.layers[0]= new RigLayer(obj.GetComponent<Rig>());
            }
            
            foreach (var layer in rig.layers)
            {
                Debug.Log("layer name : " + layer.name);
                
                #if UNITY_EDITOR
                
                for (int i = 0; i < layer.rig.transform.childCount; i++ )
                {//릭 시각화 세팅
                    var rigTransform = layer.rig.transform.GetChild(i);
    
                    Mesh LoadShape(string filename)
                    {
                        const string EditorFolder = "Packages/com.unity.animation.rigging/Editor/";
                        //const string ShadersFolder = EditorFolder + "Shaders/";
                        const string ShapesFolder = EditorFolder + "Shapes/";
                        
                        return AssetDatabase.LoadAssetAtPath<Mesh>(ShapesFolder + filename);
                    }
                    
                    var style = new RigEffectorData.Style()
                    {
                        shape =  LoadShape("LocatorEffector.asset"),
                        color = new Color(1f, 0f, 0f, 0.5f),
                        size = 0.10f,
                        position = Vector3.zero,
                        rotation = Vector3.zero
                    };
                    
                    rig.AddEffector(rigTransform, style);
                    Debug.Log("1RigEffector" + layer.rig.transform.GetChild(i).name);
                    
                    
                }
                
                #endif
                
                
            }
        }
        
        

        foreach (var dict in CSVReader.jointCsv)
        {
            string jointType = (string)dict["JointType"];
            string ikName = (string)dict["IKName"];
            string ikProperty = (string)dict["IKProperty"];



            if (jointType.Equals("Bind") || jointType.Equals("Position"))
            {

                if (!ikProperty.Equals(""))
                {
                    var targetTransform = Helpers.FindIKRig(transform, ikName).transform;
                    Helpers.SetValue(ik, ikProperty, targetTransform);
                }


            }

            if (jointType.Equals("Rotation"))
            {

                var boneName = (string)dict["IKName"];

                var refTransform = _boneReference.GetReferenceByName(boneName);
                if (refTransform == null)
                {
                    Debug.LogError("Reference transform is null, maybe forget to init reference?");
                }

                var ikRig = Helpers.FindIKRig(transform, boneName).rotation;

                Debug.Log("refTransform" +refTransform.rotation + " " + refTransform.localRotation);


                var diff = refTransform.localRotation;
                var upvector = refTransform.up;

                baseRotations.Add(boneName, new InitRotation(diff, upvector));


            }
        }
        
        
        
        

    }
    void Start()
    {
        ik = GetComponent<FullBodyBipedIK>();
        _boneReference = GetComponent<BoneReference>();
    }
    // Update is called once per frame
    void Update()
    {
        CSVReader.isMirrored = isMirroredReceving;
        
        
        
        if (isStopReceving)
        {
            return;
        }
        
        var received = receiver.GetCoord();
        
        { // 머리 부분을 따로 적용하는 코드입니다. 이것도 이미 있던 코드입니다.
            var IK = Helpers.FindIKRig(transform, "Head");

            var chin = Helpers.GetReceivedPosition(received, 32);
            var eye1 = Helpers.GetReceivedPosition(received, 41);
            var eye2 = Helpers.GetReceivedPosition(received, 50);
            var nose = Helpers.GetReceivedPosition(received, 54);
            
            var ranchor = Helpers.GetReceivedPosition(received, 25);
            var lanchor = Helpers.GetReceivedPosition(received, 39);
            
            // 머리의 위치 계산 (모든 스피어의 평균 위치)
            Vector3 headPosition = (eye1 + eye2 + nose + ranchor + lanchor) / 5.0f;
            Vector3 localHeadPosition = headPosition;
            IK.position = localHeadPosition  + transform.position;

            // 머리의 방향 계산 (여기서는 단순히 눈의 중간과 코를 기준으로 계산)
            
            Vector3 anchorCenter = (ranchor + lanchor) / 2.0f;
            Vector3 right = (lanchor- ranchor).normalized;
            Vector3 up = (anchorCenter - chin).normalized;
            //Vector3 up = Vector3.Cross((ear1 - ear2).normalized, forward).normalized;

            Vector3 front = Vector3.Cross(up, right);

            if (isMirroredReceving)
            {
                front = -front;
            }
            
            if (debug)
            {
                Debug.DrawRay(localHeadPosition, front * 0.1f, Color.blue);
                Debug.DrawRay(localHeadPosition, up * 0.1f, Color.green);
                Debug.DrawRay(localHeadPosition, right * 0.1f, Color.red);
            }
            
            // 머리의 회전 적용
            Quaternion headRotation = Quaternion.LookRotation(front, up);
            IK.rotation = headRotation;
        }
        
        { // 몸통 부분을 따로 처리하는 코드입니다. 
            var ls = Helpers.GetReceivedPosition(received, 2); // Left Shoulder
            var rs = Helpers.GetReceivedPosition(received, 5); // Right Shoulder
            var lt = Helpers.GetReceivedPosition(received, 8); // Left Thigh
            var rt = Helpers.GetReceivedPosition(received, 11); // Right Thigh

            var neck = Helpers.GetReceivedPosition(received, 1);
            
            var ms = (ls + rs) / 2.0f;
            

            ms = (ms + neck) / 2.0f;
            
            var mt = (lt + rt) / 2.0f;
            
            var body = ms * 0.3f + mt * 0.7f;
            var ikRig = Helpers.FindIKRig(transform, "Body");
            
            

            ikRig.position = body + transform.position;
        }
        
        var csv = CSVReader.jointCsv;
        
        
        
        foreach (var dict in csv)
        {
            string jointType = (string)dict["JointType"];

            if (jointType.Equals("Bind"))
            {
                continue;
            }
            
            string ikName = (string)dict["IKName"];
            int jointID = (int)dict["JointID"];
            
            
            
            var ikRig = Helpers.FindIKRig(transform, ikName);
            
            //Debug.Log(ikName);

            var unsizedCoord = Helpers.GetReceivedPosition(received,jointID);
            var ikPosition = unsizedCoord;

            switch (jointType)
            {
                case "Position":
                {
                    int TargetID = (int)dict["TargetID"];

                    // if (TargetID != -1)
                    // {
                    //     int Adjust = (int)dict["Adjust"];
                    //     
                    //     var AdjustTarget = ReceivedLocationToLocalLocation(Helpers.GetReceivedPosition(received,TargetID));
                    //
                    //
                    //     var Ori = ikPosition * ((100 - Adjust) * 0.01f);
                    //     var Adj = AdjustTarget * (Adjust * 0.01f);
                    //
                    //     ikPosition = Ori + Adj;
                    // }
                    
                    ikRig.position = ikPosition + transform.position;
                    break;
                }
                
                case "Rotation":
                {
                    int TargetID = (int)dict["TargetID"];
                    int HintID = (int)dict["HintID"];
                    
                    var boneName = (string)dict["IKName"];
                    
                    
                    bool isLeft = boneName.Contains("Left");
                    bool isHand = boneName.Contains("Hand");
                    
                    var target = Helpers.GetReceivedPosition(received, TargetID);
                    var hint = Helpers.GetReceivedPosition(received, HintID);

                    var coord = unsizedCoord;

                    var rawhintvector = hint - coord;
                    var rawtargetvector = target - coord;

                    var fix = Vector3.Project(rawhintvector, rawtargetvector);
                        
                    var targetvector = rawtargetvector.normalized;
                    var thumbhintvector = (rawhintvector - fix).normalized;

                    var initRot = baseRotations[boneName];

                    var initialUpVector = initRot.upvector;
                    var initialFootRotation = initRot.rotation;
                    
                    var handfacevector = Vector3.Cross(targetvector, thumbhintvector);
                    if (isLeft) // flip
                    {
                        handfacevector = -handfacevector;
                    }

                    
                    Quaternion targetRotation = Quaternion.LookRotation(handfacevector, targetvector);
                    
                    




                    ikRig.localRotation = targetRotation * Quaternion.Euler(initRot.customRotation);

                    if (debug)
                    {
                        Debug.DrawRay(ikRig.position,  thumbhintvector * 0.4f, Color.red); //x
                        Debug.DrawRay(ikRig.position, targetvector * 0.4f, Color.green); //y
                        Debug.DrawRay(ikRig.position, handfacevector * 0.4f, Color.black); //z
                        // Debug.DrawRay(ikRig.position, rawhintvector.normalized * 1.0f, Color.blue);
                        // Debug.DrawRay(ikRig.position, rawtargetvector.normalized  * 1.0f, Color.magenta);
                    }
                    
                    break;
                }


                case "Grip":
                {
                    int TargetID = (int)dict["TargetID"];
                    int HintID = (int)dict["HintID"];
                    string boneName = (string)dict["IKProperty"];
                    var middle = Helpers.GetReceivedPosition(received,TargetID);
                    var last = Helpers.GetReceivedPosition(received,HintID);

                    var d1 = (middle - ikPosition).normalized;
                    var d2 = (last - middle).normalized;

                    var factor = Vector3.Cross(d1, d2);

                    if (factor.magnitude > 0.5f)
                    {
                        //Debug.Log("Grip");
                    }

                    
                    
                    var link = ikRig.gameObject.GetComponent<BoneLink>();

                    if (link == null)
                    {
                        break;
                    }

                    var poser = link.bone.GetComponent<HandPoser>();
                    if (poser == null)
                    {
                        Debug.Log("Poser not found");
                        break;
                    }
                    
                    //Debug.Log("Poser found");
                    
                    poser.weight = factor.magnitude;
                    
                    if (debug)
                    {
                        //
                        // Debug.DrawRay(ikPosition, d1 * 2.0f, Color.red);
                        // Debug.DrawRay(ikPosition, d2 * 2.0f, Color.green);
                    }
                    
                    break;
                }
            }
        }
        
        
        // 배경에 수신받은 좌표 기준으로 선을 그리는 디버깅 코드입니다. 
    }
}
