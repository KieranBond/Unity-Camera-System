using UnityEngine;

namespace CameraDesign.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        //Movement
        [SerializeField]
        private float m_movementSpeed = 1f;

        //Jumping
        private bool m_isGrounded = true;
        [SerializeField]
        private float m_jumpForce = 5f;
        [SerializeField]
        private string m_floorTag = "Floor";
        private float m_previousYPos;

        //Other bits
        private Rigidbody2D m_rb;
        private Animator m_animator;

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
            //Do Idle animation
            if(m_isGrounded && m_rb.velocity.x == 0f && m_rb.velocity.y == 0f)
            {
                Debug.Log("Do Idle Animation");
                SetTrigger("Idle");
            }

            if(Input.GetKey(KeyCode.Space) && m_isGrounded)
            {
                //Do jump.
                m_isGrounded = false;

                SetTrigger("Jump");
                Debug.Log("Do Jump begin Animation");


                m_rb.AddForce(Vector2.up * m_jumpForce);
            }
            if(Input.GetKey(KeyCode.D))
            {
                m_rb.AddForce(Vector2.right * m_movementSpeed);
            }
            else if(Input.GetKey(KeyCode.A))
            {
                m_rb.AddForce(Vector2.left * m_movementSpeed);
            }

            //Do fall animation
            if(!m_isGrounded)
            {
                if(transform.position.y != m_previousYPos)
                { 
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
                Debug.Log("Do Landed Animation");
                SetTrigger("JumpFinished");
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