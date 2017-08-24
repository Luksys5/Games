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
        int leftPlayerMessage = 1; // 0 when it's right player message
        int messageHeight = 60;
        int messageCount = 0;
        int minTopMargin = Screen.height - 80)
        const int maxWidth = 250;
        const int minWidth = 80;
        const int maxHeight = 70;
        const int minHeight = 35;
        const int MAX_ROW_COUNT = 3;
        const int MAX_CHARS_IN_ROW = 23;
        const int MIN_CHARS_IN_ROW = 6;
        const int MAX_CHARS = 75;
        const int MAX_MSG_IN_SCREEN = 5;
        #endregion privateVars

        #region ChatClientInterfaceProps
        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log(message);
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log(state.ToString());
        }

        public void OnConnected()
        {
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
            {
                fullMsg += msg;
            }
            Debug.Log(fullMsg);

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
            Debug.Log("Subscribed");
        }

        public void OnUnsubscribed(string[] channels)
        {
            Debug.Log("Unsubscribed");
        }

        #endregion ChatClientInterfaceProps

        // Use this for initialization
        void Awake()
        {
            chatClient = new ChatClient(this);
            chatClient.ChatRegion = "EU";
            chatClient.Connect("29f6c1e9-d555-43c0-bbf7-ce05ad05cbbf", "1.0", new ExitGames.Client.Photon.Chat.AuthenticationValues("Test"));
            Application.runInBackground = true;
            chatContainer = GameObject.FindGameObjectWithTag(Tags.Chat).transform.parent;
            Random.InitState((int)System.DateTime.Now.Ticks);
        }

        // Update is called once per frame
        void Update()
        {
            //if (chatClient == null)
            //    return;

            //if (subscribedToRoom == false)
            //    JoinRoomChannels();

            //chatClient.Service();
        }

        private void createMessage(string newText, int side)
        {
            GameObject message = Instantiate(chatMsgImage,
                new Vector3(0, 0, 0),
                Quaternion.identity,
                chatContainer);

            if(newText.Length > MAX_CHARS)
                newText = newText.Substring(0, newText.Length % MAX_CHARS) + "...";

            int width = 0;
            int height = 0;
            int currentTextLength = newText.Length;
            int wordsInRow = currentTextLength % MAX_CHARS_IN_ROW;
            int rowCount = (currentTextLength / MAX_CHARS_IN_ROW) + 1;
            if (rowCount == 1)
            {
                if (currentTextLength <= 6)
                    width = minWidth;
                else
                    width = ((maxWidth - minWidth) / (MAX_CHARS_IN_ROW - MIN_CHARS_IN_ROW)) * (currentTextLength - MIN_CHARS_IN_ROW) + minWidth;
            }
            else
                width = maxWidth;
            height = ((maxHeight - minHeight) / 3) * rowCount + minHeight;

            Vector3 rotation = Vector3.zero;
            float marginRight = -250 + (int)(width * 0.5f);
            float marginTop = minTopMargin - (messageCount * messageHeight + height * 0.5f);
            if (side == 1)
            {
                rotation = new Vector3(0, 180, 0);
                marginRight = -20 - (width * 0.5f);
            }

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
            rectTransform.position = new Vector3(Screen.width + marginRight, marginTop, 0);
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.rotation = Quaternion.Euler(rotation);


            messageCount = Mathf.Clamp(messageCount + 1, 0, MAX_MSG_IN_SCREEN);
            chatMessages.Add(message);
            if(chatMessages.Count == MAX_MSG_IN_SCREEN)
            {
                GameObject firstMessage = chatMessages[0];
                chatMessages.Remove(firstMessage);
                Destroy(firstMessage);
                int i = 0;
                foreach(GameObject msg in chatMessages)
                {
                    currentTextLength = msg.GetComponentInChildren<Text>().text.Length;
                    rowCount = (currentTextLength / MAX_CHARS_IN_ROW) + 1;
                    height = ((maxHeight - minHeight) / 3) * rowCount + minHeight;
                    marginTop = minTopMargin - (i++ * messageHeight + height * 0.5f);

                    rectTransform = msg.GetComponent<RectTransform>();
                    rectTransform.position = new Vector2(rectTransform.position.x, marginTop);
                }
                messageCount -= 1;
            }
        }


        void JoinRoomChannels()
        {
            channelRoomName = "room" + PhotonNetwork.room.Name.GetHashCode();

            chatClient.Subscribe(new string[] { channelRoomName });
        }

        #region PublicMethods
        public void SendText()
        {
            string text = GameObject.FindGameObjectWithTag("Chat").GetComponent<InputField>().text;
            if (string.IsNullOrEmpty(text))
                return;

            //chatClient.SendPrivateMessage(channelRoomName, text);
            createMessage(text, Random.Range(0, 2));
        }
        #endregion PublicMethods

        public void OnApplicationQuit()
        {
            chatClient.Disconnect();
        }

    }
}