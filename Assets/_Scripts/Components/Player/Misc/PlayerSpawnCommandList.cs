using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;

namespace Components.Player.Misc;

public struct SpawnCommand
{
    public Entity Entity;
    public GameEntitySize GameEntitySize;
}

public struct PlayerSpawnCommandList : IComponentData
{
    public NativeList<SpawnCommand> Value;
}