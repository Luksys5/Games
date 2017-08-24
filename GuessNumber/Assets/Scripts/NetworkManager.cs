using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GuessNumber
{
    public class NetworkManager : PunBehaviour {
        public Text errInfo;
        public InputField serverAdress;
        public InputField playerName;
        public InputField gameRoomName;
        public RectTransform roomView;

        private List<Text> roomList = new List<Text>();
        private GameObject labels;
        private RectTransform inputsRect;

        private string connectState = "";
        private string lastRoomValue = "";

        private float currentRoomViewHeight;
        private float currentRoomViewPositionY;
        private float currentRoomInputsPositionY;

        private const string appId = "24d3a1f4-57b0-46d1-8e62-a96a1aa64df8";
        private const string ROOM_NAME = "GuessNumber";
        private const string SERVER_VERSION = "v1.0";
        private const float INPUT_HEIGHT = 35f;


        // Use this for initialization
        void Start()
        {
            labels = GameObject.FindGameObjectWithTag(Tags.Labels);
            labels.SetActive(false);

            if(NetworkVariables.serverAddr != "")
            {
                serverAdress.text = NetworkVariables.serverAddr;
            }

            GameObject btnContainer = GameObject.FindGameObjectWithTag("RoomList");
            foreach(Button btn in btnContainer.GetComponentsInChildren(typeof(Button), false))
            {
                roomList.Add(btn.GetComponentInChildren<Text>());
                btn.onClick.AddListener(() => { OnBtnClick(btn); });
            }
            roomView.gameObject.SetActive(false);
            inputsRect = btnContainer.GetComponent<RectTransform>();

            currentRoomViewHeight = roomView.sizeDelta.y;
            currentRoomViewPositionY = roomView.position.y;
            currentRoomInputsPositionY = inputsRect.position.y;

            gameRoomName = GameObject.FindGameObjectWithTag("gameRoomName")?.GetComponent<InputField>();

        }

        private void OnGUI()
        {
            GUI.TextField(new Rect(0, 0, 200, 30), connectState);
        }

        private void Update()
        {

            if (lastRoomValue != gameRoomName.text)
                searchRooms(gameRoomName.text);

            if (connectState != PhotonNetwork.connectionStateDetailed.ToString())
                connectState = PhotonNetwork.connectionStateDetailed.ToString();
        }


        private void searchRooms(string currentRoomName)
        {
            if(PhotonNetwork.connected == false)
            {
                labels.SetActive(true);
                errInfo.text = "To join any game room You must connect first.";
                return;
            }
            else
            {
                labels.SetActive(false);
            }

            foreach (Text roomButton in roomList)
            {
                roomButton.GetComponent<Text>().text = string.Empty;
            }

            RoomInfo[] allRooms = PhotonNetwork.GetRoomList();
            RoomInfo[] matchingRooms = allRooms.Where(roomInfo => Regex.Match(roomInfo.Name, currentRoomName).Success)?.ToArray();
            if (allRooms.Length == 0)
            {
                roomView.gameObject.SetActive(false);
                return;
            }

            ResetRoomViewPosSize();
            roomView.gameObject.SetActive(true);

            int i = 0; 
            while(i < roomList.Count && i < allRooms.Length)
            {
                roomList[i].transform.parent.gameObject.SetActive(true);
                roomList[i].GetComponent<Text>().text = allRooms[i].Name;
                i++;
            }
            
            for (int j = i; j < roomList.Count; j++)
            {
                DisableInputAndChangeView(roomList[j]);
            }
            lastRoomValue = currentRoomName;
        }

        private void ResetRoomViewPosSize()
        {
            roomView.sizeDelta = new Vector2(roomView.sizeDelta.x, currentRoomViewHeight);
            roomView.position = new Vector3(roomView.position.x, currentRoomViewPositionY, roomView.position.z);
            inputsRect.position = new Vector3(inputsRect.position.x, currentRoomInputsPositionY, inputsRect.position.z);
        }

        private void DisableInputAndChangeView(Text currentText)
        {
            currentText.transform.parent.gameObject.SetActive(false);
            roomView.sizeDelta = new Vector2(roomView.sizeDelta.x, roomView.sizeDelta.y - INPUT_HEIGHT);
            roomView.position = new Vector3(roomView.position.x, roomView.position.y + (INPUT_HEIGHT * 0.5f), roomView.position.z);
            inputsRect.position = new Vector3(inputsRect.position.x, inputsRect.position.y - (INPUT_HEIGHT * 0.5f) + 0.75f, inputsRect.position.z);
        }

        public void OnBtnClick(Button btn)
        {
            PhotonNetwork.JoinRoom(btn.GetComponentInChildren<Text>().text);
        }

        public void Connect()
        {
            //if (serverAdress == null || serverPort == null || string.IsNullOrEmpty(serverAdress.text) || string.IsNullOrEmpty(serverPort.text))
            //{
            //    labels.SetActive(true);
            //    errInfo.text = "Fill photon server adress and port";
            //    return;
            //}
            //else
            //if (int.TryParse(serverPort.text, out serverPortInt) == false)
            //{
            //    labels.SetActive(true);
            //    errInfo.text = "Check photon server port value. Port example: 5055";
            //    return;
            //}
            //else
            //{
            //    labels.SetActive(false);
            //}


            PhotonNetwork.ConnectToMaster("127.0.0.1", NetworkVariables.port, appId, SERVER_VERSION);
            //PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, SERVER_VERSION);
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.automaticallySyncScene = true;
            
            NetworkVariables.serverAddr = serverAdress.text;
        }

        public void Host()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                errInfo.text = "Couldn't host room. Try relaunching photon server";
                labels.SetActive(true);
                return;
            }
            else if (string.IsNullOrEmpty(playerName.text))
            {
                errInfo.text = "Player name input must be filled!";
                labels.SetActive(true);
                return;
            }
            else if (string.IsNullOrEmpty(gameRoomName.text))
            {
                labels.SetActive(true);
                errInfo.text = "Set game room name!";
                return;
            }

            RoomOptions roomOpts = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
            PhotonNetwork.CreateRoom(gameRoomName.text, roomOpts, TypedLobby.Default);
        }


        public void Join()
        {
            if (!PhotonNetwork.connectedAndReady)
            {
                errInfo.text = "Couldn't join room. Try relaunching photon server";
                labels.SetActive(true);
                return;
            }
            else if (string.IsNullOrEmpty(playerName.text))
            {
                errInfo.text = "Player name input must be filled!";
                labels.SetActive(true);
                return;
            } 
            else if (string.IsNullOrEmpty(gameRoomName.text))
            {
                errInfo.text = "Room name must be filled";
                labels.SetActive(true);
                return;
            }

            bool roomExist = PhotonNetwork.GetRoomList().Count(room => room.Name == gameRoomName.text) > 0;
            if(roomExist)
                PhotonNetwork.JoinRoom(gameRoomName.text);
            else
            {
                errInfo.text = "Specified room doesn't exist";
                labels.SetActive(true);
            }
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            errInfo.text = "Couldn't connect to server check if it's running. Adress: " + PhotonNetwork.ServerAddress;
            labels.SetActive(true);
            return;
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

        public void OnQuitGame()
        {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }

    }
}