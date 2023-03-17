using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "UnitData", menuName = "New Unit")]
public class UnitData : ScriptableObject
{
    public string characterName;
    public bool canAttack;
    public bool canAttackAerialUnits;
    public bool canSupport;

    public float maxHealth;
    public float damageAmount;
    public float healAmount;
    public float attackTime;
    public float minAttackRange;
    public float speed;
    public float stoppingDistance;
    public float buffAmount;
    public float buffTime;
    public float manaCost;
    public float baseOffset;


    public CharacterType characterType; // field
    public enum CharacterType
    {
        MELEE,
        RANGE,
        SUPPORT,
        AERIAL
    };


}

