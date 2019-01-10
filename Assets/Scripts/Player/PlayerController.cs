using CameraDesign.Controller.API;
using UnityEngine;

namespace CameraDesign.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, ICameraTarget
    {
        //Movement
        [SerializeField]
        private float m_movementSpeed = 1f;
        [SerializeField]
        private float m_maxXVelocity = 2f;

        //Jumping
        private bool m_isGrounded = true;
        [SerializeField]
        private float m_jumpForce = 5f;
        [SerializeField]
        private string m_floorTag = "Floor";
        private float m_previousYPos;

        [SerializeField]
        private bool m_showDebug = false;

        //Other bits
        private Rigidbody2D m_rb;
        private Animator m_animator;

        //ICameraTarget Variables.
        Transform ICameraTarget.m_transform { get => transform; }
        Vector2 ICameraTarget.m_velocity { get => GetComponent<Rigidbody2D>().velocity; }
        bool ICameraTarget.m_isGrounded { get => this.m_isGrounded; }

        // Start is called before the first frame update
        void Start()
        {
            m_previousYPos = transform.position.y;

            m_rb = GetComponent<Rigidbody2D>();
            m_animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(m_rb.velocity.x > m_maxXVelocity)
            {
                m_rb.velocity = new Vector2(m_maxXVelocity, m_rb.velocity.y);
            }
            else if (m_rb.velocity.x < -m_maxXVelocity)
            {
                m_rb.velocity = new Vector2(-m_maxXVelocity, m_rb.velocity.y);
            }

            //Do Idle animation
            if(m_isGrounded && m_rb.velocity.x == 0f && m_rb.velocity.y == 0f)
            {
                if(m_showDebug)
                    Debug.Log("Do Idle Animation");

                SetTrigger("Idle");
            }

            if(Input.GetKey(KeyCode.Space) && m_isGrounded)
            {
                //Do jump.
                m_isGrounded = false;

                if(m_showDebug)
                    Debug.Log("Do Jump begin Animation");

                SetTrigger("Jump");

                m_rb.AddForce(Vector2.up * m_jumpForce);
            }
            if(Input.GetKey(KeyCode.D))
            {
                SetTrigger("Run");

                m_rb.AddForce(Vector2.right * m_movementSpeed);
                Quaternion newRot = new Quaternion();
                newRot.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
                transform.rotation = newRot;
            }
            else if(Input.GetKey(KeyCode.A))
            {
                SetTrigger("Run");

                m_rb.AddForce(Vector2.left * m_movementSpeed);
                Quaternion newRot = new Quaternion();
                newRot.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = newRot;
            }
            else
            {
                m_animator.ResetTrigger("Run");//No movement pressed.
            }

            //Do fall animation
            if(!m_isGrounded)
            {
                if(transform.position.y != m_previousYPos)
                { 
                    if(m_showDebug)
                        Debug.Log("Do Jump end Animation");

                    SetTrigger("JumpEnd");
                }
            }

            m_previousYPos = transform.position.y;
        }

        private void OnCollisionEnter2D( Collision2D collision )
        {
            if(collision.gameObject.tag == m_floorTag)
            {
                m_isGrounded = true;
                SetTrigger("JumpFinished");

                if (m_showDebug)
                    Debug.Log("Do Landed Animation");
            }
        }

        private void OnCollisionExit2D( Collision2D collision )
        {
            if(collision.gameObject.tag == m_floorTag)
            {
                m_isGrounded = false;
            }
        }
        
        private void SetTrigger(string a_triggerName)
        {
            m_animator.WriteDefaultValues();
            m_animator.SetTrigger(a_triggerName);
        }
    }
}