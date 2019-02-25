using System;
using Framework.Extends;
using UnityEngine;
using UnityEngine.UI;

public class IllustraUtility
{

    public static void DrawLine(ref VertexHelper toFill, Vector3 p1, Vector3 p2, Color color, float Cscale = 1.0f, float thickness = 0.1f)
    {

        Vector3[] vtList = CreateLineVertex(p1, p2, Cscale, thickness);

        UIVertex v1 = CreateUIVertex(vtList[0], color.ToColor32(), new Vector2(0,1));
        UIVertex v2 = CreateUIVertex(vtList[1], color.ToColor32(), new Vector2(1,1));
        UIVertex v3 = CreateUIVertex(vtList[2], color.ToColor32(), new Vector2(1,0));
        UIVertex v4 = CreateUIVertex(vtList[3], color.ToColor32(), new Vector2(0,0));

        UIVertex[] qVerts = { v1, v2, v3, v4 };

        toFill.AddUIVertexQuad(qVerts);
    }
    
    public static void DrawMultiLine(ref VertexHelper toFill, Vector3[] plist, Color color, float Cscale = 1.0f, float thickness = 0.1f,bool mergePoint = true)
    {

        if (plist == null || plist.Length < 2) return;
        if (plist.Length < 3)
        {
            DrawLine(ref toFill, plist[0], plist[1], color, Cscale, thickness);
            return;
        }

        Vector3[] _dml_lastLinevt_cache = null;

        for (int i = 0; i < plist.Length - 1; i++)
        {

            if (i == 0)
            {
                //前端点处理
                _dml_lastLinevt_cache = CreateLineVertex(plist[i], plist[i + 1], Cscale, thickness);
                continue;
            }

            Vector3[] linevts_1 = _dml_lastLinevt_cache;
            Vector3[] linevts_2 = CreateLineVertex(plist[i], plist[i + 1], Cscale, thickness);

            Vector3 m_1 = Vector3.zero;
            Vector3 m_2 = Vector3.zero;

            if (mergePoint)
            {
                //先两两求交
                Intersection2DCallback m1cb = Intersection(linevts_1[0], linevts_1[1], linevts_2[0], linevts_2[1]);
                if (m1cb.success)
                {
                    m_1 = new Vector3(m1cb.data.x, m1cb.data.y);
                }
                else
                {
                    Debug.LogWarning("无法找到两线交点");
                    m_1 = linevts_1[1];
                }

                Intersection2DCallback m2cb = Intersection(linevts_1[2], linevts_1[3], linevts_2[2], linevts_2[3]);
                if (m2cb.success)
                {
                    m_2 = new Vector3(m2cb.data.x, m2cb.data.y);
                }
                else
                {
                    Debug.LogWarning("无法找到两线交点");
                    m_2 = linevts_2[3];
                }

            }

            //生成4点
            UIVertex v1 = CreateUIVertex(linevts_1[0], color.ToColor32(), new Vector2(0, 1));
            UIVertex v2 = mergePoint ? CreateUIVertex(m_1, color.ToColor32(), new Vector2(1, 1)) : CreateUIVertex(linevts_1[1], color.ToColor32(), new Vector2(1, 1));
            UIVertex v3 = mergePoint ? CreateUIVertex(m_2, color.ToColor32(), new Vector2(1, 0)) : CreateUIVertex(linevts_1[2], color.ToColor32(), new Vector2(1, 0));
            UIVertex v4 = CreateUIVertex(linevts_1[3], color.ToColor32(), new Vector2(0, 0));

            UIVertex[] qVertices = { v1, v2, v3, v4 };
            toFill.AddUIVertexQuad(qVertices);

            if (i == plist.Length - 2)
            {

                UIVertex v5 = mergePoint ? CreateUIVertex(m_1, color.ToColor32(), new Vector2(0, 1)) : CreateUIVertex(linevts_2[0], color.ToColor32(), new Vector2(0, 1));
                UIVertex v6 = CreateUIVertex(linevts_2[1], color.ToColor32(), new Vector2(1, 1));
                UIVertex v7 = CreateUIVertex(linevts_2[2], color.ToColor32(), new Vector2(1, 0));
                UIVertex v8 = mergePoint ? CreateUIVertex(m_2, color.ToColor32(), new Vector2(0, 0)) : CreateUIVertex(linevts_2[3], color.ToColor32(), new Vector2(0, 0));

                UIVertex[] qVertices2 = { v5, v6, v7, v8 };
                toFill.AddUIVertexQuad(qVertices2);
            }

            _dml_lastLinevt_cache = mergePoint ? new Vector3[] {m_1, linevts_2[1], linevts_2[2], m_2} : linevts_2;

        } 

    }

