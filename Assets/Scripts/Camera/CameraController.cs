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
            m_targetFollower.TrackingUpdate(new Rect(m_focusXCentreWP, m_focusYCentreWP, xPoint3.x - xPoint1.x, yPoint3.y - yPoint1.y));

            #region Debug Lines

            if (m_showDebug)
            {
                GameObject debugParent = new GameObject("DebugParent");
                //Clear debug lines
                foreach (GameObject go in m_displayLineObjs)
                    Destroy(go);

                m_displayLineObjs.Clear();
                //Draw lines on camera

                Vector3 centerPoint1 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth*0.5f, 0f, m_camera.nearClipPlane));
                Vector3 centerPoint2 = m_camera.ScreenToWorldPoint(new Vector3(m_camera.pixelWidth*0.5f, m_camera.pixelHeight, m_camera.nearClipPlane));

                GameObject centralLine = new GameObject("CentralLine");
                centralLine.transform.SetParent(debugParent.transform);
                LineRenderer centralLineR = centralLine.AddComponent<LineRenderer>();
                centralLineR.sortingOrder = 100;
                centralLineR.material = m_debugLineMaterial;
                centralLineR.useWorldSpace = true;
                centralLineR.startColor = Color.red;
                centralLineR.endColor = Color.red;
                centralLineR.startWidth = 0.05f;
                centralLineR.endWidth = 0.05f;
                centralLineR.SetPositions(new Vector3[] { centerPoint1, centerPoint2 });

                GameObject debugLine = Instantiate(centralLine);
                debugLine.name = "DebugLine";
                debugLine.transform.SetParent(debugParent.transform);
                LineRenderer lineR = debugLine.GetComponent<LineRenderer>();
                lineR.sortingOrder = 99;
                lineR.SetPositions(new Vector3[] { xPoint1, xPoint2 });
                lineR.startColor = Color.green;
                lineR.endColor = Color.green;


                GameObject debugLine2 = Instantiate(debugLine);
                debugLine2.transform.SetParent(debugParent.transform);
                LineRenderer line2R = debugLine2.GetComponent<LineRenderer>();
                line2R.SetPositions(new Vector3[] { xPoint3, xPoint4 });

                GameObject debugLine3 = Instantiate(debugLine);
                debugLine3.transform.SetParent(debugParent.transform);
                LineRenderer line3R = debugLine3.GetComponent<LineRenderer>();
                line3R.SetPositions(new Vector3[] { yPoint1, yPoint2 });

                GameObject debugLine4 = Instantiate(debugLine);
                debugLine4.transform.SetParent(debugParent.transform);
                LineRenderer line4R = debugLine4.GetComponent<LineRenderer>();
                line4R.SetPositions(new Vector3[] { yPoint3, yPoint4 });


                m_displayLineObjs.AddRange(new GameObject[] { debugParent, centralLine, debugLine, debugLine2, debugLine3, debugLine4 });
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