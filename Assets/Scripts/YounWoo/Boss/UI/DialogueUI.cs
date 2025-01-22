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

    float textElapsedTime;

    bool isAllOut;

    void Awake()
    {
        nextAnimator = NextImage.GetComponent<Animator>();
        nextAnimator.enabled = false;
        nextPos = NextImage.transform.position;
    }

    void Start()
    {
        SaveDialogue(0);
        isAllOut = false;
        DialogueText.text = string.Empty;
    }

    void Update()
    {
        //if(isAllOut)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {

        //        if(spaceBarNum < dialogues.Length)
        //        {
        //            // 몇 번 스페이스바 눌렸는지 확인
        //            spaceBarNum++;
        //            // 다음 대화 char 배열에 저장
        //            SaveDialogue(spaceBarNum);
        //        }
        //    }
        //}
        //else
        //{

        //}
        
        if(isAllOut == false)
        {
            if(nextAnimator.enabled == true)
            {
                nextAnimator.enabled = false;
            }

            textElapsedTime += Time.deltaTime;
            if(textSpeed < textElapsedTime)
            {
                PrintDialogue(charIndex, spaceBarNum);
                textElapsedTime = 0;
                charIndex++;
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
                isAllOut = false;
            }
            //DialogueText.text = string.Empty;
        }
    }

    void SaveDialogue(int dialogueIndex)
    {
        int length = dialogues[dialogueIndex].dialogue.Length;
        charLength = length;
        dialogueChar = new char[length];

        int num = 0;

        foreach(char c in dialogues[dialogueIndex].dialogue)
        {
            dialogueChar[num] = c;
            num++;
        }
    }

    void PrintDialogue(int charIndex, int dialogueIndex)
    {
        if (charIndex < charLength)
        {
            if (charIndex == 27)
            {
                ///Debug.Log(dialogueChar[charIndex - 1]);
            }
            DialogueText.text += dialogueChar[charIndex].ToString();
            isAllOut = false;
        }
        else
        {
            isAllOut = true;
        }
    }
}