    public static void DrawClosedMultiLine(ref VertexHelper toFill, Vector3[] plist, Color color, float Cscale = 1.0f, float thickness = 0.1f, bool mergePoint = true)
    {
        if (plist == null || plist.Length < 2) return;
        if (plist.Length < 3)
        {
            DrawLine(ref toFill, plist[0], plist[1], color, Cscale, thickness);
            return;
        }

        Vector3[] _dml_lastLinevt_cache = null;
        Vector3 _cmlCache1 = Vector3.zero;
        Vector3 _cmlCache2 = Vector3.zero;

        for (int i = 0; i < plist.Length; i++)
        {

            if (i == 0)
            {

                Vector3[] tmp = CreateLineVertex(plist[i], plist[i + 1], Cscale, thickness);

                //前端点处理
                if (mergePoint)
                {
                    Vector3[] linevts_s_1 = CreateLineVertex(plist[plist.Length - 1], plist[i], Cscale, thickness);
                    Vector3[] linevts_s_2 = CreateLineVertex(plist[i], plist[i + 1], Cscale, thickness);



                    Intersection2DCallback s1cb = Intersection(linevts_s_1[0], linevts_s_1[1], linevts_s_2[0], linevts_s_2[1]);
                    if (s1cb.success)
                    {
                        _cmlCache1 = s1cb.data;
                    }
                    Intersection2DCallback s2cb = Intersection(linevts_s_1[2], linevts_s_1[3], linevts_s_2[2], linevts_s_2[3]);
                    if (s2cb.success)
                    {
                        _cmlCache2 = s2cb.data;
                    }

                    _dml_lastLinevt_cache = new Vector3[] { _cmlCache1 , linevts_s_2[1], linevts_s_2[2], _cmlCache2 };

                }
                else
                {
                    _dml_lastLinevt_cache = tmp;
                }
                continue;
            }

            Vector3[] linevts_1 = _dml_lastLinevt_cache;
            Vector3[] linevts_2 = (i == plist.Length - 1) ? CreateLineVertex(plist[i], plist[0], Cscale, thickness) : CreateLineVertex(plist[i], plist[i + 1], Cscale, thickness);

            Vector3 m_1 = Vector3.zero;
            Vector3 m_2 = Vector3.zero;

            if (mergePoint)
            {
                //先两两求交
                Intersection2DCallback m1cb = Intersection(linevts_1[0], linevts_1[1], linevts_2[0], linevts_2[1]);
                if (m1cb.success)
                {
                    m_1 = new Vector3(m1cb.data.x, m1cb.data.y);
                }
                else
                {
                    Debug.LogWarning("无法找到两线交点");
                    m_1 = linevts_1[1];
                }

                Intersection2DCallback m2cb = Intersection(linevts_1[2], linevts_1[3], linevts_2[2], linevts_2[3]);
                if (m2cb.success)
                {
                    m_2 = new Vector3(m2cb.data.x, m2cb.data.y);
                }
                else
                {
                    Debug.LogWarning("无法找到两线交点");
                    m_2 = linevts_2[3];
                }

            }

            //生成4点
            UIVertex v1 = CreateUIVertex(linevts_1[0], color.ToColor32(), new Vector2(0, 1));
            UIVertex v2 = mergePoint ? CreateUIVertex(m_1, color.ToColor32(), new Vector2(1, 1)) : CreateUIVertex(linevts_1[1], color.ToColor32(), new Vector2(1, 1));
            UIVertex v3 = mergePoint ? CreateUIVertex(m_2, color.ToColor32(), new Vector2(1, 0)) : CreateUIVertex(linevts_1[2], color.ToColor32(), new Vector2(1, 0));
            UIVertex v4 = CreateUIVertex(linevts_1[3], color.ToColor32(), new Vector2(0, 0));

            UIVertex[] qVertices = { v1, v2, v3, v4 };
            toFill.AddUIVertexQuad(qVertices);

            if (i == plist.Length - 1)
            {
                //闭合线处理
                UIVertex v5 = mergePoint ? CreateUIVertex(m_1, color.ToColor32(), new Vector2(0, 1)) : CreateUIVertex(linevts_2[0], color.ToColor32(), new Vector2(0, 1));
                UIVertex v6 = mergePoint ? CreateUIVertex(_cmlCache1, color.ToColor32(), new Vector2(1, 1)) : CreateUIVertex(linevts_2[1], color.ToColor32(), new Vector2(1, 1));
                UIVertex v7 = mergePoint ? CreateUIVertex(_cmlCache2, color.ToColor32(), new Vector2(1, 0)) : CreateUIVertex(linevts_2[2], color.ToColor32(), new Vector2(1, 0));
                UIVertex v8 = mergePoint ? CreateUIVertex(m_2, color.ToColor32(), new Vector2(0, 0)) : CreateUIVertex(linevts_2[3], color.ToColor32(), new Vector2(0, 0));

                UIVertex[] qVertices2 = { v5, v6, v7, v8 };
                toFill.AddUIVertexQuad(qVertices2);
            }

            _dml_lastLinevt_cache = mergePoint ? new Vector3[] { m_1, linevts_2[1], linevts_2[2], m_2 } : linevts_2;

        }
    }

