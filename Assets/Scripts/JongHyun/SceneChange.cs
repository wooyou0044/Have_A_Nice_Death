using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public Animator animator;
    public void MainSceneChanger(string animationName)
    {        
        animator.SetTrigger("FadeIn");
        SceneManager.LoadScene("JonghyunTest");
    }
}
