using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggController : MonoBehaviour
{
    [SerializeField] private Sprite _commonColor;
    [SerializeField] private Sprite _rareColor;
    [SerializeField] private Sprite _epicColor;
    [SerializeField] private Sprite _mythicColor;
    [SerializeField] private Sprite _exoticColor;
    [SerializeField] private Sprite _legendaryColor;
    [SerializeField] private SpriteRenderer _eggSpriteRenderer;

    private List<Duck> _parents = new List<Duck>();
    private float _hatchingTime = 0f;
    private PartRarity _eggRarity = PartRarity.COMMON;
    private Action<NestState> _nestCallback = null;

    private void Start()
    {
        _eggSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetHatchingTime(float time) { _hatchingTime = time; }
    public void StartIncubation(Vector3 position, Action<NestState> nestCallback)
    {
        _nestCallback = nestCallback;
        StartCoroutine(WaitToHatch(position));
    }

    private IEnumerator WaitToHatch(Vector3 position)
    {
        while (_hatchingTime > 0)
        {
            _hatchingTime -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Hatch(position);
    }

    public void SetParents(List<Duck> parents) 
    {
        var headRarity = PartRarity.COMMON;
        var wingsRarity = PartRarity.COMMON;
        var tailRarity = PartRarity.COMMON;

        _parents = parents;

        foreach (var parent in _parents)
        {
            if (parent.head.rarity > headRarity)
                headRarity = parent.head.rarity;

            if (parent.wings.rarity > wingsRarity)
                headRarity = parent.wings.rarity;

            if (parent.tail.rarity > tailRarity)
                headRarity = parent.tail.rarity;
        }

        if (headRarity > wingsRarity && headRarity > tailRarity)
            _eggRarity = headRarity;
        else if (wingsRarity > headRarity && wingsRarity > tailRarity)
            _eggRarity = wingsRarity;
        else
            _eggRarity = tailRarity;

        SetColorAndTime();
    }

    private void Hatch(Vector3 position)
    {
        var newDuck = new Duck(0,0,0);
        if (_parents.Count == 1)
        {
            newDuck.head.index = _parents[0].head.index;
            newDuck.wings.index = _parents[0].wings.index;
            newDuck.tail.index = _parents[0].tail.index;
            GameManager.Instance.CreateNewDuck(newDuck, position);
            gameObject.SetActive(false);
        }
        else
        {
            newDuck = GameManager.Instance.GenerateNewDuck(_parents);
            GameManager.Instance.CreateNewDuck(newDuck, position);
            gameObject.SetActive(false);
        }
        _nestCallback.Invoke(NestState.FREE);
    }

    

    private void SetColorAndTime()
    {
        switch (_eggRarity)
        {
            case PartRarity.COMMON:
                _eggSpriteRenderer.sprite = _commonColor;
                _hatchingTime = 15f;
                break;
            case PartRarity.RARE:
                _eggSpriteRenderer.sprite = _rareColor;
                _hatchingTime = 30f;
                break;
            case PartRarity.EPIC:
                _eggSpriteRenderer.sprite = _epicColor;
                _hatchingTime = 45f;
                break;
            case PartRarity.MYTHIC:
                _eggSpriteRenderer.sprite = _mythicColor;
                _hatchingTime = 60f;
                break;
            case PartRarity.EXOTIC:
                _eggSpriteRenderer.sprite = _exoticColor;
                _hatchingTime = 80f;
                break;
            case PartRarity.LEGENDARY:
                _eggSpriteRenderer.sprite = _legendaryColor;
                _hatchingTime = 90f;
                break;
            default:
                break;
        }
    }
}
