using UnityEngine;
namespace CameraDesign.Controller.API
{
    public interface ICameraTarget
    {
        Transform m_transform { get; }
        Vector2 m_velocity { get; }
        bool m_isGrounded { get; }
    }
}