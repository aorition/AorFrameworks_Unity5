using AorBaseUtility;
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

    [CustomEditor(typeof(UGUI_Text_SC), true)]
    [CanEditMultipleObjects]
    public class UGUI_Text_SCEditor : GraphicEditor
    {
        protected SerializedProperty m_Text;
        protected FontData mm_FontData;

        protected UGUI_Text_SC _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            mm_FontData = (FontData) target.ref_GetField_Inst_NonPublic("m_FontData");

            _target = target as UGUI_Text_SC;
        }

        protected bool m_isDirty = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);

            //Font
            Font font_o = (Font)mm_FontData.ref_GetField_Inst_NonPublic("m_Font");
            Font font_n = (Font) EditorGUILayout.ObjectField("Font", font_o, typeof (Font), false);
            if (font_n != font_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_Font", font_n);
                _target.ref_InvokeMethod_Inst_NonPublic("OnValidate", null);
                m_isDirty = true;
            }

            //Font Style
            FontStyle fontStyle_o = (FontStyle)mm_FontData.ref_GetField_Inst_NonPublic("m_FontStyle");
            FontStyle fontStyle_n = (FontStyle) EditorGUILayout.EnumPopup("Font Style", fontStyle_o);
            if (fontStyle_n != fontStyle_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_FontStyle", fontStyle_n);
                m_isDirty = true;
            }

            //Font Size
            int fontSize_o = (int)mm_FontData.ref_GetField_Inst_NonPublic("m_FontSize");
            int fontSize_n = (int)EditorGUILayout.IntField("Font Size", fontSize_o);
            if (fontSize_n != fontSize_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_FontSize", fontSize_n);
                m_isDirty = true;
            }

            //Line Spacing
            float lineSpacing_o = (float)mm_FontData.ref_GetField_Inst_NonPublic("m_LineSpacing");
            float lineSpacing_n = (float)EditorGUILayout.FloatField("Line Spacing", lineSpacing_o);
            if (lineSpacing_n != lineSpacing_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_LineSpacing", lineSpacing_n);
                m_isDirty = true;
            }

            //Rich Text
            bool richText_o = (bool)mm_FontData.ref_GetField_Inst_NonPublic("m_RichText");
            bool richText_n = (bool)EditorGUILayout.Toggle("Rich Text", richText_o);
            if (richText_n != richText_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_RichText", richText_n);
                m_isDirty = true;
            }

            GUILayout.Space(5);

            //Alignment
            TextAnchor alignment_o = (TextAnchor)mm_FontData.ref_GetField_Inst_NonPublic("m_Alignment");
            TextAnchor alignment_n = (TextAnchor) EditorGUILayout.EnumPopup("Alignment", alignment_o);
            if (alignment_n != alignment_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_Alignment", alignment_n);
                m_isDirty = true;
            }

            //Align By Geomertry
            bool alignByGeomertry_o = (bool)mm_FontData.ref_GetField_Inst_NonPublic("m_AlignByGeometry");
            bool alignByGeomertry_n = (bool)EditorGUILayout.Toggle("Align By Geomertry",alignByGeomertry_o);
            if (alignByGeomertry_n != alignByGeomertry_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_AlignByGeometry", alignByGeomertry_n);
                m_isDirty = true;
            }

            //Horizontal Overflow
            HorizontalWrapMode horizontalOverflow_o = (HorizontalWrapMode)mm_FontData.ref_GetField_Inst_NonPublic("m_HorizontalOverflow");
            HorizontalWrapMode horizontalOverflow_n = (HorizontalWrapMode)EditorGUILayout.EnumPopup("Horizontal Overflow",horizontalOverflow_o);
            if (horizontalOverflow_n != horizontalOverflow_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_HorizontalOverflow", horizontalOverflow_n);
                m_isDirty = true;
            }

            //Vertical Overflow
            VerticalWrapMode verticalOverflow_o = (VerticalWrapMode)mm_FontData.ref_GetField_Inst_NonPublic("m_VerticalOverflow");
            VerticalWrapMode verticalOverflow_n = (VerticalWrapMode)EditorGUILayout.EnumPopup("Vertical Overflow",verticalOverflow_o);
            if (verticalOverflow_n != verticalOverflow_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_VerticalOverflow", verticalOverflow_n);
                m_isDirty = true;
            }

            //Best Fit
            bool bestFit_o = (bool)mm_FontData.ref_GetField_Inst_NonPublic("m_BestFit");
            bool bestFit_n = (bool)EditorGUILayout.Toggle("Best Fit", bestFit_o);
            if (bestFit_n != bestFit_o)
            {
                mm_FontData.ref_SetField_Inst_NonPublic("m_BestFit", bestFit_n);
                m_isDirty = true;
            }

            if (bestFit_n)
            {

                //Min Size
                int minSize_o = (int)mm_FontData.ref_GetField_Inst_NonPublic("m_MinSize");
                int minSize_n = (int)EditorGUILayout.IntField("Min Size", minSize_o);
                if (minSize_n != minSize_o)
                {
                    mm_FontData.ref_SetField_Inst_NonPublic("m_MinSize", minSize_n);
                    m_isDirty = true;
                }

                //Max Size
                int maxSize_o = (int)mm_FontData.ref_GetField_Inst_NonPublic("m_MaxSize");
                int maxSize_n = (int)EditorGUILayout.IntField("Max Size",maxSize_o);
                if (maxSize_n != maxSize_o)
                {
                    mm_FontData.ref_SetField_Inst_NonPublic("m_MaxSize", maxSize_n);
                    m_isDirty = true;
                }

            }

            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();

            if (m_isDirty)
            {
                _target.ref_InvokeMethod_Inst_NonPublic("OnRebuildRequested", null);
//                _target.SetAllDirty();
                _target.ref_InvokeMethod_Inst_Public("SetAllDirty", null);
                EditorUtility.SetDirty(_target);
                m_isDirty = false;
            }

        }
    }
}
