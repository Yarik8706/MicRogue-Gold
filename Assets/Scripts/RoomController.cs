using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Canvas;
using Enemies;
using Traps;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static GameObject[] allEnemies;

    public static bool enemiesActive;
    private GameManager _gameManager;
    private int _countCreateEnemyOrder;
    private bool _isActiveEnemies;
    private bool _isStartTurnOrder;
    
    public void Start()
    {
        _gameManager = GetComponent<GameManager>();
    }

    public IEnumerator Active()
    {
        enemiesActive = true;
        //---------------- активируются ловушки----------------
        foreach(var trap in GameObject.FindGameObjectsWithTag("Trap"))
        {
            trap.GetComponent<Trap>().SetStageAttack(1);
        }
        yield return new WaitForSeconds(1f);
        //--------------------------------------------------
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        StartCoroutine(allEnemies != null ? ActiveEnemies() : _gameManager.TurnStarted());
    }

    private IEnumerator ActiveEnemies()
    { // активирует кождого врага по очереди по их типу
        var allEnemiesType = new List<int>();
        allEnemiesType.AddRange(
            from enemy in allEnemies 
            where enemy != null 
            select enemy.GetComponent<TheEnemy>() 
            into enemyClass
            select enemyClass.enemyType);
        if (allEnemiesType.Count != 0)
        {
            var maxType = allEnemiesType.Max();
            var minType = allEnemiesType.Min();
            for (var i = minType; i <= maxType; i++)
            {
                StartCoroutine(ActiveCertainEnemies(i));
                yield return new WaitUntil(() => !_isActiveEnemies);
            }
        }

        StartCoroutine(_gameManager.TurnStarted());
    }

    private IEnumerator ActiveCertainEnemies(int type)
    { // просто знай что эта функция активирует определеных врагов 
        _isActiveEnemies = true;
        var enemies = new List<TheEnemy>();
        enemies.AddRange(
            from enemy in allEnemies 
            where enemy != null 
            select enemy.GetComponent<TheEnemy>() 
            into enemyClass 
            where enemyClass.enemyType == type && enemyClass.isActive
            select enemyClass);
        while (enemies.Count != 0)
        {
            enemies[0].Active();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => enemies[0].isTurnOver);
            enemies.Remove(enemies[0]);
        }

        _isActiveEnemies = false;
    }
}