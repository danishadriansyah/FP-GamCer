using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Konfigurasi Scene")]
    [Tooltip("Ketik nama scene game kamu di sini")]
    public string namaSceneGame = "SampleScene";

    [Header("UI References")]
    [Tooltip("Masukkan semua UI Menu Utama (Judul, Tombol Start, dll) ke sini")]
    public GameObject mainMenuUI;

    [Tooltip("Masukkan Panel Instruksi yang baru kamu buat ke sini")]
    public GameObject instructionUI;

    // Penanda status: Apakah kita sedang melihat instruksi?
    private bool isShowingInstructions = false;

    void Start()
    {
        // Pastikan saat mulai: Menu NYALA, Instruksi MATI
        if (mainMenuUI != null) mainMenuUI.SetActive(true);
        if (instructionUI != null) instructionUI.SetActive(false);
    }

    void Update()
    {
        // Deteksi jika pemain menekan tombol apapun
        if (Input.anyKeyDown)
        {
            if (!isShowingInstructions)
            {
                // TAHAP 1: Kalau belum lihat instruksi, tampilkan instruksi dulu
                ShowInstructions();
            }
            else
            {
                // TAHAP 2: Kalau sedang lihat instruksi, baru masuk game
                StartGame();
            }
        }
    }

    void ShowInstructions()
    {
        isShowingInstructions = true;

        // Tukar tampilan: Matikan Menu, Nyalakan Instruksi
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (instructionUI != null) instructionUI.SetActive(true);
    }

    void StartGame()
    {
        // Reset Uang Sesi (Penting!)
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.StartNewRun();
        }

        // Pindah Scene
        if (!string.IsNullOrEmpty(namaSceneGame))
        {
            SceneManager.LoadScene(namaSceneGame);
        }
        else
        {
            Debug.LogError("Nama Scene belum diisi di Inspector!");
        }
    }
}