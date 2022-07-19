using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Canvas;
using Enemies;
using RoomObjects;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private RoomController _roomController;
    public static Player player;
    public static GameManager gameManager;
    public static CameraShake cameraShake;
    public static GameObject coldBlackount;
    private bool _moveToTheNextRoom;
    private RoomSetting _roomSetting;
    private RoomType _difficult;

    [Header("Components")] 
    public GameObject dragon;
    public GameObject brokenWall;
    public GameObject coldBlackount1;
    public GameObject[] roomsWithRiched;
    public GameObject[] rooms;
    public Text passedRoomsCountText;
    public ScreenFader screenFader;
    public GameObject restorationShields;
    public GameObject winFrame;
    public CanvasInformationAboutObject getCanvasInformationAboutObject;
    
    [Header("Stats")]
    public int passedRoomsCount;
    public int allRoomsCount = 10;

    private void Start()
    {
        gameManager = this;
        InformationAboutObject.canvasInformationAboutObject = getCanvasInformationAboutObject;
        player = FindObjectOfType<Player>();
        coldBlackount = coldBlackount1;
        passedRoomsCount = 0;
        cameraShake = FindObjectOfType<Camera>().GetComponent<CameraShake>();
        _roomController = GetComponent<RoomController>();
        _difficult = RoomType.Easy;
        screenFader.fadeState = ScreenFader.FadeState.In;
        StartCoroutine(NextRoom(0));
        StartCoroutine(StartPlayer());
    }

    private IEnumerator StartPlayer()
    {
        player.isTurnOver = true;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !_moveToTheNextRoom);
        player.Active();
    }

    public IEnumerator TurnStarted()
    {
        RoomController.enemiesActive = false;
        // проверяем не перешел ли игрок в другую комнату если да то ждем когда она загрузится
        if (_moveToTheNextRoom)
        {
            yield return new WaitUntil(() => !_moveToTheNextRoom);
        }
        // try catch если игрока убили чтоб ошибки не было
        try
        {
            if (player.isActive)
            {
                player.Active();
            }
        }
        catch
        {
            // ignored
        }
    }

    public IEnumerator TurnEnded()
    {
        // активируем событие следущего хода 
        GameplayEventManager.NextMoveEvent();
        yield return new WaitForSeconds(0.3f);
        //активируем врагов и ловушки
        StartCoroutine(_roomController.Active());
    }
    
    //----------------- дальше код для спавна комнат и другой с этим связанным -------------

    public IEnumerator NextRoom(int count)
    {
        _moveToTheNextRoom = true;
        yield return new WaitUntil(() => player.isTurnOver);
        yield return new WaitForSeconds(0.4f);
        yield return new WaitUntil(() => !RoomController.enemiesActive);
        if(player == null) yield break;
        passedRoomsCount += count;
        if (passedRoomsCount < 0)
        {
            winFrame.SetActive(true);
            yield break;
        }
        passedRoomsCountText.text = "Floor " + (passedRoomsCount + 1) + "/10"; 
        screenFader.fadeState = ScreenFader.FadeState.In;
        yield return new WaitForSeconds(0.75f);
        var existingRoom = GameObject.FindGameObjectWithTag("Room");
        existingRoom.SetActive(false);
        Destroy(existingRoom);
        foreach (var afterDied in GameObject.FindGameObjectsWithTag("Trash"))
        {
            Destroy(afterDied);
        }
        SpawnNextRoom();
        SpawnEnemies();
        var randomNumber = Random.Range(0, 3);
        if (_roomSetting.hasRestorationShields && randomNumber == 1)
        {
            Instantiate(restorationShields, new Vector3(0, -0.5f, 0), Quaternion.identity);
        } 
        else if (randomNumber == 2)
        {
            if (!(passedRoomsCount + 2 >= allRoomsCount))
            {
                if (_roomSetting.walls.Length != 0)
                {
                    var wall = _roomSetting.walls[Random.Range(0, _roomSetting.walls.Length)];
                    Instantiate(brokenWall, wall.transform.position, Quaternion.identity).transform.SetParent(wall.transform.parent);
                    Destroy(wall);
                }
            }
        }
        var exits = GameObject.FindGameObjectsWithTag("Exit");
        foreach (var stairs in exits)
        {
            var theStairs = stairs.GetComponent<Exit>();
            if (!theStairs.isStairDown && !player.playerHasRiched
                || theStairs.isStairDown && player.playerHasRiched) continue;
            var nextPositionPlayer = theStairs.GetNextPositionPlayer();
            player.transform.position = nextPositionPlayer;
            break;
        }
        
        screenFader.fadeState = ScreenFader.FadeState.Out;
        _moveToTheNextRoom = false;
        GameplayEventManager.NextRoomEvent();
    }

    private void SpawnNextRoom()
    {
        if (passedRoomsCount+1 >= allRoomsCount)
        {
            _roomSetting = Instantiate(roomsWithRiched[Random.Range(0, roomsWithRiched.Length)], new Vector2(0, 0), Quaternion.identity)
                .GetComponent<RoomSetting>();
            return;
        }
        _definitionOfComplexity();
        var certainRooms = _selectCertainsRoom(_difficult);
        if (certainRooms.Length == 0)
        {
            certainRooms = _selectCertainsRoom(RoomType.Difficult);
        }
        _roomSetting = Instantiate(
            certainRooms[Random.Range(0, certainRooms.Length)], 
            new Vector3(0, 0), 
            Quaternion.identity
            ).GetComponent<RoomSetting>();
    }

    private GameObject[] _selectCertainsRoom(RoomType roomType)
    {
        var certainRooms = new List<GameObject> {Capacity = 0};
        certainRooms.AddRange(from room in rooms let roomSetting = room.GetComponent<RoomSetting>() where roomSetting.roomType == roomType select room);
        return certainRooms.ToArray();
    }
    
    private void _definitionOfComplexity()
    {
        //-----
        if (player.playerHasRiched)
        {
            _difficult = RoomType.VeryDifficult;
            return;
        }
        
        _difficult = passedRoomsCount switch
        {
            > 7 => RoomType.VeryDifficult,
            > 5 => RoomType.Difficult,
            > 3 => RoomType.Intermediate,
            _ => RoomType.Easy
        };
        //--------
    }

    private void SpawnEnemies()
    {
        var thisEnemySpawns = _roomSetting.enemySpawns.ToList();
        var dragonSpawn = thisEnemySpawns.Where(enemySpawn =>
        {
            if (!enemySpawn.name.Contains("DragonSpawn")) return false;
            Instantiate(dragon, enemySpawn.transform.position, Quaternion.identity);
            return true;
        }).ToArray();
        if (dragonSpawn.Length != 0)
        {
            thisEnemySpawns.Remove(dragonSpawn[0]);
            Destroy(dragonSpawn[0]);
        }
        for (var i = 0; i < _roomSetting.enemiesCount; )
        {
            var enemy = _roomSetting.enemies[Random.Range(0, _roomSetting.enemies.Length)];
            if (thisEnemySpawns.Count != 0)
            {
                var enemySpawn = thisEnemySpawns[Random.Range(0, thisEnemySpawns.Count)];
                Instantiate(enemy, enemySpawn.transform.position, Quaternion.identity);
                thisEnemySpawns.Remove(enemySpawn);
                i += enemy.GetComponent<TheEnemy>().enemyCount;
            }
            else return;
        }
    }
}
