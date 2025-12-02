using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Memastikan objek ini punya Rigidbody2D
public class EnemyAI : MonoBehaviour
{
    [Header("Status Musuh")]
    public float speed = 2f;
    public int damage = 10; 
    public float attackCooldown = 0.5f; 

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D collider;
    private float lastAttackTime;

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

            // Check if the enemy is moving left
            bool isMovingLeft = direction.x < 0;

            // Set the local scale of the sprite to mirror it
            Vector3 localScale = transform.localScale;
            // Flip only if needed
            if ((isMovingLeft && localScale.x > 0) || (!isMovingLeft && localScale.x < 0))
            {
                localScale.x *= -1;
                transform.localScale = localScale;
            }

            // Jika enemy mati maka tidak bisa disentuh
            if (animator.GetBool("IsDead"))
            {
                rb.linearVelocity = Vector2.zero; // Menghentikan gerak fisik
                rb.angularVelocity = 0f;

                // Disable the collider agar bisa dilewati
                collider.enabled = false;
            }
            // Jika enemy tidak sedang menyerang maka bergerak
            else if (!animator.GetBool("IsAttacking"))
            {
                animator.SetBool("IsRunning", true);
                rb.linearVelocity = direction * speed;
            }

            // Cek jarak untuk memicu animasi serangan
            float distance = Vector2.Distance(player.position, transform.position);
            if (!isAttacking && distance < 1.5f)
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

    // --- [BAGIAN BARU: LOGIKA PEMBERIAN DAMAGE] ---
    // Fungsi ini dipanggil Unity terus-menerus selama musuh menempel pada objek lain
    void OnCollisionStay2D(Collision2D collision)
    {
        // 1. Pastikan yang disentuh adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Jangan menyerang jika musuh sudah mati
            if (animator.GetBool("IsDead")) return;

            // 3. Cek cooldown serangan (agar HP tidak langsung habis dalam sekejap)
            if (Time.time >= lastAttackTime + attackCooldown)
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