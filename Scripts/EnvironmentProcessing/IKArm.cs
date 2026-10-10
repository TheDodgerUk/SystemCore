using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentHelpers
{
    public class IKArm : MonoBehaviour
    {
        [SerializeField]
        private Transform pivot, upper, lower, effector, tip;
        private Vector3 m_Target = Vector3.forward;
        private Vector3 Normal = Vector3.up;

        private float upperLength, lowerLength, effectorLength, pivotLength;
        Vector3 effectorTarget, tipTarget;

        // Start is called before the first frame update
        [InspectorButton]
        private void Initialise()
        {
            upperLength = (lower.position - upper.position).magnitude;
            lowerLength = (effector.position - lower.position).magnitude;
            effectorLength = (tip.position - effector.position).magnitude;
            pivotLength = (upper.position - pivot.position).magnitude;
        }

        // Update is called once per frame
        [InspectorButton]
        private void OnUpdate()
        {
            tipTarget = m_Target;
            effectorTarget = m_Target + Normal * effectorLength;
            Solve();
        }

        private void Solve()
        {
            var pivotDir = effectorTarget - pivot.position;
            pivot.rotation = Quaternion.LookRotation(pivotDir);

            var upperToTarget = (effectorTarget - upper.position);
            var a = upperLength;
            var b = lowerLength;
            var c = upperToTarget.magnitude;

            var B = Mathf.Acos((c * c + a * a - b * b) / (2f * c * a)) * Mathf.Rad2Deg;
            var C = Mathf.Acos((a * a + b * b - c * c) / (2f * a * b)) * Mathf.Rad2Deg;

            if(!float.IsNaN(C))
            {
                var upperRotation = Quaternion.AngleAxis(-B, Vector3.right);
                upper.localRotation = upperRotation;
                var lowerRotation = Quaternion.AngleAxis(180f - C, Vector3.right);
                lower.localRotation = lowerRotation;
            }

            var effectorRotation = Quaternion.LookRotation(tipTarget - effector.position);
            effector.rotation = effectorRotation;
        }
    }
}
