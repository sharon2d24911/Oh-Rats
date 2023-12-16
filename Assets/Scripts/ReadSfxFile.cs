using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadSfxFile : MonoBehaviour
{
    public TextAsset sfxFile;
    public Dictionary<string, float[]> sfxDictionary = new Dictionary<string, float[]>();

    // Start is called before the first frame update
    void Start()
    {
        // Define the path to text file
        //string filePath = Path.Combine(Application.dataPath, "sfxVolume.txt");


        // Read all lines from the text file
        string[] lines = sfxFile.ToString().Split('\n'); ;

        for (int i = 1; i < lines.Length; i++)
        {
            // Split each line into key and value
            string[] parts = lines[i].Split(',');
            string sfxName = parts[0].Trim();
            float volume = float.Parse(parts[1]);
            float pan = float.Parse(parts[2]);
            float[] array = { volume, pan }; // Store volume and pan into an array
            sfxDictionary[sfxName] = array;
        }
    }

}
