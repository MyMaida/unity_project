using System.Collections.Generic;
using OnlyNew.BodyProportions;
using UnityEngine;
// ReSharper disable InconsistentNaming




public class ExBoneReference : MonoBehaviour
{
    // bone들의 레퍼런스. ikRig의 레퍼런스가 아님.

    public Transform rootBone;
    
    public List<Transform> bones;

    public bool dontBind;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public Transform GetReferenceByName(string name)
    {
        var b = bones.Find(b => b.name.Contains(name));
        
        if (b == null)
        {
            Debug.LogWarning("ExBoneReference::GetReferenceByName::" + name + " is null!");
        }
        
        return b;
    }
    
    [ContextMenu("AutoUpdateReferences")]
    public void AutoUpdateReferences()
    {
        bones.Clear();
        
        if (rootBone == null)
        {
            return;
        }

        for (int i = 0; i < rootBone.childCount; i++)
        {
            var child = rootBone.GetChild(i);

            if (!child.name.Contains("_End") && !child.name.Contains("Base"))
            {
                bones.Add(child);
            }

            if (dontBind)
            {
                var ScalableBone = child.GetComponent<ScalableBone>();

                ScalableBone.bindRotation = false;
                ScalableBone.bindPosition = false;
            }
        }
        
        bones.Add(rootBone);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
