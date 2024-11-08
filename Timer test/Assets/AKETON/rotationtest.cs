using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationtest : MonoBehaviour
{
    public Transform defaultRot;
    public Transform receivedRot;
    public Transform resultRot;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        resultRot.rotation = defaultRot.rotation * receivedRot.rotation;
    }
}
