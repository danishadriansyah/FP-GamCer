using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    private HealthSystem health;

    void Start()
    {
        health = GetComponent<HealthSystem>();
    }

    void Update()
    {
        // Cek manual karena HealthSystem.cs mungkin pakai event
        if (health != null && health.currentHealth <= 0)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("VICTORY! Boss Kalah.");
        // Simpan progress jika perlu
        if (CurrencyManager.instance != null) CurrencyManager.instance.SaveSessionToTotal();

        // Pindah ke Scene Menang (Bisa pakai GameOverScene tapi ubah teksnya nanti)
        SceneManager.LoadScene("GameOverScene");
    }
}