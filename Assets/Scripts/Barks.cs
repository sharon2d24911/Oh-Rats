using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class Barks : MonoBehaviour
{

    public TextAsset barksFile;
    public GameObject text;
    public GameObject bubble;
    private string[] barks;
    public float minTime = 0f, maxTime = 10f;
    private int numBarks;
    private float barkTimer = 0f;
    private float currentBarkTimeMax;
    private bool hasBarked = false;
    private string barkSound;

    // Start is called before the first frame update
    void Start()
    {
        barks = barksFile.ToString().Split('\n');
        for (int i = 0; i < barks.Length; i++)
        {
            barks[i] = barks[i] + '\n';
        }
        numBarks = barks.Length;
        currentBarkTimeMax = Random.Range(minTime, maxTime);
        bubble.GetComponent<SpriteRenderer>().sortingOrder = 22;
        text.GetComponent<SortingGroup>().sortingOrder = 23;
    }

    // Update is called once per frame
    void Update()
    {
        barkTimer += Time.deltaTime;

        if (barkTimer > currentBarkTimeMax)
        {
            if (!hasBarked)
            {
                string bark = barks[(int)Random.Range(0, numBarks)];
                text.GetComponent<TextMeshPro>().text = bark;
                bubble.GetComponent<SpriteRenderer>().color = bubble.GetComponent<SpriteRenderer>().color + new Color(0, 0, 0, 1);
                text.GetComponent<TextMeshPro>().color = text.GetComponent<TextMeshPro>().color + new Color(0, 0, 0, 1);
                hasBarked = true;

                string[] barkSound = { "KingBark1", "KingBark2", "KingBark3", "KingBark4", "KingBark5", "KingBark6", "KingBark7", "KingBark8", "KingBark9", "KingBark10", "KingBark11", "KingBark12", "KingBark13" };
                this.barkSound = barkSound[Mathf.FloorToInt(Random.Range(0, 13))];
                AudioManager.Instance.PlaySFX(this.barkSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.barkSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.barkSound][1]);
            }
           
            if (barkTimer > currentBarkTimeMax + 2f)
            {
                currentBarkTimeMax = Random.Range(minTime, maxTime);
                barkTimer = 0;
                text.GetComponent<TextMeshPro>().color = text.GetComponent<TextMeshPro>().color - new Color(0, 0, 0, 1);
                bubble.GetComponent<SpriteRenderer>().color = bubble.GetComponent<SpriteRenderer>().color - new Color(0, 0, 0, 1);
                hasBarked = false;
            }
        }
    }
}
