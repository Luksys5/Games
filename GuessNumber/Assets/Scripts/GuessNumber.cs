using Photon;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GuessNumber
{

    public class GuessNumber : PunBehaviour
    {
        public GameObject errorInfo;
        public Button GuessBtn;
        public InputField input;
        public Text hostScore;
        public Text hostScoreInfo;
        public Text clientScore;
        public Text clientScoreInfo;
        
        private GameObject error;
        private GameController gc;
        private Text errorInfoText;

        private int guess;
        private int score;

        private bool changeOwner;
        private bool clientConnect = false;
        private bool notConnectedSet = false;
        private bool showClientGuess = true;

        private void Start()
        {
            if (PhotonNetwork.isMasterClient)
                hostScoreInfo.text = PhotonNetwork.playerName + " Score";
            else
            {
                clientScoreInfo.text = PhotonNetwork.playerName + " Score";
                showClientGuess = false;
            }
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

        private void Update()
        {
            if(GuessVariables.readyToGuess != GuessBtn.IsInteractable())
                GuessBtn.interactable = !GuessBtn.interactable;

            if (NetworkVariables.isHost)
                return;

            if (GuessVariables.readyToGuess == false && notConnectedSet == false)
            {
                notConnectedSet = true;
                errorInfoText.text = "Oponnent is setting number";
                error.SetActive(true);
            }
            else if(GuessVariables.readyToGuess == true && notConnectedSet == true)
            {
                notConnectedSet = false;
                error.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnGuess();
            }
        }

        public void OnGuess()
        {
            if (GuessVariables.readyToGuess == false )
                return;


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
                GuessVariables.clientGuess = guess;
                showClientGuess = true;
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
            if (GuessVariables.haveWon)
                return;

            float quarterWidth = Screen.width * 0.25f;
            if (showClientGuess)
                GUI.TextArea(new Rect(quarterWidth - 60, Screen.height * 0.5f - 20, 120, 40), "Last Set Guess: " + GuessVariables.hostGuess.ToString());
            GUI.TextArea(new Rect(Screen.width - quarterWidth - 60, Screen.height * 0.5f - 20, 120, 40), "Last Guess: " + GuessVariables.clientGuess.ToString());
        }

        void resetAndSwap()
        {
            gc.swapOwners();
            guess = 0;
            score = 0;
            changeOwner = false;
        }


        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(guess);
                stream.SendNext(score);
                stream.SendNext(changeOwner);
                stream.SendNext(clientConnect);
                stream.SendNext(PhotonNetwork.playerName);

                SetScore(score, stream.isWriting);
                if (changeOwner == true)
                    resetAndSwap();
            }
            else
            {
                guess                               = (int)stream.ReceiveNext();
                score                               = (int)stream.ReceiveNext();
                changeOwner                         = (bool)stream.ReceiveNext();
                NetworkVariables.clientConnected    = (bool)stream.ReceiveNext();
                string opponentName                 = (string)stream.ReceiveNext();

                if (PhotonNetwork.isMasterClient)
                    clientScoreInfo.text = opponentName + " Score";
                else
                    hostScoreInfo.text = opponentName + " Score";

                if (guess != 0 && GuessVariables.readyToGuess == true) {
                    GuessVariables.clientGuess = guess;
                    showClientGuess = false;
                }

                SetScore(score, stream.isWriting);
                if (changeOwner == true)
                    resetAndSwap();
            }

        }

        public void OnBackPressed()
        {
            SceneManager.LoadScene(BuildIndex.GuessMultiplayerMenu);
        }

        public void OnQuitPressed()
        {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }

    }

}