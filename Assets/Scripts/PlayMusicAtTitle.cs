using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicAtTitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn("TitleWop", "none", "none", "none", 1, 1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
