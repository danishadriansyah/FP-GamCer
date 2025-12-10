using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10;
    private float lastAttackTime;
    // --- [BAGIAN BARU: LOGIKA PEMBERIAN DAMAGE] ---
    // Fungsi ini dipanggil Unity terus-menerus selama musuh menempel pada objek lain
    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        // 1. Pastikan yang disentuh adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 3. Cek cooldown serangan (agar HP tidak langsung habis dalam sekejap)
            if (Time.time >= lastAttackTime + 0.12f)
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
