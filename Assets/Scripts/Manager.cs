using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public int Score { get; private set; }
    public int RivalScore { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Score = 0;
        RivalScore = 0;    
    }

    public void AddScore(int num)
    {
        Score += num;
    }

    public void AddRivalScore(int num)
    {
        RivalScore += num;
    }
}
