using UnityEngine;

public class VBAttackProjectile : MonoBehaviour
{
    public int damage = 10;

    private float lastAttackTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyRock()
    {
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Pastikan yang disentuh adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 3. Cek cooldown serangan (agar HP tidak langsung habis dalam sekejap)
            if (Time.time >= lastAttackTime + 1f)
            {
                // 4. Ambil komponen HealthSystem dari Player
                HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();

                // 5. Kurangi HP Player
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    // Debug.Log("Musuh menyerang! HP Player berkurang."); // Hapus komen ini jika ingin cek di Console

                    // Catat waktu serangan ini untuk cooldown selanjutnya
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}
