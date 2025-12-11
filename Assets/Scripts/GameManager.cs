using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject levelUpScreen;

    [Header("Upgrade Settings")]
    public List<UpgradeData> upgradePool;
    public UpgradeButton[] upgradeButtons;

    //private List<UpgradeData> availableUpgrades;
    
    private GameObject player;
    private GameObject xp_orb;

    void Awake()
    {
        if (instance == null) instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        xp_orb = GameObject.FindGameObjectWithTag("ExperienceOrb");
    }

    public void ShowLevelUpScreen()
    {
        Time.timeScale = 0f;
        levelUpScreen.SetActive(true);

        // 1. Shuffle the list of upgrades randomly
        List<UpgradeData> availableUpgrades = upgradePool.OrderBy(x => Random.value).ToList();

        // 2. Loop through the buttons and assign the data
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < availableUpgrades.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                // This calls the method on the LevelUpButton script to swap the image
                upgradeButtons[i].SetUpgrade(availableUpgrades[i]);
            }
            else
            {
                // Hide button if we run out of upgrades
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
        //availableUpgrades = upgradePool.OrderBy(x => Random.value).ToList();

        //for (int i = 0; i < upgradeButtons.Length; i++)
        //{
        //    if (i < availableUpgrades.Count)
        //    {
        //        int upgradeIndex = i;
        //        upgradeButtons[i].onClick.RemoveAllListeners();
        //        upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgradeIndex));
        //    }
        //}
    }

    public void ApplyUpgrade(UpgradeData chosenUpgrade)
    {
        //UpgradeData chosenUpgrade = availableUpgrades[upgradeIndex];
        Debug.Log("Upgrade dipilih: " + chosenUpgrade.type);

        if (player == null)
        {
            Debug.LogError("Player tidak ditemukan! Pastikan Player memiliki Tag 'Player'.");
            HideLevelUpScreen();
            return;
        }

  
        switch (chosenUpgrade.type)
        {
            case UpgradeType.WeaponSpeed:
                player.GetComponent<AutoGun>().fireRate *= chosenUpgrade.value;
                break;
            case UpgradeType.WeaponDamage:
                player.GetComponent<AutoGun>().projectileDamage += (int)chosenUpgrade.value;
                break;
            case UpgradeType.PlayerSpeed:
                player.GetComponent<PlayerMovement>().moveSpeed *= chosenUpgrade.value;
                break;
            case UpgradeType.HP:
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    float healValue = playerHealth.maxHealth * chosenUpgrade.value - playerHealth.maxHealth;
                    playerHealth.maxHealth *= chosenUpgrade.value;
                    playerHealth.Heal(healValue);
                }
                break;
            case UpgradeType.XP_Gained:
                xp_orb.GetComponent<ExperienceOrb>().xpValue *= chosenUpgrade.value;
                break;
            case UpgradeType.WeaponProjectileCount:
                player.GetComponent<AutoGun>().projectileCount += (int)chosenUpgrade.value;
                break;
        }
        Debug.Log("Upgrade Sukses!");

        HideLevelUpScreen();
    }

    private void HideLevelUpScreen()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
    }


}