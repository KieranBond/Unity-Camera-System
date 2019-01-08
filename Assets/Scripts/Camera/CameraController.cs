using CameraDesign.Controller.API;
using System.Collections.Generic;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        #region Settings

        [Header("Settings")]
        [Tooltip("Transform of follow target. Requires ICameraTarget interface on the GameObject.")]
        public Transform m_cameraTarget;
        private ICameraTarget cameraTarget;//This will be gotten from m_cameraTarget. It's needed, so make sure it has a script with this interface.

        [SerializeField]
        private bool m_showDebug = true;
        [SerializeField]
        private Material m_debugLineMaterial;

        #endregion

        #region Focus Zone
        [Header("Focus Zone")]
        [SerializeField]
        [Range(0.0001f, float.MaxValue)]
        private float m_focusMovementSpeed = 5f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusWidth = 5f;//Percentage of screen, width of the focus zone.
        [HideInInspector]
        public float m_pixelsFocusWidth = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusHeight = 5f;//Percentage of screen, height of the focus zone.
        [HideInInspector]
        public float m_pixelsFocusHeight = 0f;

        /// <summary>
        /// Percentage values. 50 = middle of screen. Range: 0 - 100.
        /// </summary>
        [SerializeField]
        [Range(0, 100)]
        private float m_focusXCentre = 50f;//X Centre. In perecentage. 
        [HideInInspector]
        public float m_pixelsFocusXCentre = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusYCentre = 50f;//Y Centre. In Percentage.
        [HideInInspector]
        public float m_pixelsFocusYCentre = 0f;
        #endregion

        #region Danger Zone
        [Header("Danger Zone")]
        [SerializeField]
        [Range(0.0001f, float.MaxValue)]
        private float m_dangerMovementSpeed = 5f;

        [SerializeField]
        [Range(0, 100)]
        private float m_dangerWidth = 5f;//Percentage of screen, width of the danger zone.
        [HideInInspector]
        public float m_pixelsDangerWidth = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_dangerHeight = 5f;//Percentage of screen, height of the danger zone.
        [HideInInspector]
        public float m_pixelsDangerHeight = 0f;

        /// <summary>
        /// Percentage values. 50 = middle of screen. Range: 0 - 100.
        /// </summary>
        [SerializeField]
        [Range(0, 100)]
        private float m_dangerXCentre = 50f;//X Centre. In perecentage. 
        [HideInInspector]
        public float m_pixelsDangerXCentre = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_dangerYCentre = 50f;//Y Centre. In Percentage.
        [HideInInspector]
        public float m_pixelsDangerYCentre = 0f;
        #endregion

        private Vector2 m_screenResolution;
        private Camera m_camera;

        private TargetFollower m_targetFollower;

        private List<GameObject> m_displayLineObjs;

        // Start is called before the first frame update
        void Start()
        {
            m_displayLineObjs = new List<GameObject>();
            m_camera = GetComponent<Camera>();

            //Ensures it's not able to do anything until ICameraTarget is on the object.
            cameraTarget = m_cameraTarget.GetComponent<ICameraTarget>();
            if (cameraTarget == null)
            {
                m_cameraTarget = null;
            }
            else
            {
                //Setup the target follower.
                m_targetFollower = GetComponent<TargetFollower>();
                if (m_targetFollower == null)
                    m_targetFollower = gameObject.AddComponent<TargetFollower>();

                m_targetFollower.Initialise(cameraTarget, m_camera.transform, m_showDebug);
            }

            m_screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            m_pixelsFocusXCentre = m_camera.pixelWidth * (1 - ((100 - m_focusXCentre) - (m_focusWidth * 0.5f)) * 0.01f);
            m_pixelsFocusYCentre = m_camera.pixelHeight * (1 - (100 - m_focusYCentre) * 0.01f);
            m_pixelsFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);

            m_pixelsDangerXCentre = m_camera.pixelWidth * (1 - ((100 - m_dangerXCentre) - (m_dangerWidth * 0.5f)) * 0.01f);
            m_pixelsDangerYCentre = m_camera.pixelHeight * (1 - (100 - m_dangerYCentre) * 0.01f);
            m_pixelsDangerWidth = m_camera.pixelWidth * (1 - (100 - m_dangerWidth) * 0.01f);

        }


        // Update is called once per frame
        void Update()
        {
            m_targetFollower.SettingsUpdate(m_focusMovementSpeed, m_dangerMovementSpeed);

            //  FOCUS ZONE

            m_pixelsFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);
            m_pixelsFocusHeight = m_camera.pixelHeight * (1 - (100 - m_focusHeight) * 0.01f);
            float m_focusWidthHalved = m_pixelsFocusWidth * 0.5f;
            float m_focusHeightHalved = m_pixelsFocusHeight * 0.5f;

            //Outside bracket is to normalize. FocusXCentre is percent of screen. 1 minus to get make it go left to right/top to bottom.
            m_pixelsFocusXCentre = m_camera.pixelWidth * (1 - (100 - m_focusXCentre) * 0.01f);
            m_pixelsFocusYCentre = m_camera.pixelHeight * (1 - (100 - m_focusYCentre) * 0.01f);

            Vector3 xFocusPoint1 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre - m_focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xFocusPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre - m_focusWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 xFocusPoint3 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre + m_focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xFocusPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre + m_focusWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));

            Vector3 yFocusPoint1 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsFocusYCentre - m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_pixelsFocusYCentre - m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint3 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsFocusYCentre + m_focusHeightHalved, m_camera.nearClipPlane));
            Vector3 yFocusPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_pixelsFocusYCentre + m_focusHeightHalved, m_camera.nearClipPlane));

            float m_focusXCentreWP = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre, 0f, 0f)).x;
            float m_focusYCentreWP = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsFocusYCentre, 0f)).y;

            Rect focusBounds = new Rect(m_focusXCentreWP, m_focusYCentreWP, xFocusPoint3.x - xFocusPoint1.x, yFocusPoint3.y - yFocusPoint1.y);

            //Distance between centre of Focus zone and centre of target.
            float focusOffset = Vector3.Distance(m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre, 0f, m_camera.nearClipPlane)), m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth * 0.5f, 0f, m_camera.nearClipPlane)));

            
            //  DANGER ZONE

            m_pixelsDangerWidth = m_camera.pixelWidth * (1 - (100 - m_dangerWidth) * 0.01f);
            m_pixelsDangerHeight = m_camera.pixelHeight * (1 - (100 - m_dangerHeight) * 0.01f);
            float m_dangerWidthHalved = m_pixelsDangerWidth * 0.5f;
            float m_dangerHeightHalved = m_pixelsDangerHeight * 0.5f;

            //Outside bracket is to normalize. FocusXCentre is percent of screen. 1 minus to get make it go left to right/top to bottom.
            m_pixelsDangerXCentre = m_camera.pixelWidth * (1 - (100 - m_dangerXCentre) * 0.01f);
            m_pixelsDangerYCentre = m_camera.pixelHeight * (1 - (100 - m_dangerYCentre) * 0.01f);

            Vector3 xDangerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre - m_dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xDangerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre - m_dangerWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 xDangerPoint3 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre + m_dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xDangerPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre + m_dangerWidthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));

            Vector3 yDangerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsDangerYCentre - m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_pixelsDangerYCentre - m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint3 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsDangerYCentre + m_dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 yDangerPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_pixelsDangerYCentre + m_dangerHeightHalved, m_camera.nearClipPlane));

            float m_dangerXCentreWP = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre, 0f, 0f)).x;
            float m_dangerYCentreWP = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsDangerYCentre, 0f)).y;

            Rect dangerBounds = new Rect(m_dangerXCentreWP, m_dangerYCentreWP, xDangerPoint3.x - xDangerPoint1.x, yDangerPoint3.y - yDangerPoint1.y);

            //Distance between centre of Focus zone and centre of target.
            float dangerOffset = Vector3.Distance(m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre, 0f, m_camera.nearClipPlane)), m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth * 0.5f, 0f, m_camera.nearClipPlane)));


            
            //Update follower script.
            m_targetFollower.TrackingUpdate(focusOffset, focusBounds, dangerBounds);

        }

        private void OnDrawGizmos()
        {
            if (!m_showDebug)
                return;

            if (m_camera == null)
                return;

            float focusWidthHalved = m_pixelsFocusWidth * 0.5f;
            float focusHeightHalved = m_pixelsFocusHeight * 0.5f;

            float dangerWidthHalved = m_pixelsDangerWidth * 0.5f;
            float dangerHeightHalved = m_pixelsDangerHeight * 0.5f;

            float pixelWidth = m_camera.pixelWidth;
            float pixelHeight = m_camera.pixelHeight;

            //Centre line.
            Gizmos.color = Color.magenta;
            Vector3 centerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, 0f, m_camera.nearClipPlane));
            Vector3 centerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(centerPoint1, centerPoint2);

            Gizmos.color = Color.green;

            //  FOCUS ZONE
            //Left most line of focus zone.
            Vector3 x1 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre - focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x2 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre - focusWidthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x1, x2);

            //Right most line of focus zone.
            Vector3 x3 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre + focusWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x4 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsFocusXCentre + focusWidthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x3, x4);

            //Bottom line of focus zone.
            Vector3 y1 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsFocusYCentre - focusHeightHalved, m_camera.nearClipPlane));
            Vector3 y2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_pixelsFocusYCentre - focusHeightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y1, y2);
            
            //Top line of focus zone.
            Vector3 y3 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsFocusYCentre + focusHeightHalved, m_camera.nearClipPlane));
            Vector3 y4 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_pixelsFocusYCentre + focusHeightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y3, y4);


            //  DANGER ZONE
            //Left most line of focus zone.
            Gizmos.color = Color.red;
            Vector3 x11 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre - dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x22 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre - dangerWidthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x11, x22);

            //Right most line of focus zone.
            Vector3 x33 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre + dangerWidthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x44 = m_camera.ScreenToWorldPoint(new Vector3(m_pixelsDangerXCentre + dangerWidthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x33, x44);

            //Bottom line of focus zone.
            Vector3 y11 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsDangerYCentre - dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 y22 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_pixelsDangerYCentre - dangerHeightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y11, y22);

            //Top line of focus zone.
            Vector3 y33 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_pixelsDangerYCentre + dangerHeightHalved, m_camera.nearClipPlane));
            Vector3 y44 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_pixelsDangerYCentre + dangerHeightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y33, y44);
        }
    }
}