using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VBAI : MonoBehaviour
{
    [Header("Status Void Behemoth")]
    public float speed = 1.5f; // Sedikit lebih lambat karena dia tank
    public int damage = 20;    // [BARU] Damage lebih besar dari musuh biasa
    public float attackCooldown = 1.5f; // [BARU] Serangan lebih lambat tapi sakit

    [Header("Skill Settings")]
    public GameObject rockHillPrefab;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D collider;
    private float lastAttackTime; // [BARU] Timer cooldown

    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player != null)
        {
            // Hitung arah ke pemain
            Vector2 direction = (player.position - transform.position).normalized;

            // Cek apakah musuh bergerak ke kiri (untuk membalik sprite)
            bool isMovingLeft = direction.x < 0;
            Vector3 localScale = transform.localScale;
            if ((isMovingLeft && localScale.x > 0) || (!isMovingLeft && localScale.x < 0))
            {
                localScale.x *= -1;
                transform.localScale = localScale;
            }

            // Logika saat Mati
            if (animator.GetBool("IsDead"))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                collider.enabled = false;
                isAttacking = false;
            }
            // Logika saat Bergerak (Mengejar)
            else if (!animator.GetBool("IsAttacking"))
            {
                animator.SetBool("IsRunning", true);
                rb.linearVelocity = direction * speed;
            }

            // Cek Jarak untuk Memicu Animasi Serangan
            float distance = Vector2.Distance(player.position, transform.position);
            if (distance < 2.0f) // Jarak serangan sedikit lebih jauh dari musuh biasa
            {
                isAttacking = true;
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAttacking", true);

                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
            }
            
        }
    }

    void EnableMovement()
    {
        animator.SetBool("IsAttacking", false);
        rb.bodyType = RigidbodyType2D.Dynamic;
        isAttacking = false;
    }

    // Fungsi ini dipanggil oleh Animation Event (pastikan sudah di-set di animasi serangan)
    void SummonRockHill()
    {
        if (rockHillPrefab != null)
        {
            float yOffset = -0.5f;
            Instantiate(rockHillPrefab, transform.position + new Vector3(0f, yOffset, 0f), Quaternion.identity);
        }
    }

    // --- [BAGIAN BARU: LOGIKA DAMAGE KONTAK] ---
    void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Cek apakah menabrak Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Cek apakah Behemoth sudah mati
            if (animator.GetBool("IsDead")) return;

            // 3. Cek Cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // 4. Ambil darah pemain
                HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();

                // 5. Berikan Damage
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Void Behemoth Menghantam Player! Damage: " + damage);

                    lastAttackTime = Time.time; // Reset cooldown
                }
            }
        }
    }
    
}