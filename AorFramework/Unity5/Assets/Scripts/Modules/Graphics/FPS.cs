using UnityEngine;

namespace YoukiaUnity.Graphics
{
    /// <summary>
    /// 帧率统计器
    /// </summary>
    public class FPS : MonoBehaviour
    {

        /// <summary>
        /// 每次刷新计算的时间      帧/秒
        /// </summary>
        public float updateInterval = 0.5f;
        /// <summary>
        /// 最后间隔结束时间
        /// </summary>
        private double lastInterval;
        private int frames = 0;
        public float currFPS;

        private GUIStyle _labelStyle;

        // Use this for initialization
        void Start()
        {

            _labelStyle = new GUIStyle();
            _labelStyle.fontSize = (int)((Screen.width < Screen.height ? Screen.width : Screen.height) / 48);
            _labelStyle.alignment = TextAnchor.UpperLeft;
            _labelStyle.normal.textColor = Color.red;

            lastInterval = Time.realtimeSinceStartup;
            frames = 0;

        }

        // Update is called once per frame
        void Update()
        {

            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                currFPS = (float)(frames / (timeNow - lastInterval));
                frames = 0;
                lastInterval = timeNow;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("FPS:" + currFPS.ToString("f2"), _labelStyle, GUILayout.Height(Screen.height * 0.2f), GUILayout.Width(Screen.width * 0.45f));
        }

    }
}

