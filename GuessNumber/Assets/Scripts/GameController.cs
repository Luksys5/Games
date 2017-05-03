using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GuessNumber
{
    public class GameController : PunBehaviour
    {
        public GameObject HostUI;
        public GameObject ClientUI;

        GameObject mainHostObj;
        GameObject mainClientObj;

        public void Start()
        {
            mainHostObj = HostUI.transform.parent.gameObject;
            mainClientObj = ClientUI.transform.parent.gameObject;
            NetworkVariables.isHost = PhotonNetwork.isMasterClient;
            ReInitialize();
            PhotonObjectOwnerRequest();
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
            ReInitialize();
            NetworkVariables.isHost = !NetworkVariables.isHost;
            PhotonObjectOwnerRequest();
        }

        public void OnBackPressed()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(BuildIndex.GuessMultiplayerMenu);
        }

    }

}