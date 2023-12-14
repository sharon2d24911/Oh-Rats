using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Button mixButton;
    public Button deliveryButton;
    public Button garbageButton;
    public GameObject enemyPrefab;
    public GameObject[] popUps;
    public GameObject[] toHide;
    public GameObject gameHandler;
    public Texture2D defaultCursor;
    private Canvas TutorialCanvas;
    private Canvas TransitionCanvas;
    Camera mainCamera;
    private int popUpIndex;
    private bool rat;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        TutorialCanvas = GameObject.Find("Tutorial Canvas").GetComponent<Canvas>();
        TutorialCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        TutorialCanvas.worldCamera = mainCamera;
        TransitionCanvas = GameObject.Find("Transition Canvas").GetComponent<Canvas>();
        rat = false;

        foreach (GameObject tutorial in popUps)
        {
            tutorial.SetActive(false);
        }
        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("TitleWop", "none", "none", "none", 1, 0));
        string[] tracks = { "ChewTorial", "none", "none", "none" };
        float[] volumes = { 1, 1, 1, 1 };
        float[] speeds = { 1, 1, 1, 1 };
        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn(tracks, 3, volumes, speeds));
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        // Loop through all the instructions and only show one at a time
        popUps[popUpIndex].SetActive(true);

        // Start of tutorial
        if (popUpIndex == 0)
        {
            // Welcoming to game, mini description, press space to continue
            if (Input.GetKeyDown(KeyCode.Space))
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 1) // If user picks up object and drags it into bowl
        {
            if (gameHandler.GetComponent<DragCombination>().combining.Count > 0)
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 2) // If user puts one of each item in bowl
        {
            gameHandler.GetComponent<DragCombination>().allIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
            if (gameHandler.GetComponent<DragCombination>().CheckMinimum())
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }

        }
        else if (popUpIndex == 3) // If user hits mix button, move to next step
        {
            mixButton.onClick.AddListener(() =>
            {
                toHide[3].SetActive(false);
                popUpIndex = 4;
                mixButton.onClick.RemoveAllListeners();
            });
        }
        else if (popUpIndex == 4) // If user drags unit to the correct position on the board
        {
            gameHandler.GetComponent<DragCombination>().tutorialMode = true;
            if (gameHandler.GetComponent<DragCombination>().filledPositions.ContainsKey(gameHandler.GetComponent<DragCombination>().topLeft))
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 5) // If user removes the placed unit
        {
            if (!gameHandler.GetComponent<DragCombination>().filledPositions.ContainsKey(gameHandler.GetComponent<DragCombination>().topLeft))
            {
                garbageButton.interactable = false;
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 6) // If user presses tracker to allow delivery
        {
            GameObject.Find("Shipment Truck").GetComponent<Shipment>().tutorial = true;
            deliveryButton.onClick.AddListener(() =>
            {
                toHide[6].SetActive(false);
                popUpIndex = 7;
                deliveryButton.onClick.RemoveAllListeners();
            });
        }
        else if (popUpIndex == 7) // User should put 3 of each ingredient in the bowl, hit mix
        {
            if (gameHandler.GetComponent<DragCombination>().combining.Count == 9)
                mixButton.interactable = true;
            else
                mixButton.interactable = false;

            mixButton.onClick.AddListener(() =>
            {
                toHide[7].SetActive(false);
                popUpIndex = 8;
                mixButton.onClick.RemoveAllListeners();
            });
        }
        else if (popUpIndex == 8) // User should put 3:3:3 on the board
        {
            if (gameHandler.GetComponent<DragCombination>().filledPositions.ContainsKey(gameHandler.GetComponent<DragCombination>().topLeft))
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 9) // Spawn an enemy, let it die
        {
            GameObject spawnedEnemy = null;
            GameObject unit = gameHandler.GetComponent<DragCombination>().filledPositions[gameHandler.GetComponent<DragCombination>().topLeft];
            UnitBehaviour unitScript = unit.GetComponent<UnitBehaviour>();
            if (GameObject.FindGameObjectWithTag("Enemy") == null && !rat)
            {
                rat = true;
                Vector3 position = new Vector3(17.5f, .5f, 1.5f);
                spawnedEnemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
                spawnedEnemy.GetComponent<SpriteRenderer>().sortingOrder = ((int)Mathf.Floor(position.z) * 5 + 2);
                spawnedEnemy.GetComponent<EnemyBehaviour>().health = 1000;
                spawnedEnemy.GetComponent<EnemyBehaviour>().lane = (int)(position.z - 1.5f);
                spawnedEnemy.GetComponent<EnemyBehaviour>().tutorial = true; 
                unitScript.defending = true;
            }
            if (GameObject.FindGameObjectWithTag("Enemy") == null && rat)
            {
                unitScript.defending = false;
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }

        }
        else if (popUpIndex == 10) // Say bye, press space to start game
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("ChewTorial", "none", "none", "none", 2, 0)); // Fade out music
                TransitionCanvas.GetComponent<Menu>().FadeToScene("Game");
            }
        }
    }
}
