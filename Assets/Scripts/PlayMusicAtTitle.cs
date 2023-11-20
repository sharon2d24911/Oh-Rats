using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicAtTitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] tracks = { "TitleWop", "none", "none", "none" };
        float[] volumes = { 1,1,1,1 };
        float[] speeds = { 1,1,1,1 };
        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn(tracks, 1, volumes, speeds));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
