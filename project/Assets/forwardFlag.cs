using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forwardFlag : MonoBehaviour
{
    // Start is called before the first frame update
    Transform myTransform;
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveForward()
    {
        transform.position -= transform.forward *1.2f;
        Debug.Log("moving forward");
    }

    public void moveBack()
    {
        transform.position += transform.forward*1.2f;
        Debug.Log("moving back");
    }
}
