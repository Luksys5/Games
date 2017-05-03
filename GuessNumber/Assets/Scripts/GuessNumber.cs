using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{

    public class GuessNumber : PunBehaviour
    {
        public GameObject error;
        public InputField input;
        public Text hostScore;
        public Text clientScore;

        private GameController gc;
        private Text errInfo;
        private int guess;
        private int score;
        private bool changeOwner;


        private void Start()
        {
            gc = GameObject.FindGameObjectWithTag(Tags.Loader).GetComponent<GameController>();
            errInfo = GameObject.FindGameObjectWithTag(Tags.ErrorInfo).GetComponent<Text>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            guess = 0;
            score = 0;
            error.SetActive(false);
            hostScore.text = "0";
            clientScore.text = "0";
        }

        public void OnGuess()
        {
            if (GuessVariables.readyToGuess == false)
            {
                errInfo.text = "Oponnent didn't set number yet!";
                error.SetActive(true);
                return;
            }


            if(int.TryParse(input.text, out guess))
            {
                if(GuessVariables.MIN_GUESS > guess || GuessVariables.MAX_GUESS < guess)
                {
                    errInfo.text = "Number didin't exist in given range";
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
                errInfo.text = "Input must contain a number";
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
                    hostScore.text = (outScore + currentScore).ToString();
                }
            }
            else
            {
                if (int.TryParse(clientScore.text, out outScore))
                {
                    clientScore.text = (outScore + currentScore).ToString();
                }
            }
        }

        private void OnGUI()
        {
            if(GuessVariables.GuessIsSet == false)
                GUI.TextArea(new Rect(0, 300, 100, 100), "Last Set Guess: " + GuessVariables.hostGuess.ToString());
            GUI.TextArea(new Rect(300, 300, 100, 100), "Last Guess: " + GuessVariables.clientGuess.ToString());
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(guess);
                stream.SendNext(changeOwner);
                stream.SendNext(score);

                GuessVariables.clientGuess = guess;
                SetScore(score, stream.isWriting);
                if (changeOwner == true)
                {
                    gc.swapOwners();
                    guess = 0;
                    score = 0;
                    changeOwner = false;
                }
            }
            else
            {
                guess = (int)stream.ReceiveNext();
                changeOwner = (bool)stream.ReceiveNext();
                score = (int)stream.ReceiveNext();

                GuessVariables.clientGuess = guess;
                SetScore(score, stream.isWriting);
                if(changeOwner == true)
                {
                    gc.swapOwners();

                    changeOwner = false;
                    score = 0;
                }
            }
        }

    }

}