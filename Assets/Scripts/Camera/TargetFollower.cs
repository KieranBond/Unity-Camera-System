using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    public class TargetFollower : MonoBehaviour
    {
        private Transform m_target;
        private Rect m_focusBounds;

        public void Initialise( Transform a_target )
        {
            m_target = a_target;
        }

        // Bounds are given in World Position.
        public void TrackingUpdate( Rect a_focusBounds )
        {
            m_focusBounds = a_focusBounds;

            //Start tracking.
            Vector3 targetPos = m_target.position;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;


            //Halving these because we always check from the center. 
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            if (targetPos.x > m_focusBounds.x + width)
            {
                //Out of right xBounds.
                xBounds = OutOfBounds.OutPos;
            }
            else if (targetPos.x < m_focusBounds.x - width)
            {
                //Out of left xBounds.
                xBounds = OutOfBounds.OutNeg;
            }

            if (targetPos.y > m_focusBounds.y + height)
            {
                //Out of top bounds
                yBounds = OutOfBounds.OutPos;
            }
            else if (targetPos.y < m_focusBounds.y - height)
            {
                //Out of bottom bounds
                yBounds = OutOfBounds.OutNeg;
            }

        }


        private enum OutOfBounds
        {
            In,
            OutPos,
            OutNeg
        }
    }
}