namespace SecuredSpace.ClientControl.Log {
    public class LogComponent {}
}
namespace SecuredSpace.Battle.Tank.Hull {
    public class TankManager { public long ManagerEntityId; public UnityEngine.Transform Turret = new UnityEngine.Transform(); }
    public class HullManager { public TankManager parentTankManager = new TankManager(); }
}
namespace SecuredSpace.Battle.Tank.Turret {
    public class TurretRotaion : UnityEngine.MonoBehaviour {
        public float LeftTurn; public float RightTurn; public float nowSpeed; public void ExternalManagement(float weaponRotation, float weaponRotationControl) {}
    }
}
namespace UTanksClient {
    namespace Core.Logging {
        public static class ULogger { public static void Log(string msg) {} }
    }
    namespace ECS.Types.Battle {
        public struct Vector3S {
            public Vector3S(UnityEngine.Vector3 v) { this.x = v.x; this.y = v.y; this.z = v.z; }
            public float x,y,z;
            public static Vector3S ConvertToVector3SUnScaling(UnityEngine.Vector3 v, float c) => new Vector3S(v);
        }
        public struct QuaternionS { public QuaternionS(UnityEngine.Quaternion q) { } }
    }
    namespace Network.NetworkEvents.FastGameEvents {
        public class RawMovementEvent {
            public ECS.Types.Battle.Vector3S position;
            public ECS.Types.Battle.QuaternionS rotation;
            public long PlayerEntityId;
            public float TankMoveControl;
            public float TankTurnControl;
            public ECS.Types.Battle.Vector3S velocity;
            public float WeaponRotation;
            public float WeaponRotationControl;
            public ECS.Types.Battle.QuaternionS turretRotation;
        }
    }
    namespace Extensions {
        public static class Extensions {
            public static UnityEngine.Vector3 ConvertToUnityVector3Constant007Scaling(this ECS.Types.Battle.Vector3S v) => new UnityEngine.Vector3();
            public static UnityEngine.Vector3 ConvertToUnityVector3(this ECS.Types.Battle.Vector3S v) => new UnityEngine.Vector3();
            public static ECS.Types.Battle.Vector3S ConvertToVector3SUnScaling(this UnityEngine.Vector3 v, float c) => new ECS.Types.Battle.Vector3S(v);
            public static UnityEngine.Quaternion ToQuaternion(this ECS.Types.Battle.QuaternionS q) => new UnityEngine.Quaternion();
        }
    }
}

namespace UTanksClient {
    public static class Const { public const float ResizeResourceConst = 1f; }
    public class ClientNetworkService {
        public static ClientNetworkService instance = new ClientNetworkService();
        public SocketWrapper Socket = new SocketWrapper();
        public void Send<T>(T evt) {}
        public class SocketWrapper {
            public void emit(object evt) {}
        }
    }
}

namespace System.Threading.Tasks {
    public static class TaskEx {
        public static Task Delay(int ms) => Task.Delay(ms);
        public static Task RunAsync(System.Action action) { action(); return Task.CompletedTask; }
    }
}
