using System.Collections.Generic;
using System.Linq;
using Debugging;
using GameManaging;
using UnityEngine;
using Utilities;

namespace ObjectSpawning
{

    public class ObjectSpawner : MonoBehaviour
    {
        public static Vector3 PoolingObjectDeactivatePosition { get; set; } = new Vector3(-1000, -1000, -1000);
        private static int idCounter = 0;
        private static string className = nameof(ObjectSpawner);

        private static ObjectSpawner instance;
        private Dictionary<string, Pool> poolsDict = new Dictionary<string, Pool>();
        private Transform standardParent;

        private void OnDestroy()
        {
            GameManager.LoadingFinished -= SetUpStandardParent;
            GameManager.LoadingStarted -= ClearDestroyOnLoadObjectPools;
        }

        private void Awake()
        {
            if(instance == null && instance != this)
            {
                instance = this;
                GameManager.LoadingStarted += ClearDestroyOnLoadObjectPools;
                GameManager.LoadingFinished += SetUpStandardParent;
                SetUpStandardParent();
            }
        }

        private void SetUpStandardParent()
        {
            if (standardParent != null)
                return;

            standardParent = new GameObject("RuntimeObjects").transform;
            standardParent.gameObject.AddComponent<InitializeGizmos>();
        }

        public static int GetNextId() => ++idCounter;

        #region Spawning -----------------------------------------------------------------

        public static T LocalSpawn<T>(GameObject _prefab, Vector3? _position = null, Quaternion? _rotation = null, Transform _parent = null)
        {
            if (_parent == null)
                _parent = instance.standardParent;

            if (_position == null)
                _position = Vector3.zero;

            if (_rotation == null)
                _rotation = Quaternion.identity;

            return Instantiate(_prefab, _position.Value, _rotation.Value, _parent).GetComponent<T>();
        }

        public static GameObject LocalSpawn(GameObject _prefab, Vector3? _position = null, Quaternion? _rotation = null, Transform _parent = null)
        {
            if (_parent == null)
                _parent = instance.standardParent;

            if (_position == null)
                _position = Vector3.zero;

            if (_rotation == null)
                _rotation = Quaternion.identity;

            return Instantiate(_prefab, _position.Value, _rotation.Value, _parent).gameObject;
        }
        #endregion -----------------------------------------------------------------

        #region ObjectPooling -----------------------------------------------------------------
        public static void AddPool(int _id, GameObject _prefab, int _size, bool _clearOnLoad, Transform _parent = null)
        {
            if (_parent == null)
                _parent = instance.standardParent;

            if (_prefab.GetComponent<IPooledObject>() == null)
                Debug.LogError("Trying to add pool with gameObject which does not implement IPooledObject!");

            string name = _id.ToString() + _prefab.GetHashCode().ToString();
            if (instance.poolsDict.ContainsKey(name))
                Debug.LogError("Trying to add pool \"" + name + "\" which already exist !");
            else
                instance.poolsDict.Add(name, new Pool(_id, name, _prefab, _size, _clearOnLoad, _parent));
        }

        public static void AddObjectToPool(int _id, GameObject _prefab, GameObject _gameObject, Transform _parent = null)
        {
            if (_gameObject.GetComponent<IPooledObject>() == null)
                Debug.LogError("Trying to gameObject to pool which does not implement IPooledObject!");

            string name = _id.ToString() + _prefab.GetHashCode().ToString();
            if (!instance.poolsDict.ContainsKey(name))
                Debug.LogError("Trying to add to pool \"" + name + "\" which does not exist !");
            else
                instance.poolsDict[name].AddObject(_gameObject, _parent);
        }

        public static void AddObjectsToPool(int _id, GameObject _prefab, GameObject[] _gameObject, Transform _parent = null)
        {
            for (int i = 0; i < _gameObject.Length; i++)
            {
                AddObjectToPool(_id, _prefab, _gameObject[i], _parent);
            }
        }

        private void ClearDestroyOnLoadObjectPools()
        {
            if (poolsDict.Count == 0)
                return;

            List<Pool> pools = poolsDict.Values.Where(x => x.ClearOnLoad == true).ToList();
            for (int i = pools.Count - 1; i >= 0; i--)
            {
                DespawnPool(pools[i]);
                poolsDict.Remove(pools[i].Name);
            }
        }

        private void DespawnPool(Pool _pool)
        {
            GameObject instance;
            while (_pool.ObjectPool.Count > 0)
            {
                instance = _pool.ObjectPool.Dequeue();
                Destroy(instance);
            }
        }

