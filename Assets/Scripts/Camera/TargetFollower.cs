﻿using CameraDesign.Controller.API;
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

        public bool m_showDebug = false;

        private float m_focusMovementSpeed;

        private Tween m_movementXTween;
        private Tween m_movementYTween;

        public void Initialise( ICameraTarget a_target, Transform a_camera, bool a_showDebug = false, float a_focusMovementSpeed = 5f )
        {
            m_target = a_target;
            m_targetTransform = a_target.m_transform;
            m_camera = a_camera;
            m_showDebug = a_showDebug;
            m_focusMovementSpeed = a_focusMovementSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_focusMovementSpeed"></param>
        public void SettingsUpdate( float a_focusMovementSpeed = 5f)
        {
            m_focusMovementSpeed = a_focusMovementSpeed;
        }

        /// <summary>
        ///This update should be called every frame, every time the target moves, 
        ///or every time we've finished moving to the target for best results.
        /// </summary>
        /// <param name="a_focusBounds">Bounds should be given in world position.</param>
        public void TrackingUpdate( Rect a_focusBounds )
        {
            //Update our bounds.
            m_focusBounds = a_focusBounds;

           // Vector3 targetPos = m_target.TransformPoint(m_target.position);
            Vector3 targetPos = m_targetTransform.position;

            //Check that the target is within bounds.
            OutOfBounds xBounds = OutOfBounds.In;
            OutOfBounds yBounds = OutOfBounds.In;


            //Halving these because we always check from the center when checking against bounds. 
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            xBounds = CheckBounds(targetPos.x, m_focusBounds.x, width);
            yBounds = CheckBounds(targetPos.y, m_focusBounds.y, height);

            Vector2 m_distance = new Vector2();

            if (xBounds == OutOfBounds.OutPos)
            {
                m_distance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_focusBounds.x + width);
            }
            else if (xBounds == OutOfBounds.OutNeg)
            {
                m_distance.x = GetDistanceFromBounds(m_targetTransform.position.x, m_focusBounds.x - width);
            }
            if (yBounds == OutOfBounds.OutPos)
            {
                m_distance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_focusBounds.y + height);
            }
            else if (yBounds == OutOfBounds.OutNeg)
            {
                m_distance.y = GetDistanceFromBounds(m_targetTransform.position.y, m_focusBounds.y - height);
            }

            BringToFocus(m_distance);

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

        private void BringToFocus( Vector2 a_distance)
        {
            float width = m_focusBounds.width * 0.5f;
            float height = m_focusBounds.height * 0.5f;

            if (m_showDebug)
            {
                Debug.Log("Distance: " + a_distance);
            }

            //Gamasutra article, they don't lerp. They snap. But they snap by the distance / 32.
            Vector3 currentPos = m_camera.position;
            Vector3 newPos = currentPos;
            newPos.x = newPos.x + (a_distance.x / 32f);
            m_camera.position = newPos;

            //m_movementXTween = m_camera.DOMoveX(m_camera.position.x + a_distance.x, m_focusMovementSpeed).SetEase(Ease.Linear).OnComplete(() => m_movementXTween = null);
            m_movementYTween = m_camera.DOMoveY(m_camera.position.y + a_distance.y, m_focusMovementSpeed).SetEase(Ease.Linear).OnComplete(() => m_movementYTween = null);

            //m_movementTween = m_camera.DOMoveX(m_camera.position + new Vector3(a_distance.x, a_distance.y, 0f), 5f).OnComplete(() => m_movementTween = null);
            
        }

        private float GetDistanceFromBounds(float a_position, float a_boundsEnd)
        {
            return a_position - a_boundsEnd;

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