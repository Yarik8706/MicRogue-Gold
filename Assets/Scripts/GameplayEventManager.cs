using UnityEngine.Events;

public static class GameplayEventManager
{
    public static readonly UnityEvent OnNextRoom = new UnityEvent();
    public static readonly UnityEvent OnNextMove = new UnityEvent();

    public static void NextRoomEvent()
    {
        OnNextRoom.Invoke();
    }
    
    public static void NextMoveEvent()
    {
        OnNextMove.Invoke();
    }
}
