using UnityEngine;

namespace ObjectSpawning
{
    public interface IPooledObject
    {
        public int Id { get; }

        public void Initialize();

        public void OnObjectSpawn(Vector3? _position = null, Quaternion? _rotation = null);

        public void OnDeactivated();
    }
}