using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LiftManager : MonoBehaviour
{
    [SerializeField] Lift lift1, lift2;
    [SerializeField] PlayerMovement player1, player2;
    PlayerMovement player;
    bool callingUp = false, callingDown = false;

    void FixedUpdate()
    {
        if (callingDown || callingUp)
            CallLift();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.U)
            {
                player = player1;
                callingUp = true;
            }
            else if (e.keyCode == KeyCode.J)
            {
                player = player1;
                callingDown = true;
            } else if(e.keyCode == KeyCode.Y)
            {
                player = player2;
                callingUp = true;
            }
            else if (e.keyCode == KeyCode.H)
            {
                player = player2;
                callingDown = true;
            }
        }
    }

    void CallLift()
    {
        bool lift1Moving = lift1.IsMoving(), lift2Moving = lift2.IsMoving();

        if ((lift1.GetLevel() == player.GetCurrentLevel()) || (lift2.GetLevel() == player.GetCurrentLevel()))
        {
            callingUp = false;
            callingDown = false;
            return;
        }

        if (callingUp)
        {
            callingUp = false;

            if (lift1Moving == lift2Moving && lift1.isGoingUp == lift2.isGoingUp)
            {
                if (!lift1Moving || (lift1.GetLevel() < player.GetCurrentLevel() && lift2.GetLevel() < player.GetCurrentLevel())     // both above or
                    || (lift1.GetLevel() > player.GetCurrentLevel() && lift2.GetLevel() > player.GetCurrentLevel()))                 // both below and going to the same dir
                                                                                                                                     // or aren't moving
                {
                    CallNearestLift();
                }
            }

            else if ((lift1Moving && lift1.isGoingUp && lift1.GetLevel() < player.GetCurrentLevel())
                    || (lift2Moving && lift2.isGoingUp && lift2.GetLevel() < player.GetCurrentLevel())) // below and going up
            {
                if (lift1Moving && lift1.isGoingUp && lift1.GetLevel() < player.GetCurrentLevel())
                    lift1.AddNewLevelIndex(player.GetCurrentLevel());
                else
                    lift2.AddNewLevelIndex(player.GetCurrentLevel());
            }

            else if (!lift1.IsMoving() || !lift2.IsMoving()) // if one of them is not moving
            {
                if (!lift1Moving)
                {
                    if (NearestLift() == lift2)
                        ChangeNextLocation(lift2, player.GetCurrentLevel());
                    else
                        lift1.AddNewLevelIndex(player.GetCurrentLevel());
                }
                else
                {
                    if (NearestLift() == lift1)
                        ChangeNextLocation(lift1, player.GetCurrentLevel());
                    else
                        lift2.AddNewLevelIndex(player.GetCurrentLevel());
                }
            }

            else
                callingUp = true;
        }


        if (callingDown)
        {
            callingDown = false;

            if (lift1Moving == lift2Moving && lift1.isGoingUp == lift2.isGoingUp)
            {
                if (!lift1Moving || (lift1.GetLevel() > player.GetCurrentLevel() && lift2.GetLevel() > player.GetCurrentLevel())     // both above or
                    || (lift1.GetLevel() < player.GetCurrentLevel() && lift2.GetLevel() < player.GetCurrentLevel()))                 // both below and going to the same dir
                                                                                                                                     // or aren't moving
                {
                    CallNearestLift();
                }
            }

            else if ((lift1Moving && !lift1.isGoingUp && lift1.GetLevel() > player.GetCurrentLevel())
                    || (lift2Moving && !lift2.isGoingUp && lift2.GetLevel() > player.GetCurrentLevel())) // above and going down
            {
                if (lift1Moving && !lift1.isGoingUp && lift1.GetLevel() > player.GetCurrentLevel())
                    lift1.AddNewLevelIndex(player.GetCurrentLevel());
                else
                    lift2.AddNewLevelIndex(player.GetCurrentLevel());
            }

            else if (!lift1.IsMoving() || !lift2.IsMoving()) // if one of them is not moving
            {
                if (!lift1Moving)
                {
                    if (NearestLift() == lift2) // above going up
                        ChangeNextLocation(lift2, player.GetCurrentLevel());
                    else
                        lift1.AddNewLevelIndex(player.GetCurrentLevel());
                }
                else
                {
                    if (NearestLift() == lift1)
                        ChangeNextLocation(lift1, player.GetCurrentLevel());
                    else
                        lift2.AddNewLevelIndex(player.GetCurrentLevel());
                }
            }

            else
                callingDown = true;
        }

    }

    Lift NearestLift()
    {
        int lift1Distance = lift1.GetDistance(player), lift2Distance = lift2.GetDistance(player);
        if (lift1Distance > lift2Distance)
            return lift2;
        return lift1;
    }

    void CallNearestLift()
    {
        Lift nearestLift = NearestLift();
        nearestLift.AddNewLevelIndex(player.GetCurrentLevel());
    }

    void ChangeNextLocation(Lift lift, int level)
    {
        int old = lift.GetLevel();
        lift.nextLevel = level;
        lift.AddNewLevelIndex(old);
    }
}
