using System.Collections.Generic;
using UnityEngine;

namespace ObjectSpawning
{
    public class Pool
    {
        /// <summary>
        /// Id of the pool.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name of the pool.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prefab of the pool.
        /// </summary>
        public GameObject Prefab { get; set; }

        /// <summary>
        /// PoolSize of the pool.
        /// </summary>
        public int PoolSize { get; set; }

        /// <summary>
        /// If the instances are networked.
        /// </summary>
        public bool IsNetworked { get; set; }

        /// <summary>
        /// If the pool should be cleared when loading started.
        /// </summary>
        public bool ClearOnLoad { get; set; }
    
        /// <summary>
        /// All objects from the pool as a queue.
        /// </summary>
        public Queue<GameObject> ObjectPool { get; set; }

        #region Constructors -----------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_id">Id of the pool</param>
        /// <param name="_name">Name of the pool</param>
        /// <param name="_prefab">Prefab of the pool</param>
        /// <param name="_poolSize">PoolSize of the pool</param>
        /// <param name="_clearOnLoad">If the pool should be cleared when loading started</param>
        /// <param name="_parent">Parent to set the objects transforms of</param>
        public Pool(int _id, string _name, GameObject _prefab, int _poolSize, bool _clearOnLoad, Transform _parent = null)
        {
            ID = _id;
            Name = _name;
            Prefab = _prefab;
            PoolSize = _poolSize;
            ClearOnLoad = _clearOnLoad;
            ObjectPool = new Queue<GameObject>();

            for (int i = 0; i < PoolSize; i++)
            {
                GameObject instance = ObjectSpawner.LocalSpawn(Prefab, ObjectSpawner.PoolingObjectDeactivatePosition, Quaternion.identity);

                instance.name = ID.ToString() + _prefab.name + i.ToString();

                instance.GetComponent<IPooledObject>().Initialize();

                instance.SetActive(false);

                // Set a parent an enqueue.
                if (_parent != null)
                    instance.transform.parent = _parent;

                ObjectPool.Enqueue(instance);
            }
        }

        /// <summary>
        /// Constructor with existing object list
        /// </summary>
        /// <param name="_id">Id of the pool</param>
        /// <param name="_name">Name of the pool</param>
        /// <param name="_prefab">Prefab of the pool</param>
        /// <param name="_objectList">List of previuosly instantiated objects</param>
        /// <param name="_isNetworked">If the instances are networked</param>
        /// <param name="_clearOnLoad">If the pool should be cleared when loading started</param>
        /// <param name="_parent">Parent to set the objects transforms of</param>
        public Pool(int _id, string _name, GameObject _prefab, List<GameObject> _objectList, bool _isNetworked, bool _clearOnLoad, Transform _parent = null)
        {
            ID = _id;
            Name = _name;
            Prefab = _prefab;
            PoolSize = _objectList.Count;
            IsNetworked = _isNetworked;
            ClearOnLoad = _clearOnLoad;

            // Parse objects in list and set name and parent.
            ObjectPool = new Queue<GameObject>();
            for (int i = 0; i < PoolSize; i++)
            {
                ObjectPool.Enqueue(_objectList[i]);
                _objectList[i].name = ID.ToString() + _prefab.name + i.ToString();
                _objectList[i].transform.parent = _parent;
            }
        }
        #endregion -----------------------------------------------------------------

        #region Getting Objects -----------------------------------------------------------------
        /// <summary>
        /// Peek the next object.
        /// </summary>
        /// <returns>The object to use next</returns>
        public GameObject PeekNextObject() => ObjectPool.Peek();

        /// <summary>
        /// Get next object and dequeue.
        /// </summary>
        /// <returns>The object to use next</returns>
        public GameObject GetNextObject()
        {
            GameObject nextInstance = ObjectPool.Dequeue();
            ObjectPool.Enqueue(nextInstance);
            return nextInstance;
        }
        #endregion -----------------------------------------------------------------

        public void AddObject(GameObject _gameObjectInstance, Transform _parent = null)
        {
            ObjectPool.Enqueue(_gameObjectInstance);
            PoolSize = ObjectPool.Count;
            if(_parent != null)
                _gameObjectInstance.transform.parent = _parent;
        }
    }
}