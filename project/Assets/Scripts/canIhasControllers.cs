using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class canIhasControllers : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController LeftController;

    [SerializeField]
    private ActionBasedController RightController;



    private Vector3 touchDownStartPosition;

    private Color originalColor;

    [SerializeField]
    private Donut donutScript;

    //private XRIDefaultInputActions controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (rightTouch && leftTouch)
        {
            Debug.Log("old dist " +distanceBetweenControllers);
            float newDist = Vector3.Distance(RightController.transform.position, LeftController.transform.position);
            Debug.Log("new dist " + newDist);
            float diff = newDist - distanceBetweenControllers; //if diff is negative zoom in. If positive zoom out
            int delta = Mathf.FloorToInt(diff * 10);
            Debug.Log("new dist - olddist =  " + newDist + "-" + distanceBetweenControllers + " = " + diff + ".\n Delta is " + delta);
            //int delta = Mathf.FloorToInt(Mathf.Exp(diff * 20));
            //int delta2 = Mathf.FloorToInt(Mathf.Exp(diff * 90));
            //int delta3 = Mathf.FloorToInt(d+iff * 50);
            //Debug.Log("Delta " + delta);
            float offset = 0.5f; //something for keeping un-intended jitters undetected
            if (delta < 0 - offset)
            {
               donutScript.ZoomOutSwitch();
            } else if (delta > 0 + offset)
            {
                donutScript.ZoomInSwitch();
            }
            distanceBetweenControllers = newDist;
        } else 
        if (leftTouch && !rightTouch)
        {
            if(donutScript == null)
            {
                Debug.Log("Donut is null");
                return;
            }
            
            //calculate angle from touchdownPoint
            //Vector2 Point_1 = new Vector2(touchDownStartPosition.x,touchDownStartPosition.z);
            //Vector2 Point_2 = new Vector2(LeftController.transform.position.x,LeftController.transform.position.z);
            //float angleBetween = Vector2.SignedAngle(Point_1, Point_2);
            float angleBetween = Vector3.SignedAngle(touchDownStartPosition, LeftController.transform.position, Vector3.up);
            //float angle = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * 180 / Mathf.PI;
            //Debug.Log("Left angle "+angle);
            //Debug.Log("Left angle2 "+angleBetween);
            donutScript.updatePositions(angleBetween*0.2f);
            touchDownStartPosition = LeftController.transform.position;

        } else
        if (rightTouch && !leftTouch)
        {
            //float angleBetween = Vector3.Angle(rightpos, RightController.transform.position);
            //Vector2 Point_1 = new Vector2(touchDownStartPosition.x, touchDownStartPosition.z);
            //Vector2 Point_2 = new Vector2(RightController.transform.position.y, RightController.transform.position.z);
            //float angleBetween = Vector2.SignedAngle(Point_1, Point_2);
            float angleBetween = Vector3.SignedAngle(touchDownStartPosition, RightController.transform.position, Vector3.up);
            //Debug.Log("Right angle "+angleBetween);
            //float angle = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * 180 / Mathf.PI;
            //bool toLeft = Point_2.x - Point_1.x >= 0;
            //Debug.Log("Right2Angle " + angle);
            //Debug.Log("going left " + toLeft);
            //Debug.Log("new-old " + (angle - oldangle));
            //oldangle = angle;
            donutScript.updatePositions(angleBetween*0.2f);
            touchDownStartPosition = RightController.transform.position;
        }
        
    }


    private float distanceBetweenControllers = 0f;

    private bool leftTouch = false;
    public void selectionLeftDown(ActionBasedController controller) {
        LeftController = controller;
        touchDownStartPosition = controller.transform.position;
        leftTouch = true;
        //Debug.Log("leftouch "+leftTouch);
        if (rightTouch)
        {
            distanceBetweenControllers = Vector3.Distance(RightController.transform.position, LeftController.transform.position);
        }
    }

    private bool rightTouch = false;
    //private Vector3 rightpos;
    public void selectionRightDown(ActionBasedController controller)
    {
        rightTouch = true;
        touchDownStartPosition = controller.transform.position;
        //Debug.Log("righttouch "+rightTouch);
        //rightpos = controller.transform.position;
        if (leftTouch)
        {
            distanceBetweenControllers = Vector3.Distance(RightController.transform.position, LeftController.transform.position);
        }
    }

    /*On every tick: If "bIsScaling" is true, set the object's scale to (current distance between controllers) - fInitialDist * fScaleSensitivityMultiplier. */

    public void rightUp()
    {
        rightTouch = false;
        //Debug.Log("righttouch "+rightTouch + ". LeftTouch is " + leftTouch);
    }

    public void leftUp()
    {
        leftTouch = false;
        //Debug.Log("leftouch "+leftTouch + ". Righttouch is " + rightTouch);
    }


}
