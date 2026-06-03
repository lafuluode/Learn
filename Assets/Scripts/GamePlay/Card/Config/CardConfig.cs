using Game.Framework.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GamePlay.Card
{
    public class CardConfig : IConfigRow<int>
    {
        public int ConfigId;
        public string Name;
        public string Cost;
        public string Description;
        public List<CardEffectConfig> Effects = new();

        public int Id => ConfigId;
    }
}