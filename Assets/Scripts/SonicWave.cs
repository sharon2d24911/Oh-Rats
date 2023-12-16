using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicWave : MonoBehaviour
{

    public GameObject sonicWave;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator startWaves(int numberOfWaves, float timebeforeWaves)
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            yield return new WaitForSeconds(timebeforeWaves);
            StartCoroutine(createWaves(0.01f));
        }
    }

    public IEnumerator createWaves(float wavesInterval)
    {

        GameObject newWave = Instantiate(sonicWave, gameObject.transform.position, gameObject.transform.rotation);
            while(newWave.transform.localScale.x < 12)
            {
            //Debug.Log("newWave.transform.localScale.x " + newWave.transform.localScale.x);
                newWave.transform.localScale += new Vector3 (0.1f, 0.1f, 0.1f);
                yield return new WaitForSeconds(wavesInterval);
            }

            Destroy(newWave);
        }

}
