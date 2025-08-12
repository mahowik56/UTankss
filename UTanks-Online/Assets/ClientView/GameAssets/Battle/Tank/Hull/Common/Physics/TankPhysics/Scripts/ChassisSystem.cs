using System.Collections.Generic;
using UnityEngine;
using SecuredSpace.ClientControl.Log;
using SecuredSpace.Important.TPhysics;

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
        // Existing basic wheel logic
        public List<TankWheelPair> Wheels = new List<TankWheelPair>();
        public float MotorTorque = 1500f;
        public float BrakeTorque = 3000f;

        // Additional configuration properties expected by TankChassisManager
        public float SpringScaler;
        public float SpringCoefManual;
        public float SideDamperScaler;
        public float SideSprindDamperDelta;
        public float FrontSpringDamperDelta;
        public float SwingCoef;

        public TankChassisManager chassisManager;
        public LogComponent LogComponent;

        // Data containers used by TankChassisManager
        public class ChassisInitNode
        {
            public RigidbodyComponent rigidbody;
            public SpeedComponent speed;
            public TankCollidersComponent tankColliders;
            public ChassisConfigComponent chassisConfig;
            public DampingComponent damping;
            public WeightComponent weight;
            public ChassisSmootherComponent chassisSmoother;
            public Entity Entity;
        }

        public class ChassisNode
        {
            public Entity Entity;
            public ChassisComponent chassis;
            public EffectiveSpeedComponent effectiveSpeed;
            public TrackComponent track;
            public SpeedComponent speed;
            public ChassisConfigComponent chassisConfig;
            public ChassisSmootherComponent chassisSmoother;
            public RigidbodyComponent rigidbody;
            public TankCollidersComponent tankColliders;
            public object cameraVisibleTrigger;
            public SpeedConfigComponent speedConfig;
            public object tankGroup;
        }

        // Placeholder initialization logic
        public void InitTankChassis(ChassisInitNode initNode)
        {
            // Store components on the entity for later use
            initNode.Entity.AddComponent(initNode.tankColliders);
            initNode.Entity.AddComponent(initNode.rigidbody);
            initNode.Entity.AddComponent(initNode.speed);
            initNode.Entity.AddComponent(initNode.chassisConfig);
            initNode.Entity.AddComponent(initNode.damping);
            initNode.Entity.AddComponent(initNode.weight);
            initNode.Entity.AddComponent(initNode.chassisSmoother);
        }

        // Simplified physics update expected by TankChassisManager
        public void FixedUpdate(ChassisNode chassisNode, TankJumpComponent tankJump, GameTankSettingsComponent settings, bool movable)
        {
            // Placeholder: custom chassis physics would be implemented here.
        }

        // Original Unity FixedUpdate controlling wheel colliders
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
