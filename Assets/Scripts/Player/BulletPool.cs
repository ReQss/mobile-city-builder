using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }
    public GameObject bulletProjectilePrefab;
    public GameObject magicalProjectilePrefab;
    
    public int poolSize = 10;
    private Queue<GameObject> bulletProjectilePool = new Queue<GameObject>();
    private Queue<GameObject> magicalProjectilePool = new Queue<GameObject>();
    [Header("Versus Projectiles")]
    public GameObject versusProjectilePrefab1;
    public GameObject versusProjectilePrefab2;
    public int versusPoolSize = 4;
    private Queue<GameObject> versus1ProjectilePool = new Queue<GameObject>();
    private Queue<GameObject> versus2ProjectilePool = new Queue<GameObject>();
    [Header("Enemy Damage Dealt Pool")]
    public GameObject damageDealtPrefab;
    public int damageDealtPoolSize = 15;
    private Queue<GameObject> damageDealtPool = new Queue<GameObject>();
    public GameObject damageDealtMagicPrefab;
    public int damageDealtMagicPoolSize = 15;
    private Queue<GameObject> damageDealtMagicPool = new Queue<GameObject>();

    [Header("Enemy Bullet Pool")]
    public GameObject enemyBulletPrefab;
    public int enemyBulletPoolSize = 20;
    private Queue<GameObject> enemyBulletPool = new Queue<GameObject>();



    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletProjectilePrefab, this.transform);
            bullet.SetActive(false);
            bulletProjectilePool.Enqueue(bullet);
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject magical = Instantiate(magicalProjectilePrefab, this.transform);
            magical.SetActive(false);
            magicalProjectilePool.Enqueue(magical);
        }
        for (int i = 0; i < versusPoolSize; i++)
        {
            GameObject versus1 = Instantiate(versusProjectilePrefab1, this.transform);
            versus1.SetActive(false);
            versus1ProjectilePool.Enqueue(versus1);

            GameObject versus2 = Instantiate(versusProjectilePrefab2, this.transform);
            versus2.SetActive(false);
            versus2ProjectilePool.Enqueue(versus2);
        }
        for (int i = 0; i < damageDealtPoolSize; i++)
        {
            GameObject damageDealt = Instantiate(damageDealtPrefab, this.transform);
            damageDealt.SetActive(false);
            damageDealtPool.Enqueue(damageDealt);
        }
        for (int i = 0; i < enemyBulletPoolSize; i++)
        {
            GameObject enemyBullet = Instantiate(enemyBulletPrefab, this.transform);
            enemyBullet.SetActive(false);
            enemyBulletPool.Enqueue(enemyBullet);
        }
        for (int i = 0; i < damageDealtMagicPoolSize; i++)
        {
            GameObject damageDealtMagic = Instantiate(damageDealtMagicPrefab, this.transform);
            damageDealtMagic.SetActive(false);
            damageDealtMagicPool.Enqueue(damageDealtMagic);
        }
    }
    public GameObject GetDamageDealtMagic()
    {
        GameObject damageDealtMagic;
        if (damageDealtMagicPool.Count > 0)
        {
            damageDealtMagic = damageDealtMagicPool.Dequeue();
            damageDealtMagic.SetActive(true);
        }
        else
        {
            damageDealtMagic = Instantiate(damageDealtMagicPrefab, this.transform);
        }
        StartCoroutine(ReturnDamageDealtAfterDelay(damageDealtMagic, 0.5f));
        return damageDealtMagic;
    }
    public GameObject GetEnemyBullet()
    {
        GameObject enemyBullet;
        if (enemyBulletPool.Count > 0)
        {
            enemyBullet = enemyBulletPool.Dequeue();
            enemyBullet.SetActive(true);
        }
        else
        {
            enemyBullet = Instantiate(enemyBulletPrefab, this.transform);
        }
        StartCoroutine(ReturnEnemyBulletAfterDelay(enemyBullet, 5f));
        return enemyBullet;
    }
    public void ReturnEnemyBullet(GameObject enemyBullet)
    {
        enemyBullet.SetActive(false);
        enemyBulletPool.Enqueue(enemyBullet);
    }
    private IEnumerator ReturnEnemyBulletAfterDelay(GameObject enemyBullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnEnemyBullet(enemyBullet);
    }
    public GameObject GetDamageDealt()
    {
        GameObject damageDealt;
        if (damageDealtPool.Count > 0)
        {
            damageDealt = damageDealtPool.Dequeue();
            damageDealt.SetActive(true);
        }
        else
        {
            damageDealt = Instantiate(damageDealtPrefab, this.transform);
        }
        StartCoroutine(ReturnDamageDealtAfterDelay(damageDealt, 0.5f));
        return damageDealt;
    }
    public GameObject GetBullet()
    {
        GameObject bullet;
        if (bulletProjectilePool.Count > 0)
        {
            bullet = bulletProjectilePool.Dequeue();
            bullet.SetActive(true);
        }
        else
        {
            bullet = Instantiate(bulletProjectilePrefab, this.transform);
        }
        StartCoroutine(ReturnBulletAfterDelay(bullet, 5f));
        return bullet;
    }
    public GameObject GetMagicalBullet()
    {
        GameObject magical;
        if (magicalProjectilePool.Count > 0)
        {
            magical = magicalProjectilePool.Dequeue();
            magical.SetActive(true);
        }
        else
        {
            magical = Instantiate(magicalProjectilePrefab, this.transform);
        }
        StartCoroutine(ReturnMagicalBulletAfterDelay(magical, 5f));
        return magical;
    }
    public GameObject GetVersusProjectile1()
    {
        GameObject versus1;
        if (versus1ProjectilePool.Count > 0)
        {
            versus1 = versus1ProjectilePool.Dequeue();
            versus1.SetActive(true);
        }
        else
        {
            versus1 = Instantiate(versusProjectilePrefab1, this.transform);
        }
        StartCoroutine(ReturnVersusProjectile1AfterDelay(versus1, 5f));
        return versus1;
    }
    public GameObject GetVersusProjectile2()
    {
        GameObject versus2;
        if (versus2ProjectilePool.Count > 0)
        {
            versus2 = versus2ProjectilePool.Dequeue();
            versus2.SetActive(true);
        }
        else
        {
            versus2 = Instantiate(versusProjectilePrefab2, this.transform);
        }
        StartCoroutine(ReturnVersusProjectile2AfterDelay(versus2, 5f));
        return versus2;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletProjectilePool.Enqueue(bullet);
    }
    public void ReturnMagicalBullet(GameObject magical)
    {
        magical.SetActive(false);
        magicalProjectilePool.Enqueue(magical);
    }
    public void ReturnVersusProjectile1(GameObject versus1)
    {
        versus1.SetActive(false);
        versus1ProjectilePool.Enqueue(versus1);
    }
    public void ReturnVersusProjectile2(GameObject versus2)
    {
        versus2.SetActive(false);
        versus2ProjectilePool.Enqueue(versus2);
    }
    private IEnumerator ReturnBulletAfterDelay(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnBullet(bullet);
    }
    private IEnumerator ReturnMagicalBulletAfterDelay(GameObject magical, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnMagicalBullet(magical);
    }
    private IEnumerator ReturnVersusProjectile1AfterDelay(GameObject versus1, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnVersusProjectile1(versus1);
    }
    private IEnumerator ReturnVersusProjectile2AfterDelay(GameObject versus2, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnVersusProjectile2(versus2);
    }
    private IEnumerator ReturnDamageDealtAfterDelay(GameObject damageDealt, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnDamageDealt(damageDealt);
    }

    public void ReturnDamageDealt(GameObject damageDealt)
    {
        damageDealt.SetActive(false);
        damageDealtPool.Enqueue(damageDealt);
    }
}
