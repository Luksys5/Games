using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{
    public class SetGuess : MonoBehaviour {

        public GameObject error;
        public InputField input;

        private GameController gc;
        private Text errInfo;
        private int guess;

        private void Start()
        { 
            gc = GameObject.FindGameObjectWithTag(Tags.Loader).GetComponent<GameController>();
            guess = 0;

            errInfo = GameObject.FindGameObjectWithTag(Tags.ErrorInfo).GetComponent<Text>();
            error.SetActive(false);
        }

        public void InputGuess()
        {
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
                    errInfo.text = "Guess must be a value from " + GuessVariables.MIN_GUESS.ToString() + " to " + GuessVariables.MAX_GUESS.ToString();
                    error.SetActive(true);
                }
            }
            else
            {
                errInfo.text = "Guess must be a value from " + GuessVariables.MIN_GUESS.ToString() + " to " + GuessVariables.MAX_GUESS.ToString();
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
                    GuessVariables.GuessIsSet = true;
                }
            }

        }
    }

}