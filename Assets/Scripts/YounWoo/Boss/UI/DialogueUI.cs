using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] Text SpeakerText;
    [SerializeField] Text DialogueText;
    // 대화할 때 알파값 조절
    [SerializeField] Image PlayerImage;
    [SerializeField] Image BossImage;

    [SerializeField] Dialogue[] dialogues;

    void Start()
    {
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {

        }
    }
}
