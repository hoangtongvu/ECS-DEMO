using Components.ComponentMap;
using Components.Misc.TutorialMessage;
using Core.UI.Identification;
using Core.UI.TutorialMessage;
using Core.Utilities.Helpers;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TutorialMessageSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<TutorialMessageSpawnedState>();
            this.RequireForUpdate<SpawnedTutorialMessageCtrlHolder>();
            this.RequireForUpdate<TutorialMessageList>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
        }

        protected override void OnUpdate()
        {
            var tutorialMessageSpawnedRef = SystemAPI.GetSingletonRW<TutorialMessageSpawnedState>();
            if (tutorialMessageSpawnedRef.ValueRO.Value) return;

            var tutorialMessageList = SystemAPI.GetSingleton<TutorialMessageList>().Value;
            int length = tutorialMessageList.Length;

            if (length == 0) return;

            var spawnedMessageCtrlHolderRef = SystemAPI.GetSingletonRW<SpawnedTutorialMessageCtrlHolder>();
            var firstMessageElement = tutorialMessageList[0];

            this.SpawnTutorialMessageCtrl(
                ref tutorialMessageSpawnedRef.ValueRW
                , ref spawnedMessageCtrlHolderRef.ValueRW
                , firstMessageElement.String.ToString());
        }

        private void SpawnTutorialMessageCtrl(
            ref TutorialMessageSpawnedState tutorialMessageSpawnedState
            , ref SpawnedTutorialMessageCtrlHolder messageCtrlHolder
            , string message)
        {
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>().Value;
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>().Value;

            tutorialMessageSpawnedState.Value = true;
            var tutorialMessageCtrl = (TutorialMessageCtrl)UISpawningHelper
                .Spawn(uiPrefabAndPoolMap, spawnedUIMap, UIType.TutorialMessage);

            messageCtrlHolder.Value = tutorialMessageCtrl;
            tutorialMessageCtrl.TextMeshPro.text = message;
            tutorialMessageCtrl.gameObject.SetActive(true);
        }

    }

}