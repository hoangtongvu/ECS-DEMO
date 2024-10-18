using Components.Misc.Presenter;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.Misc.Presenter
{


    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SceneSystemGroup))]
    public partial class BasePresenterMapInitSystem : SystemBase
    {

        protected override void OnCreate()
        {
            var presenterMap = new PresenterPrefabMap()
            {
                Value = new(15, Allocator.Persistent),
            };

            this.AddingPresentersToMap(presenterMap);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(presenterMap);

            this.Enabled = false;

        }

        protected override void OnUpdate()
        {
        }

        public void AddingPresentersToMap(PresenterPrefabMap presenterMap)
        {
            this.LoadPresenterPrefabs(out var presenterPrefabs);

            foreach (var presenter in presenterPrefabs)
            {
                string fileName = presenter.name;

                var parts = fileName.Split('-');
                if (parts.Length != 2)
                {
                    Debug.LogError($"Invalid format for PresenterPrefab name {fileName}");
                    continue;
                }


                string idPart = parts[0];  // "xxxyyy"
                if (idPart.Length != 6)
                {
                    Debug.LogError($"Id part for PresenterPrefab name {fileName} is not in format xxxyyy");
                    continue;
                }

                // Extract "xxx" and "yyy" and handle leading zeros
                PresenterType presenterType = (PresenterType)byte.Parse(idPart.Substring(0, 3));
                byte localIndex = byte.Parse(idPart.Substring(3, 3));

                var presenterId = new PresenterPrefabId
                {
                    PresenterType = presenterType,
                    LocalIndex = localIndex,
                };

                if (presenterMap.Value.TryAdd(presenterId, presenter))
                {
                    //Debug.Log($"Added {presenterId} into {nameof(BasePresenterMap)}");
                    continue;
                }

                Debug.LogError($"Cant add {presenterId} into {nameof(PresenterPrefabMap)}");

            }
        }

        private void LoadPresenterPrefabs(out BasePresenter[] presenterPrefabs)
        {
            presenterPrefabs = Resources.LoadAll<BasePresenter>("Presenters");
        }


    }

}