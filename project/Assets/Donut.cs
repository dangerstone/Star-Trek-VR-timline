using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class Donut : MonoBehaviour
{
    public GameObject parent;

    public int defaultLowerbound;
    public int defaultUpperbound;
    public float defaultDeadzoneSize;

    private int lowerbound;
    private int upperbound;
    private float deadZoneSize;

    public float radius;
    public float height;

    public GameObject tickPrefab;
    public GameObject deadTickPrefab;
    public GameObject instantiator; // public FlagHandler flagHandler;
    private FlagHandler flagHandler;

    private int tickGap;

    // Start is called before the first frame update
    void Start() {
        // set timeline to default time as defined in the scene (public variables)
        lowerbound = defaultLowerbound;
        upperbound = defaultUpperbound;
        deadZoneSize = defaultDeadzoneSize;
        deadZoneOffset = deadZoneSize / 2;

        tickGap = computeTickGap(lowerbound, upperbound);
        makeDeadZoneTicks();
        renderTickMarks(tickGap,0);

        flagHandler = instantiator.GetComponent<FlagHandler>();
        flagHandler.setFlags(radius, height, deadZoneSize, lowerbound, upperbound);
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    int getStartYear() {
        return lowerbound;
    }
    int getEndYear() {
        return upperbound;
    }
   
    float getDeadzoneSize() {
        return deadZoneSize;
    }

    [SerializeField]
    float radiansPerTick;
    public float getRadianPerTick()
    {
        return radiansPerTick;
    }

    public void updatePositions(float change)
    {
        //Debug.Log("change: " + change + ". radiPerTick: " + radiansPerTick);
        if(Mathf.Abs(change) > radiansPerTick) //We need to update the years
        {
            int yearsToChange = Mathf.FloorToInt(Mathf.Abs(change / radiansPerTick));
            Debug.Log(yearsToChange +" years to change " +  " from " + change + "/" + radiansPerTick);
            if(change < 0) //If negative then some way
            {
                lowerbound += yearsToChange;
                upperbound += yearsToChange;
            } else
            {//Else the other way
                lowerbound -= yearsToChange;
                upperbound -= yearsToChange;
            }
            renderTickMarks(computeTickGap(lowerbound, upperbound),0f);
        } else
        {
            updateTickMarks(change);
        }
    }

    int computeTickGap(int lower, int upper) {
        int interval = upper - lower;
        int gap = interval switch {
            < 10 => 1,
            < 50 => 5,
            < 100 => 10,
            < 500 => 50,
            < 1000 => 100,
            _ => 1000, // NOTE look at the actual timescope of ST to make better rules here maybe
        };
        Debug.Log("Gap: " + gap);
        return gap;
    }

    void updateTickMarks(float extraOfsset)
    {
        int childs = parent.transform.childCount;

        IComparer myComparer = new FlagComparer();
        //Debug.Log("number children" + childs);
        //Debug.Log("Interval [" + lowerbound + "," + upperbound + "]");
        for (int i = 0 ; i < childs; i++)//Start with removing the outdated tickmarks
        {
            Transform child = (parent.transform.GetChild(i));
            child.RotateAround(new Vector3(0, 0, 0), Vector3.up, Degrees(extraOfsset));
            var angleBetweenTickAndOrigin = Vector3.Angle(originVector, child.transform.position);
            if(angleBetweenTickAndOrigin <= Degrees(deadZoneOffset))
            {
                Debug.Log("Should kill " + child.name);
                Destroy(child.gameObject);
            }

        }
        GameObject[] allTicks = GameObject.FindGameObjectsWithTag("aTick"); //update such that the deleted are gone
        Array.Sort(allTicks, myComparer);//Sort by name such that first is lowest 

        flagHandler.updateFlagPositions(extraOfsset);
        if(extraOfsset > 0) //rotere med uret, dvs spawn new lowerbound, last born killed
        {
            firstBorn = allTicks[0];
            float angleToFirstBorn = Vector3.Angle(originVector, firstBorn.transform.position);
            if (angleToFirstBorn >= Degrees(radiansPerTick + deadZoneOffset))
            {
                float radians = Radians(angleToFirstBorn) - radiansPerTick;// 
                Vector3 pos = (new Vector3(radius * Mathf.Cos(-radians), height, radius * Mathf.Sin(-radians)));

                GameObject newChild = Instantiate(tickPrefab, pos, Quaternion.LookRotation(-pos), parent.transform);
                TMP_Text label = firstBorn.transform.Find("Label").GetComponent<TMP_Text>();
                TMP_Text labelNew = newChild.transform.Find("Label").GetComponent<TMP_Text>();
                int newYear = Int32.Parse(label.text) - tickGap;
                lowerbound -= tickGap;
                upperbound -= tickGap;
                labelNew.text = newYear.ToString();
                newChild.name = "Tick" + labelNew.text;
                newChild.tag = "aTick";
                firstBorn = newChild;
                flagHandler.enterNewFlags(newYear, newYear + tickGap, radius, height, lowerbound);
                
            }
        }
        else //rotere mod uret, spawn new upperbound, first born has been killed
        {
            if(allTicks.Length == 0)
            {
                return;
            }
            GameObject lastBorn = allTicks[allTicks.Length - 1];
            float angleToLastBorn = Vector3.Angle(originVector, lastBorn.transform.position);
            if (angleToLastBorn >= Degrees(radiansPerTick + deadZoneOffset))
            {

                float angleToRadian = Radians(angleToLastBorn);
                float radians = angleToRadian - radiansPerTick;// 
                Vector3 pos = (new Vector3(radius * Mathf.Cos(radians), height, radius * Mathf.Sin(radians)));

                GameObject newChild = Instantiate(tickPrefab, pos, Quaternion.LookRotation(-pos), parent.transform);

                TMP_Text label = lastBorn.transform.Find("Label").GetComponent<TMP_Text>();
                TMP_Text labelNew = newChild.transform.Find("Label").GetComponent<TMP_Text>();
                int newYear = Int32.Parse(label.text) + tickGap;
                lowerbound += tickGap;
                upperbound += tickGap;
                labelNew.text = newYear.ToString();
                newChild.name = "Tick" + labelNew.text;
                newChild.tag = "aTick";

                flagHandler.enterNewFlags(newYear -tickGap, newYear, radius, height, lowerbound);
            }

        }
    }
    private Vector3 deadzoneVector;
    private Vector3 originVector;
    private float deadZoneOffset;
    private GameObject firstBorn;

  
    void renderTickMarks(int gap, float extraOffset) {
        
        // compute how many ticks
        int noOfTicks = (int)(upperbound-lowerbound) / gap;
        Debug.Log("Number of ticks " + noOfTicks);

        Vector3 donutCenter = new Vector3(0, 0, 0);
    
        radiansPerTick = (Mathf.PI*2f - deadZoneSize) / noOfTicks;
        Debug.Log("radianPerTick" + radiansPerTick);

        // loop over that many ticks
        for (int i = 0; i <= noOfTicks; i++) {
            // compute position with formula
            float radians = i * radiansPerTick + deadZoneOffset;//  (i-lowerbound) * radiansPerTick + deadZoneOffset; // float radians = (i-lowerbound) * (Mathf.PI*2f - deadZoneSize) / noOfTicks + deadZoneSize/2;
            Vector3 pos = donutCenter + (new Vector3(radius *Mathf.Cos(-radians), height, radius *Mathf.Sin(-radians)));
            // rotate tick to face center
            var lookPos = donutCenter - pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            // instantiate and set the tick text
            GameObject flag = Instantiate(tickPrefab, pos, rot, parent.transform);
            flag.tag = "aTick";
            int year = lowerbound + i * gap;
            flag.name = "Tick" + year;
            TMP_Text label =  flag.transform.Find("Label").GetComponent<TMP_Text>();
            label.text = year.ToString();
        }
    }


    void makeDeadZoneTicks()
    {
        deadzoneVector = new Vector3(radius * Mathf.Cos(-deadZoneOffset), height, radius * Mathf.Sin(-deadZoneOffset));
        GameObject beginning = Instantiate(deadTickPrefab, deadzoneVector, Quaternion.identity, transform); //this is where the years come out /lowerbound on the timescale
        beginning.name = "beginning";

        Vector3 deadzoneVector2 = new Vector3(radius * Mathf.Cos(deadZoneOffset), height, radius * Mathf.Sin(deadZoneOffset));
        GameObject endOfTimelinePoint = Instantiate(deadTickPrefab, deadzoneVector2, Quaternion.identity, transform);//this is where the upperbound lies
        endOfTimelinePoint.name = "endOfTime";

        originVector = new Vector3(radius * Mathf.Cos(1), height, radius * Mathf.Sin(0));
        //GameObject deadzoneTick3 = Instantiate(deadTickPrefab, originVector, Quaternion.identity, transform);
        //deadzoneTick3.name = "origin";
    }

    public void zoomIn() {
        // update lower and upperbound based on scaling
    }
    public void zoomOut() {
        // update lower and upperbound based on scaling
    }

    float Degrees(float radian)
    {
        return radian * 180 / Mathf.PI;
    }

    float Radians(float degree)
    {
        return degree * Mathf.PI / 180;
    }
}
