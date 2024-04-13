using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public int gatesTouch;
    public Transform transformOfParts;
    public Animator animator;

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
        characterRB.velocity = new Vector2(characterRB.velocity.x, Mathf.Max(-fallSpeedLimit, characterRB.velocity.y));
    }

    public void SetPosition(int x, int y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
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
        if (Input.GetAxisRaw("Horizontal") != 0) transformOfParts.localScale = new Vector2((int)Input.GetAxisRaw("Horizontal"), 1);
        characterRB.velocity = new Vector2(Mathf.Min((Mathf.Abs(characterRB.velocity.x)-9*acceleration)+10*acceleration, movementSpeed) * Input.GetAxisRaw("Horizontal"), characterRB.velocity.y);
        animator.SetInteger("motion", (int)Input.GetAxisRaw("Horizontal"));
    }
}
