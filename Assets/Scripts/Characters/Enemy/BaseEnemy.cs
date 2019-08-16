using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseCharacter
{


    public enum Type
    {
        BEAST,
        MAGIC,
        HUMAN,
        GOD
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }

    public Type EnemyType;
    public Rarity rarity;
    public int experienceToReward;



}
