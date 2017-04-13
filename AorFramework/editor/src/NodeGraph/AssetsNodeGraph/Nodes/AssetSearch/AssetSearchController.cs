using AorFramework.editor;
using System;
using System.Collections.Generic;

namespace AorFramework.NodeGraph
{
    public class AssetSearchController : NodeController
    {
        public override void update(bool updateParentLoop = true)
        {
            string sp = (string) m_nodeGUI.data.ref_GetField_Inst_Public("SearchPath");
            if (string.IsNullOrEmpty(sp)) return;

            string sp2 = (string)m_nodeGUI.data.ref_GetField_Inst_Public("SearchPattern");
            if (string.IsNullOrEmpty(sp2))
            {
                sp2 = "*.*";
            }

            List<EditorAssetInfo> eai;

            if ((bool) m_nodeGUI.data.ref_GetField_Inst_Public("IgnoreMETAFile"))
            {
                eai = EditorAssetInfo.FindEditorAssetInfoInPath(sp, sp2).filter((e) =>
                {
                    return (e.suffix.ToLower() != ".meta");
                });
            }
            else
            {
                eai = EditorAssetInfo.FindEditorAssetInfoInPath(sp, sp2);
            }

            if (eai != null)
            {
                List<string> assetPathList = new List<string>();

                int i, len = eai.Count;
                for (i = 0; i < len; i++)
                {
                    assetPathList.Add(eai[i].path);
                }

                string[] list = assetPathList.ToArray();

                m_nodeGUI.data.ref_SetField_Inst_Public("AssetsPath", list);

                base.update();
            }

        }
    }
}
