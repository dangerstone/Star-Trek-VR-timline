using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript12 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        WWW w = new WWW("https://github.com/dangerstone/Star-Trek-VR-timline/blob/master/startrekepisodes.json");
        yield return w;
        if (w.error != null)
        {
            Debug.Log("Error .. " + w.error);
            // for example, often 'Error .. 404 Not Found'
        }
        else
        {
            EpisodeList episodeList = JsonUtility.FromJson<EpisodeList>(w.text);
            foreach (var episode in episodeList.Episodes)
            {
                //create flag and set properties from episode
                Debug.Log(episode.episodeNumber);
            }
        }

    }
}
