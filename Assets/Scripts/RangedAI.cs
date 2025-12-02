using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))] // Kita butuh ini untuk membalik badan
public class RangedAI : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float speed = 1.5f;
    public float stopDistance = 7f;

    [Header("Pengaturan Serangan")]
    public float fireRate = 2f;
    public string projectilePoolTag = "EnemyProjectile";
    public Transform firePoint; // Pastikan ini sudah diisi!

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Variabel baru
    private Collider2D collider;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Ambil komponen SpriteRenderer
        collider = GetComponent<Collider2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void OnEnable()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        fireTimer = fireRate;
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- PERBAIKAN ARAH HADAP (MANUAL FLIP) ---
        // Ini akan memaksa dia menghadap Player, baik saat lari ATAU diam
        if (directionToPlayer.x < 0)
        {
            spriteRenderer.flipX = true; // Hadap kiri
        }
        else if (directionToPlayer.x > 0)
        {
            spriteRenderer.flipX = false; // Hadap kanan
        }
        // --- AKHIR PERBAIKAN ARAH HADAP ---
        // Jika enemy mati maka tidak bisa disentuh
        if (animator.GetBool("IsDead"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // Disable the collider
            collider.enabled = false;
        }
        else if (distanceToPlayer > stopDistance)
        {
            // --- MODE: MENGEJAR ---
            collider.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic; // Biar bisa gerak
            rb.linearVelocity = directionToPlayer * speed;
            animator.SetBool("IsRunning", true);
            fireTimer = fireRate;
        }
        else
        {
            // --- MODE: BERHENTI & MENEMBAK ---
            collider.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic; // Biar GAK BISA kedorong musuh lain
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsRunning", false);

            fireTimer -= Time.fixedDeltaTime;
            if (fireTimer <= 0)
            {
                animator.SetBool("IsAttacking", true);
                // Beri tahu peluru arah tembak yang benar
                //Shoot(directionToPlayer);
                fireTimer = fireRate;
            }
        }
    }

    // Fungsi Shoot() sekarang menerima arah tembak
    void Shoot()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 spawnPosition = (firePoint != null) ? firePoint.position : transform.position;

        GameObject projectileGO = ObjectPooler.instance.SpawnFromPool(projectilePoolTag, spawnPosition, Quaternion.identity);

        if (projectileGO != null)
        {
            // --- PERBAIKAN ARAH TEMBAK ---
            // "Suntik" arah yang benar ke script peluru
            EnemyProjectile projectileScript = projectileGO.GetComponent<EnemyProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(directionToPlayer);
            }
            // --- AKHIR PERBAIKAN ARAH TEMBAK ---
            animator.SetBool("IsAttacking", false);
        }
    }
}