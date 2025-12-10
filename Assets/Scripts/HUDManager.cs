using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Target Player")]
    public HealthSystem playerHealth;
    public PlayerExperience playerExp;

    [Header("UI Components")]
    public Slider healthSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;

    void Start()
    {
        if (playerHealth != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                playerExp = player.GetComponent<PlayerExperience>();
            }
            {
                Debug.LogError("HUD: Player tidak ditemukan!");
            }
        }
    }

    void Update()
    {
        if (playerHealth == null || playerExp == null) return;

        UpdateHealthBar();
        UpdateXPBar();

        if (AIDirector.instance != null)
        {
            float timeElapsed = AIDirector.instance.gameTime;

            int minutes = Mathf.FloorToInt(timeElapsed / 60F);
            int seconds = Mathf.FloorToInt(timeElapsed % 60F);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateHealthBar()
    {
        float hpRatio = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        healthSlider.value = hpRatio;
    }

    void UpdateXPBar()
    {
        if (playerExp.xpToNextLevel > 0)
        {
            float xpRatio = (float)playerExp.currentXP / playerExp.xpToNextLevel;
            xpSlider.value = xpRatio;
        }
        else
        {
            xpSlider.value = 0;
        }
        levelText.text = "LVL " + playerExp.currentLevel.ToString();
    }
}