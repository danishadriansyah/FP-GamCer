using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float defaultSpeed;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Simpan referensi coroutine yang sedang berjalan
    private Coroutine speedBuffCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSpeed = moveSpeed;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        animator.SetBool("IsRunning", moveX != 0 || moveY != 0);

        if (moveX > 0) spriteRenderer.flipX = false;
        else if (moveX < 0) spriteRenderer.flipX = true;

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // --- LOGIKA BARU BUFF SPEED ---
    public void BoostSpeed(float multiplier, float duration)
    {
        // 1. Jika sedang ada buff aktif, hentikan dulu
        if (speedBuffCoroutine != null)
        {
            StopCoroutine(speedBuffCoroutine);
            moveSpeed = defaultSpeed; // Reset kecepatan ke normal dulu biar tidak stacking (misal: 5 * 1.5 * 1.5)
        }

        // 2. Mulai buff baru (reset timer)
        speedBuffCoroutine = StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

    IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        moveSpeed *= multiplier; // Naikkan kecepatan
        Debug.Log("Speed Boost Aktif! Durasi: " + duration);

        yield return new WaitForSeconds(duration); // Tunggu sampai durasi habis

        moveSpeed = defaultSpeed; // Kembalikan ke normal
        speedBuffCoroutine = null; // Kosongkan referensi
        Debug.Log("Speed Boost Berakhir.");
    }
}