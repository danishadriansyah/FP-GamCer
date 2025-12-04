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
        if (moveX != 0 || moveY != 0)
        {
            animator.SetBool("IsRunning", true);
        } else
        {
            animator.SetBool("IsRunning", false);
        }

        // Handle Sprite Flipping
        if (moveX > 0)
        {
            // Moving Right: specific code depends on your sprite's default direction
            spriteRenderer.flipX = false;
        }
        else if (moveX < 0)
        {
            // Moving Left: Mirror the sprite
            spriteRenderer.flipX = true;
        }
        // If moveX is 0, we do nothing, preserving the previous flip state.
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    public void BoostSpeed(float multiplier, float duration)
    {
        StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

    IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        moveSpeed *= multiplier;
        Debug.Log("Speed Boost Aktif!");
        yield return new WaitForSeconds(duration);
        moveSpeed = defaultSpeed;
        Debug.Log("Speed Boost Berakhir.");
    }
}