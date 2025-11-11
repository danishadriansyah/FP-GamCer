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
    public List<UpgradeData> upgradePool;
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeButtonTitles;
    public TextMeshProUGUI[] upgradeButtonDescs;

    private List<UpgradeData> availableUpgrades;

    
    private GameObject player; 

    void Awake()
    {
        if (instance == null) instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ShowLevelUpScreen()
    {
        Time.timeScale = 0f;
        levelUpScreen.SetActive(true);

        availableUpgrades = upgradePool.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < availableUpgrades.Count)
            {
                upgradeButtonTitles[i].text = availableUpgrades[i].upgradeName;
                upgradeButtonDescs[i].text = availableUpgrades[i].description;

                int upgradeIndex = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgradeIndex));
            }
        }
    }

    public void ApplyUpgrade(int upgradeIndex)
    {
        UpgradeData chosenUpgrade = availableUpgrades[upgradeIndex];
        Debug.Log("Upgrade dipilih: " + chosenUpgrade.upgradeName);

        if (player == null)
        {
            Debug.LogError("Player tidak ditemukan! Pastikan Player memiliki Tag 'Player'.");
            HideLevelUpScreen();
            return;
        }

  
        switch (chosenUpgrade.type)
        {
            case UpgradeType.WeaponSpeed:
                player.GetComponent<AutoGun>().fireRate *= 0.85f;
                break;
            case UpgradeType.PlayerSpeed:
                // Menambah kecepatan gerak
                player.GetComponent<PlayerMovement>().moveSpeed *= 1.1f;
                break;
            case UpgradeType.HP:
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 10;  
                }
                break;
        }

        HideLevelUpScreen();
    }

    private void HideLevelUpScreen()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
    }


}