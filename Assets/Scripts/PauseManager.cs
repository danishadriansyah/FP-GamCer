using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI; // Masukkan Panel Pause Menu ke sini

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenuScene"; // Nama scene menu utama

    private bool isPaused = false;

    void Start()
    {
        // Pastikan menu pause tertutup saat game mulai
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Deteksi tombol ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Sembunyikan UI
        Time.timeScale = 1f;          // Waktu jalan normal
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Munculkan UI
        Time.timeScale = 0f;          // Waktu beku (STOP)
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f; // PENTING: Kembalikan waktu sebelum reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // PENTING: Kembalikan waktu sebelum pindah
        SceneManager.LoadScene(mainMenuScene);
    }
}