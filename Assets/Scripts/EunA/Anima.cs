using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Anima : MonoBehaviour
{
    Collider2D animaCollider;
    public GameObject animaImage;
    public GameObject getItemEffect;

    Player player;
    float Destroytime;
    bool isGetAnima = false;

    void Start()
    {

    }

    private void Update()
    {
        if (isGetAnima == true)
        {
            Destroytime += Time.deltaTime;

            if (Destroytime >= 1.0f)
            {
                Destroy(gameObject);
            }
        }        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {        
        player = GetComponent<Player>();

        if (collision.CompareTag("Player"))
        {
            animaImage.SetActive(false);
            getItemEffect.SetActive(true);

            player?.Heal(1);

            isGetAnima = true;
        }     
    }
}
