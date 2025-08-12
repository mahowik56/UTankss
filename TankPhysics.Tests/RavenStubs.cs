using System.Collections.Generic;
namespace SecuredSpace.Important.Raven {
    public class Component {}
    public class Entity {
        public List<object> storage = new List<object>();
        public void AddComponent(object c) { storage.Add(c); }
    }
    public class RigidbodyComponent : Component {
        public RigidbodyComponent() {}
        public RigidbodyComponent(UnityEngine.Rigidbody rb) { Rigidbody = rb; }
        public UnityEngine.Rigidbody Rigidbody { get; set; }
        public float cacheRigidbodyAngularDrag;
        public float cacheRigidbodyDrag;
        public UnityEngine.Transform RigidbodyTransform => new UnityEngine.Transform();
    }
    public class SpeedComponent : Component {
        public float Speed { get; set; }
        public float TurnSpeed { get; set; }
        public float Acceleration { get; set; }
    }
    public class TankCollidersComponent : Component {
        public UnityEngine.BoxCollider BoundsCollider { get; set; }
        public UnityEngine.Collider TankToTankCollider { get; set; }
        public UnityEngine.Collider TankToStaticTopCollider { get; set; }
        public List<UnityEngine.GameObject> TargetingColliders { get; set; }
        public List<UnityEngine.GameObject> VisualTriggerColliders { get; set; }
        public List<UnityEngine.Collider> TankToStaticColliders { get; set; }
        public UnityEngine.Vector3 Extends { get; set; }
    }
    public class ChassisConfigComponent : Component {
        public float MaxRayLength { get; set; }
        public float NominalRayLength { get; set; }
        public int NumRaysPerTrack { get; set; }
        public bool ReverseBackTurn { get; set; }
        public float SuspensionRayOffsetY { get; set; }
        public float TrackSeparation { get; set; }
        public UnityEngine.LayerMask trackLayerMask { get; set; }
    }
    public class DampingComponent : Component { public float Damping { get; set; } }
    public class WeightComponent : Component { public float Weight { get; set; } }
    public class SimpleValueSmoother {
        public SimpleValueSmoother(float a,float b,float c,float d){}
    }
    public class ChassisSmootherComponent : Component {
        public SimpleValueSmoother maxSpeedSmoother { get; set; }
        public SimpleValueSmoother maxTurnSpeedSmoother { get; set; }
    }
    public class ChassisComponent : Component {
        public float MoveAxis { get; set; }
        public float TurnAxis { get; set; }
        public float EffectiveMoveAxis { get; set; }
        public float EffectiveTurnAxis { get; set; }
        public float SpringCoeff { get; set; }
        public void Reset(){ MoveAxis=TurnAxis=EffectiveMoveAxis=EffectiveTurnAxis=SpringCoeff=0f; }
    }
    public class EffectiveSpeedComponent : Component {
        public float MaxSpeed { get; set; }
        public float MaxTurnSpeed { get; set; }
    }
    public class Track { public int numContacts; }
    public class TrackComponent : Component {
        public Track LeftTrack { get; set; }
        public Track RightTrack { get; set; }
    }
    public class SpeedConfigComponent : Component {
        public float ReverseAcceleration { get; set; }
        public float ReverseTurnAcceleration { get; set; }
        public float SideAcceleration { get; set; }
        public float TurnAcceleration { get; set; }
    }
    public class TankJumpComponent : Component {}
    public class GameTankSettingsComponent : Component {
        public bool DamageInfoEnabled { get; set; }
        public bool HealthFeedbackEnabled { get; set; }
        public bool MovementControlsInverted { get; set; }
        public bool SelfTargetHitFeedbackEnabled { get; set; }
    }
}
