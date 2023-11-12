using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadSfxFile : MonoBehaviour
{
    public Dictionary<string, float> sfxDictionary = new Dictionary<string, float>();

    // Start is called before the first frame update
    void Start()
    {
        // Define the path to text file
        string filePath = Path.Combine(Application.dataPath, "sfxVolume.txt");

        // Read all lines from the text file
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            // Split each line into key and value
            string[] parts = line.Split(',');
            string key = parts[0].Trim();
            float value = float.Parse(parts[1]);
            sfxDictionary[key] = value;
        }
    }

}
