using CameraDesign.Controller.Impl;
using CameraDesign.Controller.Settings;
using UnityEditor;
using UnityEngine;

namespace CameraDesign.Controller.EditorScripts
{
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(CameraController))]
    public class ControllerEditor : Editor
    {
        private bool m_showFoldout = false;
        private SectionDisplay m_displaySection = SectionDisplay.FocusZone;

        //Debugs
        private bool showDebug;
        private bool showInGameView;

        //General bits
        private Transform cameraTarget;
        private LerpEasing movementEasing;
        private Camera m_camera;
        private Vector2 m_screenResolution;

        //Focus Zone
        private float focusMovementSpeed;
        private float focusWidth;
        private float focusWidthPixels;
        private float focusHeight;
        private float focusHeightPixels;
        private float focusXCentre;
        private float focusXCentrePixels;
        private float focusYCentre;
        private float focusYCentrePixels;

        //Danger zone
        private float dangerMovementSpeed;
        private float dangerWidth;
        private float dangerWidthPixels;
        private float dangerHeight;
        private float dangerHeightPixels;
        private float dangerXCentre;
        private float dangerXCentrePixels;
        private float dangerYCentre;
        private float dangerYCentrePixels;


        private void OnEnable()
        {


            //m_camera = serializedObject.FindProperty("m_camera");
            //cameraTarget = serializedObject.FindProperty("m_cameraTarget");
            //movementEasing = serializedObject.FindProperty("m_movementEasing");
            //showDebug = serializedObject.FindProperty("m_showDebug");

            //Focus zone
            //focusMovementSpeed = serializedObject.FindProperty("m_focusMovementSpeed");
            //focusWidth = serializedObject.FindProperty("m_focusWidth");
            //focusHeight = serializedObject.FindProperty("m_focusHeight");
            //focusXCentre = serializedObject.FindProperty("m_focusXCentre");
            //focusYCentre = serializedObject.FindProperty("m_focusYCentre");



        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CameraController controller = (CameraController) target;

            m_camera = controller.m_camera;
            if (m_camera == null)
                m_camera = controller.gameObject.GetComponent<Camera>();

            cameraTarget = controller.m_cameraTarget;
            movementEasing = controller.m_movementEasing;
            showDebug = controller.m_showDebug;
            showInGameView = controller.m_debugDrawInGameView;

            //Focus zone
            focusMovementSpeed = controller.m_focusMovementSpeed;
            focusWidth = controller.m_focusWidth;
            focusHeight = controller.m_focusHeight;
            focusXCentre = controller.m_focusXCentre;
            focusYCentre = controller.m_focusYCentre;

            //Danger zone
            dangerMovementSpeed = controller.m_dangerMovementSpeed;
            dangerWidth = controller.m_dangerWidth;
            dangerHeight = controller.m_dangerHeight;
            dangerXCentre = controller.m_dangerXCentre;
            dangerYCentre = controller.m_dangerYCentre;

            //Get variables
            m_screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            //Focus zone
            focusXCentrePixels = m_camera.pixelWidth * (1 - ((100 - focusXCentre) - (focusWidth * 0.5f)) * 0.01f);
            focusYCentrePixels = m_camera.pixelHeight * (1 - (100 - focusYCentre) * 0.01f);

            focusWidthPixels = m_camera.pixelWidth * (1 - (100 - focusWidth) * 0.01f);
            focusHeightPixels = m_camera.pixelHeight * (1 - (100 - focusHeight) * 0.01f);


            //Danger zone, ddddd danger zone
            dangerXCentrePixels = m_camera.pixelWidth * (1 - ((100 - dangerXCentre) - (dangerWidth * 0.5f)) * 0.01f);
            dangerYCentrePixels = m_camera.pixelHeight * (1 - (100 - dangerYCentre) * 0.01f);

            dangerHeightPixels = m_camera.pixelHeight * (1 - (100 - dangerHeight) * 0.01f);
            dangerWidthPixels = m_camera.pixelWidth * (1 - (100 - dangerWidth) * 0.01f);





            m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Click");

            if(m_showFoldout)
            {
                switch (m_displaySection)
                {
                    case SectionDisplay.FocusZone:

                        break;

                    case SectionDisplay.DangerZone:

                        break;
                }
                base.OnInspectorGUI();

            }

            //This will contain debug draws.

            //FOCUS ZONE

            focusWidthPixels = m_camera.pixelWidth * (1 - (100 - focusWidth) * 0.01f);
            focusHeightPixels = m_camera.pixelHeight * (1 - (100 - focusHeight) * 0.01f);
            float m_focusWidthHalved = focusWidthPixels * 0.5f;
            float m_focusHeightHalved = focusHeightPixels * 0.5f;

            //Outside bracket is to normalize. FocusXCentre is percent of screen. 1 minus to get make it go left to right/top to bottom.
            focusXCentrePixels = m_camera.pixelWidth * (1 - (100 - focusXCentre) * 0.01f);
            focusYCentrePixels = m_camera.pixelHeight * (1 - (100 - focusYCentre) * 0.01f);

            Vector3 xFocusPoint1 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels - m_focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xFocusPoint2 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels - m_focusWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 xFocusPoint3 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels + m_focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xFocusPoint4 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels + m_focusWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));

            Vector3 yFocusPoint1 = m_camera.ScreenToWorldPoint(new Vector3(0f, focusYCentrePixels - m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, focusYCentrePixels - m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint3 = m_camera.ScreenToWorldPoint(new Vector3(0f, focusYCentrePixels + m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, focusYCentrePixels + m_focusHeightHalved, m_camera.nearClipPlane));

            float m_focusXCentreWP = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels, 0f, 0f)).x;
            float m_focusYCentreWP = m_camera.ScreenToWorldPoint(new Vector3(0f, focusYCentrePixels, 0f)).y;

            Rect focusBounds = new Rect(m_focusXCentreWP, m_focusYCentreWP, xFocusPoint3.x - xFocusPoint1.x, yFocusPoint3.y - yFocusPoint1.y);

            //Distance between centre of Focus zone and centre of target.
            float focusOffset = Vector3.Distance(m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels, 0f, m_camera.nearClipPlane)), m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth * 0.5f, 0f, m_camera.nearClipPlane)));


            //  DANGER ZONE

            dangerWidthPixels = m_camera.pixelWidth * (1 - (100 - dangerWidth) * 0.01f);
            dangerHeightPixels = m_camera.pixelHeight * (1 - (100 - dangerHeight) * 0.01f);
            float m_dangerWidthHalved =  dangerWidthPixels * 0.5f;
            float m_dangerHeightHalved = dangerHeightPixels * 0.5f;

            //Outside bracket is to normalize. FocusXCentre is percent of screen. 1 minus to get make it go left to right/top to bottom.
            dangerXCentrePixels = m_camera.pixelWidth * (1 - (100 - dangerXCentre) * 0.01f);
            dangerYCentrePixels = m_camera.pixelHeight * (1 - (100 - dangerYCentre) * 0.01f);

            Vector3 xDangerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels - m_dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xDangerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels - m_dangerWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 xDangerPoint3 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels + m_dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xDangerPoint4 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels + m_dangerWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));

            Vector3 yDangerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(0f, dangerYCentrePixels - m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, dangerYCentrePixels - m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint3 = m_camera.ScreenToWorldPoint(new Vector3(0f, dangerYCentrePixels + m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, dangerYCentrePixels + m_dangerHeightHalved, m_camera.nearClipPlane));

            float m_dangerXCentreWP = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels, 0f, 0f)).x;
            float m_dangerYCentreWP = m_camera.ScreenToWorldPoint(new Vector3(0f, dangerYCentrePixels, 0f)).y;

            Rect dangerBounds = new Rect(m_dangerXCentreWP, m_dangerYCentreWP, xDangerPoint3.x - xDangerPoint1.x, yDangerPoint3.y - yDangerPoint1.y);

            //Distance between centre of Focus zone and centre of target.
            float dangerOffset = Vector3.Distance(m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels, 0f, m_camera.nearClipPlane)), m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth * 0.5f, 0f, m_camera.nearClipPlane)));

        }

        private void OnSceneGUI()
        {
            if ((EditorApplication.isPlaying && !showInGameView) || (!EditorApplication.isPlaying))
            {
                float pixelWidth = m_camera.pixelWidth;
                float pixelHeight = m_camera.pixelHeight;

                float focusWidthHalved = focusWidthPixels * 0.5f;
                float focusHeightHalved = focusHeightPixels * 0.5f;

                float dangerWidthHalved = dangerWidthPixels * 0.5f;
                float dangerHeightHalved = dangerHeightPixels * 0.5f;


                //Centre line.
                Handles.color = Color.yellow;
                Vector3 centerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, 0f, m_camera.nearClipPlane));
                Vector3 centerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, pixelHeight, m_camera.nearClipPlane));
                Handles.DrawLine(centerPoint1, centerPoint2);

                Handles.color = Color.green;

                //  FOCUS ZONE
                //Left most line of focus zone.
                Vector3 x1 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels - focusWidthHalved, 0f, m_camera.nearClipPlane));
                Vector3 x2 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels - focusWidthHalved, pixelHeight, m_camera.nearClipPlane));
                Handles.DrawLine(x1, x2);

                //Right most line of focus zone.
                Vector3 x3 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels + focusWidthHalved, 0f, m_camera.nearClipPlane));
                Vector3 x4 = m_camera.ScreenToWorldPoint(new Vector3(focusXCentrePixels + focusWidthHalved, pixelHeight, m_camera.nearClipPlane));
                Handles.DrawLine(x3, x4);

                //Bottom line of focus zone.
                Vector3 y1 = m_camera.ScreenToWorldPoint(new Vector3(0f, focusYCentrePixels - focusHeightHalved, m_camera.nearClipPlane));
                Vector3 y2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, focusYCentrePixels - focusHeightHalved, m_camera.nearClipPlane));
                Handles.DrawLine(y1, y2);

                //Top line of focus zone.
                Vector3 y3 = m_camera.ScreenToWorldPoint(new Vector3(0f, focusYCentrePixels + focusHeightHalved, m_camera.nearClipPlane));
                Vector3 y4 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, focusYCentrePixels + focusHeightHalved, m_camera.nearClipPlane));
                Handles.DrawLine(y3, y4);


                //  DANGER ZONE
                //Left most line of focus zone.
                Handles.color = Color.red;
                Vector3 x11 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels - dangerWidthHalved, 0f, m_camera.nearClipPlane));
                Vector3 x22 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels - dangerWidthHalved, pixelHeight, m_camera.nearClipPlane));
                Handles.DrawLine(x11, x22);

                //Right most line of focus zone.
                Vector3 x33 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels + dangerWidthHalved, 0f, m_camera.nearClipPlane));
                Vector3 x44 = m_camera.ScreenToWorldPoint(new Vector3(dangerXCentrePixels + dangerWidthHalved, pixelHeight, m_camera.nearClipPlane));
                Handles.DrawLine(x33, x44);

                //Bottom line of focus zone.
                Vector3 y11 = m_camera.ScreenToWorldPoint(new Vector3(0f, dangerYCentrePixels - dangerHeightHalved, m_camera.nearClipPlane));
                Vector3 y22 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, dangerYCentrePixels - dangerHeightHalved, m_camera.nearClipPlane));
                Handles.DrawLine(y11, y22);

                //Top line of focus zone.
                Vector3 y33 = m_camera.ScreenToWorldPoint(new Vector3(0f, dangerYCentrePixels + dangerHeightHalved, m_camera.nearClipPlane));
                Vector3 y44 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, dangerYCentrePixels + dangerHeightHalved, m_camera.nearClipPlane));
                Handles.DrawLine(y33, y44);
            }
        }
    }

    public enum SectionDisplay
    {
        FocusZone,
        DangerZone
    }
}