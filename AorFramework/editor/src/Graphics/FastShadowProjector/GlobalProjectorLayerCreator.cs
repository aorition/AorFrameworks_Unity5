using UnityEngine;
using UnityEditor;

namespace Framework.Graphic.FastShadowProjector.Editor
{
    [InitializeOnLoad]
    public class GlobalProjetorLayerCreator
    {
        static GlobalProjetorLayerCreator()
        {
            ShadowProjectorEditor.CheckGlobalProjectorLayer();
        }
    }
}

