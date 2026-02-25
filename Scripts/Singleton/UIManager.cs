using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager is NULL");
            }

            return _instance;
        }
    }

    [SerializeField]
    private Text _scoreText;
    private int _score = 0;

    [SerializeField]
    private Text _killedText;
    private int _maxCount;
    private int _left = 0;
    private int _killed = 0;

    [SerializeField]
    private float loseIfMoreThanPercent = 50f;
    private int _enemyEscaped = 0;
    [SerializeField]
    private Text _enemyEscapedText;

    private bool _stopTheGame = false;

    [SerializeField]
    private float _totalTime = 360f;
    private float _startTime;
    [SerializeField]
    private bool _timerIsRunning = false;
    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Text _enemyLeftText;

    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartTimer();
        _scoreText.text = "Player Score: " + 0;
        _enemyEscapedText.text = "Enemy escape: " + 0;
    }

    private void Update()
    {
        if (_stopTheGame == true)
        {
            StopTimer();

            float percent = (_enemyEscaped * 100f) / _maxCount;

            if (percent >= loseIfMoreThanPercent)
            {
                GameManager.Instance.YouLose();
            }
            else
            {
                GameManager.Instance.YouWin();
            }
        }

        if (!_timerIsRunning)
        return;

        float elapsed = Time.time - _startTime;
        float timeRemaining = _totalTime - elapsed;

        if (timeRemaining > 0)
        {
            DisplayTime(timeRemaining);
        }
        else
        {
            DisplayTime(0);
            _timerIsRunning = false;
            SpawnManager.Instance.StopSpawning();
            GameManager.Instance.GameOver();
        }
    }

    public void StartTimer()
    {
        _startTime = Time.time;
        _timerIsRunning = true;
    }
    public void StopTimer()
    {
        _timerIsRunning = false;
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        _timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }



    public void UpdateScore(int score)
    {
        _score += score;
        _scoreText.text = "Player Score: " + _score;
    }

    public void EnemyCount(int count)
    {
        _maxCount = count;

        _killedText.text = "Enemy Killed: " + _killed + " / " + _maxCount;

        EnemyLeft();
    }

    public void EnemyKilled(int killed)
    {
        _killed += killed;
        _killedText.text = "Enemy Killed: " + _killed + " / " + _maxCount;

        EnemyLeft();
    }

    public void EnemyEscaped()
    {
        _enemyEscaped++;
        _enemyEscapedText.text = "Enemy escape: " + _enemyEscaped;

        EnemyLeft();
    }

    public void EnemyLeft()
    {
        _left = _maxCount;

        _left -= (_killed + _enemyEscaped);

        _enemyLeftText.text = "Enemy Left: " + _left;

        if (_left <= 0)
        {
            _stopTheGame = true;
        }
    }
}
