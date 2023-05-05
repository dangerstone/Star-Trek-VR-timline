using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forwardFlag : MonoBehaviour
{
    private int smoothval;
    // Start is called before the first frame update
    Transform myTransform;
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (smoothval < 0)
        {
            transform.position -= transform.forward *0.075f;
            smoothval ++;
        }
        if (smoothval > 0)
        {
            transform.position += transform.forward *0.075f;
            smoothval --;
        }
    }

    public void moveForward()
    {
        smoothval -= 12;
        //transform.position -= transform.forward *1.2f;
        Debug.Log("moving forward");
    }

    public void moveBack()
    {
        smoothval += 12;
        //transform.position += transform.forward*1.2f;
        Debug.Log("moving back");
    }
}
