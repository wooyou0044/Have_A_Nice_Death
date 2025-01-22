using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueCanavas;
    [SerializeField] GameObject bossCanavas;
    [SerializeField] GameObject desk;

    GameObject canvas;
    GameObject boss;

    BossMovement getBossMove;
    GargoyleBrain getBossAi;
    DialogueUI getDialogueUI;
    DeskMovement getDeskMove;

    BossCamera getCamera;


    void Awake()
    {
        boss = GameObject.FindWithTag("Enemy");
        getBossMove = boss.GetComponent<BossMovement>();
        getBossAi = boss.GetComponent<GargoyleBrain>();

        canvas = Instantiate(dialogueCanavas);
        getDialogueUI = canvas.GetComponent<DialogueUI>();
        canvas.SetActive(false);

        getDeskMove = desk.GetComponent<DeskMovement>();

        getCamera = Camera.main.GetComponent<BossCamera>();
    }

    void Start()
    {

    }

    void Update()
    {
        if(getCamera.isArrive == true)
        {
            getBossMove.PlayerEnterBossStage();
            getCamera.isArrive = false;
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
                getBossAi.enabled = true;
                bossCanavas.SetActive(true);
            }
            getDialogueUI.ConverationEnd = false;
        }

        if(getBossMove.IsDead == true)
        {
            Debug.Log("Á×¾úÀ½");
            getBossMove.DeathAnimation();
            getBossMove.IsDead = false;
        }
    }
}
