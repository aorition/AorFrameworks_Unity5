using UnityEngine;

namespace Framework.Graphic
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(VisualCamera))]
    public class VisualCameraAnimHandler : MonoBehaviour
    {

        public float Interpolation;
        public float Level;
        public bool Solo;

        private VisualCamera _visualCamera;

        protected void OnEnable()
        {
            _visualCamera = GetComponent<VisualCamera>();
            if (_visualCamera)
            {
                Interpolation = _visualCamera.Interpolation;
                Level = _visualCamera.Level;
                Solo = _visualCamera.Solo;

                _visualCamera.OnInterpolationChanged += _onInterpolationChangeDo;
                _visualCamera.OnLevelChanged += _onLevelChangeDo;
                _visualCamera.OnSoloChanged += _onSoloChangeDo;

            }
        }

        private void _onInterpolationChangeDo(float value)
        {
            Interpolation = value;
        }

        private void _onLevelChangeDo(float value)
        {
            Level = value;
        }

        private void _onSoloChangeDo(bool value)
        {
            Solo = value;
        }

        protected void OnDisable()
        {
            if (_visualCamera)
            {
                _visualCamera.OnInterpolationChanged -= _onInterpolationChangeDo;
                _visualCamera.OnLevelChanged -= _onLevelChangeDo;
                _visualCamera.OnSoloChanged -= _onSoloChangeDo;
            }
        }

        protected void OnDestroy()
        {
            _visualCamera = null;
        }

        protected void Update()
        {
            if (_visualCamera == null) return;
            _visualCamera.Interpolation = Interpolation;
            _visualCamera.Level = Level;
            _visualCamera.Solo = Solo;
        }

    }
}


