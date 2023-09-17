using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DuckState
{
    IDLE,
    WALKING,
    WALKING_TO_NEST,
    CROUCHING,
    COOLDOWN,
    DRAGGED
}
public class DuckController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields
    [SerializeField] private Duck _thisDuck;
    [SerializeField] private DuckState _state = DuckState.IDLE;
    [SerializeField] private DuckAnimationsController _animController;

    private DuckState _previousState = DuckState.IDLE;
    private Vector3 _maxLimits = Vector3.zero;
    private Vector3 _nextPosition = Vector3.zero;
    private float _positionOffset = 2f;
    private float _cooldownTime = 0f;
    private NestController _chosenNest = null;
    private Duck _partner;
    private float _dragYOffset = .25f;
    #endregion Fields

    #region Public
    public void SetDuck(Duck duck)
    {
        _thisDuck = duck;
    }

    public void Throw2Hole()
    {
        StopAllCoroutines();

        _state = DuckState.IDLE;
        _partner = null;
        _chosenNest = null;
        _cooldownTime = 0f;
        _nextPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    #endregion Public

    #region Unity events
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_state == DuckState.CROUCHING)  // If the duck is putting an egg then it can't be dragged.
            return;

        if (_state == DuckState.WALKING || _state == DuckState.WALKING_TO_NEST)
            _state = DuckState.DRAGGED;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _state = DuckState.DRAGGED;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newPosition = new Vector3(newPosition.x, newPosition.y - _dragYOffset, 0);
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _state = DuckState.IDLE;
    }
    #endregion Unity events

    #region Private
    void Start()
    {
        InitConfig();
        InvokeRepeating(nameof(LifeLoop), 5, 10f);
    }

    private void InitConfig()
    {
        if (GameManager.Instance.ActiveDucks.Count == 1)
        {
            _thisDuck = new Duck(0, 0, 0);
        }

        _maxLimits = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - _positionOffset, Screen.height - _positionOffset, 0f));
        _cooldownTime = Random.Range(5, 16);
        MoveToNextRandomPosition();
    }

    private void LifeLoop()
    {
        var stateDecider = Random.Range(0, 2);

        // Roam
        if (stateDecider == 0)
            MoveToNextRandomPosition();

        // Hatch
        if (stateDecider == 1 && _state != DuckState.COOLDOWN && GameManager.Instance.ActiveDucks.Count < GameManager.Instance.MaxAmount)
            GoIncubate();
        else if (_state == DuckState.COOLDOWN)
            MoveToNextRandomPosition();
    }

    private void MoveToNextRandomPosition()
    {
        if (_state != DuckState.IDLE && _state != DuckState.COOLDOWN && _state != DuckState.DRAGGED)
            return;
        _previousState = _state;
        _state = DuckState.WALKING;
        _animController.GoToWalk();
        GenerateNewPosition();

        _animController.FlipSpriteToLeft(_nextPosition.x < transform.position.x);

        StartCoroutine(nameof(DoRandomMovement));
    }

    private IEnumerator DoRandomMovement()
    {
        bool flag = true;
        Vector3 newPosition;
        while (flag)
        {
            newPosition = new Vector3(_nextPosition.x, _nextPosition.y, 0f);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime);

            if (_state == DuckState.DRAGGED) // if dragged, stop walking.
            {
                _animController.GoToIdle();
                break;
            }

            if (transform.position == _nextPosition || _state == DuckState.IDLE)
            {
                if (_state != DuckState.IDLE)
                    _state = _previousState;

                _animController.GoToIdle();
                flag = false;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    private void MoveToChosenNest(Vector3 position)
    {
        if (_state != DuckState.IDLE)
            return;

        _state = DuckState.WALKING_TO_NEST;
        _animController.FlipSpriteToLeft(position.x < transform.position.x);
        _animController.GoToWalk();
        StartCoroutine(DoMovementToNest(position));
    }

    private IEnumerator DoMovementToNest(Vector3 position)
    {
        bool flag = true;
        List<Duck> parents = new List<Duck>() { _thisDuck };
        while (flag)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime);
            if (transform.position == position)
            {
                if (_partner != null)
                    parents.Add(_partner);

                _animController.GoToCrouch();
                _state = DuckState.CROUCHING;
                yield return new WaitForSeconds(2f);
                _chosenNest.ConfigEgg(parents);
                StartCooldown();
                flag = false;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        _chosenNest = null;
    }

    private void GenerateNewPosition()
    {
        var newXPosition = Random.Range(-_maxLimits.x, _maxLimits.x);
        var newYPosition = Random.Range(-_maxLimits.y, _maxLimits.y);
        _nextPosition = new Vector3(newXPosition, newYPosition, 0);
    }

    

    private void GoIncubate()
    {
        if (_state != DuckState.IDLE && _state != DuckState.COOLDOWN)
            return;

        SelectPartner();
        var nests = GameManager.Instance.Nests;
        var chosenIndex = Random.Range(0, nests.Count);
        _chosenNest = nests[chosenIndex];

        if (_chosenNest.State == NestState.BUSY)
        {
            _chosenNest = null;
            MoveToNextRandomPosition();
            return;
        }

        _chosenNest.SetState(NestState.BUSY);
        MoveToChosenNest(_chosenNest.transform.position);
    }

    private void StartCooldown()
    {
        _state = DuckState.COOLDOWN;
        _animController.GoToIdle();
        Invoke(nameof(EndCooldown), _cooldownTime);
    }

    private void EndCooldown()
    {
        _state = DuckState.IDLE;
        _animController.GoToIdle();
    }

    private void SelectPartner()
    {
        if (GameManager.Instance.ActiveDucks.Count <= 1)
            return;
        var index = Random.Range(0, GameManager.Instance.ActiveDucks.Count);

        _partner = GameManager.Instance.ActiveDucks[index]._thisDuck;
    }
    #endregion Private
}
