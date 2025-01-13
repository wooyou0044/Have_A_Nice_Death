using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private GameObject player; //�÷��̾� ��ġ
    private Rigidbody2D playerRb; //�÷��̾� ������ٵ�
    private Vector3 targetPos = Vector3.zero; //��ٸ� �߽� ��ġ, �÷��̾ �Űܾ��ؼ� Vector3
    private IEnumerator currentCoroutine; //�ڷ�ƾ �ߺ� ���� ����
    private Vector2 getLadder = new Vector2(0, 0); //��ٸ� �ö󰡴� ����� �ӵ�
    [SerializeField]private float moveUpSpeed = 25f; //�ӵ�
    private bool isLadder = false; //��ٸ� �ȿ� ������ true, �����ų� �ƴϸ� false
    float originalGravity; //�������� ���� �߷� ��
    [SerializeField] float delay = 0.05f; //������


    //private void Update() �׽�Ʈ��
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
    //
    //    }
    //}
   /// <summary>
   /// ��ٸ����� �ö� �� ���� �Լ�
   /// </summary>
    public void MoveUp()
    {
        //�ѹ��� �����ϱ� ���� �ڵ�
        if(currentCoroutine == null)
        {
            currentCoroutine = GettingUp();
            StartCoroutine(currentCoroutine);
        }
        
        
    }
    /// <summary>
    /// ��ٸ����� ����Ű�� ���� �����ϴ� �޼ҵ�
    /// </summary>
    public void MoveStop()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        //�߰��� ��� ���� ������� ���� ���� �ڵ带 ���⼭ �����
        if(playerRb.gravityScale != originalGravity)
        {
            playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            playerRb.gravityScale = originalGravity;
        }
    }

   /// <summary>
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

            yield return null;//��� �ʿ� ���� �ڵ�� ��
        }
        //��ٸ����� ����� �ٽ� ������ �� �ְ� ��
        playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        playerRb.gravityScale = originalGravity;
        currentCoroutine = null;
    }
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
                //�߷°� ����
                originalGravity = playerRb.gravityScale;
            }
            //�ö󰡴� ���� �� ����
            if (getLadder.y != moveUpSpeed)
            {
                getLadder.y = moveUpSpeed;
            }
            //��� �� ���ϱ� + �÷��̾� ���� ������
            if(targetPos.x != transform.position.x) 
            {
                targetPos.x = transform.position.x;
            }
            if(targetPos.y != player.transform.position.y)
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
            player = null;
        }
    }
}
