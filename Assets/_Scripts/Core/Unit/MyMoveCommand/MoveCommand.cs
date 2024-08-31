namespace Core.Unit.MyMoveCommand
{
    public enum MoveCommand : byte
    {
        None = 0,
        MoveToPosition = 1,
        MoveToInteractable = 2,
        MoveFromDangerSource = 3,
    }
}