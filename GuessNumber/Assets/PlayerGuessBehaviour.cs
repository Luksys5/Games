using UnityEngine;
using UnityEngine.UI;

public class PlayerGuessBehaviour : MonoBehaviour {

    public Text errorText;
    public Text inputText;

    private float width = 200;
    private float height = 50;

    private int min = 0;
    private int max = 1000;

    private int guess = -1;
    private int answer = 0;

    private bool answered = false;

	// Use this for initialization
	void Start () {
        if(errorText == null)
        {
            Debug.Log("Input error text gameobject");
            Destroy(this);
            return;
        }
        answer = Random.Range(min, max);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnGuess();
        }
    }

    

    // Update is called once per frame
    void OnGUI () {
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
                "Guess a number between \nmin: " + min.ToString() + " and max: " + max.ToString()
            );
        }
        
	}

    public void OnGuess()
    {
        
        if (int.TryParse(inputText.text, out guess))
        {
            
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
}
