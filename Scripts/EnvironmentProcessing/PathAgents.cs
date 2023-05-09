using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EnvironmentHelpers
{
    public class PathAgent : MonoBehaviour
    {
        public Action<PathAgent, int, float> OnWaypointCompleted;
        public int CurrentIndex;
        public Transform LastWaypoint;
        public Transform CurrentWaypoint;

        Quaternion currentRotation;
        Quaternion TargetRotation;

        private float m_fSpeed = 1f;

        private float distanceBetweenPoints = 0f;
        private float totalDistanceBetweenPoints = 0f;

        public void Init(float speed)
        {
            m_fSpeed = speed;
        }

        public void SetPosition(int iIndex, Transform last, Transform next, float normalisedPosition)
        {
            totalDistanceBetweenPoints = Vector3.Distance(last.position, next.position);
            distanceBetweenPoints = totalDistanceBetweenPoints * normalisedPosition;
            transform.position = Vector3.Lerp(last.position, next.position, normalisedPosition);

            if (null == CurrentWaypoint)
            {
                transform.LookAt(next);
            }

            CurrentIndex = iIndex;
            LastWaypoint = last;
            CurrentWaypoint = next;

            currentRotation = transform.rotation;
            TargetRotation = Quaternion.LookRotation(next.position - last.position);
        }

        public void ManualUpdate()
        {
            distanceBetweenPoints += m_fSpeed * Time.deltaTime;

            if (distanceBetweenPoints > totalDistanceBetweenPoints)
            {
                //Get next waypoint factoring in overshoot of this waypoint
                OnWaypointCompleted?.Invoke(this, CurrentIndex, distanceBetweenPoints - totalDistanceBetweenPoints);
            }
            else
            {
                //calculate normalised position between points
                float normalisedMovement = Mathf.InverseLerp(0f, totalDistanceBetweenPoints, distanceBetweenPoints);
                transform.position = Vector3.Lerp(LastWaypoint.position, CurrentWaypoint.position, normalisedMovement);
                transform.rotation = Quaternion.Lerp(currentRotation, TargetRotation, normalisedMovement);
            }
        }
    }
}
