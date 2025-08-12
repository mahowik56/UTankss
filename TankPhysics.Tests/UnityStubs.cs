namespace UnityEngine {
    public class Object {}
    public class Component : Object {}
    public class Behaviour : Component {}
    public class MonoBehaviour : Behaviour {
        private System.Collections.Generic.Dictionary<System.Type, object> _components = new System.Collections.Generic.Dictionary<System.Type, object>();
        public T GetComponent<T>() where T : new() {
            if (_components.TryGetValue(typeof(T), out var obj)) return (T)obj;
            var inst = new T();
            _components[typeof(T)] = inst;
            return inst;
        }
        public Transform transform = new Transform();
    }
    public class Transform : Component {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Transform transform => this;
    }
    public struct Vector3 {
        public float x,y,z;
        public static Vector3 zero => new Vector3();
        public float magnitude => 0f;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => new Vector3();
    }
    public struct Quaternion {
        public static Quaternion Lerp(Quaternion a, Quaternion b, float t) => new Quaternion();
    }
    public class GameObject : Object {
        public Transform transform = new Transform();
    }
    public class Collider : Component {
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    public class BoxCollider : Collider {}
    public class Rigidbody : Component {
        public float angularDrag;
        public float drag;
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    public class WheelCollider : Collider {
        public float motorTorque;
        public float brakeTorque;
    }
    public struct LayerMask {
        public static LayerMask GetMask(params string[] names) => new LayerMask();
    }
    public class SpaceAttribute : System.Attribute {
        public SpaceAttribute(float v) {}
    }
    public static class Input {
        public static float GetAxis(string axis) => 0f;
    }
    public static class Mathf {
        public static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
        public static bool Approximately(float a, float b) => System.Math.Abs(a - b) < 1e-6;
    }
}
