using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    // Start is called before the first frame update
    void Start() {
        // set timeline to default time as defined in the scene (public variables)
        lowerbound = defaultLowerbound;
        upperbound = defaultUpperbound;
        deadZoneSize = defaultDeadzoneSize;

        int tickGap = computeTickGap(lowerbound, upperbound);
        renderTickMarks(tickGap);
    }

    // Update is called once per frame
    void Update() {
        
    }

    int computeTickGap(int lower, int upper) {
        int interval = upper - lower;
        int gap = interval switch {
            < 10 => 1,
            < 50 => 5,
            < 100 => 10,
            < 500 => 50,
            < 1000 => 100,
            _ => 1000, // NOTE look at the actual timescope of ST to make better rules her maybe
        };
        Debug.Log("Gap: " + gap);
        return gap;
    }

    void renderTickMarks(int gap) {
        // compute how many ticks
        int noOfTicks = (int)(upperbound-lowerbound) / gap;
        Debug.Log("Number of ticks " + noOfTicks);

        Vector3 donutCenter = new Vector3(0, 0, 0);
        // loop over that many ticks
        for (int i = 0; i <= noOfTicks; i++) {
            float radians = (i-lowerbound) * (Mathf.PI*2f - deadZoneSize) / noOfTicks + deadZoneSize/2;
            Vector3 pos = donutCenter + (new Vector3(radius *Mathf.Cos(-radians), height, radius *Mathf.Sin(-radians)));
            // compute position with formula
            // instantiate and set the tick text to lowerbound + i*gap
            var lookPos = donutCenter - pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            GameObject flag = Instantiate(tickPrefab, pos, rot, parent.transform);
            int year = lowerbound + i * gap;
            flag.name = "Tick" + year;
            TMP_Text label =  flag.transform.Find("Label").GetComponent<TMP_Text>();
            Debug.Log(label.text);
            label.text = year.ToString();
            // stop when DZ reached
        }
    }

    public void zoomIn() {
        // update lower and upperbound based on scaling
    }
    public void zoomOut() {
        // update lower and upperbound based on scaling
    }
}
