using System.Collections;
using UnityEngine;

public enum EnemyDestroyType
{
    Kill = 0,
    Arrive
}

public class Enemy : MonoBehaviour
{
    private int wayPointCount;
    private Transform[] wayPoints;
    [SerializeField] private int currentIndex = 0;
    private Movement2D movement2D;
    private EnemySpanwer enemySpawner;
    [SerializeField] private int gold = 10;

    public void Setup(EnemySpanwer enemySpawner, Transform[] wayPoints)
    {
        this.enemySpawner = enemySpawner;
        movement2D = GetComponent<Movement2D>();
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            transform.Rotate(Vector3.forward * 10f);

            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        if(currentIndex < wayPointCount - 1)
        {
            // 깔끔하게 하기 위해 적의 위치를 현재 위치하고 있는 WayPoint의 위치에 일치시킴
            transform.position = wayPoints[currentIndex].position;
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestoryEnemy(type, this, gold);
    }
}
