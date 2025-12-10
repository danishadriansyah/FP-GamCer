using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Pengaturan Kenaikan Level")]
    public float xpMultiplier = 1.5f;

    // Referensi ke sistem darah
    private HealthSystem healthSystem;

    void Start()
    {
        // Cari komponen HealthSystem di object ini (Player)
        healthSystem = GetComponent<HealthSystem>();
    }

    public void AddXP(int xp)
    {
        currentXP += xp;
        // Debug.Log("Dapat XP: " + xp + ". Total XP sekarang: " + currentXP + "/" + xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        Debug.Log("LEVEL UP! Sekarang Level " + currentLevel);

        // --- FITUR BARU: HEAL 50% SAAT NAIK LEVEL ---
        if (healthSystem != null)
        {
            // Hitung 50% dari Max Health
            int healAmount = Mathf.RoundToInt(healthSystem.maxHealth * 0.3f);

            // Lakukan Healing
            healthSystem.Heal(healAmount);

            Debug.Log("Level Up Bonus: Player dipulihkan sebesar " + healAmount + " HP!");
        }
        // --------------------------------------------

        GameManager.instance.ShowLevelUpScreen();
    }
}