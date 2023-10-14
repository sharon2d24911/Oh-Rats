using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawning : MonoBehaviour
{
    private GameObject GH;
    private GameHandler GameHandler;

    // Start is called before the first frame update
    void Start()
    {
        GH = GameObject.Find("GameHandler");
        GameHandler = GH.GetComponent<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void selectLane()
    {
        GameObject grid = GameObject.Find("Grid"); ;
        GridCreate gridScript = grid.GetComponent<GridCreate>();
        List<Vector3>  gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script
        int randInd = Random.Range(0, (gridPositions.Count - 1));
        Vector3 selectedPos = new Vector3(GameHandler.EnemyStartXPosition, gridPositions[randInd].y, 0);
        
    }

    void Spawn(GameObject enemy, Vector3 position)
    {
        Instantiate(enemy, enemy.transform.position, enemy.transform.rotation);
    }
}
