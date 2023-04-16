using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EpisodeList
{
    public Episode[] Episodes;
}

[System.Serializable]
public class Episode
{
    public string uid;
    public string title;
    public string titleGerman;
    public string seriesuid;
    public string seriestitle;
    public string seasonuid;
    public string seasontitle;
    public int seasonNumber;
    public int episodeNumber;
    public string productionSerialNumber;
    public bool featureLength;
    public string stardateFrom;
    public string stardateTo;
    public string yearFrom;
    public string yearTo;
    public string usAirDate;
    public string finalScriptDate;
}


