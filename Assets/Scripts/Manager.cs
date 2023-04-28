using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }

    public GameObject player;

    /* DB */
    public static List<GemScrObj> GEM_DB;
    [SerializeField]
    private string GEM_DB_PATH = default;

    public int Score { get; private set; }
    public int RivalScore { get; private set; }

    public GameObject UI;
    public GameObject Text;

    /* Random Gen */
    public GameObject rivalprefab;
    public GameObject ghostPrefab;
    int ghostCount;

    /* Timer */
    public float timer = 100f;
    public float ghostTimer = 10f;
    private float currentTime;

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
        CreateGemDB();
        currentTime = timer;
    }

    private void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = timer;
        }

        if(currentTime <= 0)
        {
            Spawn(rivalprefab);
        }

        if (currentTime % ghostTimer == 0 && ghostCount > 0)
        {
            Spawn(ghostPrefab);
            ghostCount--;
        }
    }

    #region db
    void CreateGemDB()
    {
        Resources.LoadAll<GemScrObj>(GEM_DB_PATH);
    }

    public GemScrObj FindGem(GemType _gemType)
    {
        foreach (var _gemData in GEM_DB)
        {
            if(_gemData.gemType == _gemType)
            {
                return _gemData;
            }
        }
        return null;
    }

    #endregion

    #region spawmn
    void Spawn(GameObject objToSpawn)
    {
        Camera mainCamera = Camera.main;
        Vector3 randomPoint = Vector3.zero;
        float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHeight = mainCamera.orthographicSize;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // top
                randomPoint = new Vector3(Random.Range(-cameraWidth, cameraWidth), cameraHeight , 0);
                break;
            case 1: // bottom
                randomPoint = new Vector3(Random.Range(-cameraWidth, cameraWidth), -cameraHeight , 0);
                break;
            case 2: // left
                randomPoint = new Vector3(-cameraWidth , Random.Range(-cameraHeight, cameraHeight), 0);
                break;
            case 3: // right
                randomPoint = new Vector3(cameraWidth , Random.Range(-cameraHeight, cameraHeight), 0);
                break;
        }
        
        Instantiate(objToSpawn, randomPoint, Quaternion.identity);
    }

    public void AddGhost()
    {
        ghostCount++;
    }

    #endregion

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

    public void RaiseOpacity(GameObject gameObject, float val, float maxVal)
    {

        // Reduce opacity of SpriteRenderer based on remaining health
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        float opacity = val / maxVal;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Abs(opacity));
    }

    public void ReddenSprite(GameObject gameObject, float val, float maxVal)
    {

        // Darken SpriteRenderer based on remaining health
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        float n = val / maxVal;
        spriteRenderer.color = new Color(spriteRenderer.color.r * n, spriteRenderer.color.g, spriteRenderer.color.b);
    }
}
