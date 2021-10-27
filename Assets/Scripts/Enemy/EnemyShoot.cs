using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject BulletPrefab;

    public Transform GunPos;
    public float coldown = 0.25f;
    private float lastTime = 0f;
    public float shootAmount;//количество выстрелов

    private IEnumerator shootingCoroutine;

    public void Shoot(Transform Target,float Timee)
    {
        StopShooting();

        shootingCoroutine = Shooting(Target, Timee);
        StartCoroutine(shootingCoroutine);
    }
    IEnumerator Shooting(Transform Target, float Timee)
    {
        yield return new WaitForSeconds(Timee);
        while(shootAmount > 0)
        {
           
            if (Time.time > (lastTime + coldown))
            {
                SpawnBullet(Target);
                shootAmount--;
                yield return null;
                lastTime = Time.time;
            }
            yield return null;
        }
    }
    void SpawnBullet(Transform Target)
    {
        Vector3 Direction = Target.position - transform.position;
        GameObject bullet = Instantiate(BulletPrefab, GunPos.position, Quaternion.identity);
        Direction.Normalize();
        bullet.transform.up = Direction;
        bullet.GetComponent<Rigidbody>().AddForce(Direction * 300f);
    }

    public void StopShooting()
    {
        if (shootingCoroutine != null) StopCoroutine(shootingCoroutine);
    }
}
