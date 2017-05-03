using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GuessNumber;

public class PlayerGuessBehaviour : MonoBehaviour {

    public GameObject BackToStart;
    public Text errorText;
    public Text inputText;

    private float width = 200;
    private float height = 50;

    private int min;
    private int max;

    private int guessCount;
    private int guess;
    private int lastGuess;
    private int answer;

    private bool answered = false;

	// Use this for initialization
	void Start () {
        if(errorText == null)
        {
            Debug.Log("Input error text gameobject");
            Destroy(this);
            return;
        }
        Reinitialize();

        RectTransform rt = (RectTransform)BackToStart.transform;
        BackToStart.transform.position = new Vector3((Screen.width * 0.9f), Screen.height * 0.1f, 0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(guessCount == 0)
            {
                Reinitialize();
            }
            else
            {
                OnGuess();
            }
        }
    }

    private void Reinitialize()
    {
        min = 0;
        max = 1000;
        answer = Random.Range(min, max);
        guessCount = 10;
        lastGuess = guess;
        guess = -1;
    }
    

    // Update is called once per frame
    void OnGUI () {
        if (guessCount == 0)
        {
            GUI.TextField(
                new Rect(Screen.width * 0.5f - width, Screen.height * 0.5f - height, width, height),
                "You lost. The answer was : " + answer.ToString() + ". \nPress Enter to restart."
            );
            if(GUI.Button(new Rect(Screen.width * 0.5f - width, Screen.height * 0.5f - (height * 2), width, height), "Restart"))
            {
                Reinitialize();
            }
        }
        else
        {
            if (answered)
            {
                GUI.TextField(
                    new Rect(Screen.width * 0.5f - width, Screen.height * 0.5f - height, width, height),
                    "Congratz! Your answer is correct!"
                );
            }
            else
            {
                GUI.TextField(
                    new Rect(Screen.width * 0.5f - width, Screen.height * 0.5f - height, width, height),
                    "Guess a number between \nmin: " + min.ToString() + " and max: " + max.ToString() + "\n" + "You can guess " + guessCount + " times"
                );
            }
        }
        
	}

    public void OnGuess()
    {
        if (guessCount == 0)
            return;

        if (int.TryParse(inputText.text, out guess))
        {
            if (guess == lastGuess)
                return;

            if (guess < min || guess > max)
                return;

            lastGuess = guess;
            guessCount--;
            if (guess == answer)
            {
                answered = true;
            }
            else
            {
                if(answer < guess)
                {
                    max = guess;
                }
                else
                {
                    min = guess;
                }
                answer = Random.Range(min, max);
            }  
            errorText.enabled = false;
        }
        else
        {
            errorText.enabled = true;
        }
    }

    public void BackToStartScene()
    {
        SceneManager.LoadScene(BuildIndex.MainMenu);
    }
}
