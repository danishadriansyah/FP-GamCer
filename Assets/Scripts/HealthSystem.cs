using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public GameObject xpOrbPrefab;
    public int maxHealth;
    private int currentHealth;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Buat supaya parameter animator IsDead menjadi true
            animator.SetBool("IsDead", true);
            
            if (xpOrbPrefab != null)
            {
                Instantiate(xpOrbPrefab, transform.position, Quaternion.identity); // Tambahkan baris ini
            }
            Destroy(gameObject, 2); // argumen kedua adalah waktu dalam detik sebelum objek dihancurkan
        }
    }

    // --- DI BAWAH INI ADALAH BAGIAN YANG KITA SELIDIKI ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // PESAN DEBUG #1: Untuk tahu apakah fungsi ini pernah terpanggil
        Debug.Log(gameObject.name + " mendeteksi tabrakan trigger dengan " + other.gameObject.name);

        // PESAN DEBUG #2: Untuk tahu apa Tag dari objek yang menabrak
        Debug.Log("Tag objek yang masuk adalah: '" + other.tag + "'");


        if (other.CompareTag("Projectile"))
        {
            if (gameObject.CompareTag("Player"))
            {
                return;
            }

            TakeDamage(1);

            // PESAN DEBUG #3: Untuk tahu apakah perintah Destroy akan dijalankan
            Debug.Log("Berhasil mendeteksi Proyektil! Mencoba menghancurkan " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.CompareTag("Player") && collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}