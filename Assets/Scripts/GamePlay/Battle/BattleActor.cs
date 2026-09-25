using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GamePlay.Battle
{
    public class BattleActor
    {
        public int ActorId { get; private set; }
        public string Name { get; private set; }

        public int MaxHp { get; private set; }
        public int CurrentHp { get;private set;}

        public bool IsDead => CurrentHp <= 0;
        public BattleActor(int actorId, string name, int maxHp)
        {
            ActorId = actorId;
            Name = name;
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }
        public void TakeDamage(int damage)
        {
            if(damage<=0) return;

            CurrentHp -= damage;
            if (CurrentHp < 0)
                CurrentHp = 0;
        }
        public void Heal(int value)
        {
            if(value <= 0) return;
            CurrentHp += value;
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
        }

    }
}
