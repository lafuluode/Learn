using System;
using System.Collections;
using System.Collections.Generic;
using Game.Framework.Config;
using UnityEngine;

namespace Game.GamePlay.Card
{
    /// <summary>
    /// 卡牌配置表数据源
    /// 
    /// 这是为了适配 ConfigService的泛型加载流程;
    /// ConfigService 会先反序列化出 CardConfigTableData，
    /// 再通过 GetRows() 获取所有 CardConfig 行数据。
    /// </summary>
    [Serializable]
    public class CardConfigTableData: IConfigTableSource<CardConfig,int>
    {
        /// <summary>
        /// 所有卡牌配置行
        /// 
        /// 注意：
        /// 字段名 Cards 需要和 Json 文件里的字段名保持一致。 
        /// </summary>
        public List<CardConfig> Cards = new();
        public IEnumerable<CardConfig> GetRows()
        {
            return Cards;
        }
    }
}
