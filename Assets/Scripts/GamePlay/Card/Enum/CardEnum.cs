using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GamePlay.Card
{
    public enum CardEffectType
    {
        None = 0,

        Damage = 1,
        Heal = 2,
        DrawCard = 3,
        AddBuff = 4,
    }

    public enum CardTargetType
    {
        None = 0,
        Self = 1,
        Enemy = 2,
        AllEnemy = 3,
        AllAllies = 4,
    }
}