using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Memastikan objek ini punya Rigidbody2D
public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb; // Variabel untuk Rigidbody2D

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>(); // Ambil komponen Rigidbody2D
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // Hitung arah ke pemain
            Vector2 direction = (player.position - transform.position).normalized;

            // Check if the enemy is moving left
            bool isMovingLeft = direction.x < 0;

            // Set the local scale of the sprite to mirror it
            Vector3 localScale = transform.localScale;
            localScale.x = isMovingLeft ? -1 : 1;
            transform.localScale = localScale;

            // Jika enemy mati maka tidak bisa disentuh
            if (animator.GetBool("IsDead"))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.isKinematic = true;
                // Disable the collider
                Collider2D collider = GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
            // Berikan kecepatan pada Rigidbody, bukan mengubah posisi langsung
            // Jika enemy tidak sedang menyerang maka bergerak, jika menyerang maka diam di tempat
            else if (!animator.GetBool("IsAttacking"))
            {
                rb.linearVelocity = direction * speed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Check if the enemy is close to the player
            float distance = Vector2.Distance(player.position, transform.position);
            if (distance < 1.5f) // Replace 1.0f with the desired distance
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAttacking", true);
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }

            // Set parameter animator IsRunning menjadi true ketika objek bergerak
            animator.SetBool("IsRunning", true);
        }
    }
}