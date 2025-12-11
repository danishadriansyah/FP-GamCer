using UnityEngine;
using System; 
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Data")]
    public GameObject xpOrbPrefab;
    public float maxHealth;
    public float currentHealth;
    private Animator animator;

    [Header("Status")]
    public bool isPlayer = false;

    private bool isDead = false;

    public static event Action<float> OnPlayerDamaged;
    public static event Action OnEnemyKilled;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (gameObject.CompareTag("Player"))
        {
            isPlayer = true;
        }
        currentHealth = maxHealth;
    }

    public bool GetStatus()
    {
        return isDead;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        GetComponent<SpriteFlash>().Flash();

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
            // Buat supaya parameter animator IsDead menjadi true
            animator.SetBool("IsDead", true);
            isDead = true;
            //OnEnemyKilled?.Invoke();

            ////CurrencyManager.instance.AddSessionCurrency(1);
            //// Munculkan Orb XP menggunakan Object Pooler
            //if (xpOrbPrefab != null)
            //{
            //    ObjectPooler.instance.SpawnFromPool("XP_Orb", transform.position, Quaternion.identity);
            //}
        }
        else
        {
            // Logika jika Player mati (misal: tampilkan layar Game Over)
            Debug.Log("Player Telah Mati!");

            //CurrencyManager.instance.SaveSessionToTotal();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
            
        }

        // Kembalikan objek ke pool
        //gameObject.SetActive(false);
    }

    public void OnDeathAnimationEnd()
    {
        OnEnemyKilled?.Invoke();

        if (xpOrbPrefab != null)
        {
            ObjectPooler.instance.SpawnFromPool("XP_Orb", transform.position, Quaternion.identity);
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
        gameObject.SetActive(false);
    }

    // OnEnable dipanggil setiap kali objek diaktifkan (termasuk dari pool)
    private void OnEnable()
    {
        // Reset kesehatan setiap kali "hidup" kembali
        currentHealth = maxHealth;
        isDead = false;
    }
}