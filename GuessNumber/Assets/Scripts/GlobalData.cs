﻿using UnityEngine;

namespace GuessNumber { 
    public static class BuildIndex
    {
        public static int MainMenu = 0;
        public static int PlayerGuess = 1;
        public static int PCGUess = 2;
        public static int GuessMultiplayerMenu = 3;
        public static int GuessMultiplayer = 4;
    }

    public static class Tags
    {
        public const string Canvas  = "Canvas";
        public const string Labels  = "Labels";
        public const string RestartBtn = "Restart";
        public const string HostUI  = "HostUI";
        public const string ClientUI = "ClientUI";
        public const string ErrorInfo = "ErrorInfo";
        public const string Loader = "Loader";
        public const string Chat = "Chat";
    }

    public static class GuessVariables
    {
        public const int MAX_GUESS = 100;
        public const int MIN_GUESS = 1;
        public const int GUESS_LEFT_DEFAULT = 6;

        public static int clientGuess = 0;
        public static int hostGuess = 0;
        public static int hostScore = 0;
        public static int clientScore = 0;
        public static bool readyToGuess = false;
        public static bool haveWon = false;
        
        public static int calculateScore(int guess)
        {
            int diff = MAX_GUESS - Mathf.Abs(hostGuess - guess);
            return (diff * diff + diff * 2);
        }
    }

    public static class WinUINames
    {
        public const string HostWinUIName = "HostWin";
        public const string clientWinUIName = "ClientWin";
        public const string stalemateUIName = "Stalemate";
        public const string wonInfo = "wonInfo";
    }

    public static class NetworkVariables
    {
        public static string serverAddr = "";
        
        public static string hostName = "";
        public static string clientName = "";

        public static int currentHost;
        public static int lastHost;
        public static int hostID = 0;
        public static int clientID = 1;

        public static bool isHost = false;
        public static bool ownerChanged = false;
        public static bool clientConnected = false;

        public const int port = 5055;
    }
}