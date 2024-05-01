using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPC_data data;
    public GameObject showInteract;
    public bool isCommunicating;
    public int currentText;
    public bool canSkip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() == null) return;
        showInteract.SetActive(true);
        if (Input.GetAxisRaw("Interact") == 1 && !isCommunicating)
        {
            StartCommunicating();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() == null) return;
        showInteract.SetActive(false);
        EndCommunicating();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() == null) return;
        if (Input.GetAxisRaw("Interact") == 1 && !isCommunicating)
        {
            StartCommunicating();
        }
    }

    public void StartCommunicating()
    {
        isCommunicating = true;
        Communication c = FindFirstObjectByType<Communication>();
        if (c != null)
        {
            c.background.SetActive(true);
            c.nameText.text = data.nameOfNPC;
            currentText = 0;
            SelectText();
        }
        else Debug.LogError("Communication is not your thing, but I need it, so find it");
    }

    private void Update()
    {
        if(isCommunicating && Input.GetAxisRaw("Interact") == 1 && canSkip)
        {
            canSkip = false;
            currentText++;
            SelectText();
        }
        else if(Input.GetAxisRaw("Interact") != 1) canSkip = true;
    }

    public void SelectText()
    {
        if(currentText >= data.text.Count)
        {
            EndCommunicating();
            return;
        }
        Communication c = FindFirstObjectByType<Communication>();
        if (c != null)
        {
            c.text.text = data.text[currentText];
        }
        else Debug.LogError("Communication is not your thing, but I need it, so find it");

    }

    public void EndCommunicating()
    {
        Communication c = FindFirstObjectByType<Communication>();
        if (c != null)
        {
            c.background.SetActive(false);
            currentText = 0;
            isCommunicating = false;
        }
        else Debug.LogError("Communication is not your thing, but I need it, so find it");
    }
}
