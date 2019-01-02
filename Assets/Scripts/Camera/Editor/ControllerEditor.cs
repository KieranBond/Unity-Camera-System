using CameraDesign.Controller.Impl;
using UnityEditor;

namespace CameraDesign.Controller.Editor
{
    [CustomEditor(typeof(CameraController))]
    [CanEditMultipleObjects]
    public class ControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //This will contain debug draws.
        }
    }
}