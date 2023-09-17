using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PartRarity
{
    COMMON,
    RARE,
    EPIC,
    MYTHIC,
    EXOTIC,
    LEGENDARY,
    MUTANT
}

[System.Serializable]
public class Duck
{
    public DuckPart head;
    public DuckPart wings;
    public DuckPart tail;

    public Duck (int head, int wings, int tail)
    {
        this.head.index = head;
        this.wings.index = wings;
        this.tail.index = tail;
    }

    public Duck(int head, PartRarity headRarity, int wings, PartRarity wingsRarity, int tail, PartRarity tailRarity)
    {
        this.head.index = head;
        this.head.rarity = headRarity;
        this.wings.index = wings;
        this.wings.rarity = wingsRarity;
        this.tail.index = tail;
        this.tail.rarity = tailRarity;
    }
}

[System.Serializable]
public struct DuckPart
{
    public int index;
    public PartRarity rarity;

    public DuckPart (int index, PartRarity rarity)
    {
        this.index = index;
        this.rarity = rarity;
    }
}
