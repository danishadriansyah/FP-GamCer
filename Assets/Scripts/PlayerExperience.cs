using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("Pengaturan Kenaikan Level")]
    public float xpMultiplier = 1.5f;

    // Fungsi ini sekarang akan dipanggil oleh Orb
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
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        Debug.Log("LEVEL UP! Sekarang Level " + currentLevel);
        GameManager.instance.ShowLevelUpScreen();
    }

    // Fungsi OnTriggerEnter2D untuk Orb SUDAH DIHAPUS
    // Player tidak lagi mendeteksi Orb, Orb yang akan mendeteksi Player
}