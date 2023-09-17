using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MAX_AMOUNT = 25;
    private List<NestController> _nests = new List<NestController>();
    private List<DuckController> _activeDucks = new List<DuckController>();
    private List<DuckController> _inactiveDucks = new List<DuckController>();
    private int _duckIndex = 0;
    private EggsDictionary _eggsDictionary;

    public static GameManager _instance = null;

    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _nests = FindObjectsOfType<NestController>().ToList();
        _activeDucks = FindObjectsOfType<DuckController>().ToList();
        _inactiveDucks = FindObjectsOfType<DuckController>(true).ToList();

        _eggsDictionary = (EggsDictionary)Resources.Load<EggsDictionary>("EggsDictionary");

        for (int i = _inactiveDucks.Count - 1; i >= 0; i--)
        {
            if (_inactiveDucks[i].gameObject.activeSelf)
                _inactiveDucks.Remove(_inactiveDucks[i]);
        }
    }

    public void CreateNewDuck(Duck duck, Vector3 position)
    {
        _duckIndex++;
        if (_duckIndex >= _inactiveDucks.Count)
            return;
        _inactiveDucks[_duckIndex].SetDuck(duck);
        _inactiveDucks[_duckIndex].gameObject.SetActive(true);
        _inactiveDucks[_duckIndex].transform.position = position;
        _activeDucks.Add(_inactiveDucks[_duckIndex]);
    }


    public Duck GenerateNewDuck(List<Duck> parents)
    {
        Duck newDuck = new Duck(0, 0, 0);
        var mutate = UnityEngine.Random.Range(0, 11);

        if (mutate < 7)
        {
            List<int> inheritedParts = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                inheritedParts.Add(UnityEngine.Random.Range(0, 2));
            }

            var headIndex = parents[inheritedParts[0]].head.index;
            var wingIndex = parents[inheritedParts[1]].wings.index;
            var tailIndex = parents[inheritedParts[2]].tail.index;

            newDuck = new Duck(headIndex, wingIndex, tailIndex);
        }
        else
        {
            List<(int, PartRarity)> inheritedParts = new List<(int, PartRarity)>();
            for (int i = 0; i < 3; i++)
            {
                var (index, rarity) = IngameEquations.GetIndexAndRarity();
                inheritedParts.Add((index, rarity));
            }

            var headIndex = inheritedParts[0].Item1;
            var headRarity = inheritedParts[0].Item2;

            var wingsIndex = inheritedParts[1].Item1;
            var wingsRarity = inheritedParts[1].Item2;

            var tailIndex = inheritedParts[2].Item1;
            var tailRarity = inheritedParts[2].Item2;

            newDuck = new Duck(headIndex, headRarity, wingsIndex, wingsRarity, tailIndex, tailRarity);

        }
        return newDuck;
    }

    public (Sprite, float) GetEggInfo(PartRarity rarity)
    {
        return (_eggsDictionary.GetSprite(rarity), _eggsDictionary.GetHatchTime(rarity));
    }

    public List<NestController> Nests => _nests;
    public List<DuckController> ActiveDucks => _activeDucks;

    public int MaxAmount => MAX_AMOUNT;
}
