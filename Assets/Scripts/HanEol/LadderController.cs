using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private void Update()
    {
        if(player != null)
        {
            if(Input.GetKey(KeyCode.UpArrow) == true)
            {
                MoveUp();
            }
            if (Input.GetKey(KeyCode.Space) == true)
            {
                MoveStop();
            }
        }
    }

    public void MoveUp()
    {

    }

    public void MoveStop()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log(player.name);
                this.player = player;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && player.gameObject == collision.gameObject)
        {
            Debug.Log(player.name);
            player = null;
        }
    }
}
