using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    public Vector2Int pos;
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
    public SpriteRenderer basicCubeSpriteRenderer;
    public Light2D basicCubeLight;
    public Light2D basicCubeLightBehind;
    public SpriteRenderer timerCubeSpriteRenderer;
    public Light2D timerCubeLight;
    public Light2D timerCubeLightBehind;
    public int needUpdate;

    // Start is called before the first frame update
    void Awake()
    {
        characterC = gameObject.GetComponent<BoxCollider2D>();
        characterRB = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HorizontalMovement();
        Jump();
        characterRB.velocity = new Vector2(characterRB.velocity.x, Mathf.Max(-fallSpeedLimit, characterRB.velocity.y));
        GetPosition();
    }

    private void LateUpdate() //ha ehhez hozz�ny�lsz, mind meghalunk!!!
    {
        if (needUpdate == 1)
        {
            FindFirstObjectByType<WallManager>().SetCurrentState();
            needUpdate = 0;
        }
        else if (needUpdate == 2)
        {
            FindFirstObjectByType<WallManager>().SetDefaultState();
            needUpdate = 0;
        }
    }

    public void SetPosition(int x, int y)
    {
        Map m = FindFirstObjectByType<Map>();
        transform.position = new Vector3(m.tileX + x, m.tileY + y, 0);
    }

    public void GetPosition()
    {
        Map m = FindFirstObjectByType<Map>();
        pos.x = Mathf.RoundToInt(transform.position.x - m.tileX);
        pos.y = Mathf.RoundToInt(transform.position.y - m.tileY);
        FindFirstObjectByType<WallManager>().CheckIfThePositionIsInTheWall(pos.x, pos.y);
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
        characterRB.velocity = new Vector2(Mathf.Min((Mathf.Abs(characterRB.velocity.x)-9*acceleration*Time.deltaTime)+10*acceleration*Time.deltaTime, movementSpeed) * Input.GetAxisRaw("Horizontal"), characterRB.velocity.y);
        animator.SetInteger("motion", (int)Input.GetAxisRaw("Horizontal"));
    }

    public void GotNewCube(int colorIndex)
    {
        basicCubeSpriteRenderer.gameObject.SetActive(true);
        Color32 a = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        basicCubeSpriteRenderer.color = a;
        basicCubeLight.color = a;
        basicCubeLightBehind.color = a;
    }

    public void SetNewTimerCubeColor(int colorIndex)
    {
        timerCubeSpriteRenderer.gameObject.SetActive(true);
        Color32 a = FindFirstObjectByType<WallManager>().GetColor(colorIndex);
        timerCubeSpriteRenderer.color = a;
        timerCubeLight.color = a;
        timerCubeLightBehind.color = a;
    }

    public void NoMoreTimerCubes()
    {
        timerCubeSpriteRenderer.gameObject.SetActive(false);
    }

    public void NoMoreCubes()
    {
        if(FindObjectsByType<Cube>(default).Length == 0) basicCubeSpriteRenderer.gameObject.SetActive(false);
    }

    public void NoJumpAfterGoingDown()
    {
        jumpCooldown = Time.time;
    }
}
