using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    [SerializeField]
    private GameObject player; //�÷��̾� ��ġ
    private Rigidbody2D playerRb; //�÷��̾� ������ٵ�
    [SerializeField] private Walker playerWalker; //�÷��̾� ��Ŀ. isGrounded�˻��
    private Vector3 targetPos = Vector3.zero; //��ٸ� �߽� ��ġ, �÷��̾ �Űܾ��ؼ� Vector3
    private IEnumerator currentCoroutine; //�ڷ�ƾ �ߺ� ���� ����
    private Vector2 getLadder = new Vector2(0, 0); //��ٸ� �ö󰡴� ����� �ӵ�
    private Vector2 gettingDownLadder = new Vector2(0, 0);//��ٸ� �������� ����� �ӵ�
    [SerializeField] private float moveUpSpeed = 25f; //�ӵ�
    private bool isLadder = false; //��ٸ� �ȿ� ������ true, �����ų� �ƴϸ� false
    [SerializeField]float originalGravity; //�������� ���� �߷� ��
    [SerializeField] float delay = 0.05f; //������
    private bool movingUp = false;//�ö󰡴��� �ƴ��� �����ϴ� ����

    public bool MovingUp//���� �ִϸ��̼� ���� ������ ���ǹ��� �ɾ ���� �����ϸ�
        //�̰� �ʿ� ������ 
    {
        get { return movingUp; }
    }

    //private void Update()
    //{
    //    if(player != null)
    //    {
    //        if(Input.GetKey(KeyCode.UpArrow) == true)
    //        {
    //            MoveUp();
    //        }
    //        if(Input.GetKey(KeyCode.Space) == true)
    //        {
    //            MoveStop();
    //        }
    //        if(Input.GetKey(KeyCode.DownArrow) == true)
    //        {
    //            MoveDown();
    //        }
    //
    //    }
    //}
    /// <summary>
    /// ��ٸ����� �ö� �� ���� �Լ�
    /// </summary>
    public bool MoveUp()
    {
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }

        //�ѹ��� �����ϱ� ���� �ڵ�
        if (movingUp == false)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = GettingUp();
            StartCoroutine(currentCoroutine);
            movingUp = true;
        }
        return isLadder;


    }
    /// <summary>
    /// ��ٸ����� Ű�� ���� �����ϴ� �޼ҵ�. �÷��̾� ������ bool�� �Ѱ���
    /// </summary>
    public bool MoveStop()
    {
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            if(movingUp == true) movingUp = false;
        }
        //�����ϸ� ��ٸ����� �ȳ����� �̷��� ��
        if (playerRb.gravityScale != originalGravity)
        {
            playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            playerRb.gravityScale = originalGravity;
        }
        return isLadder;
    }
   

    public bool MoveDown()
    {
        if (player == null)
        {
            isLadder = false;
            return isLadder;
        }
       
        if (movingUp == true)
        {
            Debug.Log("�����!");
            StopCoroutine(currentCoroutine);
            currentCoroutine = GettingDown();
            StartCoroutine(currentCoroutine);
            movingUp = false;
        }
        return isLadder;
    }
    /// <summary>
    /// ��ٸ� Ÿ�� ���� �������� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator GettingDown()
    {
        while (isLadder == true && playerWalker.isGrounded == false)
        {
            playerRb.velocity = gettingDownLadder;
            yield return null;
        }
        //���ٴڿ� ������ �ʱ�ȭ
        if(playerWalker.isGrounded == true)
        {
            playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            playerRb.gravityScale = originalGravity;
        }
        currentCoroutine = null;
    }



    /// ��ٸ��� �ö󰡴� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator GettingUp()
    {
        //���� ����� �߷� ����, X ����
        if (playerRb.gravityScale != 0.001f)
        {
            playerRb.gravityScale = 0.001f;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        //��ġ�� �߰����� �̵�
        if (player.transform.position != targetPos)
        {
            player.transform.position = targetPos;
        }
        playerRb.velocity = Vector3.zero;
        yield return new WaitForSecondsRealtime(delay);//��� ���߰�


        while (isLadder == true)
        {

            playerRb.velocity = getLadder;
            yield return null;//�Ⱦ��� Boom
        }
        currentCoroutine = null;
    }
    /// <summary>
    /// ��ٸ��� �ö󰡴� ���� ��ٸ��� �������� �ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            //�÷��̾ null�̸� �ʱ�ȭ
            if (player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                isLadder = true;
                playerWalker = player.GetComponent<Player>();
                //�߷°� ����
                originalGravity = playerRb.gravityScale;
            }
            //�ö󰡴� ���� �� ����
            if (getLadder.y != moveUpSpeed)
            {
                getLadder.y = moveUpSpeed;
            }
            //�������� ���� �� ����
            if (gettingDownLadder.y != -moveUpSpeed)
            {
                gettingDownLadder.y -= moveUpSpeed;
            }
            //��� �� ���ϱ� + �÷��̾� ���� ������
            if (targetPos.x != transform.position.x)
            {
                targetPos.x = transform.position.x;
            }
            if (targetPos.y != player.transform.position.y)
            {
                targetPos.y = player.transform.position.y;
            }


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�÷��̾ ������ �ʱ�ȭ
        if (collision.CompareTag("Player") && player != null)
        {
            isLadder = false;
            //��ٸ����� ����� �ٽ� ������ �� �ְ� ��
            if (playerRb.gravityScale != originalGravity)
            {
                playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                playerRb.gravityScale = originalGravity;

            }
            if(movingUp == true) movingUp = false;
            playerRb = null;
            playerWalker = null;
            player = null;
            originalGravity = 0f;
        }
    }
}
