namespace Framework.Graphic
{
    public interface IVisualCameraBodyComponent
    {
        bool IsValid { get; }
        void MutateCameraState(float deltaTime);
    }

    public interface IVisualCameraAimComponent
    {
        bool IsValid { get; }
        void MutateCameraState(float deltaTime);
    }

    public interface IVisualCameraNoiseComponent
    {
        bool IsValid { get; }
        void MutateCameraState(float deltaTime);
    }

}
