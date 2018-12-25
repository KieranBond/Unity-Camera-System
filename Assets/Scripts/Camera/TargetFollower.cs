using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    public class TargetFollower : MonoBehaviour
    {
        private Transform m_target;
        private Rect m_focusBounds;

        private bool m_debug = false;

        public void Initialise( Transform a_target )
        {
            m_target = a_target;

#if UNITY_EDITOR
            m_debug = true;
#endif
        }

        // Bounds are given in World Position.
        public void TrackingUpdate( Rect a_focusBounds )
        {
            //Update our bounds.
            m_focusBounds = a_focusBounds;

            //Start tracking.
            Vector3 targetPos = m_target.position;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;


            //Halving these because we always check from the center when checking against bounds. 
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            xBounds = CheckBounds(targetPos.x, m_focusBounds.x, width);
            yBounds = CheckBounds(targetPos.y, m_focusBounds.y, height);

            #region DebugLogging

            if (m_debug)
            {
                Debug.Log("xBounds: " + xBounds);
                Debug.Log("yBounds: " + yBounds);
            }

            #endregion

        }

        private OutOfBounds CheckBounds(float a_position, float a_boundsCenter, float a_boundsMod)
        {
            if (a_position > a_boundsCenter + a_boundsMod)
                return OutOfBounds.OutPos;
            else if (a_position < a_boundsCenter - a_boundsMod)
                return OutOfBounds.OutNeg;
            else
                return OutOfBounds.In;
        }


        private enum OutOfBounds
        {
            In,
            OutPos,
            OutNeg
        }
    }
}