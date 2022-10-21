using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    public string playerName;

    public string TopPlayer;
    public int highScore;

    public static MainManager Instance;

    private void Awake()
    { if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSave();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space) && GameObject.Find("Name Input") == null)
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                m_Started = false;
                m_GameOver = false;
                m_Points = 0;
            }
        }

        if (GameObject.Find("Name Input") != null && playerName == "")
        {
            //playerName = GameObject.Find("Name Input")
               // .transform.Find("Text Area")
                //.transform.Find("Text")
                //.GetComponent<TextMeshProUGUI>().text;
        }
    }

    private void InitializeBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void StartGame()
    {
        playerName = GameObject.Find("Name Input")
            .transform.Find("Text Area")
            .transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        SceneManager.LoadScene(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeBricks();
        //Ball
        Ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        //scoreText text
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        HighScoreText = GameObject.Find("High Score Text").GetComponent<Text>();
        string topPlayer = TopPlayer != "" ? TopPlayer : "Tom";
        HighScoreText.text = $"Best Score: {topPlayer}: {highScore}";
        //gameOverText gameobject
        GameOverText = GameObject.Find("Canvas").transform.Find("GameoverText").gameObject;
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if(m_Points > highScore)
        {
            highScore = m_Points;
            TopPlayer = playerName;
        }
        DataSave();
    }

    [System.Serializable]
    public class SaveData
    {
        public string TopPlayer;
        public int HighScore;
    }

    public void LoadSave()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.HighScore;
            TopPlayer = data.TopPlayer;
        }
    }

    public void DataSave()
    {
        SaveData data = new SaveData();
        data.TopPlayer = TopPlayer;
        data.HighScore = highScore;
        
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

}
