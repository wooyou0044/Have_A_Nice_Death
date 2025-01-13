using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spooksmen_AI : Walker
{
    // ���� ����
    // 1. (����) ��(Attack01) - ������(Attack02) - ����(Attack03) ������ ���濡 �ָ��� �ֵθ� -> ���ط� 12 - 10 - 11
    // 2. (�Ĺ�) ���� ������ ȸ����Ű�� �յ��� ª�� �Ÿ��� ����(Attack04) -> ���ط� 10
    // 3. (����) �������� ������.(Attack02) -> ���ط� 10

    LayerMask playerLayer;
    [SerializeField] float sightRange;

    // �÷��̾� �������� �� �ѵ� ����ǥ ������Ʈ
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;

    void Start()
    {
        // ��� ����Ʈ ����
        Instantiate(surprisedMark, surprisedPos);
        surprisedMark.SetActive(false);
    }

    void Update()
    {
        DetectPlayer();
    }

    // �÷��̾� �����ϴ� �Լ�
    void DetectPlayer()
    {
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, sightRange, 6);
        if(player.CompareTag("Player"))
        {
            Debug.Log("�÷��̾�");
            surprisedMark.SetActive(true);
        }
    }
}
