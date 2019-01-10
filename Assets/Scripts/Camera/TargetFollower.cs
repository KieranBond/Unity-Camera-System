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

        [SerializeField]
        private LerpEasing m_lerpEaseType = LerpEasing.Linear;

        private float m_focusMovementSpeed;
        private float m_dangerMovementSpeed;
        private float m_currentFocusLerpTime = 0f;
        private float m_currentDangerLerpTime = 0f;

        private Tween m_focusMovementXTween;
        private Tween m_focusMovementYTween;
        private Tween m_dangerMovementXTween;
        private Tween m_dangerMovementYTween;

        private Vector2 m_previousUpdateTargetPos;

        private bool m_movingDangerX = false;
        private bool m_movingDangerY = false;
        private bool m_movingFocusX = false;
        private bool m_movingFocusY = false;

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
        /// <param name="a_dangerBounds">Bounds should be given in world position. This is for the fast/outer bounds.</param>
        public void TrackingUpdate( float a_focusOffset, Rect a_focusBounds, Rect a_dangerBounds )
        {
            //Update our bounds vars.
            m_focusBounds = a_focusBounds;
            m_dangerBounds = a_dangerBounds;

            //This is our offset from the target. We use this to make sure we're this value from the Targets orientation.
            //m_focusOffset = a_focusOffset;

            Vector3 targetPos = m_targetTransform.position;

            //Halving these because we always check from the center when checking against bounds. 
            float width = m_dangerBounds.width * 0.5f;
            float height = m_dangerBounds.height * 0.5f;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;

            ///////////////////////////////////////
            ///
            ///     Highway to the danger zone.
            ///     Ride into the danger zone.
            ///     
            ///////////////////////////////////////

            xBounds = CheckDangerBounds(targetPos.x, m_dangerBounds.x, width);
            yBounds = CheckDangerBounds(targetPos.y, m_dangerBounds.y, height);

            Vector2 m_dangerBoundsDistance = new Vector2();

            //Based on if we're positive from the bounds, we'll get the distance to the closest bound.
            if (xBounds == OutOfBounds.OutPos)
            {
                m_dangerBoundsDistance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_dangerBounds.x + width) + 0.1f;
            }
            else if (xBounds == OutOfBounds.OutNeg)
            {
                m_dangerBoundsDistance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_dangerBounds.x - width) - 0.1f;
            }
            if (yBounds == OutOfBounds.OutPos)
            {
                m_dangerBoundsDistance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_dangerBounds.y + height) + 0.1f;
            }
            else if (yBounds == OutOfBounds.OutNeg)
            {
                m_dangerBoundsDistance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_dangerBounds.y - height) - 0.1f;
            }

            //if (m_previousUpdateTargetPos != new Vector2(targetPos.x, targetPos.y))
            if (m_dangerBoundsDistance != Vector2.zero /*&& m_previousUpdateTargetPos != new Vector2(targetPos.x, targetPos.y)*/) 
                BringToDanger(m_dangerBoundsDistance, m_dangerMovementSpeed);

            //////////////////////////////////////////
            ///
            //              FOCUS ZONE              //
            ///
            //////////////////////////////////////////

            float distance = targetPos.x - m_targetTransform.position.x;
            m_focusBounds.x += distance;
            distance = targetPos.y - m_targetTransform.position.y;
            m_focusBounds.y += distance;

            //Halving these because we always check from the center when checking against bounds. 
            width = m_focusBounds.width * 0.5f;
            height = m_focusBounds.height * 0.5f;

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

            if (xBounds != OutOfBounds.In || yBounds != OutOfBounds.In)
            {
            if (m_focusBoundsDistance != Vector2.zero)
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

            m_currentFocusLerpTime += Time.deltaTime;
            if (m_currentFocusLerpTime > lerpAmount)
                m_currentFocusLerpTime = lerpAmount;

            m_previousUpdateTargetPos = m_targetTransform.position;

            if (m_showDebug)
            {
                Debug.Log("FocusDistance: " + a_distance);
            }

            //Gamasutra article, they don't lerp. They snap. But they snap by the pixel distance / 32.

            Vector3 updatePos = m_camera.position;

            if(!m_movingDangerX)
                updatePos.x += a_distance.x / 32;

            if(!m_movingDangerY)
                updatePos.y += a_distance.y / 32;

            float t = GetLerpEasing(m_lerpEaseType, m_currentFocusLerpTime, lerpAmount);
            m_camera.position = Vector3.Lerp(m_camera.position, updatePos, t);




            //Checking the equivalent danger movement tweens so we don't mess the movement up with two tweens. Danger is prioritised.
            //if(m_dangerMovementXTween == null)
            //    m_focusMovementXTween = m_camera.DOMoveX(m_camera.position.x + a_distance.x, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_focusMovementXTween = null);
            //
            //if(m_dangerMovementYTween == null)
            //    m_focusMovementYTween = m_camera.DOMoveY(m_camera.position.y + a_distance.y, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_focusMovementYTween = null);
        }

        private void BringToDanger( Vector2 a_distance, float a_movementOverride = 0f )
        {
            float lerpAmount = m_dangerMovementSpeed;

            if (a_movementOverride > 0)
                lerpAmount = a_movementOverride;

            m_currentDangerLerpTime += Time.deltaTime;
            if (m_currentDangerLerpTime > lerpAmount)
                m_currentDangerLerpTime = lerpAmount;

            m_previousUpdateTargetPos = m_targetTransform.position;

            if (m_showDebug)
            {
                Debug.Log("DangerDistance: " + a_distance);
            }

            Vector3 updatePos = m_camera.position;

            updatePos.x += a_distance.x;
            updatePos.y += a_distance.y;

            float t = GetLerpEasing(m_lerpEaseType, m_currentFocusLerpTime, lerpAmount);
            m_camera.position = Vector3.Lerp(m_camera.position, updatePos, t);


            //Clear the other movement tweens (if needed). Danger tween has priority.
            //if (m_focusMovementXTween != null && a_distance.x != 0f)
            //    m_focusMovementXTween.Kill();
            //
            //if (m_focusMovementYTween != null && a_distance.y != 0f)
            //    m_focusMovementYTween.Kill();
            //
            //m_dangerMovementXTween = m_camera.DOMoveX(m_camera.position.x + a_distance.x, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_dangerMovementXTween = null);
            //m_dangerMovementYTween = m_camera.DOMoveY(m_camera.position.y + a_distance.y, lerpAmount).SetEase(Ease.Linear).OnComplete(() => m_dangerMovementYTween = null);
        }

        private float GetLerpEasing( LerpEasing a_lerpEaseType, float a_currentLerpTime, float a_lerpMoveSpeed )
        {
            float t = a_currentLerpTime / a_lerpMoveSpeed;

            ///https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            switch (a_lerpEaseType)
            {
                case LerpEasing.Linear:
                    return a_currentLerpTime / a_lerpMoveSpeed;

                case LerpEasing.SmoothStep:
                    return t * t * (3f - 2f * t);

                case LerpEasing.SmootherStep:
                    return t * t * t * (t * (6f * t - 15f) + 10f);


                case LerpEasing.Out:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);

                case LerpEasing.In:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

                case LerpEasing.InExpo:
                    return t * t;
            }

            return t;
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

        private OutOfBounds CheckDangerBounds( float a_position, float a_boundsCenter, float a_boundsMod )
        {
            //a_position is the target's position.

            if (a_position >= a_boundsCenter + a_boundsMod)
                return OutOfBounds.OutPos;
            else if (a_position <= a_boundsCenter - a_boundsMod)
                return OutOfBounds.OutNeg;
            else
                return OutOfBounds.In;
        }

        private enum LerpEasing
        {
            Linear,
            SmoothStep,
            SmootherStep,
            Out,
            In,
            InExpo,
        }

        private enum OutOfBounds
        {
            In,
            OutPos,
            OutNeg
        }
    }
}