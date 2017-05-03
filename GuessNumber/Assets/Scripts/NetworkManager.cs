using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GuessNumber
{
    public class NetworkManager : PunBehaviour {
        public Text errInfo;
        public InputField serverAdress;
        public InputField serverPort;

        private GameObject labels;

        private string connectState = "";
        private const string ROOM_NAME = "GuessNumber";
        private const string SERVER_VERSION = "v1.0";


        // Use this for initialization
        void Start()
        {
            labels = GameObject.FindGameObjectWithTag(Tags.Labels);
            labels.SetActive(false);

            if(NetworkVariables.serverAddr != "")
            {
                serverAdress.text = NetworkVariables.serverAddr;
            }
            if(NetworkVariables.port != "")
            {
                serverPort.text = NetworkVariables.port;
            }
        }

        private void OnGUI()
        {
            GUI.TextField(new Rect(0, 0, 200, 30), connectState);
        }

        private void Update()
        {
            if (connectState != PhotonNetwork.connectionStateDetailed.ToString())
                connectState = PhotonNetwork.connectionStateDetailed.ToString();
        }

        public void Connect()
        {
            int serverPortInt = 0;
            
            if(serverAdress == null || serverPort == null || string.IsNullOrEmpty(serverAdress.text) || string.IsNullOrEmpty(serverPort.text))
            {
                labels.SetActive(true);
                errInfo.text = "Fill photon server adress and port";
                return;
            }
            else if(int.TryParse(serverPort.text, out serverPortInt) == false)
            {
                labels.SetActive(true);
                errInfo.text = "Check photon server port value. Port example: 5055";
                return;
            }
            else
            {
                labels.SetActive(false);
            }

            PhotonNetwork.ConnectToMaster(serverAdress.text, serverPortInt, "", SERVER_VERSION);
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.automaticallySyncScene = true;

            NetworkVariables.serverAddr = serverAdress.text;
            NetworkVariables.port = serverPort.text;
        }

        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }


        public void Host()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                labels.SetActive(true);
                errInfo.text = "Couldn't host room. Try relaunching photon server";
                return;
            }
            RoomOptions roomOpts = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(ROOM_NAME, roomOpts, TypedLobby.Default);
        }


        public void Join()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                GameObject.FindGameObjectWithTag(Tags.Labels).SetActive(true);
                errInfo.text = "Couldn't join room. Try relaunching photon server";
                return;
            }
            PhotonNetwork.JoinRoom(ROOM_NAME);
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            labels.SetActive(true);
            errInfo.text = "Couldn't connect to server check if it's running. Adress: " + PhotonNetwork.ServerAddress;
            return;
        }

        void OnPhotonRandomJoinFailed()
        {
            Debug.Log("Can't join random room!");
            PhotonNetwork.CreateRoom(ROOM_NAME);
        }

        public override void OnJoinedRoom()
        {
            labels.SetActive(false);
            SceneManager.LoadScene(BuildIndex.GuessMultiplayer);
        }

        public void OnBackPresed()
        {
            SceneManager.LoadScene(BuildIndex.MainMenu);
        }
    }
}