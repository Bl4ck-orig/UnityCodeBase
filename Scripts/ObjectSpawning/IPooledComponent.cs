namespace ObjectSpawning
{
    public interface IPooledComponent
    {
        public void OnObjectSpawn();
     
        public void OnDeactivated();
    }
}
