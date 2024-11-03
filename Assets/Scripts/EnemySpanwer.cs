using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpanwer : MonoBehaviour
{
    //[SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyHPSliderPrefab;
    [SerializeField] private Transform canvasTransform;
    //[SerializeField] private float spawnTime;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private PlayerGold playerGold;
    private Wave currentWave;
    private int currentEnemyCount;
    private List<Enemy> enemyList; // 현재 맵에 존재하는 모든 적의 정보
    public List<Enemy> EnemyList => enemyList;

    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;
        currentEnemyCount = currentWave.maxEnemyCount;
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;

        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHpSlider(clone);

            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestoryEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if(type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }

        currentEnemyCount--;

        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
