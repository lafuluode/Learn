using Game.Framework.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GamePlay.Card
{
    /// <summary>
    /// 卡牌配置表中的一行数据
    /// 
    /// 注意：
    /// 1. 这是配置数据，不是战斗运行时数据。
    /// 2. 字段使用 public field，是为了兼容 Unity JsonUtility 反序列化。
    /// 3. 运行时不要随便修改这里的数据。
    /// </summary>
    [Serializable]
    public class CardConfig : IConfigRow<int>
    {
        /// <summary>
        /// 配置表主键
        /// </summary>
        public int ConfigId;
        /// <summary>
        /// 卡牌名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 卡牌消耗
        /// </summary>
        public int Cost;
        /// <summary>
        /// 卡牌描述文本
        /// </summary>
        public string Description;
        /// <summary>
        /// 卡牌效果列表
        /// </summary>
        public List<CardEffectConfig> Effects = new();
        /// <summary>
        /// IConfigRow 要求暴露的配置主键。
        /// ConfigTable 会通过这个 Id 建立字典索引
        /// </summary>
        public int Id => ConfigId;
    }
}