using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int colorIndex;
    public Rigidbody2D characterRB;
    public Collider2D characterC;
    
    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<WallManager>().SubscribeToBeACube(this);
        FindFirstObjectByType<WallManager>().GetColor(colorIndex);

        characterC = characterRB.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(characterC, gameObject.GetComponent<BoxCollider2D>(), true);
    }

    private void Update()
    {
        Teleport();
    }

    public void BeActive()
    {

    }

    public void DontBeActive()
    {
        
    }

    public void Teleport()
    {
        if (Input.GetAxisRaw("CubeTP") > 0)
        {
            gameObject.transform.position = characterRB.position;
        }
    }
}
