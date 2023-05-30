using UnityEngine;

namespace RandomNumberGeneration
{
    public class RNG : MonoBehaviour
    {
        [SerializeField] private int seed = 0;

        private static System.Random prng = new System.Random();


        /// <summary>
        /// Initializes the prgn's.
        /// </summary>
        private void Awake()
        {
            prng = (seed == 0) ? new System.Random() : new System.Random(seed);
            if (seed != 0) UnityEngine.Random.InitState(seed);
        }

        /// <summary>
        /// Gets random int from min to max values.
        /// </summary>
        /// <param name="_minInclusive">Min inclusivly</param>
        /// <param name="_maxInclusive">Max inclusivly</param>
        /// <param name="_synced">If the call should be synced over the network</param>
        /// <param name="_log">If the coll should be logged</param>
        /// <returns>Random value ranging from _minInclusive to _maxInclusive</returns>
        public static int Range(int _minInclusive, int _maxInclusive)
        {
            return prng.Next(_minInclusive, _maxInclusive + 1);
        }

        /// <summary>
        /// Gets random float from min to max values.
        /// </summary>
        /// <param name="_min">Min inclusivly</param>
        /// <param name="_max">Max inclusivly</param>
        /// <param name="_synced">If the call should be synced over the network</param>
        /// <param name="_log">If the coll should be logged</param>
        /// <returns>Random value ranging from _minInclusive to _maxInclusive</returns>
        public static float RangeSingle(float _min, float _max)
        {
            return (float)prng.NextDouble() * (_max - _min) + _min;
        }

        /// <summary>
        /// Gets random Vector2 on unit sphere.
        /// </summary>
        /// <param name="_synced">If the call should be synced over the network</param>
        /// <param name="_log">If the coll should be logged</param>
        /// <returns>Random on unit sphere Vector2</returns>
        public static Vector2 OnUnitSphere()
        {
            int negate = Range(0, 1);
            float x = RangeSingle(-1, 1);
            float y = Mathf.Sqrt(1 - Mathf.Pow(x,2));
            if (negate == 1)
                y = -y;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Gets random Vector2 inside unit sphere.
        /// </summary>
        /// <param name="_synced">If the call should be synced over the network</param>
        /// <param name="_log">If the coll should be logged</param>
        /// <returns>Random inside unit sphere Vector2</returns>
        public static Vector2 InsideUnitSphere()
        {
            float x,y,z = 0;
            do
            {
                x = RangeSingle(-1, 1);
                y = RangeSingle(-1, 1);
                z = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
            }
            while (z > 1);
            return new Vector2(x, y);
        }

        public static Quaternion Rotation()
        {
            return UnityEngine.Random.rotation;
        }
    }
}

#region Zombie Code
/* ZOMBIE CODE
 
In all RNG Calls at the beginnig:
if (_log && _synced && isNetworkSetup)
     networkData.PushRNGCall(<CORRESPONDING_CALL>);

public void InvokeInitNetwork(bool _result) => StartCoroutine(InitNetwork(_result));
    
    public IEnumerator InitNetwork(bool _result)
    {
        yield break;
        if (!_result)
            yield break;

        yield return new WaitForSeconds(0.1f);

        try
        {
            syncedPrng = new System.Random(networkData.SyncedRNGSeed);
            isNetworkSetup = true;
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }




*/
#endregion