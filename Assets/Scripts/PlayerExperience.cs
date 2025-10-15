using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Pengaturan Kenaikan Level")]
    public float xpMultiplier = 1.5f;

    public void AddXP(int xp)
    {
        currentXP += xp;
        Debug.Log("Dapat XP: " + xp + ". Total XP sekarang: " + currentXP + "/" + xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        // Gunakan variabel baru kita di sini, bukan angka mati
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        Debug.Log("LEVEL UP! Sekarang Level " + currentLevel);
        GameManager.instance.ShowLevelUpScreen();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExperienceOrb"))
        {
            int xpValue = other.GetComponent<ExperienceOrb>().xpValue;
            AddXP(xpValue);
            Destroy(other.gameObject);
        }
    }
}