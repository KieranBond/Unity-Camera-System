using UnityEngine;
namespace CameraDesign.Controller.API
{
    public interface ICameraTarget
    {
        Transform m_transform { get; }

        bool m_isGrounded { get; }
    }
}