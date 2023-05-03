using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FlagHandler : MonoBehaviour
{
    public GameObject flagParent;
    public GameObject flagPrefab;

    // private int startYear = 2350;
    // private int endYear = 2380;
    // private int interval;
    // private float year_radian;
    // private float deadZoneAngle = 0.2f;

    public TextAsset jsonFile;

    // Start is called before the first frame update
    void Start()
    {
        /* interval = endYear - startYear;
        year_radian = (Mathf.PI*2f-deadZoneAngle)/(interval);
        setFlags(); */
    }

    /* public void changeYears(int start, int end)
    {
        startYear = start;
        endYear = end;
        interval = endYear - startYear;
        year_radian = (Mathf.PI*2-deadZoneAngle)/(interval);
    }*/

    // Update is called once per frame
    void Update()
    {

    }

    public void updateFlagPositions(float extraOffset)
    {
        var childs = flagParent.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            Transform child = flagParent.transform.GetChild(i);
            child.RotateAround(new Vector3(0, 0, 0), Vector3.up, extraOffset * 180 / Mathf.PI);
            var angleBetweenFlagAndOrigin = Vector3.Angle(originVector, child.transform.position);
            if (angleBetweenFlagAndOrigin <= (degrees(deadZoneOffset)))
            {
                //Debug.Log("Removing flag " + child.name);
                Destroy(child.gameObject);
            }

        }
    }

    float degrees(float radian)
    {
        return radian * 180 / Mathf.PI;
    }

    float radians(float degree)
    {
        return degree * Mathf.PI / 180;
    }

    public void enterNewFlags(int fromYear, int toYear, float radius, float height, int startYear)
    {
        bool previousYearRaised = true;
        int previousYear = fromYear;
        float extraOffset = 0;
        for (int i = 0; i < episodes.Length; i++)
        {
            var episode = episodes[i];
            Vector3 donutCenter = new Vector3(0, 0, 0);

            if (episode.yearFrom < fromYear || episode.yearFrom >= toYear){ continue; }

            if (episode.yearFrom == previousYear)
            {
                extraOffset += radiansPerYear / 3f;
            }
            else
            {
                extraOffset = 0;
            }

            int episode_year_place = episode.yearFrom - startYear;
            //Debug.Log("[" + fromYear + "," + toYear + "]" + " startYear " + startYear);
            //Debug.Log("episode_year_place for " + episode.yearFrom + ": " + episode_year_place);
            float radians = episode_year_place * radiansPerYear + deadZoneOffset + extraOffset;

            Vector3 pos = donutCenter + (new Vector3(radius * Mathf.Cos(-radians), height, radius * Mathf.Sin(-radians)));
            var lookPos = donutCenter + pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            pos.y = 1f;
            GameObject flag = Instantiate(flagPrefab, pos, rot, flagParent.transform); // make face center
            
            if (!previousYearRaised)
            {
                flag.transform.Find("Cylinder").transform.localScale += new Vector3(0, 200, 0);
                flag.transform.Find("Cylinder").transform.position += new Vector3(0, 200, 0);
                flag.transform.Find("Flag").transform.position += new Vector3(0, 400, 0);
                flag.transform.Find("BoxCollider").transform.position += new Vector3(0, 400, 0);
                previousYearRaised = true;
                //flag.transform.Find("Flag").transform.position += new Vector3(0,flag.transform.Find("Cylinder").transform.position.y+50, 0);
            }
            else
            {
                previousYearRaised = false;
            }
            flag.transform.localScale = new Vector3(0.002f, 0.0025f, 0.0025f);
            flag.transform.position += new Vector3(0, 0.65f, 0);
            setFlagText(flag, episode);

        }
    }

    private Vector3 originVector;
    private float deadZoneOffset;
    private Episode[] episodes;
    private float radiansPerYear;
    public void setFlags(float radius, float height, float deadZoneAngle, int startYear, int endYear)
    {
        int spanOfYears = endYear - startYear;
        radiansPerYear = (Mathf.PI * 2 - deadZoneAngle) / (spanOfYears);
        EpisodeList episodeList = JsonUtility.FromJson<EpisodeList>(jsonFile.text);
        episodes = episodeList.Episodes;
        Array.Sort(episodes, new EpisodeComparer());
        float extraOffset = 0f;
        int prviousYear = startYear;
        bool previousRaised = true;
        int noOfEpisodes = episodes.Length;

        originVector = new Vector3(radius * Mathf.Cos(1), height, radius * Mathf.Sin(0));
        deadZoneOffset = deadZoneAngle / 2;

        for (int i = 0; i < noOfEpisodes; i++)
        {
            var episode = episodes[i];
            Vector3 donutCenter = new Vector3(0, 0, 0);

            //float radians = i * Mathf.PI*2f / noOfEpisodes;

            int episode_year_place = episode.yearFrom - startYear; //[2,14] 5-2=3 means third number on donut
            if (episode.yearFrom < startYear || episode.yearFrom > endYear)
            {
                continue;
            }

            //To not place episodes in same year on top of each other
            if (episode.yearFrom == prviousYear)
            {
                extraOffset += radiansPerYear / 3f;
            }
            else
            {
                extraOffset = 0;
            }
            prviousYear = episode.yearFrom;

            float radians = episode_year_place * radiansPerYear + deadZoneOffset + extraOffset;
            //Debug.Log(episode_year_place);

            Vector3 pos = donutCenter + (new Vector3(radius * Mathf.Cos(-radians), height, radius * Mathf.Sin(-radians)));
            var lookPos = donutCenter + pos;
            lookPos.y = 0;
            var rot = Quaternion.LookRotation(lookPos);
            pos.y = 1f;
            GameObject flag = Instantiate(flagPrefab, pos, rot, flagParent.transform); // make face center
            // Debug.Log(flag.transform.position);
            flag.name = "Flag " + i;
            if (!previousRaised)
            {
                flag.transform.Find("Cylinder").transform.localScale += new Vector3(0, 200, 0);
                flag.transform.Find("Cylinder").transform.position += new Vector3(0, 200, 0);
                flag.transform.Find("Flag").transform.position += new Vector3(0, 400, 0);
                flag.transform.Find("BoxCollider").transform.position += new Vector3(0, 400, 0);
                previousRaised = true;
                //flag.transform.Find("Flag").transform.position += new Vector3(0,flag.transform.Find("Cylinder").transform.position.y+50, 0);
            }
            else
            {
                previousRaised = false;
            }
            flag.transform.localScale = new Vector3(0.002f, 0.0025f, 0.0025f);
            flag.transform.position += new Vector3(0, 0.65f, 0);
            setFlagText(flag, episode);
        }

    }
    void setFlagText(GameObject flag, Episode episode)
    {
        flag.transform.Find("Flag/Panel/Title").GetComponentInChildren<TMP_Text>().text = episode.title;
        flag.transform.Find("Flag/Panel/Series").GetComponentInChildren<TMP_Text>().text = episode.seriestitle;
        flag.transform.Find("Flag/Panel/Panel/Episode").GetComponentInChildren<TMP_Text>().text = "Episode " + episode.episodeNumber.ToString();
        flag.transform.Find("Flag/Panel/Panel/Season").GetComponentInChildren<TMP_Text>().text = "Season " + episode.seasonNumber.ToString();
        flag.transform.Find("Flag/Panel/Year").GetComponentInChildren<TMP_Text>().text = "Year " + episode.yearFrom.ToString();
        flag.name = "Flag " + episode.yearFrom + " " + episode.title;
    }
}
