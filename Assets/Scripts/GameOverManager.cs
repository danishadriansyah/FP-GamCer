using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // JANGAN LUPA TAMBAHIN INI

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI sessionCurrencyText; // Text buat duit run ini
    public TextMeshProUGUI totalCurrencyText;   // Text buat total duit

    void Start()
    {
        // Pastikan CurrencyManager ada
        if (CurrencyManager.instance != null)
        {
            // Tampilkan duit yang didapet di run ini
            sessionCurrencyText.text = "Duit Didapat: " + CurrencyManager.instance.GetSessionCurrency();

            // Tampilkan total duit sekarang
            totalCurrencyText.text = "Total Duit: " + CurrencyManager.instance.GetTotalCurrency();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}