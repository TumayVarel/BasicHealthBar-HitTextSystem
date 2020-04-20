using UnityEngine;

public class BasicCharacter : MonoBehaviour, IDamageable
{
    CharacterStats characterStats;
    int defaultHealth = 1000;

    public int CurrentHealth { get => characterStats.health; }

    public event DamageableEvent DamageableEvent;

    private void OnEnable()
    {
        characterStats = new CharacterStats(defaultHealth);
    }

    /// <summary>
    /// Reduces health and invoke hit damage event with the damage amount.
    /// </summary>
    /// <param name="amount"></param>
    public void GetHit(int amount)
    {
        if (characterStats.health < 0) // Die with some functions.
            return;
        characterStats.health -= amount;
        DamageableEvent?.Invoke(DamageType.Hit, amount);
    }

    /// <summary>
    /// Reduces health and invoke bomb damage event with the damage amount.
    /// </summary>
    /// <param name="amount"></param>
    public void GetBomb(int amount)
    {
        if (characterStats.health < 0) // Die with some functions.
            return;
        characterStats.health -= amount;
        DamageableEvent?.Invoke(DamageType.Bomb, amount);
    }

    /// <summary>Invoke dodge damage event.</summary>
    public void GetDodge()
    {
        if (characterStats.health < 0)
            return;
        DamageableEvent?.Invoke(DamageType.Dodge);
    }

    /// <summary>
    /// Increases health and invoke heal damage event.
    /// </summary>
    /// <param name="amount"></param>
    public void GetHeal(int amount)
    {
        if (characterStats.health < 0)
            return;
        characterStats.health += amount;
        characterStats.health = characterStats.health > defaultHealth ? defaultHealth : characterStats.health;
        DamageableEvent?.Invoke(DamageType.Heal, amount);
    }

}

/// <summary>Interface for all damageables.</summary>
public interface IDamageable
{
    int CurrentHealth { get; }

    event DamageableEvent DamageableEvent;

    void GetHit(int amount);

    void GetBomb(int amount);

    void GetDodge();

    void GetHeal(int amount);
}

/// <summary>
/// Delegate that will be used to invoke damage event.
/// </summary>
/// <param name="damageType"></param>
/// <param name="hitAmount"></param>
public delegate void DamageableEvent(DamageType damageType, int? hitAmount = null);

/// <summary>Enum for hit type.</summary>
public enum DamageType { Hit, Bomb, Dodge, Heal};

/// <summary>Basic character stats.</summary>
public struct CharacterStats
{
    public int health;
    public CharacterStats(int health)
    {
        this.health = health;
    }
}