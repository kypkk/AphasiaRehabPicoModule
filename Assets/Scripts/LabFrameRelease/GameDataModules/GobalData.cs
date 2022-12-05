using DataSync;

namespace GameData
{
    public class GlobalData : LabDataBase
    {     
        /// <summary>
        /// 主 UI 場景名稱
        /// </summary>
        public const string MainUiScene = "MainUI";

        /// <summary>
        /// 主 UI 場景名稱
        /// </summary>
        public const string MainScene = "MainScene";


        /* -------------------------------------------------------------------------- */
        /*                               各 Manager 執行優先度                              */
        /* -------------------------------------------------------------------------- */
        
        public const int GameUIManagerWeight = 0;
        public const int GameEntityManagerWeight = 30;
        public const int GameSceneManagerWeight = 50;
        public const int GameTaskManagerWeight = 80;
        public const int GameDataManagerWeight = 100;

    }

}

