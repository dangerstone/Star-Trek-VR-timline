using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public GameObject leader;
    private Quaternion rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        rotationOffset = new Quaternion(0, 0, 0, 1); //leader.transform.rotation *  c.transform.localRotation.eulerAngles.x
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotationOffset;
        transform.Rotate(Vector3.up, leader.transform.localRotation.eulerAngles.y);
    }
}
