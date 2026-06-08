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
        public IEnumerable<CardConfig> GetRows()
        {
            throw new System.NotImplementedException();
        }
    }
}
