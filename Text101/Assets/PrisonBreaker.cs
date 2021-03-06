﻿using System.Collections;
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
        floor_2, bathroom_1, bathroom_2, bathroom_3, stairs_1, stairs_2, stairs_3, officer_1, officer_2,
        officer_3, corner, locker, shelf, drawers
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

    private string bustedInfo = "";
    private string finishInfo = "";
    

    private float timeToAnalyze;
    private float timeBeforeAlert;

    private int smashDoorCount = 0;
    private int bathroomBeenCount = 0;
    private int upstairsBeenCount = 0;

    private bool checkingCells;
    private bool haveHairClip;
    private bool haveKey;
    private bool busted;
    private bool finished;

    private const float ANALYZING_TIME = 5.0f;
    private const float ALERT_TIME = 60f;

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
        timeBeforeAlert = ALERT_TIME;
        timeToAnalyze = ANALYZING_TIME;
        checkingCells = false;
        haveHairClip = false;
        busted = false;
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
        if (finished == true)
        {
            onFinish();
            return;
        }
        else if(busted == true)
        {
            onBusted();
            return;
        }

        if(checkingCells == true)
        {
            timeBeforeAlert = Mathf.Clamp(timeBeforeAlert - Time.deltaTime, 0, ALERT_TIME);
            if(timeBeforeAlert < 0.1f)
            {
                busted = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            corrState = lastCorrState;
        }

        if(corrState == CorridorStates.corridor_1)
        {
            if (haveHairClip == true)
            {
                prisonStateInfo.text = "You've found hairclip.\n" +
                                        "You can check out this corridor or either move forward to next one.\n" +
                                        "[ Press A to move Ahead, C to check Bathroom, S to check Stairs ]";
            }
            else
            {
                prisonStateInfo.text = "You're in the prison corridor.\n" +
                                        "You can check out this corridor or either move forward to next one.\n" +
                                        "[ Press A to move Ahead, C to check Bathroom, S to check Stairs, F to check Floor]";
            }


            if(Input.GetKeyDown(KeyCode.F))
            {
                haveHairClip = true;
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                corrState = CorridorStates.corridor_2;
                lastCorrState = corrState;
                bathroomBeenCount = 0;
                upstairsBeenCount = 0;
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                prisonStateInfo.text = "You climbed stairs where you find electrical paneling with some open unsecured wires.\n" +
                                            "[ Press T to try turn off the power, R for Return ]";
                corrState = CorridorStates.stairs_1;
            }
            else if(Input.GetKeyDown(KeyCode.C))
            {
                if (bathroomBeenCount == 3)
                {
                    reportOnBathroom("first corridor toilet. ");
                }
                else
                {
                    reportOnBathroom("first corridor bathroom");
                    bathroomBeenCount++;
                }
                corrState = CorridorStates.bathroom_1;   
            }

        }
        else if(corrState == CorridorStates.corridor_2)
        {
            prisonStateInfo.text = "You've moved to next corridor. You see light from officers room.\n" +
                                    "You can lookout in bathroom or stairs in new corridor.\n" +
                                    "[ Press O to inspect Officers room, C for check Bathroom, S for check Stairs]";

            if (Input.GetKeyDown(KeyCode.O))
            {
                prisonStateInfo.text = "You're next to the officers room.\n" +
                                        "[ Hold I to inspect room, R to Return ]";

                corrState = CorridorStates.officer_1;

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                reportOnStairs("second corridor stairs");
                corrState = CorridorStates.stairs_2;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (bathroomBeenCount == 3)
                {
                    reportOnBathroom("second corridor toilet");
                } 
                else
                { 
                    reportOnBathroom("second corridor bathroom");
                    bathroomBeenCount++;
                }
                corrState = CorridorStates.bathroom_1;
                
            }
        }
        else if (corrState == CorridorStates.corridor_3 && Input.GetKey(KeyCode.I) == false)
        {
            prisonStateInfo.text = "You don't have much time before officer goes out to check cells.\n" + 
                                    "You need to choose a place to hide.\n" +
                                    "[ Press C for hide in Bathroom, S for climb stairs and hide behind corner ]";

            if (Input.GetKeyDown(KeyCode.S))
            {
                corrState = CorridorStates.stairs_2;
                lastCorrState = corrState;
                bustedInfo = "As you were staying upstairs officer found empty cell and alerted others...\n" +
                                "[ Press R to Retry ]";
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                corrState = CorridorStates.bathroom_2;
                lastCorrState = corrState;
                bustedInfo = "As you were deciding what you gonna do in the bathroom officer have found you..." +
                                "[ Press R to Retry ]";
            }
        }
        else if (corrState == CorridorStates.corridor_4)
        {
            prisonStateInfo.text = "You've climbed down stairs.\n" +
                                    "The only choice right now would be to find something useful in officer room. \n" +
                                    "[ Press G to Go to officers room ]";
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                corrState = CorridorStates.officer_2;
                lastCorrState = corrState;
                timeBeforeAlert = ALERT_TIME;
                bustedInfo = "Officer alerted others of escaped prisoner while you were looking in the officer room...\n" +
                                "[ Press R to Retry ]";
            }

        }
        else if (corrState == CorridorStates.corridor_5)
        {
            prisonStateInfo.text = "You've dressed as cleaner and pretending to work at the corridor.\n" +
                                    "Officer is approaching from his room and it seems he doesn't recognize you... \n" +
                                    "He passed you. And he stoped a few meters behind you, turned around and started walking towards you.\n " +
                                    "And right after opening door sound you've relieved that you didn't wait in bathroom becouse that's where he's gone.\n" +
                                    "[ Press M whenever you're ready to Move ]";

            if(Input.GetKeyDown(KeyCode.M))
            {
                finishInfo = "You've dressed as cleaner and walked out of a jail.\n" +
                                "Officers at the exit didin't recognized you as they were talking to each other.\n" +
                                "Congratulations! You are free... for now";
                finished = true;
            }

        }
        else if(corrState == CorridorStates.corridor_6)
        {
            prisonStateInfo.text = "You've climbed down in dark.\n" +
                                    "You hear noise from officer room. You should hide\n" +
                                    "[ Press C to hide in Bathroom, W to Wait while officer passes in corner]";
            if(Input.GetKeyDown(KeyCode.C))
            {
                corrState = CorridorStates.bathroom_3;
                lastCorrState = corrState;
            }
            else if(Input.GetKeyDown(KeyCode.W))
            {
                corrState = CorridorStates.corner;
                lastCorrState = corrState;
                bustedInfo = "You wait in the corner for officer to pass by...\n" +
                                "And the moment you've remembered that officer uses flashlight\n" +
                                "you saw a light which flashed right from corridor wall to your face\n" +
                                "[ Press R to Retry not beeing cought ]";
                busted = true;
            }

        }
        else if(corrState == CorridorStates.stairs_1 && Input.GetKeyDown(KeyCode.T))
        {
            int random = Random.Range(0, 10);
            if(random >5)
            {
                busted = true;
                bustedInfo = "You've been electrified. " + 
                                "It seems that this time you couldn't turn the lights off without beein shocked\n" + 
                                "[ Press R to Retry ]";
            }
            else
            {
                corrState = CorridorStates.stairs_3;
                lastCorrState = corrState;
                checkingCells = true;
                bustedInfo = "Officer found you lurking around power station in upstairs. \n" +
                                "You had maybe forgotten that officer will first search here..." + 
                                "[ Press R to Retry ]";
                timeBeforeAlert = ALERT_TIME;
            }
        }
        else if(corrState == CorridorStates.stairs_2 && checkingCells == true)
        {
            prisonStateInfo.text = "As you sit and wait behind corner you hear closing doors\n" +
                                        "and steps in lower floor. As the steps sound fades away you think it's time to move.\n" +
                                        "[ Press D for climb Down ]";
            if(Input.GetKeyDown(KeyCode.D))
            {
                corrState = CorridorStates.corridor_4;
                bustedInfo = "Officer found empty cell it seems you were not so fast this time";
            }
        }
        else if(corrState == CorridorStates.stairs_3)
        {

            prisonStateInfo.text = "You've turned the light off.\n" + 
                                    "You've some time to climb down and hide before someone comes to check the light\n" + 
                                    "[ Press D for climb Down ]";

            if (Input.GetKeyDown(KeyCode.D))
            {
                corrState = CorridorStates.corridor_6;
                lastCorrState = corrState;
            }
        }
        else if(corrState == CorridorStates.bathroom_2)
        {
            prisonStateInfo.text = "You're in bathroom. Therefore afer a moment you find something useful.\n" +
                                    "There lies cleaner uniform and a broom hidden under the sink.\n" + 
                                    "You can pretend to be cleaner or just wait officer to pass bathroom.\n" +
                                    "[ Press D to Dress as a cleaner, W to Wait for officer to pass ]";

            if(Input.GetKeyDown(KeyCode.D))
            {
                corrState = CorridorStates.corridor_5;
                lastCorrState = corrState;
                checkingCells = false;
            }
            else if(Input.GetKeyDown(KeyCode.W))
            {
                busted = true;
                bustedInfo = "You hear steps nearby. After a few seconds they've passed by.\n" +
                                "But right away you see doors opening.\n" +
                                "It seems that you and officer both wanted to be in the same place...\n" +
                                "[ Press R to retry escape from corridors ]";
            }
        }
        else if(corrState == CorridorStates.bathroom_3)
        {
            prisonStateInfo.text = "You hear that someone passes by and starts to climb stairs.\n" +
                                    "It's a good opportunity to move to officer room.\n" +
                                    "[ Press L to Leave bathroom and go to officer room]";
            if(Input.GetKeyDown(KeyCode.L))
            {
                corrState = CorridorStates.officer_2;
                lastCorrState = corrState;
                bustedInfo = "Officer alerted others of escaped prisoner while you were looking in the officer room...\n" +
                                "[ Press R to Retry ]";
            }
        }
        else if(corrState == CorridorStates.officer_1)
        {
            if(Input.GetKey(KeyCode.I))
            {
                timeToAnalyze = Mathf.Clamp(timeToAnalyze - Time.deltaTime, 0, ANALYZING_TIME);
                prisonStateInfo.text = "Officer is sitting on the chair against TV.\n" +
                                        "He must be into it becouse he pays no attention elsewhere.\n" +
                                        "You see check cells schedule, current time\n" +
                                        "There're few minutes left before he gets out";

                if(timeToAnalyze < 0.1f)
                {
                    corrState = CorridorStates.corridor_3;
                    lastCorrState = corrState;
                    checkingCells = true;
                    bustedInfo = "You were choosing hiding place too long and prison officer have found you." + 
                                    "[ Press R to Restart ]";
                    prisonStateInfo.text += "\n[ You can Release I ]";
                }              
            }
            else if(timeToAnalyze > (ANALYZING_TIME - 0.1f))
            {
                prisonStateInfo.text = "You're next to officer room.\n" +
                                        "[ Hold I longer to fully inspect room, R to Return ]";
            }
            else
            {
                prisonStateInfo.text = "You have not inspected all neccessary things in room.\n" +
                                        "[ Hold I longer to fully inspect room, R to Return ]";
                timeToAnalyze = ANALYZING_TIME - 0.2f;
            }

        }
        else if(corrState == CorridorStates.officer_2)
        {
            prisonStateInfo.text = "You're in officers room where in front of you is locker \n" +
                                        "on the right side a shelf and desk with drawers on the left side.\n" +
                                        "[ Press L to check Locker, S to check Shelf, D to check Drawers ]";

            
            if (Input.GetKeyDown(KeyCode.L))
            {
                prisonStateInfo.text = "The locker is locked and you cannot unlock it.\n" +
                                        "[ Press R to Return ]";
                corrState = CorridorStates.locker;
                bustedInfo = "You've got yourself busted while looking at the officer room locker.\n" +
                                "[ Press R to Retry ]";
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                prisonStateInfo.text = "The look up the shelf but you dont see anything useful.\n" +
                                        "[ Press R to Return ]";
                corrState = CorridorStates.shelf;
                bustedInfo = "Officer found empty cell and alerted others while you were looking at the shelf.\n" + 
                                "[ Press R to Return ]";
            }
            else if(Input.GetKeyDown(KeyCode.D) && haveHairClip == false)
            {
                if (timeBeforeAlert > 4f)
                {
                    timeBeforeAlert = 4.0f;
                }

                prisonStateInfo.text = "You try to open the drawers but you see that they are locked.\n" + 
                                        "You lack a tool to open it maybe you missed it somewhere..\n" +
                                        "[ Press R to Return ]";
                corrState = CorridorStates.drawers;
                bustedInfo = "Officer found empty cell and alerted others while you were trying to open officer room drawers.\n" +
                                "[ Press R to Retry ]";
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                corrState = CorridorStates.officer_3;
                lastCorrState = corrState;
                bustedInfo = "You've got yourself busted while searching officer room drawers.\n" +
                                "[ Press R to Retry ]";
            }
        }
        else if(corrState == CorridorStates.officer_3)
        {
            prisonStateInfo.text = "You've picked a drawers lock. You open it and take keys that may prove to be useful.\n" +
                                    "[ Press L to check Locker, S to check Shelf ]";

            if (Input.GetKeyDown(KeyCode.L))
            {
                prisonStateInfo.text = "You use keys to unlock locker. There you find a additional prison officer suit.\n" +
                                        "[ Press D to dress ]";
                corrState = CorridorStates.locker;

                bustedInfo = "You've got yourself busted while looking at the officer room locker.\n" +
                                "[ Press R to Retry ]";
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                prisonStateInfo.text = "The look up the shelf but you dont see anything useful.\n" +
                                        "[ Press R to Return ]";
                corrState = CorridorStates.shelf;
                bustedInfo = "Officer found empty cell and alerted others while you were looking at the shelf.\n" +
                                "[ Press R to Return ]";
            }

        }
        else if(corrState == CorridorStates.locker && lastCorrState == CorridorStates.officer_3)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                finished = true;
                finishInfo = "You've dressed as prison officer and walked out of a jail.\n" +
                                "Officers at the exit didin't recognized you as they were talking to each other.\n" +
                                "Congratulations! You are free... for now";
            }
        }

    }

    void onFinish()
    {
        prisonStateInfo.text = finishInfo;
    }

    void onBusted()
    {
        prisonStateInfo.text = bustedInfo;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reinitialize();
        }
    }

    void reportOnBathroom(string location)
    {
        if(bathroomBeenCount == 0)
        {
            prisonStateInfo.text = "You're in the " + location + ".\n" + 
                                    "It doesn't smell very well\n" + 
                                    "[ Press R to Return ]";
        }
        else if (bathroomBeenCount == 1)
        {
            prisonStateInfo.text = "You're again in the " + location + ".\n" + 
                                    "It smells horribly and that you already knew.\n" +
                                    "[ Press R to Return ]";
        }
        else if (bathroomBeenCount == 2)
        {
            prisonStateInfo.text = "You keep going to the " + location + " with no reason.\n" + 
                                    "Or maybe you think that you will see something new here...\n" +
                                    "[ Press R to Return ]";
        }
        else if (bathroomBeenCount >= 3)
        {
            prisonStateInfo.text = "Your favorite place - " + location + ".\n" + 
                                    "You've missed it so much and especially that smell.\n" +
                                    "[ Press R to Return ]";
        }
        bathroomBeenCount++;
    }

    void reportOnStairs(string location)
    {
        if (upstairsBeenCount == 0)
        {
            prisonStateInfo.text = "Stairs are quite long.\n" + 
                                    "You've climbed up to the " + location + ".\n" +
                                    "There's nothing interesting here\n" +
                                    "[ Press R to Return ]";
        }
        else if (upstairsBeenCount == 1)
        {
            prisonStateInfo.text = "You're again climbing up the " + location + ".\n" +
                                    "And yet this time also you've not find anything new.\n" +
                                    "[ Press R to Return ]";
        }
        else if (upstairsBeenCount == 2)
        {
            prisonStateInfo.text = "You're climbing to the " + location + "...\n" +
                                    "You sure are naive to thing that somethink have changed there...\n" +
                                    "[ Press R to Return ]";
        }
        else if (upstairsBeenCount == 3)
        {
            prisonStateInfo.text = "Your new hoby - climbing up and down the " + location + ".\n" +
                                    "Sports never do harm except that you feel exhausted...\n" +
                                     "[ Press R to Return ]";
        }
        else if(upstairsBeenCount >= 4)
        {
            prisonStateInfo.text = "You've lost count of how many times you've been up and down " + location + ".\n" +
                                    "It seems that you're getting mad with the stairs or just want to prove something...\n" +
                                    "Maybe it's time to stop? but would you listen to your own self?\n" +
                                    "[ Press R to Return ]";
        }
        upstairsBeenCount++;
    }


}
