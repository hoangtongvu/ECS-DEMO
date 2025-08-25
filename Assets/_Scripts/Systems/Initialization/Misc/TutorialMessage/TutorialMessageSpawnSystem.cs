using Components.Misc.TutorialMessage;
using Components.UI.Pooling;
using Core.UI.Identification;
using Core.UI.Pooling;
using Core.UI.TutorialMessage;
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
            this.RequireForUpdate<UIPoolMapInitializedTag>();
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
            tutorialMessageSpawnedState.Value = true;

            var tutorialMessageCtrl = (TutorialMessageCtrl)UICtrlPoolMap.Instance
                .Rent(UIType.TutorialMessage);

            messageCtrlHolder.Value = tutorialMessageCtrl;
            tutorialMessageCtrl.TextMeshPro.text = message;
            tutorialMessageCtrl.gameObject.SetActive(true);
        }

    }

}