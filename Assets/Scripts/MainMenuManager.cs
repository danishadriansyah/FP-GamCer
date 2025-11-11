using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // JANGAN LUPA TAMBAHIN INI

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI totalCurrencyText; // Seret UI Text buat nampilin duit

    void Start()
    {
        // Pastikan CurrencyManager udah siap
        if (CurrencyManager.instance != null)
        {
            // Tampilkan total duit yang kita punya
            totalCurrencyText.text = "Total Duit: " + CurrencyManager.instance.GetTotalCurrency();
        }
    }

    public void PlayGame()
    {
        // Kasih tau CurrencyManager kita mau mulai run baru
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.StartNewRun();
        }
        
        SceneManager.LoadScene("SampleScene"); 
    }
}