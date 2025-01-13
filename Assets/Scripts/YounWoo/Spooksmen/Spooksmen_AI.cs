using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spooksmen_AI : Walker
{
    // 공격 패턴
    // 1. (전방) 잽(Attack01) - 어퍼컷(Attack02) - 슬램(Attack03) 순서로 전방에 주먹을 휘두름 -> 피해량 12 - 10 - 11
    // 2. (후방) 몸을 빠르게 회전시키며 앞뒤의 짧은 거리를 공격(Attack04) -> 피해량 10
    // 3. (상향) 어퍼컷을 날린다.(Attack02) -> 피해량 10

    LayerMask playerLayer;
    [SerializeField] float sightRange;

    // 플레이어 감지했을 때 켜둘 느낌표 오브젝트
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;

    void Start()
    {
        // 놀란 이펙트 생성
        Instantiate(surprisedMark, surprisedPos);
        surprisedMark.SetActive(false);
    }

    void Update()
    {
        DetectPlayer();
    }

    // 플레이어 감지하는 함수
    void DetectPlayer()
    {
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, sightRange, 6);
        if(player.CompareTag("Player"))
        {
            Debug.Log("플레이어");
            surprisedMark.SetActive(true);
        }
    }
}
