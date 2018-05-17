using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 图文混排Text组件. 支持Unity版本 5.6
/// Author : Aorition
/// 
/// 重写代码, (未验证图形功能)
/// 
/// </summary>
public class AorTextIncSprites : AorText {

    [SerializeField]
    private bool _supportSpriteGraphic;
    public bool supportSpriteGraphic {
        get { return _supportSpriteGraphic; }
        set {
            if (Application.isEditor) {
                _supportSpriteGraphic = value;
                switchSpriteGraphic();
            }
            else {
                if (value != _supportSpriteGraphic) {
                    _supportSpriteGraphic = value;
                    switchSpriteGraphic();
                }
            }
        }
    }

    [SerializeField]
    private Material _spriteGraphic_Material;
    public Material spriteGraphic_Material {
        get {
            return _spriteGraphic_Material;
        }
        set {
            _spriteGraphic_Material = value;

            if (_spriteGraphic_Material != null && m_spriteGraphic != null) {
                m_spriteGraphic.material = _spriteGraphic_Material;
            }

        }
    }

    [SerializeField]
    private Color _spriteGraphic_Color;
    public Color spriteGraphic_Color {
        get {
            return _spriteGraphic_Color;
        }
        set {
            _spriteGraphic_Color = value;

            if (m_spriteGraphic != null) {
                m_spriteGraphic.color = _spriteGraphic_Color;
            }

        }
    }
    [SerializeField]
    private SpriteAsset _spriteGraphic_SpriteAsset;

    public SpriteAsset spriteGraphic_SpriteAsset {
        get {
            return _spriteGraphic_SpriteAsset;
        }
        set {
            _spriteGraphic_SpriteAsset = value;
            if (_spriteGraphic_SpriteAsset != null && m_spriteGraphic != null) {
                m_spriteGraphic.m_spriteAsset = _spriteGraphic_SpriteAsset;
            }
        }
    }

    public int id;

    private void switchSpriteGraphic() {
        if (_supportSpriteGraphic) {
            spriteGraphicInit();
        } else {
            if (m_spriteGraphic != null) {
                m_spriteGraphic.gameObject.Dispose();
                m_spriteGraphic = null;
            }
        }
    }

    private void spriteGraphicInit() {
        if (m_spriteGraphic == null) {
            m_spriteGraphic = transform.GetComponentInChildren<SpriteGraphic>();

            if (m_spriteGraphic == null) {
                GameObject sgGo = CreatePrefab_UIBase(transform, 0, 0, 0, 0, 0, 0, 1f, 1f, 0, 1f);
                sgGo.name = "spriteGraphic#";
                m_spriteGraphic = sgGo.AddComponent<SpriteGraphic>();
            }
        }
    }

    protected override void OnEnable() {
        base.OnEnable();

        if (_CanvasRenderer == null) {
            _CanvasRenderer = GetComponent<CanvasRenderer>();
        }
        if (_supportSpriteGraphic) {
            spriteGraphicInit();
        }

        if (_spriteGraphic_SpriteAsset != null && _spriteGraphic_Material != null && m_spriteGraphic != null) {
            m_spriteGraphic.m_spriteAsset = _spriteGraphic_SpriteAsset;
            m_spriteGraphic.material = _spriteGraphic_Material;
            m_spriteGraphic.color = _spriteGraphic_Color;
        }

        StartCoroutine(waitingForUpdate());
    }

    private IEnumerator waitingForUpdate() {
        yield return new WaitForEndOfFrame();
        SetAllDirty();
    }

