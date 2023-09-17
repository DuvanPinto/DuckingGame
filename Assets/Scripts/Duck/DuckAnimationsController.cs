using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DuckAnimationsController : MonoBehaviour
{
    #region Fields
    private const string IDLE_TRIGGER = "IDLE";
    private const string WALK_TRIGGER = "WALK";
    private const string CROUCH_TRIGGER = "CROUCH";
    private const string DRAG_TRIGGER = "DRAG";

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    #endregion Fields

    #region Life cycle

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #endregion Life cycle

    #region Public

    public void GoToIdle()
    {
        _animator.SetTrigger(IDLE_TRIGGER);
    }

    public void GoToWalk()
    {
        GoToIdle();
        _animator.SetTrigger(WALK_TRIGGER);
    }

    public void GoToCrouch()
    {
        GoToIdle();
        _animator.SetTrigger(CROUCH_TRIGGER);
    }

    public void GoToDrag()
    {
        GoToIdle();
        _animator.SetTrigger(DRAG_TRIGGER);
    }

    public void FlipSpriteToLeft(bool flip)
    {
        _spriteRenderer.flipX = flip;
    }

    #endregion Public
}
