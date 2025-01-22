using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] float textSpeed;
    [SerializeField] Text SpeakerText;
    [SerializeField] Text DialogueText;
    // 대화할 때 알파값 조절
    [SerializeField] Image PlayerImage;
    [SerializeField] Image BossImage;
    [SerializeField] Image NextImage;

    [SerializeField] Dialogue[] dialogues;

    Animator nextAnimator;
    Vector2 nextPos;

    char[] dialogueChar;

    int spaceBarNum;
    int charLength;
    int charIndex;

    string currentSpeaker;

    float textElapsedTime;

    bool isAllOut;
    bool isConversationEnd;
    bool isSpaceBar;

    public bool ConverationEnd
    {
        get
        {
            return isConversationEnd;
        }
        set
        {
            isConversationEnd = value;
        }
    }

    void Awake()
    {
        nextAnimator = NextImage.GetComponent<Animator>();
        nextAnimator.enabled = false;
        nextPos = NextImage.transform.position;
    }

    void Start()
    {
        currentSpeaker = dialogues[0].character;
        SaveDialogue(0);
        isAllOut = false;
        DialogueText.text = string.Empty;
        SpeakerText.text = string.Empty;
        isConversationEnd = false;
        ChangeSpeaker(0);
        ChangeImage(0);
    }

    void Update()
    {
        if (spaceBarNum >= dialogues.Length)
        {
            // 죽고 나서 대화하려면 0으로 하면 안 됨
            spaceBarNum = 0;
            isConversationEnd = true;
            gameObject.SetActive(false);
        }

        if (isAllOut == false)
        {
            if(nextAnimator.enabled == true)
            {
                NextImage.transform.position = nextPos;
                nextAnimator.enabled = false;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                PrintAllDialogue(charIndex);
            }

            else
            {
                textElapsedTime += Time.deltaTime;
                if (textSpeed < textElapsedTime)
                {
                    PrintDialogue(charIndex, spaceBarNum);
                    textElapsedTime = 0;
                    charIndex++;
                }
            }
        }
        else
        {
            if(nextAnimator.enabled == false)
            {
                nextAnimator.enabled = true;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                DialogueText.text = string.Empty;
                charIndex = 0;
                textElapsedTime = 0;
                spaceBarNum++;
                SaveDialogue(spaceBarNum);
                ChangeSpeaker(spaceBarNum);
                ChangeImage(spaceBarNum);
                isAllOut = false;
            }
        }
    }

    void SaveDialogue(int dialogueIndex)
    {
        if(dialogueIndex < dialogues.Length)
        {
            int length = dialogues[dialogueIndex].dialogue.Length;
            charLength = length;
            dialogueChar = new char[length];

            int num = 0;

            foreach (char c in dialogues[dialogueIndex].dialogue)
            {
                dialogueChar[num] = c;
                num++;
            }
        }
    }

    void PrintDialogue(int charIndex, int dialogueIndex)
    {
        if (charIndex < charLength)
        {
            if (charIndex == 27)
            {
                DialogueText.text += "\n";
            }
            DialogueText.text += dialogueChar[charIndex].ToString();
            isAllOut = false;
        }
        else
        {
            isAllOut = true;
        }
    }

    void PrintAllDialogue(int charIndex)
    {
        for(int i=charIndex; i< dialogueChar.Length; i++)
        {
            if (i == 27)
            {
                DialogueText.text += "\n";
            }
            DialogueText.text += dialogueChar[i].ToString();
        }
        isAllOut = true;
    }

    void ChangeSpeaker(int dialogueIndex)
    {
        if (dialogueIndex < dialogues.Length)
        {
            SpeakerText.text = dialogues[dialogueIndex].character;
        }
    }

    void ChangeImage(int dialogueIndex)
    {
        if (dialogueIndex < dialogues.Length)
        {
            currentSpeaker = dialogues[dialogueIndex].character;
        }

        if (currentSpeaker == "데스")
        {
            PlayerImage.color = new Color(1,1,1,1);
            BossImage.color = new Color(1, 1, 1, 0.9f);
        }
        else
        {
            PlayerImage.color = new Color(1, 1, 1, 0.9f);
            BossImage.color = new Color(1, 1, 1, 1);
        }
    }
}
