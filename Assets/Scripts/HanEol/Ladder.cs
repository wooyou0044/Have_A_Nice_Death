using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]private static Vector2 targetPos;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            targetPos = new Vector2(this.transform.position.x, collision.transform.position.y);
        }
    }

    public static void MovePlayer(Transform player)
    {
        player.transform.position = targetPos;
        Debug.Log("¿Å±ä´Ù");
    }

}
