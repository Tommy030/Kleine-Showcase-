using System.Collections.Generic;
using UnityEngine;
//Creates objects and adds them to the GenericObjectPooler on awake
[System.Serializable] 
public class ObjectToPool
{
    public int m_amount = 10;
    public GameObject m_prefab;

    public ObjectToPool(GameObject prefab, int amount)
    {
        m_amount = amount;
        m_prefab = prefab;
    }
}
//Objects that are pooled and used in the GenericObjecPooler class
public class PooledObjects
{
    public GameObject m_prefab;
    public GameObject m_gameObject;

    public PooledObjects(Transform parent, GameObject prefab, GameObject gameObject)
    {
        m_prefab = prefab;
        m_gameObject = gameObject;
        m_gameObject.transform.SetParent(parent); 
    }
}

//Can request a gameobject with GenericObjectPooler.m_Instance.GetPooledObject
public class GenericObjectPooler : MonoBehaviour
{
    public static GenericObjectPooler m_Instance;
    public List<ObjectToPool> m_objectToPool = new List<ObjectToPool>();
    public List<List<PooledObjects>> m_objectPooler;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        m_objectPooler = new List<List<PooledObjects>>();

        foreach (ObjectToPool pool in m_objectToPool)
        {
            m_objectPooler.Add(CreateList(pool));
        }
    }

    //Returns requested prefab with the right position and rotation
    public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int indexPrefab = 0;
        bool rightPrefab = false;
        Transform parent = null;
        //Checks every list if it has the right prefab, if it doesnt continue to the next list and runs the code starting from line 78 and below
        for (int i = 0; i < m_objectPooler.Count; i++)
        {
            foreach (PooledObjects obj in m_objectPooler[i])
            {
                if (obj.m_prefab == prefab)
                {
                    rightPrefab = true;
                    indexPrefab = i;
                    parent = obj.m_gameObject.transform.parent;
                    if (!obj.m_gameObject.activeInHierarchy)
                    {
                        TurnOnObject(obj, position, rotation);
                        return obj.m_gameObject;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        //If the list contains the prefab but all of them are in use, create a new gameobject and adds it to the current list
        if (rightPrefab)
        {
            GameObject gameObject = Instantiate(prefab, position, rotation);
            m_objectPooler[indexPrefab].Add(new PooledObjects(parent, prefab, gameObject));
            return gameObject;
        }
        //If prefab isnt in the objectpooler yet, creates a new list with the prefabs and returns one
        List<PooledObjects> newPooledObjectList = CreateList(new ObjectToPool(prefab, 10));
        m_objectPooler.Add(newPooledObjectList);    
        PooledObjects newObject = newPooledObjectList[0];
        TurnOnObject(newObject, position, rotation);
        return newObject.m_gameObject;
    }

    //Turns on object and sets the position and rotation
    public void TurnOnObject(PooledObjects obj, Vector3 position, Quaternion rotation)
    {
        obj.m_gameObject.SetActive(true);
        obj.m_gameObject.transform.position = position;
        obj.m_gameObject.transform.rotation = rotation;
    }

    //Creates a list with pooledobjects and sets them inactive and returns list 
    public List<PooledObjects> CreateList(ObjectToPool pool)
    {
        List<PooledObjects> pooledObjects = new List<PooledObjects>();
        GameObject parent = new GameObject();
        parent.name = pool.m_prefab.name;
        for (int i = 0; i < pool.m_amount; i++)
        {
            GameObject gameObject = Instantiate(pool.m_prefab);
            pooledObjects.Add(new PooledObjects(parent.transform, pool.m_prefab, gameObject));
            gameObject.SetActive(false);
        }
        return pooledObjects;
    }
}

/// Voorbeeld Bullet met gebruik van een object pooler
public class Bullet : MonoBehaviour
{
    public float m_damage = 10;
public float m_bulletSpeed = 50;
private float m_timer = 2;

private void Update()
{
    if (m_timer > 0)
    {
        m_timer -= Time.deltaTime;
    }
    else
    {
        m_timer = 2;
        gameObject.SetActive(false);
    }
}

public void StartingValues(float damage, float bulletSpeed)
{
    m_damage = damage;
    m_bulletSpeed = bulletSpeed;
    GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
}

private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        collision.gameObject.GetComponent<IHealth>().ReduceOrAddHealth(-m_damage);
        gameObject.SetActive(false);
    }
}
}