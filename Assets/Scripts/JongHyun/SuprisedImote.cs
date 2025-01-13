using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuprisedImote : MonoBehaviour
{
    GameObject game;
    [SerializeField]Animator suprisedAnimator;
    Vector2 womanGhostPos;
    private void Start()
    {
        womanGhostPos = transform.position;

        if (suprisedAnimator == null)
        {
            suprisedAnimator = GetComponent<Animator>();
        }
    }
    private void Update()
    {
        
    }

    void SupriseImote()
    {
        if(suprisedAnimator.gameObject == null)
        {
            suprisedAnimator.gameObject.SetActive(false);
            game.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            suprisedAnimator.gameObject.SetActive(true);
            game.GetComponent<SpriteRenderer>().enabled = true;
        }
        
    }
}
