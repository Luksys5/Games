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
        public GameObject winByInfo;

        public Text GuessLeft;
        
        Dictionary<string, GameObject> NameToWinUI;
        GameObject mainHostObj;
        GameObject mainClientObj;
        GameObject restartBtn;

        int guessesLeft;

        public void Start()
        { 
            NameToWinUI = new Dictionary<string, GameObject>();
            for(int i = 0; i < StatusUIs.transform.childCount; i++)
            {
                GameObject child = StatusUIs.transform.GetChild(i).gameObject;
                if (child.name == WINUINames.HostWinUIName)
                    NameToWinUI.Add(WINUINames.HostWinUIName, child.gameObject);
                else if (child.name == WINUINames.clientWinUIName)
                    NameToWinUI.Add(WINUINames.clientWinUIName, child.gameObject);
                else if (child.name == WINUINames.stalemateUIName)
                    NameToWinUI.Add(WINUINames.stalemateUIName, child.gameObject);
                child.SetActive(false);
            }

            mainHostObj = HostUI.transform.parent.gameObject;
            mainClientObj = ClientUI.transform.parent.gameObject;
            restartBtn = GameObject.FindGameObjectWithTag(Tags.RestartBtn);
            restartBtn.SetActive(false);
            NetworkVariables.isHost = PhotonNetwork.isMasterClient;
            ReInitialize();
            PhotonObjectOwnerRequest();
            guessesLeft = GuessVariables.GUESS_LEFT_DEFAULT;
        }

        public void ReInitialize()
        {
            GuessVariables.readyToGuess = false;
            GuessVariables.clientGuess = 0;
            GuessVariables.hostGuess = 0;
            NetworkVariables.ownerChanged = false;
        }

        public void PhotonObjectOwnerRequest()
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
            PhotonObjectOwnerRequest();
        }

        public void OnRestart()
        {
            mainClientObj.GetComponent<GuessNumber>().Reinitialize();

            guessesLeft = GuessVariables.GUESS_LEFT_DEFAULT;
            GuessLeft.text = guessesLeft.ToString();
            ReInitialize();
            NetworkVariables.isHost = !NetworkVariables.isHost;
            PhotonObjectOwnerRequest();

            restartBtn.SetActive(false);
            foreach(string key in NameToWinUI.Keys)
            {
                NameToWinUI[key].SetActive(false);
            }
        }

        private void SetWinnerHideUI(bool guessedRight)
        {
            // hide gameobjects
            HostUI.SetActive(false);
            ClientUI.SetActive(false);

            // unhide restart button 
            restartBtn.SetActive(true);

            // unhide win texts
            if (guessedRight)
            {
                winByInfo.GetComponent<Text>().text = "Won becouse guessed number";
                winByInfo.SetActive(true);
                if (PhotonNetwork.isMasterClient)
                {
                    NameToWinUI[WINUINames.HostWinUIName].GetComponent<Text>().text = NetworkVariables.hostName + " Wins!";
                    NameToWinUI[WINUINames.HostWinUIName].SetActive(true);
                }
                else
                {
                    NameToWinUI[WINUINames.clientWinUIName].GetComponent<Text>().text = NetworkVariables.clientName + " Wins!";
                    NameToWinUI[WINUINames.clientWinUIName].SetActive(true);
                }
            }
            else
            {
                if (GuessVariables.hostScore == GuessVariables.clientScore)
                {
                    NameToWinUI[WINUINames.stalemateUIName].SetActive(true);
                }
                else if (GuessVariables.hostScore > GuessVariables.clientScore)
                {
                    winByInfo.GetComponent<Text>().text = "Won by higher Score";
                    winByInfo.SetActive(true);
                    NameToWinUI[WINUINames.HostWinUIName].GetComponent<Text>().text = NetworkVariables.hostName + " Wins!";
                    NameToWinUI[WINUINames.HostWinUIName].SetActive(true);
                }
                else
                {
                    winByInfo.GetComponent<Text>().text = "Won by higher Score";
                    winByInfo.SetActive(true);
                    NameToWinUI[WINUINames.clientWinUIName].GetComponent<Text>().text = NetworkVariables.clientName + " Wins!";
                    NameToWinUI[WINUINames.clientWinUIName].SetActive(true);
                }
            }
        }

        public void OnBackPressed()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(BuildIndex.GuessMultiplayerMenu);
        }

    }

}