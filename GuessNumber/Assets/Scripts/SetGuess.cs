using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{
    public class SetGuess : MonoBehaviour {

        public GameObject errorInfo;
        public InputField input;
        public Text hostScoreInfo;

        private GameController gc;
        private GameObject error;
        private Text errorInfoText;

        private string hostName;

        private int guess;

        private bool connectStatusChanged;

        private void Start()
        {
            hostName = NetworkVariables.hostName;
            hostScoreInfo.text = NetworkVariables.hostName + " Score";

            connectStatusChanged = false;
            gc = GameObject.FindGameObjectWithTag(Tags.Loader).GetComponent<GameController>();
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
                    guess = outGuess;
                    NetworkVariables.ownerChanged = true;
                    error.SetActive(false);
                }
                else
                {
                    errorInfoText.text = "Guess must be a value from " + GuessVariables.MIN_GUESS.ToString() + " to " + GuessVariables.MAX_GUESS.ToString();
                    error.SetActive(true);
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
                stream.SendNext(hostName);
                GuessVariables.hostGuess = guess;
            }
            else
            {
                guess = (int)stream.ReceiveNext();
                bool ownerChanged = (bool)stream.ReceiveNext();
                hostName = (string)stream.ReceiveNext();
                hostScoreInfo.text = hostName + " Score";

                GuessVariables.hostGuess = guess;
                if (guess != 0 && ownerChanged == true)
                {
                    GuessVariables.readyToGuess = true;
                    GuessVariables.GuessIsSet = true;
                }
            }

        }
    }

}