        public static GameObject SpawnPooled(int _id, GameObject _prefab, Vector3? _position = null, Quaternion? _rotation = null)
        {
            string poolName = _id.ToString() + _prefab.GetHashCode().ToString();
            if (!instance.poolsDict.ContainsKey(poolName))
            {
                Debug.LogError("Trying to spawn object pooled but corresponding pool not present!");
            }

            GameObject objectInstance = instance.poolsDict[poolName].GetNextObject();

            if (objectInstance == null)
            {
                Debug.LogError("Could not get object from pool!");
                return null;
            }
            IPooledObject pooledObject = objectInstance.GetComponent<IPooledObject>();
            HandleActivePoolObject(objectInstance, pooledObject);

            objectInstance.gameObject.SetActive(true);
            pooledObject.OnObjectSpawn(_position, _rotation);
            if (objectInstance.transform.parent != null)
                objectInstance.transform.SetAsFirstSibling();

            return objectInstance;
        }

        public static V SpawnPooled<V>(int _id, GameObject _prefab, Vector3? _position = null, Quaternion? _rotation = null)
        {
            string poolName = _id.ToString() + _prefab.GetHashCode().ToString();
            if (!instance.poolsDict.ContainsKey(poolName))
            {
                Debug.LogError("Trying to spawn object pooled but corresponding pool not present!");
            }

            GameObject objectInstance = instance.poolsDict[poolName].GetNextObject();

            if (objectInstance == null)
            {
                Debug.LogError("Could not get object from pool!");
                return default;
            }

            IPooledObject pooledObject = objectInstance.GetComponent<IPooledObject>();
            HandleActivePoolObject(objectInstance, pooledObject);

            objectInstance.gameObject.SetActive(true);
            pooledObject.OnObjectSpawn(_position, _rotation);
            if (objectInstance.transform.parent != null)
                objectInstance.transform.SetAsFirstSibling();

            return objectInstance.GetComponent<V>();
        }

        public static GameObject SpawnPooledUnactive(int _id, GameObject _prefab, Vector3? _position = null, Quaternion? _rotation = null)
        {
            string poolName = _id.ToString() + _prefab.GetHashCode().ToString();
            if (!instance.poolsDict.ContainsKey(poolName))
            {
                Debug.LogError("Trying to spawn object pooled but corresponding pool not present!");
            }

            GameObject objectInstance = instance.poolsDict[poolName].GetNextObject();

            if (objectInstance == null)
            {
                Debug.LogError("Could not get object from pool!");
                return null;
            }

            IPooledObject pooledObject = objectInstance.GetComponent<IPooledObject>();
            HandleActivePoolObject(objectInstance, pooledObject);

            pooledObject.OnObjectSpawn(_position, _rotation);
            if (objectInstance.transform.parent != null)
                objectInstance.transform.SetAsFirstSibling();

            return objectInstance;
        }

        public static GameObject PoolDequeue(int _id, GameObject _prefab)
        {
            string poolName = _id.ToString() + _prefab.GetHashCode().ToString();
            if (!instance.poolsDict.ContainsKey(poolName))
            {
                Debug.LogError("Trying to spawn object pooled but corresponding pool not present!");
            }

            GameObject dequeuedInstance = instance.poolsDict[poolName].GetNextObject();
            HandleActivePoolOIbject(dequeuedInstance);
            return dequeuedInstance;
        }

        private static void HandleActivePoolObject(GameObject _instance, IPooledObject _pooledObject)
        {
            if (!_instance.activeSelf)
                return;

            DebugManager.OutputWarning(EScriptGroup.Spawning, className, "Spawning pooled object which is still active!");
            _pooledObject.OnDeactivated();
        }

        private static void HandleActivePoolOIbject(GameObject _instance)
        {
            if (!_instance.activeSelf)
                return;

            DebugManager.OutputWarning(EScriptGroup.Spawning, className, "Spawning pooled object which is still active!");
            _instance.GetComponent<IPooledObject>().OnDeactivated();
        }

        public static void SetPoolDeactivatePosition(Transform _transform) => _transform.position = PoolingObjectDeactivatePosition;

        public static void UnparentPooledObject(Transform _transform) => _transform.parent = instance.standardParent;

        public static void SetPoolObjectDeactivated(GameObject _gameObject)
        {
            SetPoolDeactivatePosition(_gameObject.transform);
            _gameObject.SetActive(false);
        }
        #endregion -----------------------------------------------------------------
    }
}
