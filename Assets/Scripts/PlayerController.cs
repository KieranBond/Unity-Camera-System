using UnityEngine;

namespace CameraDesign.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float m_movementSpeed = 1f;

        private Rigidbody2D m_rb;
        private Animator m_animator;

        // Start is called before the first frame update
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKey(KeyCode.D))
            {
                m_rb.AddForce(Vector2.right * m_movementSpeed);
            }
            else if(Input.GetKey(KeyCode.A))
            {
                m_rb.AddForce(Vector2.left * m_movementSpeed);
            }

        }
    }
}