using System;
using System.Collections;
using UnityEngine;


namespace YoukiaUnity.Graphics
{

    public class GraphicsLauncher : MonoBehaviour
    {
        

        void setupGraphic()
        {
            //建立管理器
            GraphicsManager.CreateInstance();
            //设置绘制卡
            GraphicsManager.GetInstance().SetupDrawCrad = () =>
            {
                BaseDrawCard baseCard = new BaseDrawCard();
                GraphicsManager.GetInstance().DrawCard = baseCard;
            };
            //初始化
            GraphicsManager.GetInstance().Init(null, () =>
            {
                Launcher();
                GlobalCoroutiner.ins.StartCoroutine(delay());
            });
        }

        IEnumerator delay()
        {
            yield return new WaitForEndOfFrame();
            AfterLauncher();
        }

        protected virtual void AfterLauncher()
        {

        }

        //直接使用该MonoBehaviour的协程有风险不触发AfterLauncher，或者报协程不能执行的错误。
        void Awake()
        {

            if (GraphicsManager.GetInstance() != null)
            {

                if (GraphicsManager.isInited)
                {
                    Launcher();
                    GlobalCoroutiner.ins.StartCoroutine(delay());
                }
                else
                {
                    StartAfterGraphicMgr(this, () =>
                    {
                        Launcher();
                        GlobalCoroutiner.ins.StartCoroutine(delay());
                    });
                }

            }
            else
            {
                setupGraphic();
            }


        }

        protected virtual void Launcher()
        {

        }

        //旧方法 有风险
        public static void StartAfterGraphicMgr(MonoBehaviour mono, Action finish)
        {
            if (GraphicsManager.isInited)
            {
                finish();
            }
            else
            {
                mono.StartCoroutine((IEnumerator)waitForMgr(finish));
            }
        }

        public static void StartAfterGraphicMgr(Action finish)
        {
            if (GraphicsManager.isInited)
            {
                finish();
            }
            else
            {
                GlobalCoroutiner.ins.StartCoroutine((IEnumerator)waitForMgr(finish));
            }
        }

        public static IEnumerator waitForMgr(Action finish)
        {
            while (!GraphicsManager.isInited)
            {
                yield return 1;
            }

            finish();
        }
    }
}
