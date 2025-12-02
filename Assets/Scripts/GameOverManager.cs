using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Navigasi")]
    [Tooltip("Ketik nama scene tujuan di sini (misal: MainMenuScene atau SampleScene)")]
    public string namaSceneTujuan = "MainMenuScene";

    void Update()
    {
        if (Input.anyKeyDown)
        {
            PindahScene();
        }
    }

    public void PindahScene()
    {
        // Pindah ke scene yang sudah kamu tentukan di Inspector
        if (!string.IsNullOrEmpty(namaSceneTujuan))
        {
            SceneManager.LoadScene(namaSceneTujuan);
        }
        else
        {
            Debug.LogWarning("Nama Scene Tujuan belum diisi di Inspector GameOverManager!");
        }
    }
}