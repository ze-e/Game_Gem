using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }

    public GameObject player;

    public int Score { get; private set; }
    public int RivalScore { get; private set; }

    public GameObject UI;
    public GameObject Text;

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

    public GameObject GetEquipmentUI()
    {
        Transform[] children = UI.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.name == "Equipment")
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public void UpdateEquip(string[] equipment, string equipped)
    {
        GameObject textObj = GetEquipmentUI();
        Transform[] textChildren = textObj.GetComponentsInChildren<Transform>();

        foreach (var child in textChildren)
        {
            var _name = child.gameObject.name;
            TMP_Text _text = child.gameObject.GetComponentInChildren<TMP_Text>();

            if (Array.IndexOf(equipment, _name) == -1)
            {
                _text.color = Color.grey;
            }
            
            else _text.color = Color.white;

            if (_name == equipped)
            {
                _text.color = Color.cyan;
            }
        }
    }

    public void UpdateEquipOne(string[] equipment, string equipped)
    {
        GameObject textObj = GetEquipmentUI();

        Transform[] textChildren = textObj.GetComponentsInChildren<Transform>();
        foreach (var child in textChildren)
        {
            var _name = child.gameObject.name;
            TMP_Text _text = child.gameObject.GetComponentInChildren<TMP_Text>();

            if (Array.IndexOf(equipment, _name) == -1)
            {
                _text.color = Color.grey;
            }
            
            else _text.color = Color.white;


            if (_name == equipped )
            {
                _text.color = Color.cyan;
            }
            return;
        }
    }

    public void ShowText(Transform _transform, string _text, Color _color)
    {
        var gameObject = Instantiate(Text, _transform.transform.position, Quaternion.identity);
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.name == "text")
            {
                var textObj = child.gameObject.GetComponentInChildren<TMP_Text>();
                textObj.text = _text;
                textObj.color = _color;
            }
        }
    }
}
