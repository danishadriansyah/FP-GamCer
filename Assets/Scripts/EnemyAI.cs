using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Memastikan objek ini punya Rigidbody2D
public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private Rigidbody2D rb; // Variabel untuk Rigidbody2D

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>(); // Ambil komponen Rigidbody2D
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // Hitung arah ke pemain
            Vector2 direction = (player.position - transform.position).normalized;

            // Berikan kecepatan pada Rigidbody, bukan mengubah posisi langsung
            rb.linearVelocity = direction * speed;
        }
    }
}