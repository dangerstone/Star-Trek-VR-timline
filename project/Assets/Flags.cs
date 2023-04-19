using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour
{
    public GameObject parent;
    public GameObject flagPrefab;
    public GameObject donut;
    // public List<???> flagItems = ???? // TODO iterate over this when setting flags instead (currently it's just a dummy iteration)

    // Start is called before the first frame update
    void Start()
    {
        // setFlags();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setFlags() {

        // currently just iterates over a certain number (8) - when we make the 
        // dummy/static timeline, we can just use the length of the episode list, 
        // which would place them uniformly
        // FIXME struggling bc the flags are offset from the donut (or vice versa) and also trying to figure out what the radius actually is 
        int noOfEpisodes = 8;
        float height = 1.2f;
        float radius = 3;
        for (int i = 0; i < noOfEpisodes; i++)
        {
            
            Vector3 donutCenter = new Vector3(0, 0, 0);
            float radians = i * Mathf.PI*2f / noOfEpisodes;
            Vector3 pos = donutCenter + (new Vector3(radius *Mathf.Cos(radians), height, radius *Mathf.Sin(radians)));
            var lookPos = donutCenter - pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            GameObject flag = Instantiate(flagPrefab, pos, rot, donut.transform); // make face center
            // make every other flag taller
            if (i % 2 == 0) {
                flag.transform.localScale += new Vector3(0, 0.5f, 0);
            }
            flag.name = "Flag " + i;
        }
    }

}
