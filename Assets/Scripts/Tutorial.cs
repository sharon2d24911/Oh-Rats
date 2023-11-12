using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    //public GameObject prop;
    public Button mixButton;
    public GameObject[] popUps;
    public GameObject[] toHide;
    public GameObject gameHandler;
    private Canvas TutorialCanvas;
    private Canvas TransitionCanvas;
    Camera mainCamera;
    private int popUpIndex;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        TutorialCanvas = GameObject.Find("Tutorial Canvas").GetComponent<Canvas>();
        TutorialCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        TutorialCanvas.worldCamera = mainCamera;
        TransitionCanvas = GameObject.Find("Transition Canvas").GetComponent<Canvas>();
        
        foreach (GameObject tutorial in popUps)
        {
            tutorial.SetActive(false);
        }
        StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeIn("BassyMain", 2, 1)); // Fade in music
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
        } else if (popUpIndex == 1)
        {
            // If user picks up object and drags it into bowl
            if (gameHandler.GetComponent<DragCombination>().combining.Count > 0)
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 2)
        {
            gameHandler.GetComponent<DragCombination>().allIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
            // If user puts one of each item in bowl
            if (gameHandler.GetComponent<DragCombination>().CheckMinimum())
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
            
        }
        else if (popUpIndex == 3)
        {
            // If user hits mix button, move to next step
            mixButton.onClick.AddListener(() => {
                toHide[3].SetActive(false);
                popUpIndex = 4;
            });

        }
        else if (popUpIndex == 4)
        {
            // If user drags unit to a correct position on the board
            // CURRENTLY WILL ALLOW PLACEMENT ON ANY GRID SPACE. FIX
            //gameHandler.GetComponent<DragCombination>().tutorialMode = true;
            // ContainsKey(gameHandler.GetComponent<DragCombination>().topLeft)
            if (gameHandler.GetComponent<DragCombination>().filledPositions.Count > 0)
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        }
        else if (popUpIndex == 5)
        {
            // Discuss the tracker, press space to continue
            if (Input.GetKeyDown(KeyCode.Space))
            {
                toHide[popUpIndex].SetActive(false);
                popUpIndex++;
            }
        } else if (popUpIndex == 6)
        {
            // Props can be clicked on??? does this need to be a step at all if story optional? if not then just say bye, press space to continue
            if (Input.GetKeyDown(KeyCode.Space))
                TransitionCanvas.GetComponent<Menu>().FadeToScene("Game");
                StartCoroutine(GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().FadeOut("BassyMain", 2, 0)); // Fade out music
        }
    }
}
