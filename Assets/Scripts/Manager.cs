using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    public int Score { get; private set; }
    public int RivalScore { get; private set; }

    public GameObject UI;

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
        UpdateUIScore("Score", "Score:" + Score);
    }

    public void AddRivalScore(int num)
    {
        RivalScore += num;
        UpdateUIScore("RivalScore", "RivalScore:" + RivalScore);

    }

    public void UpdateUIScore(string key, string newVal)
    {
        Transform[] children = UI.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.name == key)
            {
                child.gameObject.GetComponentInChildren<TMP_Text>().text = newVal;
            }
        }

    }
}
