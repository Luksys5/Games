using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PCGuessBehaviour : MonoBehaviour
{
    public GameObject BeginGuessUI;
    public GameObject GuessGameUI;
    public GameObject BackToStart;
    public Text GuessInput;
    public Text GuessCountInput;
    public Text GuessOutput;
    public Text WinOutput;
    public Text ErrorOutput;

    private int min;
    private int max;

    private int guessCount;
    private int guess;

    private bool quarterLeftRight = false;
    private bool startedGuess = false;

    private const float LEFT_QUARTER = 0.25f;
    private const float RIGHT_QUARTER = 0.75f;

    private const int MIN_START = 0;
    private const int MAX_START = 1000;

    // Use this for initialization tarp 5 ir 10 taktika imt ketvirtadalius ir bandyt islost laiko. 
    // jei tarp 1 ir 5 tada tiesiog random ir jei 10 ar daugiau duoda ėjimu tada is dvieju dalint 
    void Start()
    {
        RectTransform rt = (RectTransform)BeginGuessUI.transform;
        BeginGuessUI.transform.position = new Vector3(Screen.width * 0.25f - (rt.rect.width), Screen.height * 0.5f, 0);
        rt = (RectTransform)GuessGameUI.transform;
        GuessGameUI.transform.position = new Vector3((Screen.width * 0.5f) + (rt.rect.width), Screen.height * 0.5f, 0);
        rt = (RectTransform)BackToStart.transform;
        BackToStart.transform.position = new Vector3((Screen.width * 0.9f), Screen.height * 0.1f, 0);
        GuessGameUI.SetActive(false);
        WinOutput.enabled = false;
    }
    
    private void Update()
    {
        if(startedGuess == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnHigher();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnLower();
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            OnExact();
        }
    }

    public void OnLower()
    {
        max = guess;
        GuessNumber();
    }

    public void OnHigher()
    {
        min = guess + 1;
        GuessNumber();
    }

    public void OnExact()
    {
        WinOutput.enabled = true;
        WinOutput.text = "PC Won too EASY!";
        GuessGameUI.SetActive(false);
        BeginGuessUI.SetActive(true);
        startedGuess = false;
    }

    public void OnBegin()
    {

        if (int.TryParse(GuessCountInput.GetComponent<Text>().text, out guessCount))
        {
            min = MIN_START;
            max = MAX_START;
            ErrorOutput.enabled = false;
            WinOutput.enabled = false;
            GuessGameUI.SetActive(true);
            BeginGuessUI.SetActive(false);
            startedGuess = true;
            GuessNumber();
        }
        else
        {
            ErrorOutput.enabled = true;
        }
    }

    private void GuessNumber()
    {
        if (startedGuess == false)
            return;

        if(guessCount > 10)
        {
            guess = (int)((max + 1) * 0.5f);
        }
        else if(guessCount > 2)
        {
            if(quarterLeftRight == false)
            {
                guess = min + (int)((max - min) * 0.25f);
            }
            else
            {
                guess = min + (int)((max - min) * RIGHT_QUARTER);
            }
            quarterLeftRight = !quarterLeftRight;
        }
        else if(guessCount > 0)
        {
            guess = Random.Range(min, max);
        }
        else
        {
            WinOutput.enabled = true;
            WinOutput.text = "You Won. Pick a higher guess count if you are not afraid!";
            startedGuess = false;
            BeginGuessUI.SetActive(true);
            GuessGameUI.SetActive(false);
            return;
        }
        guessCount--;
        GuessOutput.text = "PC Guess: " + guess.ToString() + ". Times to guess: " + guessCount.ToString();
    }

    public void BackToStartScene()
    {
        SceneManager.LoadScene(GlobalData.StartSceneBuildIndex);
    }
}
