using UnityEngine;

public enum RoomType
{
    Easy,
    Intermediate,
    Difficult,
    VeryDifficult
}

public class RoomSetting : MonoBehaviour
{
    // у каждой комнаты свою настройка
    public bool hasRestorationShields;
    public RoomType roomType;
    public GameObject[] enemies;
    public GameObject[] walls; // для случайного появления стены которую можно сломать огнем
    public GameObject[] enemySpawns;
    public int enemiesCount;
}
