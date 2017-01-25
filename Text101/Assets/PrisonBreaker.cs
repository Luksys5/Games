using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonBreaker : MonoBehaviour {

    private Text prisonStateInfo;

    enum CellStates
    {
        cell_1, cell_2, cell_3, cell_4, sheet_1, cell_door_1, cell_door_2, cell_door_3, mirror_1, floor_1
    }

    enum CorridorStates
    {
        corridor_1, corridor_2, corridor_3, corridor_4, corridor_5, corridor_6, corridor_7, floor_1,
        floor_2, closet_1, closet_2, stairs_1, stairs_2, officer_1, officer_2, busted, finished
    }

    enum PrisonState
    {
        cell, corridor
    }

    CellStates cellState;
    CellStates lastCellState;
    CorridorStates corrState;
    CorridorStates lastCorrState;

    PrisonState prisonState;

    private float timeToAnalyze;
    private float timeBeforeAlert;

    private int smashDoorCount = 0;

    private bool checkingCells;
    private bool haveHairClip;
    private bool haveKey;

    

    private const float ANALYZING_TIME = 5.0f;
    private const float ALERTING_TIME = 60f;

	// Use this for initialization
	void Start () {
        cellState = CellStates.cell_1;
        lastCellState = cellState;
        prisonState = PrisonState.cell;
        prisonStateInfo = GetComponent<Text>();
        haveKey = false;
        Reinitialize();
	}

    void Reinitialize()
    {
        corrState = CorridorStates.corridor_1;
        lastCorrState = corrState;
        timeBeforeAlert = ALERTING_TIME;
        timeToAnalyze = ANALYZING_TIME;
        checkingCells = false;
        haveHairClip = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(prisonState == PrisonState.cell)
        {
            onCellState();
        }
        else
        {
            onCorridorState();
        }
	}

    void onCellState()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(smashDoorCount);
            Debug.Log(cellState);
            Debug.Log(corrState);
            Debug.Log(prisonState);
        }
        // Finished state
        if(smashDoorCount == 3)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                prisonState = PrisonState.corridor;
            }
            return;
        }


        if(Input.GetKeyDown(KeyCode.R))
        {
            cellState = lastCellState;
            smashDoorCount = 0;
        }

        if (cellState == CellStates.cell_1)
        {
            prisonStateInfo.text = "You're in the prison. It's midnight and you search for way to escape.\n" +
                                    "You first need to search your cell room.\n" +
                                    "[ Press S to check bed Sheets, press M to check Mirror, Press D to check cell Door ]";

            if (Input.GetKeyDown(KeyCode.S))
            {
                cellState = CellStates.sheet_1;
                prisonStateInfo.text = "You check dirty sheets, but you found nothing.\n" +
                                        "[ Press R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                cellState = CellStates.mirror_1;
                prisonStateInfo.text = "You see Mirror. It makes you mad while you look at it.\n" +
                                        "[ Press B for Break mirror, Press R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                cellState = CellStates.cell_door_1;
                prisonStateInfo.text = "Cell door is locked. You don't have any tool to open it\n" +
                                        "[ Press B to try Break it Press R to Return ]";

            }
        }
        else if (cellState == CellStates.cell_2)
        {
            prisonStateInfo.text = "You lose your anger as the mirror falls.\n" +
                                    "Mirror fractured to pieces on the ground.\n" +
                                    "[ Press S to check Sheets, F to check floor, D to check door]";

            if (Input.GetKeyDown(KeyCode.S))
            {
                cellState = CellStates.sheet_1;
                prisonStateInfo.text = "You check dirty sheets, but you found nothing.\n" +
                                        "[ Press R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                cellState = CellStates.floor_1;
                prisonStateInfo.text = "You see only glass from mirror and some kind piece of metal from it.\n" +
                                        "[ Press E to Examine it, R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                cellState = CellStates.cell_door_1;
                prisonStateInfo.text = "Cell door is locked. You don't have any tool to open it\n" +
                                        "[ Press B to try Break it Press R to Return ]";
            }
        }
        else if (cellState == CellStates.cell_3)
        {
            prisonStateInfo.text = "You found a piece of metal which resembles a key structure.\n" +
                                    "Maybe someone have left it in mirror without a chance to use it.\n" +
                                    "[ Press S to check Sheets, D to check door ]";

            if (Input.GetKeyDown(KeyCode.S))
            {
                cellState = CellStates.sheet_1;
                prisonStateInfo.text = "You check dirty sheets, but you found nothing.\n" +
                                        "[ Press R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                cellState = CellStates.cell_door_2;
                prisonStateInfo.text = "You're in front of the door.\n" +
                                        "[ Press U to try unlock, B to try Break it, R to Return ]";
                lastCellState = CellStates.cell_4;
            }
        }
        else if (cellState == CellStates.cell_4)
        {
            prisonStateInfo.text = "You step back from door. And retought what you can do.\n" +
                                    "[ Press S to view Sheets, Press D to check Door ]";

            if (Input.GetKeyDown(KeyCode.S))
            {
                cellState = CellStates.sheet_1;
                prisonStateInfo.text = "You've checked dirty sheets altought you don't see how it could help \n" +
                                        "[ Press R to Return ]";
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                cellState = CellStates.cell_door_3;
                prisonStateInfo.text = "You're in front of the door.\n" +
                                        "[ B to try Break it, R to Return ]";
            }
        }
        else if (cellState == CellStates.cell_door_1 && Input.GetKeyDown(KeyCode.B))
        {
            prisonStateInfo.text = "You run and smash into door with shoulder. \n" +
                                    "There was a sound, but it not enough to draw attention of prison officers.\n" +
                                    "It seems there's no effect on door whereas on your shoulder\n" +
                                    "[ Press R to Return ]";
        }
        else if (cellState == CellStates.cell_door_2)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                prisonStateInfo.text = "As You try to rotate metal piece inside a lock it breaks.\n" +
                                        "It almost rotated to it's unlocked position.\n" +
                                        "[ Press B to try Break, R for Return ]";
                cellState = CellStates.cell_door_3;
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {

                prisonStateInfo.text = "You run and smash into the door with shoulder. \n" +
                                        "There was a sound, but it not enough to draw attention of prison officers.\n" +
                                        "It seems there's no effect on door whereas on your shoulder\n" +
                                        "[ Press B to try Break it, R for Return ]";
                

            }
        }
        else if (cellState == CellStates.cell_door_3)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (smashDoorCount == 0)
                {
                    prisonStateInfo.text = "You run and smash into the door with shoulder. \n" +
                                            "There was a sound, but it not enough to draw attention of prison officers.\n" +
                                            "It seems there's no effect on door whereas on your shoulder\n" +
                                            "[ Press B to try Break it, R for Return ]";
                }
                if (smashDoorCount == 1)
                {
                    prisonStateInfo.text = "You run and smash into door with shoulder. \n" +
                                            "You are getting dizzy, and your shoulder starts to hurt\n" +
                                            "[ Press B to try Break it, R for Return ]";
                }
                if (smashDoorCount == 2)
                {
                    prisonStateInfo.text = "You run and smash into door with shoulder. \n" +
                                            "There was a sound, a sound of shifting key and unlocking door" + 
                                            "You step outside. You are free from cell but not yet from prison...\n" +
                                            "[ Press Enter to continue prison break ]";
                }
                smashDoorCount++;
            }
        }
        else if (cellState == CellStates.mirror_1 && Input.GetKeyDown(KeyCode.B))
        {
            cellState = CellStates.cell_2;
            lastCellState = cellState;
            
        }
        else if(cellState == CellStates.floor_1 && Input.GetKeyDown(KeyCode.E))
        {
            cellState = CellStates.cell_3;
            lastCellState = cellState;
            
        }
    }

    void onCorridorState()
    {
        prisonStateInfo.text = "Corridor...";
    }

    void onFinish()
    {

    }

    void onBusted()
    {

    }


}
