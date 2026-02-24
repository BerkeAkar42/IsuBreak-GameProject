using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Pause menü paneli
    public GameObject player;      // Player objesini buraya sürükle
    private PlayerMove playerMove; // PlayerMove scriptine referans
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        // PlayerMove scriptini al
        if (player != null)
            playerMove = player.GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Oyun duraklatıldıysa, imleci sürekli aktif tut
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void AnaMenuyeDon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // İmleç aktif
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // PlayerMove scriptini devre dışı bırak
        if (playerMove != null)
            playerMove.enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // İmleç gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // PlayerMove scriptini yeniden etkinleştir
        if (playerMove != null)
            playerMove.enabled = true;
    }
}
