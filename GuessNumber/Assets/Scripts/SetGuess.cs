using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{
    public class SetGuess : MonoBehaviour {

        public GameObject errorInfo;
        public Button setGuessBtn;
        public InputField input;
        public Text hostScoreInfo;
        public Text clientScoreInfo;

        private GameObject error;
        private Text errorInfoText;

        private int guess;

        private bool connectStatusChanged;

        private void Start()
        {
            connectStatusChanged = false;
            guess = 0;

            errorInfoText = errorInfo.GetComponent<Text>();
            error = errorInfo.transform.parent.gameObject;
            error.SetActive(false);
        }

        public void Reinitialize()
        {
            connectStatusChanged = false;
            guess = 0;
        }

        private void Update()
        {
            if (GuessVariables.readyToGuess == false && setGuessBtn.IsInteractable() == false)
                setGuessBtn.interactable = true;

            if (NetworkVariables.isHost == false)
                return;

            if (NetworkVariables.clientConnected == false)
            {
                errorInfoText.text = "Client has not connected yet";
                error.SetActive(true);
            }
            else if(connectStatusChanged == false)
            {
                connectStatusChanged = true;
                error.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                InputGuess();
            }
        }

        public void InputGuess()
        {
            if(NetworkVariables.clientConnected == false)
                return;

            int outGuess = 0;
            if (int.TryParse(input.text, out outGuess))
            {
                if(GuessVariables.MIN_GUESS <= outGuess && GuessVariables.MAX_GUESS >= outGuess)
                {
                    setGuessBtn.interactable = false;
                    guess = outGuess;

                    error.SetActive(false);
                    input.selectionColor = Color.blue;
                    NetworkVariables.ownerChanged = true;
                    GuessVariables.readyToGuess = true;
                }
                else
                {
                    errorInfoText.text = "Guess must be a value from " + GuessVariables.MIN_GUESS.ToString() + " to " + GuessVariables.MAX_GUESS.ToString();
                    error.SetActive(true);
                    input.selectionColor = Color.red;
                }

            }
            else
            {
                errorInfoText.text = "Guess must be a value from " + GuessVariables.MIN_GUESS.ToString() + " to " + GuessVariables.MAX_GUESS.ToString();
                error.SetActive(true);
                input.selectionColor = Color.red;
            }
        }


        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(guess);
                stream.SendNext(NetworkVariables.ownerChanged);

                GuessVariables.hostGuess = guess;
            }
            else
            {
                guess = (int)stream.ReceiveNext();
                bool ownerChanged = (bool)stream.ReceiveNext();

                GuessVariables.hostGuess = guess;
                if (guess != 0 && ownerChanged == true)
                {
                    GuessVariables.readyToGuess = true;
                }

                
            }

        }
    }

}