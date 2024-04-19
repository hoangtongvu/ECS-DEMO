using Core.MyEvent.PubSub.Messengers;
using Core.UI;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;
using UnityEngine;
using Core.MyEvent.PubSub.Messages;
using Components.ComponentMap;

namespace Systems.Simulation
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnUISystem : SystemBase
    {

        private NativeQueue<UISpawnMessage> eventDataQueue;

        protected override void OnCreate()
        {
            this.eventDataQueue = new(Allocator.Persistent);
            // sub to event queue
            GameplayMessenger.MessageSubscriber.Subscribe<UISpawnMessage>(this.UISpawnEventHandle);

        }

        protected override void OnUpdate()
        {

            UIPoolMap uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();
            SpawnedUIMap spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();

            while (this.eventDataQueue.TryDequeue(out var message))
            {
                if (!uiPoolMap.Value.TryGetValue(message.UIType, out var uiPoolMapValue))
                {
                    Debug.LogError($"Can't find UI prefab of type {message.UIType}");
                    continue;
                }

                uint newID = uiPoolMapValue.GlobalID + 1;
                var uiPool = uiPoolMapValue.UIPool;

                // Obj pool has no ref to prefab so it can't spawn new instance itself.
                if (!uiPool.TryGetFromPool(out BaseUICtrl baseUICtrl))
                {
                    baseUICtrl =
                        Object.Instantiate(
                            uiPoolMapValue.Prefab
                            , message.Position
                            , message.Quaternion).GetComponent<BaseUICtrl>();
                }


                // Set ID.
                baseUICtrl.UIID.LocalId = newID;
                uiPoolMapValue.GlobalID = newID;

                // Set parent for newly spawned UI Element.
                baseUICtrl.transform
                    .SetParent(this.GetParentTransform(uiPoolMapValue, message, spawnedUIMap));

                // Set active true.
                baseUICtrl.gameObject.SetActive(true);

                this.AddSpawnedUIIntoMap(spawnedUIMap, baseUICtrl);
            }
        }


        private void UISpawnEventHandle(UISpawnMessage uiSpawnMessage) => this.eventDataQueue.Enqueue(uiSpawnMessage);

        private Transform GetParentTransform(
            UIPoolMapValue uiPoolMapValue
            , UISpawnMessage message
            , SpawnedUIMap spawnedUIMap)
        {
            Transform parentTransform = uiPoolMapValue.UIPool.transform;
            if (message.ParentUIID.HasValue)
            {
                if (!spawnedUIMap.Value.TryGetValue(message.ParentUIID.Value, out BaseUICtrl parentUICtrl))
                    Debug.LogError($"Can't find BaseUICtrl with ID = {message.ParentUIID.Value}");

                parentTransform = parentUICtrl.transform;
            }

            return parentTransform;
        }


        private void AddSpawnedUIIntoMap(SpawnedUIMap spawnedUIMap, BaseUICtrl baseUICtrl)
        {
            if (spawnedUIMap.Value.TryAdd(baseUICtrl.UIID, baseUICtrl)) return;
            Debug.LogError($"SpawnedUIMap has already contained ID = {baseUICtrl.UIID}");
        }
    }
}