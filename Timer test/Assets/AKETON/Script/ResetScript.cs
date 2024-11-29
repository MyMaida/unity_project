using System;
using System.Collections;
using System.Collections.Generic;
using OnlyNew.BodyProportions;
using UnityEngine;

[Serializable]
public class TransformData
{

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public static TransformData identity
    {
        get
        {
            return new TransformData();
        }
    }
    
    public TransformData (Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
    {
        position = newPosition;
        rotation = newRotation;
        localScale = newLocalScale;
    }

    public TransformData ()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        localScale = Vector3.one;
    }

    public TransformData (Transform transform)
    {
        copyFrom (transform);
    }

    public void copyFrom (Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
    }

    public void copyTo (Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = localScale;
    }

}

public class ResetScript : MonoBehaviour
{
    public List<TransformData> defaultData = new();
    
    BoneReference reference;
    
    // Start is called before the first frame update
    void Start()
    {
        reference = GetComponent<BoneReference>();

        void FindBoneAnd(Transform transform)
        {
            if (transform.GetComponent<ProtectChildrenFromScalableBonesManager>() != null)
                return;
            
            defaultData.Add(new TransformData(transform));
            
            foreach (Transform t in transform)
            {
                FindBoneAnd(t);
            }
            
            
        }
        
        FindBoneAnd(reference.root);
    }

    // Update is called once per frame
    private void Update()
    {
        int i = 0;
        
        void FindBoneAndReset(Transform transform)
        {
            if (transform.GetComponent<ProtectChildrenFromScalableBonesManager>() != null)
                return;
            
            defaultData[i].copyTo(transform);
            i += 1;
            
            foreach (Transform t in transform)
            {
                FindBoneAndReset(t);
            }
        }
        
        FindBoneAndReset(reference.root);
    }
}
