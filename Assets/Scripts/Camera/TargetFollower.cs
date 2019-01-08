using CameraDesign.Controller.API;
using DG.Tweening;
using System;
using UnityEngine;

namespace CameraDesign.Controller.Impl
{
    public class TargetFollower : MonoBehaviour
    {
        private Transform m_camera;
        private ICameraTarget m_target;
        private Transform m_targetTransform;
        private Rect m_focusBounds;
        private Rect m_dangerBounds;
        private float m_focusOffset;

        [SerializeField]
        private bool m_showDebug = false;
        [SerializeField]
        private bool m_showAdvancedDebug = false;

        private float m_focusMovementSpeed;
        private float m_dangerMovementSpeed;

        private Tween m_movementXTween;
        private Tween m_movementYTween;

        private Vector2 m_previousUpdateTargetPos;

        public void Initialise( ICameraTarget a_target, Transform a_camera, bool a_showDebug = false, float a_focusMovementSpeed = 5f )
        {
            m_target = a_target;
            m_targetTransform = a_target.m_transform;
            m_camera = a_camera;
            m_showDebug = a_showDebug;
            m_focusMovementSpeed = a_focusMovementSpeed;
        }

        /// <summary>
        /// Update any settings, before the tracking update is called.
        /// </summary>
        /// <param name="a_focusMovementSpeed">Movement speed for chasing the target outside the focus bounds. Smaller is faster.</param>
        /// <param name="a_dangerMovementSpeed">Movement speed for chasing the target outside the danger bounds. Smaller is faster - recommended below 1.</param>
        public void SettingsUpdate( float a_focusMovementSpeed = 5f, float a_dangerMovementSpeed = 0.1f )
        {
            m_focusMovementSpeed = a_focusMovementSpeed;
            m_dangerMovementSpeed = a_dangerMovementSpeed;
        }

        /// <summary>
        ///This update should be called every frame, every time the target moves, 
        ///or every time we've finished moving to the target for best results.
        /// </summary>
        /// <param name="a_focusOffset">X offset from the target. Distance from centre of focus zone to centre of target.</param>
        /// <param name="a_focusBounds">Bounds should be given in world position.</param>
        public void TrackingUpdate( float a_focusOffset, Rect a_focusBounds, Rect a_dangerBounds )
        {
            //Update our bounds.
            m_focusBounds = a_focusBounds;
            m_dangerBounds = a_dangerBounds;

            //This is our offset from the target. We use this to make sure we're this value from the Targets orientation.
            m_focusOffset = a_focusOffset;

            Vector3 targetPos = m_targetTransform.position;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;


            //Halving these because we always check from the center when checking against bounds. 
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            xBounds = CheckBounds(targetPos.x, m_focusBounds.x, width);
            yBounds = CheckBounds(targetPos.y, m_focusBounds.y, height);

            Vector2 m_focusBoundsDistance = new Vector2();

            //Based on if we're positive from the bounds, we'll get the distance to the closest bound.
            if (xBounds == OutOfBounds.OutPos)
            {
                m_focusBoundsDistance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_focusBounds.x + width);
            }
            else if (xBounds == OutOfBounds.OutNeg)
            {
                m_focusBoundsDistance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_focusBounds.x - width);
            }
            if (yBounds == OutOfBounds.OutPos)
            {
                m_focusBoundsDistance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_focusBounds.y + height);
            }
            else if (yBounds == OutOfBounds.OutNeg)
            {
                m_focusBoundsDistance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_focusBounds.y - height);
            }

            if(xBounds != OutOfBounds.In || yBounds != OutOfBounds.In)
            {
                if(m_previousUpdateTargetPos != new Vector2(targetPos.x, targetPos.y))
                    BringToFocus(m_focusBoundsDistance);
            }

            #region DebugLogging

            if (m_showDebug && m_showAdvancedDebug)
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

        private void BringToFocus( Vector2 a_distance, float a_movementOverride = 0f )
        {
            float lerpAmount = m_focusMovementSpeed;

            if (a_movementOverride > 0)
                lerpAmount = a_movementOverride;

            m_previousUpdateTargetPos = m_targetTransform.position;

            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            if (m_showDebug)
            {
                Debug.Log("Distance: " + a_distance);
            }

            //Gamasutra article, they don't lerp. They snap. But they snap by the pixel distance / 32.

            m_movementXTween = m_camera.DOMoveX(m_camera.position.x + a_distance.x, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_movementXTween = null);
            m_movementYTween = m_camera.DOMoveY(m_camera.position.y + a_distance.y, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_movementYTween = null);

            //m_movementTween = m_camera.DOMoveX(m_camera.position + new Vector3(a_distance.x, a_distance.y, 0f), 5f).OnComplete(() => m_movementTween = null);

        }

        private float GetDistanceFromBounds( float a_position, float a_boundsEnd )
        {
            return a_position - a_boundsEnd;
        }

        private OutOfBounds CheckBounds( float a_position, float a_boundsCenter, float a_boundsMod )
        {
            //a_position is the target's position.

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