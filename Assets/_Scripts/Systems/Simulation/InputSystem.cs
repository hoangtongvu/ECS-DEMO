using Components;
using Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class InputSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.CreateInputDataSingleton();
            this.RequireForUpdate<InputData>();
        }

        protected override void OnUpdate()
        {
            var inputDataRef = this.GetInputDataSingletonRef();
            this.SetMoveDir(ref inputDataRef.ValueRW);
            this.SetBackspaceButtonDown(ref inputDataRef.ValueRW);

            this.SetMouseData(ref inputDataRef.ValueRW.LeftMouseData, 0);
            this.SetMouseData(ref inputDataRef.ValueRW.RightMouseData, 1);
        }

        private void CreateInputDataSingleton()
        {
            SingletonUtilities.GetInstance(EntityManager)
                .AddComponent<InputData>();
        }

        private RefRW<InputData> GetInputDataSingletonRef() => SystemAPI.GetSingletonRW<InputData>();

        private void SetMoveDir(ref InputData inputData)
        {
            float2 rawDir = new();
            rawDir.y += Input.GetKey(KeyCode.W) ? 1 : 0;
            rawDir.y += Input.GetKey(KeyCode.S) ? -1 : 0;
            rawDir.x += Input.GetKey(KeyCode.D) ? 1 : 0;
            rawDir.x += Input.GetKey(KeyCode.A) ? -1 : 0;

            inputData.MoveDirection.Value = rawDir;
        }
        
        private void SetBackspaceButtonDown(ref InputData inputData)
        {
            inputData.BackspaceButtonDown = Input.GetKeyDown(KeyCode.Backspace);
        }

        private void SetMouseData(ref MouseData mouseData, int mouseValue)
        {
            mouseData.Down = Input.GetMouseButtonDown(mouseValue);
            mouseData.Hold = Input.GetMouseButton(mouseValue);
            mouseData.Up = Input.GetMouseButtonUp(mouseValue);
        }
    }
}