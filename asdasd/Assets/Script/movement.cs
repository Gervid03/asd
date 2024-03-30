using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Collider2D characterC;
    public Rigidbody2D characterRB;
    public float movementSpeed;
    public float jumpSpeed;
    public float jumpCooldown;
    public float runningFor;
    public float acceleration;
    public float fallSpeedLimit;

    // Start is called before the first frame update
    void Start()
    {
        characterC = gameObject.GetComponent<BoxCollider2D>();
        characterRB = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        Jump();
        characterRB.velocity = new Vector2(characterRB.velocity.x, Mathf.Min(fallSpeedLimit, characterRB.velocity.y));
    }

    public void Jump()
    {
        if (Input.GetAxisRaw("Jump") == 1 && Mathf.Abs(characterRB.velocity.y) < 0.01f && jumpCooldown + 0.1f < Time.time)
        {
            jumpCooldown = Time.time;
            characterRB.AddForce(new Vector2(0, jumpSpeed));
        }
    }
    public void HorizontalMovement()
    {
        characterRB.velocity = new Vector2(Mathf.Min((Mathf.Abs(characterRB.velocity.x)-9*acceleration)+10*acceleration, movementSpeed) * Input.GetAxisRaw("Horizontal"), characterRB.velocity.y);
    }
}
