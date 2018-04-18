using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGraphic : MaskableGraphic {

    private CanvasRenderer _canvasRenderer;
	private bool _fillEmpty = true;
    //public CanvasGroup canvasGroup;

    protected override void OnEnable() {
        base.OnEnable();
        /*
    if (canvasGroup == null) {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.ignoreParentGroups = false;
        canvasGroup.alpha = 0;
    }*/

    }

    protected override void Awake() {
        base.Awake();
    }

    public void updateFBVEmpty() {
        _fillEmpty = true;
    }

    private List<UIVertex> _spriteUiVertices;

    public void updateFBV(List<UIVertex> spriteUiVertices) {
        _spriteUiVertices = spriteUiVertices;
        _fillEmpty = false;
    }


    public SpriteAsset m_spriteAsset;

    public override Texture mainTexture {
        get {
            if (m_spriteAsset == null)
                return s_WhiteTexture;

            if (m_spriteAsset.texSource == null)
                return s_WhiteTexture;
            else
                return m_spriteAsset.texSource;
        }
    }

    protected override void OnRectTransformDimensionsChange() {
        // base.OnRectTransformDimensionsChange();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        rectTransform.pivot = new Vector2(0, 1f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.sizeDelta = Vector2.zero;

    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //base.OnPopulateMesh(vh);

        vh.Clear();

        if (!_fillEmpty)
        {
            vh.AddUIVertexTriangleStream(_spriteUiVertices);
        }
    }
    /*
    protected override void OnFillVBO(List<UIVertex> vbo) {
        base.OnFillVBO(vbo);

        if (_fillEmpty) {
            vbo.Clear();
        }
        else {
            vbo = _spriteUiVertices;
        }
    }*/
}