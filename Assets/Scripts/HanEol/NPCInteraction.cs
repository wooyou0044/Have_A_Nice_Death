using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class NPCInteraction : MonoBehaviour
{
    [SerializeField] bool hadInteracted = false; //��ȭ �ѹ��� �ϵ��� �����ϱ� ���� ��
    [SerializeField] GameObject conversationCanvas; //��ȭ�� �� ���� Canvas ���� ������Ʈ
    [SerializeField] GameObject showInteratable; //��ȣ�ۿ� ������ �� ǥ�����ִ� ��ư ��� Ű
    [SerializeField]private Text npcName;
    [SerializeField]private Text conversation;
    private string[] conversationLines;
    private NPCInteraction script;
    byte conversationTimes;

    public bool Interacted
    {
        get { return hadInteracted; }
    }

    private void Awake()
    {
     
        conversationCanvas = GameObject.Find("InteractCanvus");//�̸����� ã���ֱ�
        showInteratable = GameObject.Find("showInteratable");
        npcName = GameObject.Find("Name").GetComponent<Text>();
        conversation = GameObject.Find("Contents").GetComponent<Text>();
        script = GetComponent<NPCInteraction>();
        conversationCanvas.SetActive(false);
        showInteratable.SetActive(false);
        conversationTimes = 0;
        #region ��ȭ ����
        conversationLines = new string[6];
        conversationLines[0] = ("���׷��� ������� �Ͻô� ���� ������ϱ�!\n���⿡�� ������ ��Ģ�̶� �� �ֽ��ϴ�.\n�׷��� ����...");
        conversationLines[1] = ("�ƴ�, �̰� ������! ȸ����̽��ݾ�!\n���� �� ���� ���̽��ϴ�, ȸ���.");
        conversationLines[2] = ("�� ������ ������ �����!\n�귡�尡 ������ ���� ���� �ƹ� ��ȥ�̳�\n�ŵֵ��̱�� �����߰ŵ��.");
        conversationLines[3] = ("���� �� �μ��� ������ ������\n���ʰ� �Ǿ� �־��, ��ó����. ��.");
        conversationLines[4] = ("������ ��⸦ �غ�����.\n�ҷο�� ���̿���.\n�� ��� ������ å���� �����ϱ��.");
        conversationLines[5] = ("���ƿ��ż� ��޴ϴ�, ȸ���.\n�׸��� ���� ��������. ����.");
        #endregion
    }

    //private void Update()//�׽�Ʈ�� �ڵ�
    //{
    //    if (UnityEngine.Input.GetKeyDown(KeyCode.F))
    //    {
    //        if(hadInteracted == false)
    //        {
    //            Interact();//�ڷ�ƾ ����� �̰� �ϳ� �ҷ��ָ� �� ����
    //        }
    //        if(hadInteracted == true)
    //        {
    //            ContinueConversation();
    //        }
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            if(hadInteracted == false)
            {
                //��ư�� ���� ��ȭ�� �սô�
                showInteratable.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(hadInteracted == false)
            {
                //��ȭ�� �� ������ �ٽ� ��ȭ ���ϰ� ����
                showInteratable.SetActive(false);
            }
            
        }
    }

    public void Interact()
    {
        hadInteracted=true;
        if (showInteratable == true)
        {
            showInteratable.SetActive(false);
        }
        conversationCanvas.SetActive(true);
        //�ڷ�ƾ ����� �ڵ�
        //StartCoroutine(Conversation());
    }

    public void ContinueConversation()
    {
        if (conversationTimes < conversationLines.Length)
        {
            conversation.text = conversationLines[conversationTimes];
            conversationTimes++;
            if(Time.timeScale != 0f) Time.timeScale = 0f;

        }
        else
        {
            EndInteraction();
            script.enabled = false;
        }

        
    }

    private void EndInteraction()
    {
        conversationCanvas.SetActive(!hadInteracted);
        Time.timeScale = 1.0f;
        StopAllCoroutines();//���� ����� currentCoroutine ���� ���� �� �ʱ�ȭ ����
    }

    IEnumerator Conversation()
    {
        while(hadInteracted == true)
        {
            if (conversationTimes < conversationLines.Length)
            {
                conversation.text = conversationLines[conversationTimes];
                conversationTimes++;
                if (Time.timeScale != 0f) Time.timeScale = 0f;

            }
            else
            {
                EndInteraction();
                script.enabled = false;
            }

            yield return null;
        }
        
    }

}
