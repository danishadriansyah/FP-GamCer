using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 4f;
    public int damage = 1;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 lockedDirection; // Arah yang sudah "dikunci"

    void Awake()
    {
        // Kita pindah ke Awake biar pasti ada sebelum OnEnable
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    // Fungsi baru yang dipanggil oleh RangedAI
    public void SetDirection(Vector2 direction)
    {
        lockedDirection = direction.normalized;
    }

    void OnEnable()
    {
        // Langsung tembak ke arah yang sudah diberikan
        rb.linearVelocity = lockedDirection * speed;

        // Atur rotasi peluru (bonus biar keren)
        float angle = Mathf.Atan2(lockedDirection.y, lockedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void FixedUpdate()
    {
        // Jaga kecepatan tetap konstan
        rb.linearVelocity = lockedDirection * speed;

        // Cek keluar layar
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPoint.x < -0.1f || screenPoint.x > 1.1f || screenPoint.y < -0.1f || screenPoint.y > 1.1f)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            gameObject.SetActive(false);
        }
    }
}