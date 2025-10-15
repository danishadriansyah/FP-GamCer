using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject levelUpScreen;

    [Header("Upgrade Settings")]
    public List<UpgradeData> upgradePool; // Pastikan ini menggunakan UpgradeData
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeButtonTitles;
    public TextMeshProUGUI[] upgradeButtonDescs;

    private List<UpgradeData> availableUpgrades; // Pastikan ini menggunakan UpgradeData

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void ShowLevelUpScreen()
    {
        Time.timeScale = 0f;
        levelUpScreen.SetActive(true);

        availableUpgrades = upgradePool.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeButtonTitles[i].text = availableUpgrades[i].upgradeName;
            upgradeButtonDescs[i].text = availableUpgrades[i].description;

            int upgradeIndex = i;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgradeIndex));
        }
    }

    public void ApplyUpgrade(int upgradeIndex)
    {
        UpgradeData chosenUpgrade = availableUpgrades[upgradeIndex]; // Pastikan ini menggunakan UpgradeData
        Debug.Log("Upgrade dipilih: " + chosenUpgrade.upgradeName);

        // --- LOGIKA PENERAPAN UPGRADE AKAN DITARUH DI SINI ---

        HideLevelUpScreen();
    }

    private void HideLevelUpScreen()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
    }
}