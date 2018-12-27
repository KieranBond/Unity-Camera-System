using System;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    public class TargetFollower : MonoBehaviour
    {
        private Transform m_target;
        private Rect m_focusBounds;

        public bool m_showDebug = false;

        public void Initialise( Transform a_target, bool a_showDebug = false )
        {
            m_target = a_target;
            m_showDebug = a_showDebug;
        }

        // Bounds are given in World Position.
        public void TrackingUpdate( Rect a_focusBounds )
        {
            //Update our bounds.
            m_focusBounds = a_focusBounds;

           // Vector3 targetPos = m_target.TransformPoint(m_target.position);
            Vector3 targetPos = m_target.position;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;


            //Halving these because we always check from the center when checking against bounds. 
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            xBounds = CheckBounds(targetPos.x, m_focusBounds.x, width);
            yBounds = CheckBounds(targetPos.y, m_focusBounds.y, height);

            if(xBounds == OutOfBounds.OutPos || xBounds == OutOfBounds.OutNeg)
            {
                BringToFocusX();
            }
            if (yBounds == OutOfBounds.OutPos || yBounds == OutOfBounds.OutNeg)
            {

            }

            #region DebugLogging

            if (m_showDebug)
            {
                Debug.Log("TargetPosX: " + targetPos.x);
                Debug.Log("FocusBoundsX: " + m_focusBounds.x);
                Debug.Log("TargetPosY: " + targetPos.y);
                Debug.Log("FocusBoundsY: " + m_focusBounds.y);
                Debug.Log("xBounds: " + xBounds);
                Debug.Log("yBounds: " + yBounds);
            }

            #endregion

        }

        private void BringToFocusX()
        {
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;
            float distance = GetDistanceToBounds(m_target.position.x, width);

            

        }

        private float GetDistanceToBounds(float a_position, float a_boundsEnd)
        {
            if (a_boundsEnd > a_position)
                return a_boundsEnd - a_position;
            else
                return a_position - a_boundsEnd;
        }

        private OutOfBounds CheckBounds(float a_position, float a_boundsCenter, float a_boundsMod)
        {
            if (a_position > a_boundsCenter + a_boundsMod)
                return OutOfBounds.OutNeg;
            else if (a_position < a_boundsCenter - a_boundsMod)
                return OutOfBounds.OutPos;
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