using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.XR;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
public enum WeaponState
{
    SearchTarget = 0,
    TryAttackCannon,
    TryAttackLaser,
    //AttackToTarget
}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private Transform spawnPoint;
    
    [SerializeField] private WeaponType weaponType;

    [Header("Cannon")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitEffect;
    [SerializeField] private LayerMask targetLayer;

    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private EnemySpanwer enemySpawner;
    private SpriteRenderer spriteRenderer;
    private TowerSpawner towerSpawner;
    private PlayerGold playerGold;
    private Tile ownerTile;

    private float addedDamage;
    private int buffLevel;

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    //public float Damage => attackDamage;
    //public float Rate => attackRate;
    //public float Range => attackRange;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;

    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }

    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public void Setup(TowerSpawner towerSpawner ,EnemySpanwer enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x; // x 축 거리 계산
        float dy = attackTarget.position.y - transform.position.y; // y 축 거리 계산

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg; // 라디안을 도로 변환
        transform.rotation = Quaternion.Euler(0, 0, degree); // 객체를 회전시킴
    }

    public void OnBuffAroundTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.BuffLevel > Level)
            {
                continue;
            }

            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    weapon.BuffLevel = Level;
                }
            }
        }
    }

    private IEnumerator SearchTarget()
    {
        while ( true )
        {
            //float closestDistSqr = Mathf.Infinity;

            //for(int i = 0; i < enemySpawner.EnemyList.Count; ++i)
            //{
            //    float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            //    if(distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            //    {
            //        closestDistSqr = distance;
            //        attackTarget = enemySpawner.EnemyList[i].transform;
            //    }
            //}

            attackTarget = FindClosesAttackTarget();

            if(attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            //if(attackTarget == null)
            //{
            //    ChangeState(WeaponState.SearchTarget);
            //    break;
            //}

            //float distance = Vector3.Distance(attackTarget.position, transform.position);

            //if(distance > towerTemplate.weapon[level].range)
            //{
            //    attackTarget = null;
            //    ChangeState(WeaponState.SearchTarget);
            //    break;
            //}

            if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while ( true )
        {
            if(IsPossibleToAttackTarget() == false)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            SpawnLaser();

            yield return null;
        }
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].transform == attackTarget)
            {
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;
                //attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    private Transform FindClosesAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        for(int i =0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        if(attackTarget == null)
        {
            return false;
        }

        float distance = Vector3.Distance(attackTarget.position, transform.position);

        if(distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone =  Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    public bool Upgrade()
    {
        if(playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }

        level++;
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        if(weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);
    }
}
