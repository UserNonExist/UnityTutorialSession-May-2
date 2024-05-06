using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public List<GameObject> Enemies { get; set; }= new List<GameObject>();
    
    public bool isPaused = false;
    public bool isGameOver = false;
    public bool isGameWon = false;
    
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject gameWonMenu;
    
    public static float timeStamp = 0;
    
    public NavMeshSurface navMeshSurface;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        navMeshSurface.BuildNavMesh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        
        if (Enemies.Count == 0)
        {
            isGameWon = true;
        }

        if (isGameOver)
        {
            GameOver();
        }
        
        if (isGameWon)
        {
            GameWon();
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        timeStamp = Time.time;
        Time.timeScale = 1f;
        
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemies.Add(enemy);
        }
    }

    public void PauseGame()
    {
        if (isGameOver || isGameWon || pauseMenu.gameObject == null) return;
        
        isPaused = !isPaused;

        if (isPaused)
        {
            Timing.PauseCoroutines();
        }
        else
        {
            Timing.ResumeCoroutines();
        }
        
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
    
    public void GameOver()
    {
        isPaused = true;
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void GameWon()
    {
        isPaused = true;
        gameWonMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void RestartGame()
    {
        Timing.KillCoroutines();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReturnToMainMenu()
    {
        Timing.KillCoroutines();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Application.Quit();
    }
}
