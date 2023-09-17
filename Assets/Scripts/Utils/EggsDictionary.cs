using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "EggsDictionary", menuName = "Dictionaries/Eggs")]
public class EggsDictionary : ScriptableObject
{
    public List<EggItem> _eggs;

    public Sprite GetSprite(PartRarity rarity)
    {
        Sprite tempSprite = null;
        foreach (var egg in _eggs)
        {
            if (egg.eggRarity == rarity)
                tempSprite = egg.eggSprite;
        }
        return tempSprite;
    }

    public float GetHatchTime(PartRarity rarity)
    {
        var time = 0f;
        foreach (var egg in _eggs)
        {
            if (egg.eggRarity == rarity)
                time = egg.hatchTime;
        }
        return time;
    }
}

[System.Serializable]
public struct EggItem
{
    public PartRarity eggRarity;
    public Sprite eggSprite;
    public float hatchTime;
}