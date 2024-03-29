using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Collider2D characterC;
    public Collider2D groundedC;
    public Rigidbody2D characterRB;
    public float movementSpeed;
    public float jumpSpeed;
    public float jumpCooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        Jump();
    }

    public void Jump()
    {
        Debug.Log(characterRB.velocity.y);
        if (Input.GetAxisRaw("Jump") == 1 && Mathf.Abs(characterRB.velocity.y) < 0.01f && jumpCooldown + 0.1f < Time.time)
        {
            jumpCooldown = Time.time;
            characterRB.AddForce(new Vector2(0, jumpSpeed));
        }
    }
    public void HorizontalMovement()
    {
        characterRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*movementSpeed, characterRB.velocity.y);
    }
}
