using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueCanavas;
    [SerializeField] GameObject desk;

    GameObject canvas;
    GameObject boss;

    BossMovement getBossMove;
    GargoyleBrain getBossAi;
    DialogueUI getDialogueUI;
    DeskMovement getDeskMove;

    // 임시로
    EnterSpot getEnterSpot;
    Collider2D enterSpotCollider;

    void Awake()
    {
        boss = GameObject.FindWithTag("Enemy");
        getBossMove = boss.GetComponent<BossMovement>();
        getBossAi = boss.GetComponent<GargoyleBrain>();

        canvas = Instantiate(dialogueCanavas);
        getDialogueUI = canvas.GetComponent<DialogueUI>();
        canvas.SetActive(false);

        getEnterSpot = GameObject.Find("TempSpot").GetComponent<EnterSpot>();
        enterSpotCollider = getEnterSpot.gameObject.GetComponent<Collider2D>();

        getDeskMove = desk.GetComponent<DeskMovement>();
    }

    void Start()
    {

    }

    void Update()
    {
        // 임시로
        if(getEnterSpot.IsEnter == true)
        {
            getBossMove.PlayerEnterBossStage();
            getEnterSpot.IsEnter = false;

            // 임시
            enterSpotCollider.enabled = false;
        }
        if(getBossMove.IsMeetPlayer == true && getBossMove.IsEndSurprised() == true)
        {
            canvas.SetActive(true);
            getBossMove.IsMeetPlayer = false;
        }
        if(getDialogueUI.ConverationEnd == true)
        {
            getBossMove.FightParticipation();
            getDeskMove.TurnOnAnimator();
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1.5f);
                getBossMove.MoveStop();
                getBossAi._skill = GargoyleBrain.Skill.Dash;
            }
            getDialogueUI.ConverationEnd = false;
        }
    }
}
