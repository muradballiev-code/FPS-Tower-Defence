using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private GameObject _gameOverObj;
    [SerializeField]
    private GameObject _loseObj;
    [SerializeField]
    private GameObject _winObj;
    [SerializeField]
    private Button _gameRest;

    private void Start()
    {
        _gameOverObj.gameObject.SetActive(false);
        _loseObj.gameObject.SetActive(false);
        _winObj.gameObject.SetActive(false);
        _gameRest.gameObject.SetActive(false);
    }

    public void YouWin()
    {
        _winObj.gameObject.SetActive(true);
        _gameRest.gameObject.SetActive(true);
    }

    public void YouLose()
    {
        _loseObj.gameObject.SetActive(true);
        _gameRest.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        _gameOverObj.gameObject.SetActive(true);
        _gameRest.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene("GLI_1");
        SceneManager.LoadScene(0);
    }
}
