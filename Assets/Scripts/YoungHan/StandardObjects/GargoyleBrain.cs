using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossMovement))]
public class GargoyleBrain : MonoBehaviour
{
    [SerializeField, Header("스킬 준비 시간"), Range(0, byte.MaxValue)]
    private float _preparationTime = 2f;

    private enum Skill
    {
        Scratching, //할퀴기
        Dash,       //돌진
        Stone,      //돌 던지기
        Fall,       //낙하
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Trace(IHittable hittable)
    {
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                yield return null;
            }
        }
    }
}
