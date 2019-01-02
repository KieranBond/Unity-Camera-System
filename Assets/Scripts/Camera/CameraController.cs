using CameraDesign.Controller.API;
using System.Collections.Generic;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        //TODO: Hide the 'actual' values. They should be private, only public for Debug.

        [Header("Settings")]
        [Tooltip("Transform of follow target. Requires ICameraTarget interface on the GameObject.")]
        public Transform m_cameraTarget;
        private ICameraTarget cameraTarget;//This will be gotten from m_cameraTarget. It's needed, so make sure it has a script with this interface.

        [SerializeField]
        private bool m_showDebug = true;
        [SerializeField]
        private Material m_debugLineMaterial;

        [Header("Focus Zone")]
        [SerializeField]
        [Range(0.01f, float.MaxValue)]
        private float m_focusMovementSpeed = 5f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusWidth = 5f;//Percentage of screen, width of the focus zone.
        public float m_actualFocusWidth = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusHeight = 5f;//Percentage of screen, height of the focus zone.
        public float m_actualFocusHeight = 0f;

        /// <summary>
        /// Percentage values. 50 = middle of screen. Range: 0 - 100.
        /// </summary>
        [SerializeField]
        [Range(0, 100)]
        private float m_focusXCentre = 50f;//X Centre. In perecentage. 
        public float m_actualFocusX = 0f;

        [SerializeField]
        [Range(0, 100)]
        private float m_focusYCentre = 50f;//Y Centre. In Percentage.
        public float m_actualFocusY = 0f;

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

            m_actualFocusX = m_camera.pixelWidth * (1 - ((100 - m_focusXCentre) - (m_focusWidth * 0.5f)) * 0.01f);
            m_actualFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);

            m_actualFocusY = m_camera.pixelHeight * (1 - (100 - m_focusYCentre) * 0.01f);
        }


        // Update is called once per frame
        void Update()
        {
            m_targetFollower.SettingsUpdate(m_focusMovementSpeed);

            m_actualFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);
            m_actualFocusHeight = m_camera.pixelHeight * (1 - (100 - m_focusHeight) * 0.01f);
            float m_widthHalved = m_actualFocusWidth * 0.5f;
            float m_heightHalved = m_actualFocusHeight * 0.5f;

            //Outside bracket is to normalize. FocusXCentre is percent of screen. 1 minus to get make it go left to right/top to bottom.
            m_actualFocusX = m_camera.pixelWidth * (1 - (100 - m_focusXCentre) * 0.01f);
            m_actualFocusY = m_camera.pixelHeight * (1 - (100 - m_focusYCentre) * 0.01f);

            Vector3 xPoint1 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX - m_widthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX - m_widthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 xPoint3 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + m_widthHalved, 0f, m_camera.nearClipPlane));
            Vector3 xPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + m_widthHalved, m_camera.pixelHeight, m_camera.nearClipPlane));

            Vector3 yPoint1 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_actualFocusY - m_heightHalved, m_camera.nearClipPlane));
            Vector3 yPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_actualFocusY - m_heightHalved, m_camera.nearClipPlane));
            Vector3 yPoint3 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_actualFocusY + m_heightHalved, m_camera.nearClipPlane));
            Vector3 yPoint4 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth, m_actualFocusY + m_heightHalved, m_camera.nearClipPlane));

            float m_focusXCentreWP = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, 0f, 0f)).x;
            float m_focusYCentreWP = m_camera.ScreenToWorldPoint(new Vector3(0f, m_actualFocusY, 0f)).y;

            Rect focusBounds = new Rect(m_focusXCentreWP, m_focusYCentreWP, xPoint3.x - xPoint1.x, yPoint3.y - yPoint1.y);

            //Distance between centre of Focus zone and centre of target.
            float focusOffset = Vector3.Distance(m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, 0f, m_camera.nearClipPlane)), m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth * 0.5f, 0f, m_camera.nearClipPlane)));
            float targetYRot = m_cameraTarget.rotation.eulerAngles.y;

            //if (targetYRot % 180 == 0)
            //{
            //    //It's facing left. Move our offset to the other side.
            //
            //    focusBounds.center = new Vector2(focusBounds.center.x - focusOffset, focusBounds.center.y);
            //}

            m_targetFollower.TrackingUpdate(focusOffset, focusBounds);

        }

        private void OnDrawGizmos()
        {
            if (!m_showDebug)
                return;

            if (m_camera == null)
                return;

            float widthHalved = m_actualFocusWidth * 0.5f;
            float heightHalved = m_actualFocusHeight * 0.5f;
            float pixelWidth = m_camera.pixelWidth;
            float pixelHeight = m_camera.pixelHeight;

            //Centre line.
            Gizmos.color = Color.red;
            Vector3 centerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, 0f, m_camera.nearClipPlane));
            Vector3 centerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth * 0.5f, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(centerPoint1, centerPoint2);

            Gizmos.color = Color.green;
            //Left most line of focus zone.
            Vector3 x1 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX - widthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x2 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX - widthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x1, x2);

            //Right most line of focus zone.
            Vector3 x3 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + widthHalved, 0f, m_camera.nearClipPlane));
            Vector3 x4 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + widthHalved, pixelHeight, m_camera.nearClipPlane));
            Gizmos.DrawLine(x3, x4);

            //Bottom line of focus zone.
            Vector3 y1 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_actualFocusY - heightHalved, m_camera.nearClipPlane));
            Vector3 y2 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_actualFocusY - heightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y1, y2);
            
            //Top line of focus zone.
            Vector3 y3 = m_camera.ScreenToWorldPoint(new Vector3(0f, m_actualFocusY + heightHalved, m_camera.nearClipPlane));
            Vector3 y4 = m_camera.ScreenToWorldPoint(new Vector3(pixelWidth, m_actualFocusY + heightHalved, m_camera.nearClipPlane));
            Gizmos.DrawLine(y3, y4);
        }
    }
}