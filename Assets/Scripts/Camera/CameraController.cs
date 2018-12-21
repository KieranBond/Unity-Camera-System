using System.Collections.Generic;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Transform m_cameraTarget;

        [Header("Focus Zone")]
        [SerializeField]
        [Range(0, 100)]
        private float m_focusWidth = 5f;//Percentage of screen, width of the focus zone.
        public float m_actualFocusWidth = 0f;

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

        private List<GameObject> m_displayLineObjs;

        // Start is called before the first frame update
        void Start()
        {
            m_displayLineObjs = new List<GameObject>();
            m_camera = GetComponent<Camera>();

            m_screenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            m_actualFocusX = m_camera.pixelWidth * (1 - (100 - m_focusXCentre) * 0.01f);
            m_actualFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);

            m_actualFocusY = m_camera.pixelHeight * (1 - (100 - m_focusYCentre) * 0.01f);


            //Vector3 point = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, 0f, m_camera.nearClipPlane));
            //Vector3 point2 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, m_camera.pixelHeight, m_camera.nearClipPlane));

            //Vector3 point = new Vector3();
            //Event currentEvent = Event.current;
            //Vector2 lineTopX = new Vector2();
            //Vector2 lineBotX = new Vector2();

            //// Get the mouse position from Event.
            //// Note that the y position from Event is inverted.
            //lineTopX.x = currentEvent.mousePosition.x;
            //lineBotX.y = cam.pixelHeight - currentEvent.mousePosition.y;

        }



        // Update is called once per frame
        void Update()
        {

            m_actualFocusX = m_camera.pixelWidth * (1 - (100 - m_focusXCentre) * 0.01f);
            m_actualFocusWidth = m_camera.pixelWidth * (1 - (100 - m_focusWidth) * 0.01f);

            m_actualFocusY = m_camera.pixelHeight * ((100 - m_focusYCentre) * 0.01f);

            Vector3 newPos = new Vector3(m_cameraTarget.position.x, m_cameraTarget.position.y, m_camera.transform.position.z);
            m_camera.transform.position = newPos;

            //Clear debug lines
            foreach (GameObject go in m_displayLineObjs)
                Destroy(go);

            m_displayLineObjs.Clear();
            //Draw lines on camera

            Vector3 point = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, 0f, m_camera.nearClipPlane));
            Vector3 point2 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX, m_camera.pixelHeight, m_camera.nearClipPlane));
            Vector3 point3 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + m_actualFocusWidth, 0f, m_camera.nearClipPlane));
            Vector3 point4 = m_camera.ScreenToWorldPoint(new Vector3(m_actualFocusX + m_actualFocusWidth, m_camera.pixelHeight, m_camera.nearClipPlane));
            //Vector3 xLineTop = new Vector3(m_actualFocusX, m_actualFocusY, 0f);
            //Vector3 xLine = new Vector3(m_actualFocusX, 0f, 0f);

            GameObject line = new GameObject("DebugLine");
            LineRenderer lineR = line.AddComponent<LineRenderer>();
            lineR.useWorldSpace = true;
            lineR.startWidth = 0.1f;
            lineR.endWidth = 0.1f;
            lineR.SetPositions(new Vector3[] { point, point2 });

            GameObject line2 = new GameObject("DebugLine1");
            LineRenderer line2R = line2.AddComponent<LineRenderer>();
            line2R.useWorldSpace = true;
            line2R.startWidth = 0.1f;
            line2R.endWidth = 0.1f;
            line2R.SetPositions(new Vector3[] { point3, point4 });

            m_displayLineObjs.Add(line);
            m_displayLineObjs.Add(line2);
        }


    }
}