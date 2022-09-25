using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimationManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    private string currentState;

    public void ChangeAnimationState(string newState)
    {
        if (newState == currentState)
        {
            return;
        }

        animator.Play(newState);
        currentState = newState;
    }
    public float GetCurrentAnimationDuration()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
