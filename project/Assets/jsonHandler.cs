using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class jsonHandler: MonoBehaviour
{
    [SerializeField]
    GameObject flagprefab;
    public TextAsset jsonFile;
    // Start is called before the first frame update
    void Start()
    {
        OpenFromFileButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFromFileButton()
    {
        StartCoroutine(OpenFromFile());
    }

    public IEnumerator OpenFromFile()
    {
        WWW w = new("https://github.com/dangerstone/Star-Trek-VR-timline/blob/master/startrekepisodes.json");
        yield return w;
        if (w.error != null)
        {
            Debug.Log("Error .. " + w.error);
            // for example, often 'Error .. 404 Not Found'
        }
        else
        {
            float counter = 0.2f;
            EpisodeList episodeList = JsonUtility.FromJson<EpisodeList>(jsonFile.text);
            foreach (var episode in episodeList.Episodes)
            {
                //create flag and set properties from episode
                Debug.Log(episode.episodeNumber);
                GameObject newFlag = Instantiate(flagprefab, new Vector3(0, 0f, 0), Quaternion.identity);
                newFlag.transform.position = new Vector3(counter, 0f, 0f);
                counter += counter;
                //Debug.Log("Search");
                //Debug.Log(newFlag.transform.Find("Flag/Panel/Title"));
                //Debug.Log(newFlag.transform.Find("Flag/Panel/Title").GetComponentInChildren<TMP_Text>());
                newFlag.transform.Find("Flag/Panel/Title").GetComponentInChildren<TMP_Text>().text = episode.title;
                newFlag.transform.Find("Flag/Panel/Series").GetComponentInChildren<TMP_Text>().text = episode.seriestitle;
                newFlag.transform.Find("Flag/Panel/Panel/Episode").GetComponentInChildren<TMP_Text>().text += " " + episode.episodeNumber;
                newFlag.transform.Find("Flag/Panel/Panel/Season").GetComponentInChildren<TMP_Text>().text += " " + episode.seasonNumber;
            }
        }

    }
}
