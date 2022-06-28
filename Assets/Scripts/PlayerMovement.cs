using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    int currentLevel = 0;
    [SerializeField] string control = "Horizontal";

    void Update()
    {
        float steerAmount = Input.GetAxis(control) * moveSpeed * Time.deltaTime;
        transform.Translate(steerAmount, 0, 0);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Level1")
        {
            SetCurrentLevel(0);
        }
        else if (other.gameObject.tag == "Level2")
        {
            SetCurrentLevel(1);
        }
        else if (other.gameObject.tag == "Level3")
        {
            SetCurrentLevel(2);
        }
        else if (other.gameObject.tag == "Level4")
        {
            SetCurrentLevel(3);
        }
        else if (other.gameObject.tag == "Level5")
        {
            SetCurrentLevel(4);
        }else if (other.gameObject.tag == "Level6")
        {
            SetCurrentLevel(5);
        }
    }
}
