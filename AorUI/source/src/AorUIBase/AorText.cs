using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AorBaseUtility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum HrefColor
{
    blue,
    red,
    green,
}

/// <summary>
/// 
/// AorText ::
/// 
/// 已知问题: 
///     1. Spacing > 1 时, 自动换行不准确.
///     2. 多行 && Spacing > 1 时, 偶见部分行发生偏移.
///     3. 多行 && Spacing > 1 时, 富文本<color>标签会发生多余的Spacing偏移(包括<a>标签)
/// 
/// </summary>
[AddComponentMenu("UI/AorText", 10)]
[ExecuteInEditMode]
public class AorText : UGUI_Text_SC, IPointerClickHandler
{

    [Serializable]
    public class HrefClickEvent : UnityEvent<string> { }

    /// <summary>
    /// 超链接信息类
    /// </summary>
    private class HrefInfo
    {
        public int startIndex;

        public int endIndex;

        public string name;

        public readonly List<Rect> boxes = new List<Rect>();
    }

    private static readonly Regex s_tag_b_Regex = new Regex(@"<b>(.*?)(</b>)", RegexOptions.Singleline);
    private static readonly Regex s_tag_i_Regex = new Regex(@"<i>(.*?)(</i>)", RegexOptions.Singleline);
    private static readonly Regex s_tag_size_Regex = new Regex(@"<size=([^>\n\s]+)>(.*?)(</size>)", RegexOptions.Singleline);
    private static readonly Regex s_tag_color_Regex = new Regex(@"<color=([^>\n\s]+)>(.*?)(</color>)", RegexOptions.Singleline);

    /// <summary>
    /// 超链接信息列表
    /// </summary>
    private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

    /// <summary>
    /// 超链接正则
    /// </summary>
    private static readonly Regex s_HrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

    [SerializeField]
    private HrefClickEvent m_OnHrefClick = new HrefClickEvent();
    /// <summary>
    /// 超链接点击事件
    /// </summary>
    public HrefClickEvent onHrefClick
    {
        get { return m_OnHrefClick; }
        set { m_OnHrefClick = value; }
    }

    

    // 超链接颜色
    protected HrefColor _href_Color = HrefColor.blue;
    public HrefColor HrefColor
    {
        get { return _href_Color; }
        set { _href_Color = value; }
    }

    [SerializeField]
    protected float m_spacing = 0f;
    public float spacing
    {
        get { return m_spacing; }
        set
        {
            if (m_spacing == value) return;
            m_spacing = value;
            this.SetVerticesDirty();
        }
    }

