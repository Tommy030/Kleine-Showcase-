using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    private float m_maxHealth;

    private float m_currentHealth;

    public void GiveStartingValues(float maxHealth)
    {
        m_currentHealth = maxHealth;
        m_maxHealth = maxHealth;
    }

    public void ReduceOrAddHealth(float amount)
    {
        m_currentHealth = m_currentHealth + amount;
        CheckHealth();
    }

    public void CheckHealth()
    {
        if (m_currentHealth <= 0)
        {
            NoHealthLeft();
        }
        if (m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }
    }
    public virtual void NoHealthLeft()
    {
        gameObject.SetActive(false);
    }
    public float ReturnCurrentHealth()
    {
        return m_currentHealth;
    }
}

public interface IHealth
{
    public void ReduceOrAddHealth(float amount);

    public void CheckHealth();
}
/// Gebruik van Generieke Health
/// using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private float m_playerMaxHealth;

    private void Start()
    {
        GiveStartingValues(m_playerMaxHealth);
    }
}
/// Tweede Voorbeeld
public class Enemy : Health
{
    public float m_enemyDamage = 50;
    public float m_maxHealth { get; private set; }

    private void Start()
    {
        GiveStartingValues(m_maxHealth);
    }

    public override void NoHealthLeft()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IHealth>().ReduceOrAddHealth(-m_enemyDamage);
        }
    }
}