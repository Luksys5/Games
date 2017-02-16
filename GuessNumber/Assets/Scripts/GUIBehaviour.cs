using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIBehaviour : MonoBehaviour {

    public Toggle playerToogle;
    public Toggle pcToogle;
    public Toggle multiToggle;

    // 0 - player, 1 - pc, 2 multiplayer
    public int whoIsActive;

    private void Update()
    {
        if(playerToogle.isOn == true && whoIsActive != 0)
        {
            whoIsActive = 0;
            pcToogle.isOn = false;
            multiToggle.isOn = false;
        }
        else if (pcToogle.isOn == true && whoIsActive != 1)
        {
            whoIsActive = 1;
            playerToogle.isOn = false;
            multiToggle.isOn = false;
        }
        else if (multiToggle.isOn == true && whoIsActive != 2)
        {
            whoIsActive = 2;
            playerToogle.isOn = false;
            pcToogle.isOn = false;
        }
    }

    public void loadLevel()
    {
        
        if (playerToogle.isOn == true)
        {
            SceneManager.LoadScene(GlobalData.PlayerGuessBuildIndex);
        }
        else if(pcToogle.isOn == true)
        {
            SceneManager.LoadScene(GlobalData.PCGUessBuildIndex);
        }
        else if (multiToggle.isOn == true)
        {
            SceneManager.LoadScene(GlobalData.MultiplayerBuildIndex);
        }

    }

}