    /// <summary>
    /// 用正则取标签属性 名称-大小-宽度比例
    /// </summary>
    private static readonly Regex SpriteRegex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) />", RegexOptions.Singleline);

    private CanvasRenderer _CanvasRenderer;

    /// <summary>
    /// 图片资源
    /// </summary>
    [SerializeField]
    private SpriteGraphic m_spriteGraphic;

    /// <summary>
    /// 标签的信息列表
    /// </summary>
    private List<TextPlusDataSpriteInfo> listTagInfo = new List<TextPlusDataSpriteInfo>();

    public override void SetAllDirty() {

        //解析标签属性
        listTagInfo = new List<TextPlusDataSpriteInfo>();
        MatchCollection mc = SpriteRegex.Matches(text);
        foreach (Match match in mc) {

            TextPlusDataSpriteInfo info = new TextPlusDataSpriteInfo(
                match.Index,
                match.Length,
                match.Groups[1].Value,
                int.Parse(match.Groups[2].Value)
                );
            listTagInfo.Add(info);
        }

        if (m_spriteGraphic != null) {
            if (listTagInfo.Count == 0) {
                m_spriteGraphic.gameObject.SetActive(false);
            } else {
                m_spriteGraphic.gameObject.SetActive(true);
            }
        }

        base.SetAllDirty();
    }

    protected override void OnPopulateMeshDo (VertexHelper toFill)
    {

        if (!supportRichText) {
            return;
        }

        //计算图片信息
        List<UIVertex> spriteUiVertices = new List<UIVertex>();

        for (int i = 0; i < listTagInfo.Count; i++) {

            UIVertex[] spUiVertex = new UIVertex[4];
            int spUiVertexIndex = 0;

            //UGUIText不支持<quad/>标签，表现为乱码，我这里将他的uv全设置为0,清除乱码
            int spStartNum = listTagInfo[i].index * 4;
            int spMaxNum = listTagInfo[i].index * 4 + listTagInfo[i].length * 4;

            if (toFill.currentVertCount >= spStartNum && toFill.currentVertCount >= spMaxNum) {

                for (int m = spStartNum; m < spMaxNum; m++) {

//                    UIVertex tempVertex = vbo[m];
                    UIVertex tempVertex = new UIVertex();
                    toFill.PopulateUIVertex(ref tempVertex, m);
                    tempVertex.uv0 = Vector2.zero;
                    //vbo[m] = tempVertex;
                    toFill.SetUIVertex(tempVertex, m);

                    if (m >= (spMaxNum - 4)) {
                        int id = m - (spMaxNum - 4);
                        spUiVertex[id] = tempVertex;
                    }
                }

                //计算其uv
                if (_spriteGraphic_SpriteAsset != null) {
                    Rect spriteRect = _spriteGraphic_SpriteAsset.listSpriteInfo[0].rect;
                    for (int j = 0; j < _spriteGraphic_SpriteAsset.listSpriteInfo.Count; j++) {
                        //通过标签的名称去索引spriteAsset里所对应的sprite的名称
                        if (listTagInfo[i].name == _spriteGraphic_SpriteAsset.listSpriteInfo[j].name) {
                            spriteRect = _spriteGraphic_SpriteAsset.listSpriteInfo[j].rect;
                            Vector2 texSize = new Vector2(_spriteGraphic_SpriteAsset.texSource.width, _spriteGraphic_SpriteAsset.texSource.height);

                            spUiVertex[0].uv0 = new Vector2(spriteRect.x / texSize.x, (spriteRect.y + spriteRect.height) / texSize.y);
                            spUiVertex[1].uv0 = new Vector2((spriteRect.x + spriteRect.width) / texSize.x, (spriteRect.y + spriteRect.height) / texSize.y);
                            spUiVertex[2].uv0 = new Vector2((spriteRect.x + spriteRect.width) / texSize.x, spriteRect.y / texSize.y);
                            spUiVertex[3].uv0 = new Vector2(spriteRect.x / texSize.x, spriteRect.y / texSize.y);

                            break;
                        }
                    }
                }
                spriteUiVertices.AddRange(spUiVertex);

            }
        }

        if (spriteUiVertices.Count > 0 && m_spriteGraphic != null) {
            m_spriteGraphic.updateFBV(spriteUiVertices);
        }

    }

    /// <summary>
    /// 创建基础UIGameObject
    /// 包含组件[RectTransform]
    /// </summary>
    private static GameObject CreatePrefab_UIBase(Transform parent = null,
                                                    float x = 0, float y = 0, float w = 0, float h = 0,
                                                    float anchorsMinX = 0, float anchorsMinY = 0,
                                                    float anchorsMaxX = 1f, float anchorsMaxY = 1f,
                                                    float pivotX = 0.5f, float pivotY = 0.5f) {
        GameObject _base = new GameObject();
        _base.layer = 5;

        if (parent != null) {
            _base.transform.SetParent(parent, false);
        }

        RectTransform rt = _base.AddComponent<RectTransform>();

        rt.pivot = new Vector2(pivotX, pivotY);
        rt.anchorMin = new Vector2(anchorsMinX, anchorsMinY);
        rt.anchorMax = new Vector2(anchorsMaxX, anchorsMaxY);

        rt.localPosition = new Vector3(x, y);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);

        return _base;
    }

}