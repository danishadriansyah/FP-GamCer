using UnityEngine;
using System; // Diperlukan untuk Action
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Data")]
    public GameObject xpOrbPrefab;
    public int maxHealth;
    private int currentHealth;

    [Header("Status")]
    public bool isPlayer = false; // Tandai ini di Inspector jika script ini untuk Player

    // --- SALURAN SIARAN (EVENTS) ---
    // Siaran saat Player kena damage
    public static event Action<int> OnPlayerDamaged;
    // Siaran saat Musuh terbunuh
    public static event Action OnEnemyKilled;

    void Start()
    {
        // Cek jika ini adalah Player
        if (gameObject.CompareTag("Player"))
        {
            isPlayer = true;
        }
        currentHealth = maxHealth;
    }

    // Fungsi ini ditambahkan agar upgrade HP bisa mengisi darah
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Jika ini adalah Player, siarkan event OnPlayerDamaged
        if (isPlayer)
        {
            OnPlayerDamaged?.Invoke(damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Jika ini adalah Musuh, siarkan event OnEnemyKilled
        if (!isPlayer)
        {
            OnEnemyKilled?.Invoke();

            CurrencyManager.instance.AddSessionCurrency(1);
            // Munculkan Orb XP menggunakan Object Pooler
            if (xpOrbPrefab != null)
            {
                ObjectPooler.instance.SpawnFromPool("XP_Orb", transform.position, Quaternion.identity);
            }
        }
        else
        {
            // Logika jika Player mati (misal: tampilkan layar Game Over)
            Debug.Log("Player Telah Mati!");

            CurrencyManager.instance.SaveSessionToTotal();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
            
        }

        // Kembalikan objek ke pool
        gameObject.SetActive(false);
    }

    // OnEnable dipanggil setiap kali objek diaktifkan (termasuk dari pool)
    private void OnEnable()
    {
        // Reset kesehatan setiap kali "hidup" kembali
        currentHealth = maxHealth;
    }
}