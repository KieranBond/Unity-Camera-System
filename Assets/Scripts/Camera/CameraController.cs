using System.Collections.Generic;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Tooltip("Transform of follow target. Requires ICameraTarget interface on the GameObject.")]
        private Transform m_cameraTarget;
        private ICameraTarget cameraTarget;//This will be gotten from m_cameraTarget. It's needed, so make sure it has a script with this interface.

        [SerializeField]
        private bool m_showDebug = true;

        [Header("Focus Zone")]
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
            m_targetFollower.TrackingUpdate(new Rect(m_focusXCentreWP, m_focusYCentreWP, xPoint3.x - xPoint1.x, yPoint3.y - yPoint1.y));

            #region Debug Lines

            if (m_showDebug)
            {
                //m_targetFollower.m_showDebug = m_showDebug;

                //Clear debug lines
                foreach (GameObject go in m_displayLineObjs)
                    Destroy(go);

                m_displayLineObjs.Clear();
                //Draw lines on camera

                GameObject line = new GameObject("DebugLine");
                LineRenderer lineR = line.AddComponent<LineRenderer>();
                lineR.useWorldSpace = true;
                lineR.startWidth = 0.1f;
                lineR.endWidth = 0.1f;
                lineR.SetPositions(new Vector3[] { xPoint1, xPoint2 });

                GameObject line2 = new GameObject("DebugLine1");
                LineRenderer line2R = line2.AddComponent<LineRenderer>();
                line2R.useWorldSpace = true;
                line2R.startWidth = 0.1f;
                line2R.endWidth = 0.1f;
                line2R.SetPositions(new Vector3[] { xPoint3, xPoint4 });

                GameObject line3 = new GameObject("DebugLine2");
                LineRenderer line3R = line3.AddComponent<LineRenderer>();
                line3R.useWorldSpace = true;
                line3R.startWidth = 0.1f;
                line3R.endWidth = 0.1f;
                line3R.SetPositions(new Vector3[] { yPoint1, yPoint2 });

                GameObject line4 = new GameObject("DebugLine3");
                LineRenderer line4R = line4.AddComponent<LineRenderer>();
                line4R.useWorldSpace = true;
                line4R.startWidth = 0.1f;
                line4R.endWidth = 0.1f;
                line4R.SetPositions(new Vector3[] { yPoint3, yPoint4 });

                m_displayLineObjs.Add(line);
                m_displayLineObjs.Add(line2);
                m_displayLineObjs.Add(line3);
                m_displayLineObjs.Add(line4);
            }
            else
            {
                //Clear any remaining debug.

                if (m_displayLineObjs.Count > 0)
                {
                    foreach (GameObject go in m_displayLineObjs)
                        Destroy(go);

                    m_displayLineObjs.Clear();
                }
            }

            #endregion

        }


    }
}