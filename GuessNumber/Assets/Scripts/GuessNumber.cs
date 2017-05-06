using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{

    public class GuessNumber : PunBehaviour
    {
        public GameObject errorInfo;
        public InputField input;
        public Text hostScore;
        public Text clientScore;
        public Text clientScoreInfo;

        private GameObject error;
        private GameController gc;
        private Text errorInfoText;

        private string clientName;

        private int guess;
        private int score;

        private bool changeOwner;
        private bool clientConnect = false;


        private void Start()
        {
            clientName = NetworkVariables.clientName;
            if(string.IsNullOrEmpty(clientName) == false)
                clientScoreInfo.text = clientName + " Score";

            error = errorInfo.transform.parent.gameObject;
            errorInfoText = errorInfo.GetComponent<Text>();
            gc = GameObject.FindGameObjectWithTag(Tags.Loader).GetComponent<GameController>();

            Reinitialize();
        }

        public void Reinitialize()
        {
            error.SetActive(false);
            hostScore.text = "0";
            clientScore.text = "0";
            guess = 0;
            score = 0;
            clientConnect = true;
        }

        public void OnGuess()
        {
            if (GuessVariables.readyToGuess == false)
            {
                errorInfoText.text = "Oponnent didn't set number yet!";
                error.SetActive(true);
                return;
            }


            if(int.TryParse(input.text, out guess))
            {
                if(GuessVariables.MIN_GUESS > guess || GuessVariables.MAX_GUESS < guess)
                {
                    errorInfoText.text = "Enter value from " + GuessVariables.MIN_GUESS.ToString() + " to" + GuessVariables.MAX_GUESS.ToString();
                    error.SetActive(true);
                    return;
                }
                error.SetActive(false);
                score = GuessVariables.calculateScore(guess);
                changeOwner = true;
                GuessVariables.GuessIsSet = false;
            }
            else
            {
                errorInfoText.text = "Input must contain a number";
                error.SetActive(true);
            }
        }

        private void SetScore(int currentScore , bool isWriting)
        {
            if (currentScore == 0)
                return;

            int outScore;
            if (PhotonNetwork.isMasterClient == isWriting)
            {
                if (int.TryParse(hostScore.text, out outScore))
                {
                    GuessVariables.hostScore = outScore + currentScore;
                    hostScore.text = GuessVariables.hostScore.ToString();
                }
            }
            else
            {
                if (int.TryParse(clientScore.text, out outScore))
                {
                    GuessVariables.clientScore = outScore + currentScore;
                    clientScore.text = GuessVariables.clientScore.ToString();
                }
            }
        }

        private void OnGUI()
        {
            float quarterWidth = Screen.width * 0.25f;
            float quarterHeight = Screen.height * 0.25f;
            if (GuessVariables.GuessIsSet == false)
                GUI.TextArea(new Rect(quarterWidth - 60, Screen.height - quarterHeight, 120, 40), "Last Set Guess: " + GuessVariables.hostGuess.ToString());
            GUI.TextArea(new Rect(Screen.width - quarterWidth - 60, Screen.height - quarterHeight, 120, 40), "Last Guess: " + GuessVariables.clientGuess.ToString());
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(guess);
                stream.SendNext(score);
                stream.SendNext(changeOwner);
                stream.SendNext(clientConnect);
                stream.SendNext(clientName);

                GuessVariables.clientGuess = guess;
                SetScore(score, stream.isWriting);
                if (changeOwner == true)
                {
                    gc.swapOwners();
                    guess = 0;
                    score = 0;
                    changeOwner = false;
                }
                clientScoreInfo.text = NetworkVariables.clientName + " Score";
            }
            else
            {
                guess                               = (int)stream.ReceiveNext();
                score                               = (int)stream.ReceiveNext();
                changeOwner                         = (bool)stream.ReceiveNext();
                NetworkVariables.clientConnected    = (bool)stream.ReceiveNext();
                clientName                          = (string)stream.ReceiveNext();

                GuessVariables.clientGuess = guess;
                SetScore(score, stream.isWriting);
                if(changeOwner == true)
                {
                    gc.swapOwners();

                    changeOwner = false;
                    score = 0;
                }
                if (string.IsNullOrEmpty(clientName))
                    clientName = NetworkVariables.clientName;
                clientScoreInfo.text = clientName + " Score";
            }
        }
    }

}