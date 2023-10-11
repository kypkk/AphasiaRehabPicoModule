using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
using GameData;
using UnityEngine.UI;

namespace TestGameFrame
{
    /*
    ####################################
    ############ AIOT 參數 ##############
    ####################################
    */
    public class GameParams
    {
        public string userId;
        public string userName; 
        public string CourseType;
        public string CourseParams;
        public string DeviceId;
        public string MotiondataId;
    }
    public class CourseParam
    {

    }
    
    public class Test_MenuUI : MonoBehaviour
    {

        

        public Button SatrtButton;
        public Button easy;
        public Button normal;
        public Button hard;

        public InputField playerUID;

        public Canvas gameLevelCanvas;
        public Canvas gameStartCanvas;

        GanglionController _ganglionController;

        public void Start()
        {

            /*
            #######################################
            ############ AIOT Settings ############
            #######################################
            */ 
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject Activity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = Activity.Call<AndroidJavaObject>("getIntent");
            bool hasExtra = (intent.Call<bool>("hasExtra", "User_Info"));
            if (hasExtra)
            {
                AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
                string User_Info_Str = extras.Call<string>("getString", "User_Info");

                string CourseParamStr = JsonUtility.FromJson<GameParams>(User_Info_Str).CourseParams;
                string MotionDataId = JsonUtility.FromJson<GameParams>(User_Info_Str).MotiondataId;
                string UserId = JsonUtility.FromJson<GameParams>(User_Info_Str).userId;
            }
            //---------------------------- Main UI Script starts from below -------------------------------------

            SatrtButton.onClick.AddListener(StartButtonClick);

            easy.onClick.AddListener(() => levelButtonClick(GameLevel.easy));
            normal.onClick.AddListener(() => levelButtonClick(GameLevel.normal));
            hard.onClick.AddListener(() => levelButtonClick(GameLevel.hard));

            // Ganglion
            DontDestroyOnLoad(GameObject.Find("GanglionController"));
            _ganglionController = GameObject.Find("GanglionController").GetComponent<GanglionController>();
            StartCoroutine(GanglionConnnect());
        }

        public void StartButtonClick()
        {
            GameDataManager.FlowData.UserId = playerUID.text;

            // push lab data
            if (string.IsNullOrWhiteSpace(playerUID.text))
            {
                PromptBox.CreateMessageBox("請填入玩家 ID ！");
                return;
            }

            LabDataTestComponent.LabDataManager.LabDataCollectInit(() => playerUID.text);
            // 遊戲開始前，即可先存GameFlowData
            LabDataTestComponent.LabDataManager.SendData(GameDataManager.FlowData);

            GameSceneManager.Instance.Change2MainScene();
        }

        public void levelButtonClick(GameLevel level)
        {
            GameDataManager.FlowData.level = level;
            gameStartCanvas.gameObject.SetActive(true);
            gameLevelCanvas.gameObject.SetActive(false);
        }
        
        IEnumerator GanglionConnnect()
        {
            while (!_ganglionController.connectionStatus)
            {
                Debug.Log("ganglion reconnect... ");
                _ganglionController.InitGanglion();
                yield return new WaitForSeconds(3f);
            }
            Debug.Log("ganglion connect success!!!");
            SatrtButton.gameObject.SetActive(true);
        }
    }
}
