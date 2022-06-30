using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    [System.NonSerialized] public ScriptableGun m_gun;

    public void StartingValues(ScriptableGun gun)
    {
        m_gun = Instantiate(gun);
    }

    public virtual void Reload()
    {
        if (m_gun.m_ammo < m_gun.m_maxAmmo && m_gun.m_clipAmmo > 0)
        {
            m_gun.m_ammo = m_gun.m_maxAmmo;
            m_gun.m_clipAmmo--;
        }
    }

    public virtual void AddAmmo()
    {
        if (m_gun.m_clipAmmo < m_gun.m_maxClips)
        {
            m_gun.m_clipAmmo++;
        }
    }
    public virtual void Fire(Transform bulletSpawn)
    {
        GameObject bullet = GenericObjectPooler.m_Instance.GetPooledObject(m_gun.projectile, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Bullet>().StartingValues(m_gun.m_damage, m_gun.m_bulletSpeed);
        m_gun.m_ammo--;
    }
}

public interface IWeapon
{
    public void Reload();

    public void AddAmmo();

    public void Fire(Transform bulletSpawn);
}
/// Gebruik van de Generieke wapen
using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] public ScriptableGun m_scriptableGun;
    public Transform m_bulletSpawn;

    public static PlayerWeapon m_Instance { get; private set; }

    private float nextFire = 0f;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        StartingValues(m_scriptableGun);
    }

    private void FixedUpdate()
    {
        if ((Time.time >= nextFire) && (Input.GetButton("Fire1")) && m_gun.m_ammo > 0)
        {
            Fire(m_bulletSpawn);
            nextFire = Time.
            time + m_scriptableGun.m_fireRate;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}