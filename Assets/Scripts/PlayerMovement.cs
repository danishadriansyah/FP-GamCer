using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float defaultSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultSpeed = moveSpeed;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
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