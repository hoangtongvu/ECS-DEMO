using Core.Spawner;

namespace Core.UI.HouseUI
{
    public class HouseUISpawner : SpawnerGeneric<HouseUICtrl>
    {
        private static HouseUISpawner instance;
        public static HouseUISpawner Instance => instance;


        protected override void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}