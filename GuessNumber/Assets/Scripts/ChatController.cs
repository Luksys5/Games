using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{
    [RequireComponent(typeof(InputField))]
    public class ChatController : Photon.MonoBehaviour, IChatClientListener
    {

        #region publicVars
        public GameObject chatMsgImage;
        #endregion publicVars

        #region privateVars

        List<GameObject> chatMessages = new List<GameObject>();
        ChatClient chatClient;
        Transform chatContainer;

        string channelRoomName = string.Empty;

        float marginTop;
        public float MarginTop
        {
            get { return marginTop; }
            set { marginTop = minTopMargin - (value * messageHeight + Height * 0.5f); }
        }

        int minTopMargin = Screen.height - 60;
        int messageHeight = 60;
        int messageCount = 0;
        int textLength = 0;
        int rowHeight;

        int rowCount;
        public int RowCount
        {
            get { return rowCount; }
            set { rowCount = Mathf.Clamp((value / MAX_CHARS_IN_ROW) + 1, 0, MAX_ROW_COUNT); } // value is text length
        }

        int height;
        private int Height
        {
            get { return height; }
            set { height = (rowHeight * value) + minHeight; }
        }

        private int width;
        private int Width
        {
            get { return width; }
            set { 
                if (rowCount != -1)
                    width = maxWidth;

                if (value <= 6)
                    width = minWidth;
                else
                    width = ((maxWidth - minWidth) / (MAX_CHARS_IN_ROW - MIN_CHARS_IN_ROW)) * (value - MIN_CHARS_IN_ROW) + minWidth;
            }
        }

        const int maxWidth = 250;
        const int minWidth = 80;
        const int maxHeight = 70;
        const int minHeight = 35;
        const int MAX_ROW_COUNT = 3;
        const int MAX_CHARS_IN_ROW = 23;
        const int MIN_CHARS_IN_ROW = 6;
        const int MAX_CHARS = 75;
        const int MAX_MSG_IN_SCREEN = 5;

        bool subscribedToRoom = false;
        bool myMessage = false;
        #endregion privateVars

        // Use this for initialization
        void Awake()
        {
            chatClient = new ChatClient(this);
            chatClient.ChatRegion = "EU";
            chatClient.Connect("29f6c1e9-d555-43c0-bbf7-ce05ad05cbbf", "1.0", new ExitGames.Client.Photon.Chat.AuthenticationValues("Test"));
            Application.runInBackground = true;
            chatContainer = GameObject.FindGameObjectWithTag(Tags.Chat).transform.parent;

            rowHeight = (maxHeight - minHeight) / 3;
        }

        #region ChatClientInterfaceProps
        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log(message);
        }

        public void OnChatStateChange(ChatState state)
        {
        }

        public void OnConnected()
        {
            chatClient.Subscribe(new[] { "global" });
            chatClient.SetOnlineStatus(ChatUserStatus.Online);
        }

        public void OnDisconnected()
        {
            Debug.Log("Disonnected" + chatClient.DisconnectedCause.ToString());
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            string fullMsg = string.Empty;
            foreach (string msg in messages)
                fullMsg += msg;

            createMessage(fullMsg, myMessage);
            myMessage = false;
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log("Private msg: " + message);
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log("status update: " + user + ' ' + status.ToString() + ' ' + message.ToString());
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            subscribedToRoom = true;
            Debug.Log("Subscribed to channel" + channels[0]);
        }

        public void OnUnsubscribed(string[] channels)
        {
            
        }

        #endregion ChatClientInterfaceProps



        // Update is called once per frame
        void Update()
        {
            if (chatClient == null)
                return;

            chatClient.Service();
        }

        private void createMessage(string newText, bool myMessage)
        {
            GameObject message = Instantiate(chatMsgImage,
                new Vector3(0, 0, 0),
                Quaternion.identity,
                chatContainer);

            textLength = newText.Length;
            if(textLength > MAX_CHARS)
                textLength = (newText.Substring(0, newText.Length % MAX_CHARS) + "...").Length;

            RowCount = textLength;
            Height = rowCount;
            Width = textLength;
                       


            Vector3 rotation = Vector3.zero;
            float marginRight = -250 + (int)(Width * 0.5f);
            MarginTop = messageCount;
            if (!myMessage)
            {
                rotation = new Vector3(0, 180, 0);
                marginRight = -20 - (Width * 0.5f);
            }

            SetMessageTextAndLocation(message, new Vector3(Screen.width + marginRight, marginTop, 0), rotation, newText);
            AppendMessageOnOthers(message);
        }

        public void AppendMessageOnOthers(GameObject message)
        {
            messageCount = Mathf.Clamp(messageCount + 1, 0, MAX_MSG_IN_SCREEN);
            chatMessages.Add(message);
            if (chatMessages.Count == MAX_MSG_IN_SCREEN)
            {
                GameObject firstMessage = chatMessages[0];
                chatMessages.Remove(firstMessage);
                Destroy(firstMessage);

                // Set height of each message one message and space higher
                RectTransform rectTransform;
                int i = 0;
                foreach (GameObject msg in chatMessages)
                {
                    textLength = msg.GetComponentInChildren<Text>().text.Length;

                    // Using Setters
                    RowCount = textLength;
                    Height = rowCount;
                    MarginTop = i++;

                    rectTransform = msg.GetComponent<RectTransform>();
                    rectTransform.position = new Vector2(rectTransform.position.x, marginTop);
                }
                messageCount -= 1;
            }
        }

        public void SetMessageTextAndLocation(GameObject message, Vector3 position, Vector3 rotation, string newText)
        {
            RectTransform rectTransform;
            Text text = message.GetComponentInChildren<Text>();
            rectTransform = text.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width - 20, height);
            rectTransform.position = Vector3.zero;
            rectTransform.rotation = Quaternion.Euler(rotation);

            text.alignment = TextAnchor.MiddleCenter;
            text.text = newText;
            text.supportRichText = true;


            rectTransform = message.GetComponent<RectTransform>();
            rectTransform.position = position;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.rotation = Quaternion.Euler(rotation);
        }


        void JoinRoomChannels()
        {
            channelRoomName = "room" + PhotonNetwork.room.Name.GetHashCode();
            chatClient.Subscribe(new string[] { channelRoomName });
        }

        #region PublicMethods
        public void SendText()
        {
            if(subscribedToRoom == false)
            {
                Debug.Log("Cannot send msg because not subscribed to room");
                return;
            }

            string text = GameObject.FindGameObjectWithTag("Chat").GetComponent<InputField>().text;
            text = text == null ? string.Empty : text;

            myMessage = true;
            chatClient.PublishMessage("global", text);
        }
        #endregion PublicMethods

        public void OnApplicationQuit()
        {
            chatClient.Disconnect();
        }

    }
}