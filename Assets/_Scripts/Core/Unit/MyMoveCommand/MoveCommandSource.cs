namespace Core.Unit.MyMoveCommand
{
    [GenerateEnumLength]
    public enum MoveCommandSource : byte
    {
        None = 0,
        PlayerCommand = 1,
        Danger = 2,
        ToolCall = 3,
        AutoAttack = 4,
    }

}