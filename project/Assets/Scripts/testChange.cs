using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testChange : MonoBehaviour
{
    private float lastTime;
    private float firstTime;
    public GameObject instantiator;
    private Donut donut;
    // Start is called before the first frame update
    void Start()
    {
        lastTime = Time.time;
        firstTime = Time.time;

        donut = instantiator.GetComponent<Donut>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Time.time - lastTime > 0.2f && Time.time-firstTime > 5f)
        {
            float radi = donut.getRadianPerTick();
            //donut.updatePositions(-radi*0.01f);
            donut.zoomOut(10);
            lastTime = Time.time;
        }
    }
}
