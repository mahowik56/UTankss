using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    [System.Serializable]
    public class TankWheelPair
    {
        public WheelCollider Left;
        public WheelCollider Right;
    }

    /// <summary>
    /// Simple tank chassis controller based on Unity WheelColliders.
    /// Provides differential steering by applying
    /// separate torque values to left and right wheel sets.
    /// </summary>
    public class ChassisSystem : MonoBehaviour
    {
        public List<TankWheelPair> Wheels = new List<TankWheelPair>();
        public float MotorTorque = 1500f;
        public float BrakeTorque = 3000f;

        private void FixedUpdate()
        {
            float move = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal");

            float leftTorque = Mathf.Clamp(move - turn, -1f, 1f) * MotorTorque;
            float rightTorque = Mathf.Clamp(move + turn, -1f, 1f) * MotorTorque;

            foreach (var pair in Wheels)
            {
                pair.Left.motorTorque = leftTorque;
                pair.Right.motorTorque = rightTorque;

                float brake = Mathf.Approximately(move, 0f) ? BrakeTorque : 0f;
                pair.Left.brakeTorque = brake;
                pair.Right.brakeTorque = brake;
            }
        }
    }
}
