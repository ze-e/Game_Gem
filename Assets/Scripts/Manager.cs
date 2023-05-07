using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }
    enum GameState { Play, Pause, GameOver }
    GameState currentState; 
    public GameObject player;
    /* DB */
    public static List<GemScrObj> GEM_DB;
    [SerializeField]
    private string GEM_DB_PATH = default;

    public int Score { get; private set; }
    public int RivalScore { get; private set; }



    /* Random Gen */
    public GameObject rivalprefab;
    public GameObject ghostPrefab;
    public int ghostCount;

    /* Items */
    public GameObject[] itemPrefabs;

    /* Timer */
    [Range(1, 1000)]
    public int rivalSpawnTimer = 10;
    [Range(1, 1000)]
    public int ghostSpawnTimer = 5;
    [Range(1, 1000)]
    public int itemSpawnTimer = 30;
    private int timer = 10000;
    private int currentTime;

    /* UI */
    public GameObject UI;
    public GameObject Text;
    [SerializeField]
    GameObject GameOverUI;

    /* Anim */
    public GameObject bloodPrefab;
    public GameObject deathPrefab;


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
        currentTime = 0;
        GameOverUI.SetActive(false);
        currentState = GameState.Play;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            switch (currentState)
            {
                case GameState.Play:
                    currentState = GameState.Pause;
                    Time.timeScale = 0f;
                    break;
                case GameState.Pause:
                    currentState = GameState.Play;
                    Time.timeScale = 1f;
                    break;
                case GameState.GameOver:
                    RestartGame();
                    break;
                default:
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        currentTime++;

        if (currentTime > timer)
        {
            currentTime = 0;
        }

        if(currentTime % formatTime(rivalSpawnTimer) == 0)
        {
            Spawn(rivalprefab);
        }

        if (currentTime % formatTime(ghostSpawnTimer) == 0 && ghostCount > 0)
        {
            for (int i = 0; i < ghostCount; i++)
            {
                Spawn(ghostPrefab);
                ghostCount--;
            }
        }

        if (currentTime % formatTime(ghostSpawnTimer) == 0 && ghostCount > 0)
        {
            for (int i = 0; i < ghostCount; i++)
            {
                Spawn(ghostPrefab);
                ghostCount--;
            }
        }

        if (currentTime % formatTime(itemSpawnTimer) == 0)
        {
                SpawnItem();
        }
    }

    public void GameOver()
    {
        GameOverUI.SetActive(true);
        currentState = GameState.GameOver;
    }

    void RestartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
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

    void SpawnItem()
    {
        // Get the camera's position and size
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Calculate the bounds of the camera view
        float leftBound = cameraPosition.x - cameraWidth / 2f;
        float rightBound = cameraPosition.x + cameraWidth / 2f;
        float bottomBound = cameraPosition.y - cameraHeight / 2f;
        float topBound = cameraPosition.y + cameraHeight / 2f;

        // Generate a random position within the camera view
        Vector3 randomPosition = new Vector3(Random.Range(leftBound, rightBound), Random.Range(bottomBound, topBound), 0f);

        // Spawn the item at the random position

        int chosenItem = Random.Range(0, itemPrefabs.Length);
        if (chosenItem < itemPrefabs.Length)
        {
            Instantiate(itemPrefabs[chosenItem], randomPosition, Quaternion.identity);
        }
    }

    public void AddGhost()
    {
        ghostCount++;
    }

    #endregion

    public void AddScore(int num)
    {
        Score += num;
        UpdateUI("Score", Score.ToString());
    }

    public void AddRivalScore(int num)
    {
        RivalScore += num;
        UpdateUI("RivalScore", RivalScore.ToString());
    }

    public void UpdateUI(string key, string newVal)
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


#region util
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
                textObj.color = new Color(_color.r, _color.g, _color.b, 1) ;
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

    public int formatTime(int time)
    {
        return time * 100;
    }

    public void BloodAnim(Vector3 pos)
    {
        Instantiate(bloodPrefab, pos, Quaternion.identity);
    }

    public void DeathAnim(Vector3 pos)
    {
        Instantiate(deathPrefab, pos, Quaternion.identity);
    }


    #endregion
}
