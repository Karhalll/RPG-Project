namespace RPG.Control
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HendleRaycast(PlayerController callingController);
    }
}