    public override float preferredHeight
    {
        get
        {
            var settings = GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0.0f));
            return cachedTextGeneratorForLayout.GetPreferredHeight(GetOutputText(), settings) / pixelsPerUnit;

        }
    }

    public override float preferredWidth
    {
        get
        {
            var settings = GetGenerationSettings(Vector2.zero);
            return cachedTextGeneratorForLayout.GetPreferredWidth(GetOutputText(), settings) / pixelsPerUnit;
        }
    }

    protected string SupportBRTag(string tex)
    {
        return tex.Replace("<br>", "\n").Replace("<br />", "\n").Replace("<br/>", "\n").Replace("<BR>", "\n").Replace("<BR />", "\n").Replace("<BR/>", "\n");
    }

    /// <summary>
    /// 获取超链接解析后的最后输出文本
    /// </summary>
    /// <returns></returns>
    protected string GetOutputText()
    {
        m_HrefInfos.Clear();
        string srcText = text;

        //这里处理支持<br>|<br />|<br/>标签
        srcText = SupportBRTag(srcText);

        RegexMatchLoop(s_HrefRegex, ref srcText, (m) =>
        {
            int mIdx = m.Index;
            string url = m.Result("$1");
            string n = m.Result("$2");
            string nStr = "<color=" + _href_Color + ">" + n + "</color>";
            srcText = srcText.Replace(m.Value, nStr);
            var hrefInfo = new HrefInfo
            {
                startIndex = mIdx * 4, // 超链接里的文本起始顶点索引
                endIndex = (mIdx + nStr.Length - 1) * 4 + 3,
                name = url
            };
            m_HrefInfos.Add(hrefInfo);
        });
        return srcText;
    }

    private void RegexMatchLoop(Regex regex, ref string MatchSrcStr, Action<Match> OnSuccessFunc)
    {
        Match match = regex.Match(MatchSrcStr);
        if (match.Success)
        {
            OnSuccessFunc(match);
            if (match.NextMatch().Success)
            {
                RegexMatchLoop(regex, ref MatchSrcStr, OnSuccessFunc);
            }
        }
    }

    protected int[] GetTextFilter(string srcText)
    {

        srcText = srcText.Replace("\n", "|");
        int[] filter = new int[srcText.Length];
        for (int i = 0; i < filter.Length; i++)
        {
            filter[i] = srcText[i] == '|' ? 2 : 1;
        } 

        //过滤<b>
        RegexMatchLoop(s_tag_b_Regex, ref srcText, (m) =>
        {
            int start = m.Index;
            int end = m.Groups[1].Index;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
            string n = m.Result("$1");
            start = m.Groups[1].Index + n.Length;
            end = m.Index + m.Value.Length;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
        });
        //过滤<i>
        RegexMatchLoop(s_tag_i_Regex, ref srcText, (m) =>
        {
            int start = m.Index;
            int end = m.Groups[1].Index;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
            string n = m.Result("$1");
            start = m.Groups[1].Index + n.Length;
            end = m.Index + m.Value.Length;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
        });
        //过滤<size>
        RegexMatchLoop(s_tag_size_Regex, ref srcText, (m) =>
        {
            int start = m.Index;
            int end = m.Groups[1].Index;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
            string n = m.Result("$2");
            start = m.Groups[2].Index + n.Length;
            end = m.Index + m.Value.Length;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
        });
        //过滤<color>
        RegexMatchLoop(s_tag_color_Regex, ref srcText, (m) =>
        {
            int start = m.Index;
            int end = m.Groups[1].Index;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
            string n = m.Result("$2");
            start = m.Groups[2].Index + n.Length;
            end = m.Index + m.Value.Length;
            for (int i = start; i < end; i++)
            {
                filter[i] = 0;
            }
        });
        return filter;
    }

    protected string GetDisplayText(string srcText)
    {
        //过滤<b>
        RegexMatchLoop(s_tag_b_Regex, ref srcText, (m) =>
        {
            srcText = srcText.Replace(m.Value, m.Result("$1"));
        });
        //过滤<i>
        RegexMatchLoop(s_tag_i_Regex, ref srcText, (m) =>
        {
            srcText = srcText.Replace(m.Value, m.Result("$1"));
        });
        //过滤<size>
        RegexMatchLoop(s_tag_size_Regex, ref srcText, (m) =>
        {
            srcText = srcText.Replace(m.Value, m.Result("$2"));
        });
        //过滤<color>
        RegexMatchLoop(s_tag_color_Regex, ref srcText, (m) =>
        {
            srcText = srcText.Replace(m.Value, m.Result("$2"));
        });
        return srcText;
    }

    /// <summary>
    /// 点击事件检测是否点击到超链接文本
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        foreach (var hrefInfo in m_HrefInfos)
        {
            var boxes = hrefInfo.boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    m_OnHrefClick.Invoke(hrefInfo.name);
                    return;
                }
            }
        }
    }

    protected List<List<char>> textLayoutMap = new List<List<char>>();

    protected override void OnPopulateMeshDo (VertexHelper toFill)
    {

        #region base.OnPopulateMesh 源码实现 HACK
        
        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        string buildText = supportRichText ? GetOutputText() : m_Text;

        string mapStr = buildText.Replace("\n", "");
        textLayoutMap.Clear();
        

        int strMapId = 0;
        int mapYIdx = -1;
        float lineYCache = float.MaxValue;
        

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.PopulateWithErrors(buildText, settings, gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        float lineYOffset = lineSpacing * (float)fontSize / unitsPerPixel;
        //Last 4 verts are always a new line... (\n)
        int vertCount = verts.Count - 4;
        bool initTextMap = true;
        char lastReadChar = char.MinValue;

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);

                    if (m_TempVerts[tempVertsIndex].position.y + lineYOffset < lineYCache)
                    {

                        lineYCache = m_TempVerts[tempVertsIndex].position.y;

                        textLayoutMap.Add(new List<char>());
                        mapYIdx++;

                        if (initTextMap)
                        {
                            initTextMap = false;
                        }
                        else
                        {
                            continue;
                        }

                    }

                    if (strMapId < mapStr.Length)
                    {
                        textLayoutMap[mapYIdx].Add(mapStr[strMapId]);
                        strMapId++;
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);

                    if (m_TempVerts[tempVertsIndex].position.y + lineYOffset < lineYCache)
                    {

                        lineYCache = m_TempVerts[tempVertsIndex].position.y;

                        textLayoutMap.Add(new List<char>());
                        mapYIdx ++;

                        if (initTextMap)
                        {
                            initTextMap = false;
                        }
                        else
                        {
                            continue;
                        }

                    }

                    if (strMapId < mapStr.Length)
                    {

                        //靠顶点判定自动换行有个极其坑爹的问题: 在RichText模式下,原生cachedTextGenerator会吧"<"(尖括号)留在上一行... 造成map采集错乱
                        //只能代码判断并校正问题了
                        if (supportRichText && lastReadChar == '<' && textLayoutMap[mapYIdx].Count == 0)
                        {
                            if (mapYIdx - 1 >= 0)
                            {
                                textLayoutMap[mapYIdx - 1].RemoveAt(textLayoutMap[mapYIdx - 1].Count - 1);
                                textLayoutMap[mapYIdx].Add(lastReadChar);
                            }
                        }

                        textLayoutMap[mapYIdx].Add(mapStr[strMapId]);
                        lastReadChar = mapStr[strMapId];
                        strMapId++;
                    }
                }
            }
        }

        #endregion
        
        //Spacing 间距处理
        SpacingPopulateMesh(toFill);

        if (!supportRichText) return;

        // 处理超链接包围框
        foreach (var hrefInfo in m_HrefInfos)
        {
            hrefInfo.boxes.Clear();

            if (hrefInfo.startIndex >= toFill.currentVertCount)
            {
                continue;
            }

            // 将超链接里面的文本顶点索引坐标加入到包围框
            UIVertex uiv = new UIVertex();
            toFill.PopulateUIVertex(ref uiv, hrefInfo.startIndex);
            var p = uiv.position;
            var bounds = new Bounds(p, Vector3.zero);
            for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }
                UIVertex upVt = new UIVertex();
                toFill.PopulateUIVertex(ref upVt, i);

                p = upVt.position;
                if (p.x < bounds.min.x) // 换行重新添加包围框
                {

                    hrefInfo.boxes.Add(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
                    bounds = new Bounds(p, Vector3.zero);
                }
                else
                {
                    bounds.Encapsulate(p); // 扩展包围框
                }
            }

            hrefInfo.boxes.Add(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
        }

    }

    /// <summary>
    /// 从textLayoutMap中取出字符串
    /// </summary>
    /// <returns></returns>
    private string GetBuildTextFormMap()
    {
        if (textLayoutMap == null) return null;

        string o = String.Empty;

        for (int i = 0; i < textLayoutMap.Count; i++)
        {
            for (int j = 0; j < textLayoutMap[i].Count; j++)
            {
                o += textLayoutMap[i][j];
            }
            o += "\n";
        }

        return o;
    }

    private void SpacingPopulateMesh(VertexHelper toFill)
    {
        string buildText = GetBuildTextFormMap();
        string[] lines = buildText.Split('\n');
        string[] DLines = supportRichText ? GetDisplayText(buildText).Split('\n') : buildText.Split('\n');

        int[] flist = GetTextFilter(buildText);
        float letterOffset = spacing * (float)fontSize / 100f;
        Vector3 pos;
        
        float alignmentFactor = 0;
        switch (alignment)
        {
            case TextAnchor.LowerLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.UpperLeft:
                alignmentFactor = 0f;
                break;

            case TextAnchor.LowerCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.UpperCenter:
                alignmentFactor = 0.5f;
                break;

            case TextAnchor.LowerRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.UpperRight:
                alignmentFactor = 1f;
                break;
        }

        int glyphIdx = 0;

        for (int lineIdx = 0; lineIdx < textLayoutMap.Count; lineIdx++)
        {
            string line = lines[lineIdx];
            string DLine = DLines[lineIdx];

            int DcharIdx = 0;
    
            float lineOffset = (DLine.Length - 1) * letterOffset * alignmentFactor;

            for (int charIdx = 0; charIdx < line.Length; charIdx++)
            {

                // Check for truncated text (doesn't generate verts for all characters)
                if (glyphIdx * 4 + 3 > toFill.currentVertCount - 1) return;

                if (flist[glyphIdx] == 1)
                {

                    pos = Vector3.right * (letterOffset * DcharIdx - lineOffset);

                    int idx;
                    for (int i = 0; i < 4; i++)
                    {
                        idx = glyphIdx * 4 + i;
                        UIVertex vert = new UIVertex();
                        toFill.PopulateUIVertex(ref vert, idx);
                        vert.position += pos;
                        toFill.SetUIVertex(vert, idx);
                    }

                    if(DcharIdx + 1 < DLine.Length)
                        DcharIdx++;
                }

                //过滤\n
                glyphIdx++;

            }

            // Offset for carriage return character that still generates verts
            glyphIdx++;
        }

    }

}
