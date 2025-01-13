using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private GameObject player; //플레이어 위치
    private Rigidbody2D playerRb; //플레이어 리지드바디
    private Vector3 targetPos = Vector3.zero; //사다리 중심 위치, 플레이어를 옮겨야해서 Vector3
    private IEnumerator currentCoroutine; //코루틴 중복 실행 방지
    private Vector2 getLadder = new Vector2(0, 0); //사다리 올라가는 방향과 속도
    [SerializeField]private float moveUpSpeed = 25f; //속도
    private bool isLadder = false; //사다리 안에 들어오면 true, 나가거나 아니면 false
    float originalGravity; //복구해줄 원본 중력 값
    [SerializeField] float delay = 0.05f; //딜레이


    //private void Update() 테스트용
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
   /// 사다리에서 올라갈 때 쓰는 함수
   /// </summary>
    public void MoveUp()
    {
        //한번만 실행하기 위한 코드
        if(currentCoroutine == null)
        {
            currentCoroutine = GettingUp();
            StartCoroutine(currentCoroutine);
        }
        
        
    }
    /// <summary>
    /// 사다리에서 점프키를 눌러 정지하는 메소드
    /// </summary>
    public void MoveStop()
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        //중간에 끊어서 마저 실행되지 않은 복구 코드를 여기서 재실행
        if(playerRb.gravityScale != originalGravity)
        {
            playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            playerRb.gravityScale = originalGravity;
        }
    }

   /// <summary>
   /// 사다리를 올라가는 코루틴
   /// </summary>
   /// <returns></returns>
    IEnumerator GettingUp()
    {
        //최초 실행시 중력 제거, X 정지
        if (playerRb.gravityScale != 0.001f)
        {
            playerRb.gravityScale = 0.001f;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        //위치를 중간으로 이동
        if (player.transform.position != targetPos)
        {
            player.transform.position = targetPos;
        }
        playerRb.velocity = Vector3.zero;
        yield return new WaitForSecondsRealtime(delay);//잠깐 멈추고
        
        
        while (isLadder == true)
        {
          
            playerRb.velocity = getLadder;

            yield return null;//얘는 필요 없는 코드긴 함
        }
        //사다리에서 벗어나면 다시 움직일 수 있게 함
        playerRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        playerRb.gravityScale = originalGravity;
        currentCoroutine = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == true)
        {
            //플레이어가 null이면 초기화
            if (player == null)
            {
                player = collision.gameObject;
                playerRb = player.GetComponent<Rigidbody2D>();
                isLadder = true;
                //중력값 저장
                originalGravity = playerRb.gravityScale;
            }
            //올라가는 벡터 값 설정
            if (getLadder.y != moveUpSpeed)
            {
                getLadder.y = moveUpSpeed;
            }
            //가운데 값 구하기 + 플레이어 높이 가지기
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
        //플레이어가 나가면 초기화
        if (collision.CompareTag("Player") && player != null)
        {
            isLadder = false;
            player = null;
        }
    }
}
