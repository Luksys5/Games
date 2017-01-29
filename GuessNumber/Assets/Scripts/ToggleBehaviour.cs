using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBehaviour : MonoBehaviour {

    public Toggle playerToogle;
    public Toggle pcToogle;
    // 1 - player, 0 - pc
    public bool whoIsActive;


    private void Start()
    {
        whoIsActive = playerToogle.isOn;
    }

    private void Update()
    {
        if(playerToogle.isOn == pcToogle.isOn)
        {
            whoIsActive = !whoIsActive;
            playerToogle.isOn = whoIsActive;
            pcToogle.isOn = !whoIsActive;
            if(whoIsActive)
            {
                Debug.Log("Player Active");
            }
            else
            {
                Debug.Log("PC Active");
            }
        }
    }

}