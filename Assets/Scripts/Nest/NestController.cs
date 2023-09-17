using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NestState { FREE, BUSY}
public class NestController : MonoBehaviour
{
    [SerializeField] private NestState _state = NestState.FREE;
    [SerializeField] private EggController _egg;

    public void SetState(NestState state)
    {
        _state = state;
    }

    public void ConfigEgg(List<Duck> parents)
    {
        if (_state != NestState.BUSY)
            return;
        _egg.gameObject.SetActive(true);
        _egg.SetParents(parents);
        _egg.StartIncubation(transform.position, (Action<NestState>)SetState);
    }

    public NestState State => _state;
}
