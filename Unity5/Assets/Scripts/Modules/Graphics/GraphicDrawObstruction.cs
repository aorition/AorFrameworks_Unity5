using System.Collections.Generic;
using UnityEngine;
//using YoukiaUnity.App;

namespace YoukiaUnity.Graphics
{

    /// <summary>
    /// 图形绘制阻挠器,场景中只要有任何一个阻挠器,图形管理器就不再更新游戏画面(排除UI)
    /// </summary>
    [DisallowMultipleComponent]
    public class GraphicDrawObstruction : MonoBehaviour {

        static List<GraphicDrawObstruction> obstructions=new List<GraphicDrawObstruction>();

        private GraphicsManager _graphicsManager;

        void Awake() {
            if (_graphicsManager == null) {
                _graphicsManager = GameObject.Find("GraphicsManager").GetComponent<GraphicsManager>();
            }
        }

        void check()
        {
            if (obstructions.Count > 0)
                //                YKApplication.Instance.GetManager<GraphicsManager>().StopRender = true;
                _graphicsManager.StopRender = true;
            else
            {
                //                YKApplication.Instance.GetManager<GraphicsManager>().StopRender = false;
                _graphicsManager.StopRender = false;
            }
        }

//        protected override void OnDisable()
        void OnDisable()
        {
//            base.OnDisable();
            obstructions.Remove(this);
            check();
        }

//        protected override void OnEnable()
        void OnEnable()
        {
//            base.OnEnable();
            obstructions.Add(this);
            check();
        }
    }
}
