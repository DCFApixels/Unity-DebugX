using UnityEngine;

namespace DCFApixels.DebugXCore
{
    public interface IDrawHandler
    {
        Color Color { get; }
        float Duration { get; }
    }
    public interface IDrawPositionHandler
    {
        Vector3 Position { get; }
    }
    public interface IDrawRotationHandler
    {
        Quaternion Rotation { get; }
    }
    public interface IDrawScaleHandler
    {
        Vector3 Scale { get; }
    }
    public interface IDrawLineHandler
    {
        Vector3 Start { get; }
        Vector3 End { get; }
    }
}
