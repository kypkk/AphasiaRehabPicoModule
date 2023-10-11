using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSync;


namespace GameData
{
    /// <summary>
    /// 遊戲、玩家資訊
    /// </summary>
    public class GameFlowData : LabDataBase
    {
        /// <summary>
        /// 語言
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = "pronounceGame_Test01";

        public GameLevel level = GameLevel.easy;

        public bool AutoRecordVoice { get; set; } = true;
    }
}