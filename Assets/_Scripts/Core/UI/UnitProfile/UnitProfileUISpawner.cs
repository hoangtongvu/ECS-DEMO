using Core.Spawner;

namespace Core.UI.UnitProfile
{
    public class UnitProfileUISpawner : SpawnerGeneric<UnitProfileUICtrl>
    {
        private static UnitProfileUISpawner instance;
        public static UnitProfileUISpawner Instance => instance;


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