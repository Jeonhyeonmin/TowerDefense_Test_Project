using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private         TowerTemplate[] towerTemplate;
    [SerializeField] private EnemySpanwer enemySpanwer;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private SystemTextViewer systemTextViewer;
    private bool isOnTowerButton = false;
    private GameObject followTowerClone = null;
    private int towerType;

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        if(isOnTowerButton == true)
        {
            return;
        }

        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        isOnTowerButton = true;
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        if(isOnTowerButton == false)
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        if(tile.IsBuildTower == true)
        {
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        isOnTowerButton = false;

        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpanwer, playerGold, tile);
        OnBuffAllBuffTowers();
        Destroy(followTowerClone);
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while(true)
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;

                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }
    }

    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i< towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.WeaponType == WeaponType.Buff)
            {
                weapon.OnBuffAroundTower();
            }
        }
    }
}
