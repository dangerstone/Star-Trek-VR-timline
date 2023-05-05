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

    private bool touchDown;
    private bool twoTouchDown;

    private Vector3 touchDownStartPosition;

    private Renderer renderer;
    private Color originalColor;

    [SerializeField]
    private Donut donutScript;

    //private XRIDefaultInputActions controller;
    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetComponent<Renderer>();
        //originalColor = renderer.shader.;
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
            Debug.Log("Delta " + delta);
            if (delta < 0)
            {
               // donutScript.zoomIn(delta);
            } else
            {
                //donutScript.zoomOut(delta);
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

    private void FixedUpdate()
    {
        
    }

/*    public void selectionDown(ActionBasedController controller)
    {
        *//*if (touchDown)
        {
            Debug.Log("Two touching");
            twoTouchDown = true;
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = Color.green;
            }
        } else
        {
            Debug.Log("One touching");
        }*//*
        renderer.material.color = Color.blue;
        touchDownStartPosition = controller.transform.position;
        touchDown = true;
        Debug.Log("selected " + LeftController.transform.position.ToString());
        
        Debug.Log("selected " + RightController.transform.position.ToString());

    }*/

    private float distanceBetweenControllers = 0f;

    private bool leftTouch = false;
    public void selectionLeftDown(ActionBasedController controller) {
        LeftController = controller;
        touchDownStartPosition = controller.transform.position;
        leftTouch = true;
        renderer.material.color = Color.blue;
        Debug.Log("leftouch "+leftTouch);
        if (rightTouch)
        {
            renderer.material.color = Color.green;
            distanceBetweenControllers = Vector3.Distance(RightController.transform.position, LeftController.transform.position);
        }
    }

    private bool rightTouch = false;
    //private Vector3 rightpos;
    public void selectionRightDown(ActionBasedController controller)
    {
        rightTouch = true;
        touchDownStartPosition = controller.transform.position;
        Debug.Log("righttouch "+rightTouch);
        renderer.material.color = Color.blue;
        //rightpos = controller.transform.position;
        if (leftTouch)
        {
            renderer.material.color = Color.green;
            distanceBetweenControllers = Vector3.Distance(RightController.transform.position, LeftController.transform.position);
        }
    }

    /*On every tick: If "bIsScaling" is true, set the object's scale to (current distance between controllers) - fInitialDist * fScaleSensitivityMultiplier. */

    public void rightUp()
    {
        rightTouch = false;
        Debug.Log("righttouch "+rightTouch + ". LeftTouch is " + leftTouch);
        if (!leftTouch)
        {
            renderer.material.color = originalColor;
        }
    }

    public void leftUp()
    {
        leftTouch = false;
        Debug.Log("leftouch "+leftTouch + ". Righttouch is " + rightTouch);
        if (!rightTouch)
        {
            renderer.material.color = originalColor;
        }
    }

    public void selectionUp()
    {
       /* if (twoTouchDown)
        {
            twoTouchDown = false;
            if (renderer != null && renderer.material != null)
            {
            }
            Debug.Log("no more two touching");
            //one should still be touching
          
        } else { //on selectUp where only touching they should both be false
                renderer.material.color = originalColor;

        }*/
            renderer.material.color = originalColor;
            touchDown = false;
            Debug.Log("no one touching");
    }

}
