using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Memastikan objek ini punya Rigidbody2D
public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private int damage;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
        }
        else
        {
            // Hitung arah dan berikan kecepatan
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            // Kondisi 2: Hancurkan diri jika sudah sangat dekat dengan target
            if (Vector2.Distance(transform.position, target.position) < 0.2f)
            {
                HealthSystem enemyHealth = target.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                }
                gameObject.SetActive(false);
                return;
            }
        }

        // Kondisi 3: Hancurkan diri jika keluar dari layar
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPoint.x < -0.1f || screenPoint.x > 1.1f || screenPoint.y < -0.1f || screenPoint.y > 1.1f)
        {
            gameObject.SetActive(false);
        }
    }
}