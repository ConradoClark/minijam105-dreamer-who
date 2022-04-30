using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamCharacterAnimator : MonoBehaviour
{
    public Animator Animator;
    public void Face(float direction)
    {
        Animator.SetBool("FacingRight", Mathf.Sign(direction) > 0);
    }

    public void SetWalking(bool walking)
    {
        Animator.SetBool("Walking", walking);
    }

    public void SetFalling(bool falling)
    {
        Animator.SetBool("Falling", falling);
    }

    public void SetJumping(bool jumping)
    {
        Animator.SetBool("Jumping", jumping);
    }
}
