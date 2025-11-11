using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    public int totalCurrency { get; private set; }
    public int sessionCurrency { get; private set; }

    private const string CURRENCY_SAVE_KEY = "TotalPlayerCurrency";

    void Awake()
    {
        // --- Setup Singleton & DontDestroyOnLoad ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Jika udah ada, hancurkan yang baru ini
            Destroy(gameObject);
            return;
        }
        // --- Akhir Setup ---

        // Langsung load total duit dari "brankas"
        LoadTotalCurrency();
    }

    private void LoadTotalCurrency()
    {
        // Ambil data dari PlayerPrefs, kalo nggak ada, nilainya 0
        totalCurrency = PlayerPrefs.GetInt(CURRENCY_SAVE_KEY, 0);
    }

    public void StartNewRun()
    {
        // Reset duit sesi ini pas mulai main
        sessionCurrency = 0;
    }

    public void AddSessionCurrency(int amount)
    {
        // Tambah duit yang didapet di run ini
        sessionCurrency += amount;
    }

    public void SaveSessionToTotal()
    {
        // Tambah duit sesi ini ke total duit
        totalCurrency += sessionCurrency;

        // Simpen total duit baru ke "brankas" PlayerPrefs
        PlayerPrefs.SetInt(CURRENCY_SAVE_KEY, totalCurrency);
        PlayerPrefs.Save();
    }

    public int GetTotalCurrency()
    {
        return totalCurrency;
    }

    public int GetSessionCurrency()
    {
        return sessionCurrency;
    }
}