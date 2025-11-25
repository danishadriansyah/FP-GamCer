using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Konfigurasi Scene")]
    [Tooltip("Ketik nama scene game kamu di sini (misal: SampleScene)")]
    public string namaSceneGame;

    void Update()
    {
        // Mendeteksi tekanan tombol keyboard atau klik mouse apa saja
        if (Input.anyKeyDown)
        {
            MulaiGame();
        }
    }

    void MulaiGame()
    {
        // 1. Reset uang sesi (Penting biar duitnya mulai dari 0 lagi pas main)
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.StartNewRun();
        }

        // 2. Pindah Scene sesuai nama yang ditulis di Inspector
        if (!string.IsNullOrEmpty(namaSceneGame))
        {
            SceneManager.LoadScene(namaSceneGame);
        }
        else
        {
            Debug.LogError("Waduh! Nama Scene belum diisi di Inspector 'MainMenuManager'!");
        }
    }
}