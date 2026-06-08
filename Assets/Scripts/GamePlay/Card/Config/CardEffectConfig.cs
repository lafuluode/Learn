using System;

namespace Game.GamePlay.Card
{
    /// <summary>
    /// 单个卡牌效果配置
    /// 
    /// 一张卡牌可以配置多个CardEffectConfig
    /// 例如
    /// 1. 对敌人造成6伤害
    /// 2. 自己抽 1 张牌
    /// </summary>
    [Serializable]
    public class CardEffectConfig
    {
        /// <summary>
        /// 效果类型
        /// 例如 Damage, Heal, DrawCard 等等
        /// </summary>
        public CardEffectType EffectType;

        /// <summary>
        /// 目标类型
        /// 例如 Self, Enemy 等等
        /// </summary>
        public CardTargetType TargetType;
        /// <summary>
        /// 效果参数
        /// Damage 时表示伤害值
        /// Heal 时表示治疗值；
        /// DrawCard 时表示抽牌数量。
        /// </summary>
        public int Value;
    }
}