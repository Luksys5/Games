using Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GuessNumber
{
    public class GameController : PunBehaviour
    {
        public GameObject HostUI;
        public GameObject ClientUI;
        public GameObject StatusUIs;
        public Text GuessLeft;

        private GameObject restartBtn;
        private InputField hostInputField;
        private InputField clientInputField;

        private Dictionary<string, GameObject> NameToWinUI;

        private int guessesLeft;

        public void Start()
        {
            NameToWinUI = new Dictionary<string, GameObject>();
            for(int i = 0; i < StatusUIs.transform.childCount; i++)
            {
                GameObject child = StatusUIs.transform.GetChild(i).gameObject;
                if (child.name == WinUINames.HostWinUIName)
                    NameToWinUI.Add(WinUINames.HostWinUIName, child.gameObject);
                else if (child.name == WinUINames.clientWinUIName)
                    NameToWinUI.Add(WinUINames.clientWinUIName, child.gameObject);
                else if (child.name == WinUINames.stalemateUIName)
                    NameToWinUI.Add(WinUINames.stalemateUIName, child.gameObject);
                else
                    NameToWinUI.Add(WinUINames.wonInfo, child.gameObject);

                child.SetActive(false);
            }
            StatusUIs.SetActive(false);

            hostInputField = HostUI.GetComponentInChildren<InputField>();
            clientInputField = ClientUI.GetComponentInChildren<InputField>();

            restartBtn = GameObject.FindGameObjectWithTag(Tags.RestartBtn);
            restartBtn.SetActive(false);
            NetworkVariables.isHost = PhotonNetwork.isMasterClient;
            GuessVariables.hostGuess = 0;
            GuessVariables.clientGuess = 0;
            GuessVariables.haveWon = false;
            ReInitialize();
            PhotonObjectOwnerRequest(true);
            guessesLeft = GuessVariables.GUESS_LEFT_DEFAULT;
        }

        public void ReInitialize()
        {
            GuessVariables.readyToGuess = false;
            NetworkVariables.ownerChanged = false;
        }

        public void PhotonObjectOwnerRequest(bool reinitializeID)
        {
            if (NetworkVariables.isHost)
            {
                HostUI.SetActive(true);
                HostUI.GetComponentInParent<PhotonView>().TransferOwnership(PhotonNetwork.player.ID);
                ClientUI.SetActive(false);

            }
            else
            {
                ClientUI.SetActive(true);
                ClientUI.GetComponentInParent<PhotonView>().TransferOwnership(PhotonNetwork.player.ID);
                HostUI.SetActive(false);
            }
        }

        public void swapOwners()
        {
            guessesLeft--;
            hostInputField.text = "";
            clientInputField.text = "";
            if (guessesLeft == 0)
            {
                GuessLeft.text = guessesLeft.ToString();
                SetWinnerHideUI(false);
                return;
            }
            else if (guessesLeft < 0)
                return;
            else if(GuessVariables.clientGuess == GuessVariables.hostGuess)
            {
                guessesLeft = -1;
                GuessLeft.text = "0";
                SetWinnerHideUI(true);
                return;
            }

            GuessLeft.text = guessesLeft.ToString();
            ReInitialize();
            NetworkVariables.isHost = !NetworkVariables.isHost;
            PhotonObjectOwnerRequest(false);
        }

        public void OnRestart()
        {
            guessesLeft = GuessVariables.GUESS_LEFT_DEFAULT;
            GuessLeft.text = guessesLeft.ToString();
            ReInitialize();
            NetworkVariables.isHost = PhotonNetwork.isMasterClient;
            GuessVariables.clientGuess = 0;
            GuessVariables.hostGuess = 0;
            GuessVariables.haveWon = false;
            PhotonObjectOwnerRequest(true);

            restartBtn.SetActive(false);
            foreach(string key in NameToWinUI.Keys)
            {
                NameToWinUI[key].SetActive(false);
            }
            StatusUIs.SetActive(false);
        }

        public void SetDisconnectedPlayerLost()
        {
            GuessVariables.haveWon = true;

            // hide gameobjects
            HostUI.SetActive(false);
            ClientUI.SetActive(false);

            // unhide restart button 
            restartBtn.SetActive(true);

            GuessVariables.clientGuess = 0;
            GuessVariables.hostGuess = 0;

            StatusUIs.SetActive(true);

            NameToWinUI[WinUINames.HostWinUIName].GetComponent<Text>().text = "You have Won!";
            NameToWinUI[WinUINames.HostWinUIName].SetActive(true);

            NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Because opponent has left the game";
            NameToWinUI[WinUINames.wonInfo].SetActive(true);
        }
    
        private void SetWinnerHideUI(bool guessedRightNumber)
        {
            GuessVariables.haveWon = true;
            // hide gameobjects
            HostUI.SetActive(false);
            ClientUI.SetActive(false);

            // unhide restart button 
            restartBtn.SetActive(true);

            GuessVariables.clientGuess = 0;
            GuessVariables.hostGuess = 0;

            StatusUIs.SetActive(true);

            // unhide win texts
            if (guessedRightNumber)
            {
                if (NetworkVariables.isHost)
                {
                    NameToWinUI[WinUINames.clientWinUIName].GetComponent<Text>().text = "You Lost!";
                    NameToWinUI[WinUINames.clientWinUIName].SetActive(true);

                    NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Opponent have won because the right number was guessed";
                }
                else
                {
                    NameToWinUI[WinUINames.HostWinUIName].GetComponent<Text>().text = "You Won!";
                    NameToWinUI[WinUINames.HostWinUIName].SetActive(true);

                    NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "You've won because the right number was guessed";
                }
            }
            else
            {
                if (GuessVariables.hostScore == GuessVariables.clientScore)
                {
                    NameToWinUI[WinUINames.stalemateUIName].SetActive(true);
                    NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Opponent and your score are same";
                    NameToWinUI[WinUINames.wonInfo].SetActive(true);
                }
                else if (GuessVariables.hostScore > GuessVariables.clientScore)
                {
                    if(NetworkVariables.isHost)
                    {
                        NameToWinUI[WinUINames.clientWinUIName].GetComponent<Text>().text = "You Lost!";
                        NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Your score was lesser to opponent";
                        NameToWinUI[WinUINames.clientWinUIName].SetActive(true);
                    }
                    else
                    {
                        NameToWinUI[WinUINames.HostWinUIName].GetComponent<Text>().text = "You Won!";
                        NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Your score was major to opponent";
                        NameToWinUI[WinUINames.HostWinUIName].SetActive(true);
                    }
                }
                else
                {
                    if (NetworkVariables.isHost)
                    {
                        NameToWinUI[WinUINames.clientWinUIName].GetComponent<Text>().text = "You Won!";
                        NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Your score was major to opponent";
                        NameToWinUI[WinUINames.clientWinUIName].SetActive(true);
                    }
                    else
                    {
                        NameToWinUI[WinUINames.HostWinUIName].GetComponent<Text>().text = "You Lost!";
                        NameToWinUI[WinUINames.wonInfo].GetComponent<Text>().text = "Your score was lesser to opponent";
                        NameToWinUI[WinUINames.HostWinUIName].SetActive(true);
                    }
                }
            }
            NameToWinUI[WinUINames.wonInfo].SetActive(true);
        }


    }

}