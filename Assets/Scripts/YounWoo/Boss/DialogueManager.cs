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
    [SerializeField] GameObject clearCanvas;

    GameObject canvas;
    GameObject boss;
    GameObject clearWindow;

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

        clearWindow = Instantiate(clearCanvas);
        clearWindow.SetActive(false);
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
        if(getBossMove.IsMeetPlayer == true && getBossMove.IsEndAnimation() == true)
        {
            canvas.SetActive(true);
            getBossMove.IsMeetPlayer = false;
        }
        if(getBossMove.isAlive == true && getDialogueUI.ConverationEnd == true)
        {
            getBossMove.FightParticipation();
            getDeskMove.TurnOnAnimator();
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1.5f);
                getBossAi.enabled = true;
                getBossMove.MoveStop();
                bossCanavas.SetActive(true);
            }
            getDialogueUI.ConverationEnd = false;
        }

        if (getBossMove.IsDead == true && getBossMove.IsEndAnimation() == true)
        {
            StartCoroutine(PlayDialogue());
            IEnumerator PlayDialogue()
            {
                yield return new WaitForSeconds(1.0f);
                canvas.SetActive(true);
                bossCanavas.SetActive(false);
            }
            getBossMove.deathState = BossMovement.DeathType.Awake;
            getBossMove.IsDead = false;
        }

        if (getBossMove.deathState == BossMovement.DeathType.Awake && getDialogueUI.ConverationEnd == true)
        {
            getBossMove.DeathAnimation(getBossMove.deathState);
            getBossMove.deathState = BossMovement.DeathType.Be_AnotherBoss;
            getDialogueUI.ConverationEnd = false;
        }

        if(getBossMove.deathState == BossMovement.DeathType.Be_AnotherBoss && getBossMove.IsEndAnimation() == true)
        {
            getBossMove.DeathAnimation(getBossMove.deathState);
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1.5f);
                getBossMove.MoveStop();
            }
            getBossMove.deathState = BossMovement.DeathType.GoOutside;
        }

        if (getBossMove.deathState == BossMovement.DeathType.GoOutside && getBossMove.IsEndAnimation() == true)
        {
            getBossMove.DeathAnimation(getBossMove.deathState);
            StartCoroutine(DoStop());
            IEnumerator DoStop()
            {
                yield return new WaitForSeconds(1f);
                getBossMove.MoveStop();
            }
            getBossMove.deathState = BossMovement.DeathType.Death;
        }

        if (getBossMove.deathState == BossMovement.DeathType.Death && getBossMove.IsEndAnimation() == true)
        {
            getBossMove.DeathAnimation(getBossMove.deathState);
        }

        if(getBossMove.IsDisappear && clearWindow.activeSelf == false)
        {
            clearWindow.SetActive(true);
        }
    }
}
