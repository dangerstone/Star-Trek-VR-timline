using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class Donut : MonoBehaviour
{
    public GameObject tickParent;

    public int defaultLowerbound;
    public int defaultUpperbound;
    public float defaultDeadzoneSize;

    private int lowerbound;
    private int upperbound;
    private float deadZoneSize;

    public float radius;
    public float height;

    public GameObject tickPrefab;
    public GameObject deadZoneIndicatorPrefab;
    public GameObject deadZoneParent;
    public GameObject instantiator; // public FlagHandler flagHandler;
    private FlagHandler flagHandler;

    private Vector3 originVector;

    private int tickGap;

    Vector3 donutCenter = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start() {
        // set timeline to default time as defined in the scene (public variables)
        lowerbound = defaultLowerbound;
        upperbound = defaultUpperbound;
        deadZoneSize = defaultDeadzoneSize;
        deadZoneOffset = deadZoneSize / 2;

        originVector = new Vector3(radius * Mathf.Cos(1), height, radius * Mathf.Sin(0));

        tickGap = computeTickGap(lowerbound, upperbound);
        makeDeadZoneTicks();
        renderTickMarks(tickGap);

        flagHandler = instantiator.GetComponent<FlagHandler>();
        flagHandler.setFlags(radius, height, deadZoneSize, lowerbound, upperbound);
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public int getStartYear() {
        return lowerbound;
    }
    public int getEndYear() {
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
            updateTickMarks(change);
    }

    int computeTickGap(int lower, int upper) {
        int interval = upper - lower;
        int gap = interval switch {
            <= 10 => 1,
            < 50 => 5,
            < 100 => 10,
            < 500 => 50,
            < 1000 => 100,
            _ => 200, // NOTE look at the actual timescope of ST to make better rules here maybe
        };
        Debug.Log("Gap: " + gap);
        return gap;
    }

    void updateTickMarks(float extraOfsset)
    {
        int childs = tickParent.transform.childCount;
        for (int i = childs-1 ; i >= 0; i--)//Start with removing the outdated tickmarks
        {
            Transform child = (tickParent.transform.GetChild(i));
            child.RotateAround(new Vector3(0, 0, 0), Vector3.up, Degrees(extraOfsset));
            var angleBetweenTickAndOrigin = Vector3.Angle(originVector, child.transform.position);
            if(angleBetweenTickAndOrigin <= Degrees(deadZoneOffset))
            {
                Debug.Log("Should kill " + child.name);
                Destroy(child.gameObject);
            }
            Debug.Log("Child number " + i);

        }
        IComparer myComparer = new FlagComparer();
        GameObject[] allTicks = GameObject.FindGameObjectsWithTag("aTick"); //update such that the deleted are gone
        Debug.Log("Tick count " + allTicks.Length);

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

                GameObject newChild = Instantiate(tickPrefab, pos, Quaternion.LookRotation(-pos), tickParent.transform);
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

                GameObject newChild = Instantiate(tickPrefab, pos, Quaternion.LookRotation(-pos), tickParent.transform);

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
    
    private float deadZoneOffset;
    private GameObject firstBorn;

  
    void renderTickMarks(int gap) {
        
        // compute how many ticks
        int noOfTicks = (int)(upperbound-lowerbound) / gap;
        Debug.Log("Number of ticks " + noOfTicks);

        // Vector3 donutCenter = new Vector3(0, 0, 0);
    
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
            GameObject flag = Instantiate(tickPrefab, pos, rot, tickParent.transform);
            flag.tag = "aTick";
            int year = lowerbound + i * gap;
            flag.name = "Tick" + year;
            TMP_Text label =  flag.transform.Find("Label").GetComponent<TMP_Text>();
            label.text = year.ToString();
        }
    }

    void makeDeadZoneTicks()
    {   
        // #ruskode :)
        Vector3 deadzoneVectorStart = new Vector3(radius * Mathf.Cos(-7*deadZoneOffset/8), height, radius * Mathf.Sin(-7*deadZoneOffset/8));
        Vector3 deadzoneVectorBeep = new Vector3(radius * Mathf.Cos(4*deadZoneOffset/9), height, radius * Mathf.Sin(-4*deadZoneOffset/9));
        Vector3 deadzoneVectorMiddle = new Vector3(radius, height, 0);
        Vector3 deadzoneVectorBoop = new Vector3(radius * Mathf.Cos(4*deadZoneOffset/9), height, radius * Mathf.Sin(4*deadZoneOffset/9));
        Vector3 deadzoneVectorEnd = new Vector3(radius * Mathf.Cos(7*deadZoneOffset/8), height, radius * Mathf.Sin(7*deadZoneOffset/8));

        List<Vector3> deadzoneIndicatorPositions = new List<Vector3>();
        deadzoneIndicatorPositions.Add(deadzoneVectorStart);
        deadzoneIndicatorPositions.Add(deadzoneVectorBeep);
        deadzoneIndicatorPositions.Add(deadzoneVectorMiddle);
        deadzoneIndicatorPositions.Add(deadzoneVectorBoop);
        deadzoneIndicatorPositions.Add(deadzoneVectorEnd);

        foreach (Vector3 pos in deadzoneIndicatorPositions) {
            var lookPos = donutCenter - pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            Instantiate(deadZoneIndicatorPrefab, pos, rot, deadZoneParent.transform);
        }
    }

    public void zoomIn(int delta) {
        // update lower and upperbound based on scaling
        int newLower = getStartYear() + delta;
        int newUpper = getEndYear() - delta; //decrease the interval
        // if(newUpper - newLower <= 5)
        // {
        //     newLower = getStartYear(); //Idk how to make sure we dont go to small scale 
        //     newUpper = getEndYear();
        //}//will still do the hole redraw even though there might not be changes to the interval...
        //what happens if we zoom in at 3300? Idk so dont try
        lowerbound = newLower;
        upperbound = newUpper;
        Debug.Log("Interval is now [" + lowerbound + "," + upperbound + "]");
        RerenderScene();
    }
    public void zoomOut(int delta) {
        // update lower and upperbound based on scaling
        int newLower = getStartYear() - delta;
        int newUpper = getEndYear() + delta; //increase the interval
        // if(newLower < 1900)
        // {
        //     newLower = 1900; //cannot go more back than this but can still change upper
        // }
        // if(newUpper > 3500)
        // {
        //     newUpper = 3500;//cannot go higher than this
        // }
        if(getStartYear()==1900 && getEndYear() == 3500) { return; } //no need to rerender again
        lowerbound = newLower; //zooming out
        upperbound = newUpper;   //zooming out
        Debug.Log("Interval is now [" + lowerbound + "," + upperbound + "]");
        RerenderScene();
    }

    public void RerenderScene()
    {
        tickGap = computeTickGap(lowerbound, upperbound);
        KillChildrenTicks();//remove all old tickMarks
        flagHandler.KillAllFlags();//Remove flags
        renderTickMarks(tickGap); //Redraw from scratch
        flagHandler.setFlags(radius, height, deadZoneSize, lowerbound, upperbound);
    }

    float Degrees(float radian)
    {
        return radian * 180 / Mathf.PI;
    }

    float Radians(float degree)
    {
        return degree * Mathf.PI / 180;
    }

    //Find all tickmarks and destroy the gameobjects. 
    void KillChildrenTicks()
    {
        int childCount = tickParent.transform.childCount;
        for(int i = childCount-1; i>=0; i--)
        {
            Transform child = (tickParent.transform.GetChild(i));
            Destroy(child.gameObject);
        }
    }
}
