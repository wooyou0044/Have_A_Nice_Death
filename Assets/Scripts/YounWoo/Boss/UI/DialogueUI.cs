using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] Text SpeakerText;
    [SerializeField] Text DialogueText;
    // ��ȭ�� �� ���İ� ����
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