    public static void DrawCircularLine(ref VertexHelper toFill, Vector3 center, float radius, Color color, int segments = 48, float Cscale = 1.0f, float thickness = 0.1f, bool mergePoint = true)
    {
        float deg = 360f/segments;
        Vector3[] vlist = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            vlist[i] = new Vector3(
                 Mathf.Cos(i * deg * Mathf.PI / 180) * radius + center.x,
                 Mathf.Sin(i * deg * Mathf.PI / 180) * radius + center.y
                );
        }
        DrawClosedMultiLine(ref toFill, vlist, color, Cscale, thickness, mergePoint);
    }

    public static void DrawRectangleLine(ref VertexHelper toFill, Vector3 min, Vector3 max, Color color, float Cscale = 1.0f, float thickness = 0.1f, bool mergePoint = true)
    {
        Vector3[] vlist = new Vector3[]
        {
            min,
            new Vector3(min.x,max.y),
            max,
            new Vector3(max.x,min.y)    
        };
        DrawClosedMultiLine(ref toFill, vlist, color, Cscale, thickness, mergePoint);
    }

    ///////////////////////////////

    #region CreateUIVertex

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color)
    {
        UIVertex v = UIVertex.simpleVert;
        v.position = pos;
        v.color = color;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector2 uv0)
    {
        UIVertex v = CreateUIVertex(pos, color);
        v.uv0 = uv0;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal)
    {
        UIVertex v = CreateUIVertex(pos, color);
        v.normal = normal;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal, Vector4 tangent)
    {
        UIVertex v = CreateUIVertex(pos, color, normal);
        v.tangent = tangent;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal, Vector4 tangent, Vector2 uv0)
    {
        UIVertex v = CreateUIVertex(pos, color, normal, tangent);
        v.uv0 = uv0;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal, Vector4 tangent, Vector2 uv0,
        Vector2 uv1)
    {
        UIVertex v = CreateUIVertex(pos, color, normal, tangent, uv0);
        v.uv1 = uv1;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal, Vector4 tangent, Vector2 uv0,
        Vector2 uv1, Vector2 uv2)
    {
        UIVertex v = CreateUIVertex(pos, color, normal, tangent, uv0, uv1);
        v.uv2 = uv2;
        return v;
    }

    public static UIVertex CreateUIVertex(Vector3 pos, Color32 color, Vector3 normal, Vector4 tangent, Vector2 uv0,
        Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        UIVertex v = CreateUIVertex(pos, color, normal, tangent, uv0, uv1, uv2);
        v.uv3 = uv3;
        return v;
    }

    #endregion

    public static Vector3[] CreateLineVertex(Vector3 p1, Vector3 p2, float Cscale = 1.0f, float thickness = 0.1f)
    {
        //计算方向
        Vector3 dir = (p2 - p1).normalized;
        //生成4点
        return new Vector3[]
        {
            (p1 + Quaternion.AngleAxis(90, Vector3.forward) * dir * thickness) * Cscale,
            (p2 + Quaternion.AngleAxis(90, Vector3.forward) * dir * thickness) * Cscale,
            (p2 + Quaternion.AngleAxis(-90, Vector3.forward) * dir * thickness) * Cscale,
            (p1 + Quaternion.AngleAxis(-90, Vector3.forward) * dir * thickness) * Cscale
        };
    }

    /// <summary>
    /// 线段延长(2D)
    /// </summary>
    /// <param name="SP">线段起始点</param>
    /// <param name="EP">线段结束点</param>
    /// <param name="ex">延长点的x坐标</param>
    /// <returns>延长点的y坐标</returns>
    public static float ExtendLineEP(Vector2 SP, Vector2 EP, float ex)
    {
        return (EP.y - SP.y) / (EP.x - SP.x) * (ex - SP.x) + SP.y;
    }

    public struct Intersection2DCallback
    {
        public Intersection2DCallback(bool s, Vector2 d)
        {
            this.success = s;
            this.data = d;
        }

        public Intersection2DCallback(bool s)
        {
            this.success = s;
            this.data = Vector2.zero;
        }

        public bool success;
        public Vector2 data;
    }

    /// <summary>
    /// 两直线求交点(2D)
    /// </summary>
    /// <param name="a">线段1起始点</param>
    /// <param name="b">线段1结束点</param>
    /// <param name="c">线段2起始点</param>
    /// <param name="d">线段2结束点</param>
    /// <returns>Intersection2DCallback数据,表示是否找到交点以及交点数据</returns>
    public static Intersection2DCallback Intersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float D = (b.x - a.x) * (c.y - d.y)
            - (d.x - c.x) * (a.y - b.y);
        float D1 = (c.y * d.x - c.x * d.y)
            * (b.x - a.x)
            - (a.y * b.x - a.x * b.y)
            * (d.x - c.x);
        float D2 = (a.y * b.x - a.x * b.y)
            * (c.y - d.y)
            - (c.y * d.x - c.x * d.y)
            * (a.y - b.y);

        if (D1 != 0 && D2 != 0)
        {
            return new Intersection2DCallback(true, new Vector2(D1/D, D2/D));
        }

        return new Intersection2DCallback(false);

    }

}
