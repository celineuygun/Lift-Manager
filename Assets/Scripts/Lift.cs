using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lift : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] int numberOfLevels = 5;

    public SortedList levelIndexes;
    public int nextLevel = 0, currLevel = 0;
    Slider levelSlider;
    public bool isGoingUp = true;
    bool isRunning = false, isInside = false;

    private void Start()
    {
        levelSlider = GetComponent<Slider>();
        levelIndexes = new();
    }

    void FixedUpdate()
    {
        if (levelIndexes.Count > 0)
        {
            if (nextLevel != -1)
            {
                if (currLevel <= nextLevel) isGoingUp = true;
                else isGoingUp = false;
            }
            TryLaunchCoroutine();
        }
    }

    public int GetNextLevel(int key)
    {
        int index = levelIndexes.IndexOfKey(key);
        if (nextLevel != -1 && levelIndexes.Count <= 1)
            return -1;
        else if (levelIndexes.Count == 1)
            return (int)levelIndexes.GetKey(0);
        else if (isGoingUp && index + 1 < levelIndexes.Count)
            return (int)levelIndexes.GetKey(index + 1);
        else if (!isGoingUp && index - 1 >= 0)
            return (int)levelIndexes.GetKey(index - 1);


        isGoingUp = !isGoingUp;
        return GetNextLevel(key);
    }

    void TryLaunchCoroutine()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(Work());
        }
    }

    IEnumerator Work()
    {
        yield return new WaitForSecondsRealtime(.01f);
        if (nextLevel != -1 && levelSlider.value < nextLevel * (levelSlider.maxValue / numberOfLevels) && levelSlider.value != levelSlider.maxValue)
            levelSlider.value++;
        else if (nextLevel != -1 && levelSlider.value > nextLevel * (levelSlider.maxValue / numberOfLevels) && levelSlider.value != 0)
            levelSlider.value--;
        else // if arrived
        {
            GetDistance(player);
            yield return new WaitForSecondsRealtime(1f);
            if (nextLevel != -1) // while working
            {
                currLevel = nextLevel;
                nextLevel = GetNextLevel(nextLevel);
            }
            else // on stop
            {
                AddNewLevelIndex(currLevel);
                nextLevel = GetNextLevel(currLevel);
                levelIndexes.Remove(currLevel);
            }
            if (currLevel != -1 && levelIndexes.ContainsKey(currLevel))
                levelIndexes.Remove(currLevel);

        }
        isRunning = false;
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (isInside && e.isKey)
        {
            int keyCode = e.keyCode.GetHashCode();
            if (keyCode >= KeyCode.Alpha0.GetHashCode() && keyCode <= KeyCode.Alpha9.GetHashCode())
            {
                int key = keyCode - KeyCode.Alpha0.GetHashCode();
                AddNewLevelIndex(key);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //AddNewLevelIndex(player.GetCurrentLevel());
            isInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isInside = false;
            //player.SetCurrentLevel(GetCurrentLevel());
        }
    }

    public int GetLevel()
    {
        if (nextLevel != -1)
            return nextLevel;
        return currLevel;
    }

    public bool IsMoving()
    {
        return !(nextLevel == -1 || levelIndexes.Count == 0 || nextLevel == currLevel);
    }

    public int GetDistance(PlayerMovement player)
    {
        int dist = Math.Abs(player.GetCurrentLevel() - GetLevel());
        if (nextLevel == -1 || levelIndexes.Count == 0
                || (isGoingUp && GetLevel() <= player.GetCurrentLevel())
                || (!isGoingUp && GetLevel() >= player.GetCurrentLevel()))
            return dist;

        if (isGoingUp)
        {
            int highestLevel = (int)levelIndexes.GetKey(levelIndexes.Count - 1);
            return dist + highestLevel * 2;
        }
        int lowestLevel = (int)levelIndexes.GetKey(0);
        return dist + lowestLevel * 2;
    }

    public void AddNewLevelIndex(int level)
    {
        if (!levelIndexes.ContainsKey(level))
        {
            levelIndexes.Add(level, level);
        }
    }
}
