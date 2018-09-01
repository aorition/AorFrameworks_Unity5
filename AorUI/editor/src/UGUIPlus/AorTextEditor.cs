using AorBaseUtility.Extends;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    // TODO REVIEW
    // Have material live under text
    // move stencil mask into effects *make an efects top level element like there is
    // paragraph and character

    /// <summary>
    /// Editor class used to edit UI Labels.
    /// </summary>

    [CustomEditor(typeof(AorText), true)]
    [CanEditMultipleObjects]
    public class AorTextEditEditor : UGUI_Text_SCEditor
    {
        private AorText m_targeText;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_targeText = _target as AorText;
        }

        public override void OnInspectorGUI()
        {

            //spacing
            float spacing_o = (float)m_targeText.ref_GetField_Inst_NonPublic("m_spacing");
            float spacing_n = (float)EditorGUILayout.FloatField("Spacing", spacing_o);
            if (spacing_n != spacing_o)
            {
                m_targeText.ref_SetField_Inst_NonPublic("m_spacing", spacing_n);
                m_isDirty = true;
            }

            //HrefColor
            HrefColor hrefColor_o = (HrefColor)m_targeText.ref_GetField_Inst_NonPublic("_href_Color");
            HrefColor hrefColor_n = (HrefColor)EditorGUILayout.EnumPopup("HrefColor", hrefColor_o);
            if (hrefColor_n != hrefColor_o)
            {
                m_targeText.ref_SetField_Inst_NonPublic("_href_Color", hrefColor_n);
                m_isDirty = true;
            }

            GUILayout.Space(10);

            base.OnInspectorGUI();
        }
    }
}
