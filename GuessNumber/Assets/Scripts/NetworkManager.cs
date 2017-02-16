using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkManager : PunBehaviour {
    public Transform spawnPoint;

    public string roomName = "testRoom";
    public string playerPrefabName = "Cube";
    private string VERSION = "v1.0";
    private string lastState = "";

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(0,0, 200, 30), lastState);
    }

    private void Update()
    {
        if (lastState != PhotonNetwork.connectionStateDetailed.ToString())
        {
            lastState = PhotonNetwork.connectionStateDetailed.ToString();
            Debug.Log(lastState);
        }
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOpts = new RoomOptions() { IsVisible = true, maxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOpts, TypedLobby.Default);
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        GameObject cube = PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
        CubeMovement movement = cube.GetComponent<CubeMovement>();
        movement.enabled = true;
        cube.GetComponentInChildren<Camera>().enabled = true;
    }

}
