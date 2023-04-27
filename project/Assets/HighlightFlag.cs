using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightFlag : MonoBehaviour
{
    public Object testInstance;
    private Color startcolor;
    
    // Start is called before the first frame update
    void Start()
    {
        startcolor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
        Instantiate(testInstance);
    }
    public void OnPointExit()
    {
        GetComponent<Renderer>().material.color = startcolor;
    }
}
