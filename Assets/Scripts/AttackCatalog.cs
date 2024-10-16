using System.Collections; // Asegúrate de que esta línea está presente
using System.Collections.Generic;
using UnityEngine;

// Enumerado para las categorías de ataque
public enum AttackCategory
{
    Physical,
    Special,
    Status
}

[System.Serializable]
public class Attack
{
    public string name;
    public bool makesContact;
    public string type;
    public int power;
    public int accuracy;
    public string description;
    public int priority;  // Prioridad del movimiento
    public PokemonBase.StatusCondition statusEffect;  // Estado alterado que puede causar el ataque
    public float statusEffectChance;      // Probabilidad de causar el estado alterado (0-100%)
    public AttackCategory category; // Nueva propiedad para la categoría
    

    // Constructor modificado para incluir la categoría
    public Attack(string name, bool makesContact, string type, int power, int accuracy, string description, AttackCategory category, PokemonBase.StatusCondition statusEffect = PokemonBase.StatusCondition.None, float statusEffectChance = 0, int priority = 0)
    {
        this.name = name;
        this.makesContact = makesContact;
        this.type = type;
        this.power = power;
        this.accuracy = accuracy;
        this.description = description;
        this.category = category; // La categoría ahora se establece correctamente
        this.statusEffect = statusEffect;
        this.statusEffectChance = statusEffectChance;
        this.priority = priority; // La prioridad ahora se establece correctamente
    }

    // Método para determinar si el ataque causa su efecto de estado
    public bool TryApplyStatusEffect()
    {
        if (statusEffect != PokemonBase.StatusCondition.None && Random.Range(0f, 100f) < statusEffectChance)
        {
            return true;  // Se aplica el estado alterado
        }
        return false;  // No se aplica el estado alterado
    }
}

[System.Serializable]
public class TM
{
    public string tmName;  // Nombre de la TM
    public Attack attack;   // Ataque asociado con la TM
}

[System.Serializable]
public class EggMove
{
    public Attack attack;   // Ataque asociado con la TM
}

public class AttackCatalog : MonoBehaviour
{
    public List<Attack> allAttacks;
    public List<TM> tmCatalog; // Lista para almacenar las TMs
    public List<EggMove> eggMoveCatalog;
    public bool isSelectedForAttack;
    public bool isAttacking;
    public GameObject attackTilePrefab; // Prefab del tile a instanciar
    Dictionary<Vector3, TileAttack> instantiatedTiles = new Dictionary<Vector3, TileAttack>(); // Lista para almacenar los tiles instanciados
    public Material grayMaterial;
    public float tileSize = 1f; // Tamaño del tile
    public int mapWidth = 7; // Ancho del mapa
    public int mapHeight = 7; // Alto del mapa

    public GameObject tacklePrefab;
    public GameObject vineWhipPrefab;
    public GameObject scratchPrefab;
    public GameObject emberPrefab;
    public GameObject waterGunPrefab;
    public GameObject nuzzlePrefab;

    public int damage = 0;

    void Start()
        {
            instantiatedTiles = new Dictionary<Vector3, TileAttack>(); // Inicializa la lista
        }

    public void InitializeAttacks()
    {
        
        allAttacks = new List<Attack>
        {
            // Ejemplos de ataques básicos sin estado alterado
            new Attack("Tackle", true, "Normal", 40, 100, "A full-body charge attack.", AttackCategory.Physical), // Prioridad normal
            new Attack("Vine Whip", true, "Grass", 45, 100, "Whips the foe with slender vines.", AttackCategory.Physical, PokemonBase.StatusCondition.Esnared, 0f), // Prioridad normal
            new Attack("Scratch", true, "Normal", 40, 100, "Scratches with sharp claws.", AttackCategory.Physical),
            new Attack("Dragon Claw", true, "Dragon", 80, 100, "Slashes the foe with sharp claws.", AttackCategory.Physical),
            new Attack("Water Gun", false, "Water", 40, 100, "Squirts water to attack.", AttackCategory.Special),

            // Ejemplos de ataques con efectos de estado alterado
            new Attack("Ember", false, "Fire", 40, 100, "An attack that may inflict a burn.", AttackCategory.Special, PokemonBase.StatusCondition.Burn, 10f),
            new Attack("Heat Wave", false, "Fire", 95, 90, "Exhales a hot breath on the foe. May inflict a burn.", AttackCategory.Special, PokemonBase.StatusCondition.Burn, 10f),
            new Attack("Nuzzle", true, "Electric", 20, 100, "The user attacks by nuzzling its electrified cheeks against the target. This also leaves the target with paralysis.", AttackCategory.Physical, PokemonBase.StatusCondition.Paralysis, 100f), // Prioridad normal
            new Attack("Thunder Shock", false, "Electric", 40, 100, "An attack that may cause paralysis.", AttackCategory.Special, PokemonBase.StatusCondition.Paralysis, 10f), // Prioridad normal
            new Attack("Discharge", false, "Electric", 80, 100, "A flare of electricity is loosed to strike all Pokémon in battle. It may also cause paralysis.", AttackCategory.Special, PokemonBase.StatusCondition.Paralysis, 30f), // Prioridad normal
            new Attack("Spark", true, "Electric", 65, 100, "An attack that may cause paralysis.", AttackCategory.Physical, PokemonBase.StatusCondition.Paralysis, 30f), // Prioridad normal
            new Attack("Thunder Bolt", false, "Electric", 90, 100, "An attack that may cause paralysis.", AttackCategory.Special, PokemonBase.StatusCondition.Paralysis, 10f), // Prioridad normal


            // Ataques de alta prioridad
            new Attack("Quick Attack", false, "Normal", 40, 100, "A fast attack that strikes first.", AttackCategory.Physical, PokemonBase.StatusCondition.None, 0, 1), // Alta prioridad
            new Attack("Extreme Speed", false, "Normal", 80, 100, "An extremely fast attack.", AttackCategory.Physical, PokemonBase.StatusCondition.None, 0, 2), // Alta prioridad

            // Ejemplos de ataques adicionales
            new Attack("Thunder Wave", false, "Electric", 0, 90, "A jolt of electricity that paralyzes the opponent.", AttackCategory.Status, PokemonBase.StatusCondition.Paralysis, 100f), // Prioridad normal
            new Attack("Will-O-Wisp", false, "Fire", 0, 85, "Engulfs the opponent in a damaging fire, causing a burn.", AttackCategory.Status, PokemonBase.StatusCondition.Burn, 85f) // Prioridad normal
        };

        // Inicializar el catálogo de TMs
        tmCatalog = new List<TM>
        {
            new TM { tmName = "TM10", attack = AttackCatalog.GetAttackByName("Thunderbolt") },
            new TM { tmName = "TM15", attack = AttackCatalog.GetAttackByName("Flamethrower") },
            // Agregar más TMs según sea necesario
        };

        // Inicializar el catálogo de movimientos por huevo
        eggMoveCatalog = new List<EggMove>
        {
            new EggMove { attack = AttackCatalog.GetAttackByName("Dragon Dance") },
            new EggMove { attack = AttackCatalog.GetAttackByName("Hydro Pump") },
            new EggMove { attack = AttackCatalog.GetAttackByName("Play Rough") },
            // Agregar más movimientos por huevo según sea necesario
        };
    }

    public bool ApplyAttack(PokemonBase attacker, PokemonBase defender, Attack attack)
{
    bool isCriticalHit = false;
    int damage = 0;
    float statusEffectChance = 0f; // Añadir una variable para la probabilidad de efecto

    // Calcular el daño y obtener la probabilidad de efecto de estado
    (isCriticalHit, damage, statusEffectChance) = CalculateDamage(attack, attacker, defender);

    // Aplicar daño
    defender.stats.hp -= damage;
    if(damage >= 1)
    {
        defender.enraged += 1;
    }

    if(attacker.enraged >= 1)
    {
        attacker.enraged += 1;
    }

    Debug.Log($"{attacker.pokemonName} dealt {damage} damage to {defender.pokemonName} using {attack.name}!");

    if (isCriticalHit)
    {
        Debug.Log("It's a critical hit!");
    }

    // Aplicar estado alterado con la probabilidad recibida de CalculateDamage
    if (attack.statusEffect != PokemonBase.StatusCondition.None)
    {
        float randomChance = Random.Range(0f, 100f);
        if (randomChance <= statusEffectChance * 100) // Multiplicamos por 100 ya que statusEffectChance está entre 0 y 1
        {
            defender.ApplyStatusCondition(attack.statusEffect, 3); // Ejemplo: duración de 3 turnos
            Debug.Log($"{defender.pokemonName} is now {attack.statusEffect} due to {attack.name}!");
        }
        else
        {
            Debug.Log($"{defender.pokemonName} resisted the {attack.statusEffect} from {attack.name}.");
        }
    }

    return isCriticalHit; // Retorna solo el valor booleano
}


    public void CancelAttack()
    {
        // Restablecer el estado del Pokémon
        isSelectedForAttack = false; // Asegúrate de que el Pokémon no esté seleccionado para el ataque

        // Si tienes alguna lógica específica que necesites cancelar o restablecer, puedes agregarla aquí
        // Por ejemplo, detener animaciones de ataque o restablecer la posición del cursor
    }

    private float GetTypeEffectiveness(string attackType, string defenderType)
{
    // Tabla de efectividad de tipos
    Dictionary<string, Dictionary<string, float>> effectivenessTable = new Dictionary<string, Dictionary<string, float>>
    {
        { "Normal", new Dictionary<string, float>
            {
                { "Rock", 0.5f }, // Mitad de daño
                { "Ghost", 0.0f }, // Sin daño
                { "Steel", 0.5f }  // Mitad de daño
            }
        },
        { "Fire", new Dictionary<string, float>
            {
                { "Grass", 2.0f }, // Doble daño
                { "Water", 0.5f }, // Mitad de daño
                { "Fire", 0.5f },  // Daño normal
                { "Rock", 0.5f },  // Mitad de daño
                { "Bug", 2.0f },   // Doble daño
                { "Ice", 2.0f },   // Doble daño
                { "Steel", 2.0f },
                { "Dragon", 0.5f }
            }
        },
        { "Water", new Dictionary<string, float>
            {
                { "Fire", 2.0f },  // Doble daño
                { "Grass", 0.5f },  // Mitad de daño
                { "Water", 0.5f },  // Daño normal
                { "Dragon", 0.5f }, // Mitad de daño
                { "Ground", 2.0f }, // Daño normal
                { "Rock", 2.0f }
            }
        },
        { "Grass", new Dictionary<string, float>
            {
                { "Water", 2.0f }, // Doble daño
                { "Fire", 0.5f },  // Mitad de daño
                { "Grass", 0.5f },  // Daño normal
                { "Flying", 0.5f }, // Mitad de daño
                { "Bug", 0.5f },    // Mitad de daño
                { "Poison", 0.5f },
                { "Dragon", 0.5f },
                { "Steel", 0.5f },
                { "Rock", 2.0f },   // Doble daño
                { "Ground", 2.0f }
            }
        },
        { "Electric", new Dictionary<string, float>
            {
                { "Water", 2.0f }, // Doble daño
                { "Ground", 0.0f }, // Sin daño
                { "Electric", 0.5f }, // Daño normal
                { "Grass", 0.5f },
                { "Dragon", 0.5f },
                { "Flying", 2.0f } // Doble daño
            }
        },
        { "Ice", new Dictionary<string, float>
            {
                { "Grass", 2.0f }, // Doble daño
                { "Fire", 0.5f },  // Mitad de daño
                { "Ice", 0.5f },   // Daño normal
                { "Water", 0.5f },
                { "Steel", 0.5f },
                { "Flying", 2.0f }, // Doble daño
                { "Dragon", 2.0f }, // Doble daño
                { "Ground", 2.0f } // Daño normal
            }
        },
        { "Fighting", new Dictionary<string, float>
            {
                { "Normal", 2.0f }, // Doble daño
                { "Flying", 0.5f }, // Mitad de daño
                { "Psychic", 0.5f }, // Mitad de daño
                { "Rock", 2.0f },   // Daño normal
                { "Ice", 2.0f },
                { "Steel", 2.0f },
                { "Dark", 2.0f },
                { "Bug", 0.5f },    // Daño normal
                { "Poison", 0.5f },
                { "Ghost", 0.0f },
                { "Fairy", 0.5f }   // Mitad de daño
            }
        },
        { "Poison", new Dictionary<string, float>
            {
                { "Grass", 2.0f }, // Doble daño
                { "Ground", 0.5f }, // Daño normal
                { "Poison", 0.5f }, // Mitad de daño
                { "Rock", 0.5f },    // Daño normal
                { "Fairy", 2.0f },
                { "Steel", 0.0f },  // Daño normal
                { "Ghost", 0.5f }   // Mitad de daño
            }
        },
        { "Ground", new Dictionary<string, float>
            {
                { "Fire", 2.0f },  // Daño normal
                { "Grass", 0.5f },  // Mitad de daño
                { "Electric", 2.0f }, // Doble daño
                { "Poison", 2.0f },
                { "Steel", 2.0f },
                { "Flying", 0.0f }, // Daño normal
                { "Bug", 0.5f }, // Daño normal
                { "Rock", 2.0f }    // Doble daño
            }
        },
        { "Flying", new Dictionary<string, float>
            {
                { "Grass", 2.0f }, // Doble daño
                { "Electric", 0.5f }, // Mitad de daño
                { "Fighting", 2.0f }, // Doble daño
                { "Bug", 2.0f },    // Daño normal
                { "Stel", 0.5f },
                { "Rock", 0.5f }    // Mitad de daño
            }
        },
        { "Psychic", new Dictionary<string, float>
            {
                { "Fighting", 2.0f }, // Doble daño
                { "Poison", 2.0f },   // Doble daño
                { "Psychic", 0.5f },  // Daño normal
                { "Dark", 0.0f },
                { "Steel", 0.5f }       // Mitad de daño
            }
        },
        { "Bug", new Dictionary<string, float>
            {
                { "Grass", 2.0f },  // Daño normal
                { "Fire", 0.5f },   // Mitad de daño
                { "Fighting", 0.5f }, // Mitad de daño
                { "Flying", 0.5f }, // Mitad de daño
                { "Psychic", 2.0f }, // Daño normal
                { "Dark", 2.0f },   // Daño normal
                { "Poison", 0.5f },   // Mitad de daño
                { "Ghost", 0.5f },
                { "Fairy", 0.5f },
                { "Steel", 0.5f }    // Daño normal
            }
        },
        { "Rock", new Dictionary<string, float>
            {
                { "Fire", 2.0f },   // Daño normal
                { "Flying", 2.0f }, // Doble daño
                { "Bug", 2.0f },    // Doble daño
                { "Fighting", 0.5f }, // Mitad de daño
                { "Ice", 2.0f }, // Daño normal
                { "Ground", 0.5f },
                { "Steel", 0.5f }    // Daño normal
            }
        },
        { "Ghost", new Dictionary<string, float>
            {
                { "Normal", 0.0f }, // Mitad de daño
                { "Dark", 0.5f }, // Daño normal
                { "Ghost", 2.0f },
                { "Psychic", 2.0f }   // Doble daño
            }
        },
        { "Dragon", new Dictionary<string, float>
            {
                { "Dragon", 2.0f }, // Doble daño
                { "Fairy", 0.0f },  // Mitad de daño
                { "Steel", 0.5f }    // Daño normal
            }
        },
        { "Dark", new Dictionary<string, float>
            {
                { "Psychic", 2.0f }, // Doble daño
                { "Ghost", 2.0f },   // Doble daño
                { "Fighting", 0.5f }, // Mitad de daño
                { "Dark", 0.5f },    // Daño normal
                { "Fairy", 0.5f }    // Mitad de daño
            }
        },
        { "Steel", new Dictionary<string, float>
            {
                { "Ice", 2.0f }, // Doble daño
                { "Rock", 2.0f },   // Doble daño
                { "Fairy", 2.0f },
                { "Fire", 0.5f }, // Mitad de daño
                { "Water", 0.5f },    // Daño normal
                { "Steel", 0.5f },
                { "Electric", 0.5f }    // Mitad de daño
            }
        },
        { "Fairy", new Dictionary<string, float>
            {
                { "Fighting", 2.0f }, // Doble daño
                { "Dark", 2.0f },     // Doble daño
                { "Dragon", 2.0f },   // Doble daño
                { "Fire", 0.5f },    // Daño normal
                { "Poison", 0.5f },   // Mitad de daño
                { "Steel", 0.5f }     // Mitad de daño
            }
        }
    };

    // Comprobar si el tipo de ataque está en la tabla
    if (effectivenessTable.ContainsKey(attackType))
    {
        // Comprobar si el tipo defensor está en la tabla de efectividad del tipo atacante
        if (effectivenessTable[attackType].ContainsKey(defenderType))
        {
            return effectivenessTable[attackType][defenderType]; // Retorna la efectividad
        }
    }

    return 1.0f; // Daño normal si no hay interacción específica
}

    private float GetAttackHeight(Attack attack, PokemonBase attacker)
{
    // Usamos la altura real del atacante para calcular la altura del ataque
    float attackerHeight = attacker.realHeight;

    switch (attack.name)
    {
        case "Tackle":
            return attackerHeight * 0.5f; // Un tackle generalmente va hacia el torso del oponente.
        case "Vine Whip":
            return attackerHeight * 0.6f; // Un látigo cepa probablemente tenga una altura media-alta, dirigido a la parte superior del cuerpo.
        case "Scratch":
            return attackerHeight * 0.4f; // Un rasguño va dirigido hacia la parte baja del cuerpo, como el abdomen o las piernas.
        case "Ember":
            return attackerHeight * 0.7f; // Un ataque de fuego como Ember es lanzado a una altura media-alta, intentando impactar el torso o la cabeza.
        case "Dragon Claw":
            return attackerHeight * 0.8f; // Las garras de dragón son usualmente ataques altos, dirigidos a la cabeza o parte superior del cuerpo.
        case "Water Gun":
            return attackerHeight * 0.5f; // Un chorro de agua se lanza a la altura del torso o la parte media del cuerpo del oponente.
        case "Heat Wave":
            return attackerHeight * 0.9f; // Una ola de calor es más alta, alcanzando incluso la cabeza del oponente.
        case "Nuzzle":
            return attackerHeight * 0.3f; // Un ataque eléctrico cercano como Nuzzle se ejecuta desde muy bajo, normalmente tocando el cuerpo o las piernas.
        case "Thunder Shock":
            return attackerHeight * 0.5f; // Thunder Shock es un ataque eléctrico estándar dirigido a la mitad del cuerpo.
        case "Discharge":
            return attackerHeight * 0.7f; // Discharge es más potente y tiene un alcance más amplio, dirigiéndose a partes más altas del cuerpo.
        case "Spark":
            return attackerHeight * 0.4f; // Un ataque rápido y cercano como Spark golpea en la parte baja del oponente.
        case "Thunder Bolt":
            return attackerHeight * 0.8f; // Thunder Bolt es un ataque eléctrico fuerte y dirigido hacia la parte alta, como la cabeza o el torso alto.
        default:
            return attackerHeight * 0.5f; // Valor por defecto, en el medio.
    }
}

    public (bool, int, float) CalculateDamage(Attack attack, PokemonBase attacker, PokemonBase defender)
{
    int damage = 0;
    bool isCriticalHit = false;
    float statusEffectChance = 0f; // Variable para almacenar la probabilidad del efecto de estado

    // Ajuste de daño basado en la transformación
    if (attacker.currentTransformation == PokemonBase.TransformationType.Dynamax)
    {
        damage = CalculateDynamaxDamage(attack, defender);
    }
    else if (attacker.currentTransformation == PokemonBase.TransformationType.MegaEvolve)
    {
        damage = CalculateMegaEvolveDamage(attack, defender);
    }
    else
    {
        // Lógica normal de daño
        (isCriticalHit, damage) = CalculateNormalDamage(attacker, defender, attack);
    }

    // Verificar la altura del ataque
    float attackHeight = GetAttackHeight(attack, attacker); // Calcula la altura del ataque basado en el atacante
    PokemonBase.BodyParts defenderBody = defender.pokemonBody;

    // Determinar qué parte del cuerpo del defensor recibió el golpe
    if (attackHeight >= defenderBody.legsStart && attackHeight <= defenderBody.legsEnd)
    {
        // Golpe en las piernas
        damage = Mathf.RoundToInt(damage * 0.8f); // Menos daño en las piernas
        Debug.Log("Golpe en las piernas, menos daño.");

        // Solo para el ataque Vine Whip, aplicar 100% de probabilidad de esnared
        if (attack.name == "Vine Whip")
        {
            statusEffectChance = 1f; // 100% probabilidad de aplicar esnared
            Debug.Log("Golpe en las piernas con Vine Whip, menos daño pero aplica esnared.");
        }

    }
    else if (attackHeight > defenderBody.bodyStart && attackHeight <= defenderBody.bodyEnd)
    {
        // Golpe en el cuerpo
        Debug.Log("Golpe en el cuerpo, daño normal.");
        // Para Vine Whip, no se aplica esnared aquí, por lo que statusEffectChance sigue siendo 0f
    }
    else if (attackHeight > defenderBody.headStart && attackHeight <= defenderBody.headEnd)
    {
        // Golpe en la cabeza
        damage = Mathf.RoundToInt(damage * 1.1f); // Más daño en la cabeza
        Debug.Log("Golpe en la cabeza, más daño.");
    }
    else if (attackHeight > defenderBody.headEnd)
    {
        // Golpe por encima de la cabeza, "full body contact"
        damage = Mathf.RoundToInt(damage * 1.2f); // Modificador de daño más grande
        Debug.Log("Full body contact! Golpe en todas las áreas, daño máximo.");
    }

    // Aplicar modificador del enraged antes de retornar el resultado
    if (attacker.enraged > 0)
    {
        float enragedModifier = 1 + (attacker.enraged * 0.05f); // Cada nivel de enraged incrementa el daño en un 5%
        damage = Mathf.RoundToInt(damage * enragedModifier); // Redondear a un número entero
    }

    // Retornamos el crítico, el daño y la probabilidad del efecto de estado
    return (isCriticalHit, damage, statusEffectChance);
}


    private (bool, int) CalculateNormalDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        // Implementación original de daño
        int damage = 0;
        bool isCriticalHit = false;

        // Llamar a los métodos de daño físico o especial según la categoría del ataque
        if (attack.category == AttackCategory.Physical)
        {
            // Desestructurar el retorno de CalculatePhysicalDamage para obtener si es crítico y el daño
            (isCriticalHit, damage) = CalculatePhysicalDamage(attacker, defender, attack);
        }
        else if (attack.category == AttackCategory.Special)
        {
            // Asumimos que CalculateSpecialDamage también devolvería una tupla similar
            (isCriticalHit, damage) = CalculateSpecialDamage(attacker, defender, attack); 
        }

        return (isCriticalHit, damage);
    }

    private (bool, int) CalculatePhysicalDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        float contactMultiplier = 1;

        if(attack.makesContact == true)
        {
            contactMultiplier = (attacker.mass * attacker.agility) / (defender.mass * attacker.agility);
        }
        
        // Lógica para calcular el daño físico con tipo, crítico y STAB
        float effectivenessAgainstType1 = GetTypeEffectiveness(attack.type, defender.type1); // Tipo del ataque contra tipo1 del defensor
        float effectivenessAgainstType2 = GetTypeEffectiveness(attack.type, defender.type2); // Tipo del ataque contra tipo2 del defensor

        // Usar el máximo de las efectividades contra ambos tipos
        float effectiveness = Mathf.Max(effectivenessAgainstType1, effectivenessAgainstType2);

        float stabMultiplier = (attacker.type1 == attack.type || attacker.type2 == attack.type) ? 1.5f : 1.0f; // STAB

        // Calcular el daño base
        int baseDamage = Mathf.Max(0, (attacker.stats.atk - defender.stats.def) + attack.power);
        
        // Aplicar el valor de afinidad para incrementar el daño base
        float affinityMultiplier = 1.0f + (attacker.affinity / 100f * 0.1f); // Aumentar el daño basado en la afinidad

        // Determinar si es un golpe crítico
        bool isCriticalHit = Random.Range(0, 100) < 20; // 20% de probabilidad de golpe crítico

        // Calcular daño crítico
        int criticalHitDamage = isCriticalHit ? (int)(baseDamage * 1.5f * affinityMultiplier) : (int)(baseDamage * affinityMultiplier); 
        
        return (isCriticalHit, Mathf.FloorToInt(criticalHitDamage * effectiveness * stabMultiplier * contactMultiplier / 4));
    }

    private (bool, int) CalculateSpecialDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        float contactMultiplier = 1;

        if(attack.makesContact == true)
        {
            contactMultiplier = (attacker.mass * attacker.agility) / (defender.mass * attacker.agility);
        }

        // Lógica para calcular el daño especial con tipo, crítico y STAB
        float effectivenessAgainstType1 = GetTypeEffectiveness(attack.type, defender.type1); // Tipo del ataque contra type1 del defensor
        float effectivenessAgainstType2 = GetTypeEffectiveness(attack.type, defender.type2); // Tipo del ataque contra type2 del defensor

        // Usar el máximo de las efectividades contra ambos tipos
        float effectiveness = Mathf.Max(effectivenessAgainstType1, effectivenessAgainstType2);

        float stabMultiplier = (attacker.type1 == attack.type || attacker.type2 == attack.type) ? 1.5f : 1.0f; // STAB

        // Calcular el daño base
        int baseDamage = Mathf.Max(0, (attacker.stats.spAtk - defender.stats.spDef) + attack.power);
        
        // Aplicar el valor de afinidad para incrementar el daño base
        float affinityMultiplier = 1.0f + (attacker.affinity / 100f * 0.1f); // Aumentar el daño basado en la afinidad

        // Determinar si es un golpe crítico
        bool isCriticalHit = Random.Range(0, 100) < 20; // 20% de probabilidad de golpe crítico

        // Calcular daño crítico
        int criticalHitDamage = isCriticalHit ? (int)(baseDamage * 1.5f * affinityMultiplier) : (int)(baseDamage * affinityMultiplier); 

        // Lanzar un debug si se produce un golpe crítico
        if (isCriticalHit)
        {
            Debug.Log("It's a critical hit!");
        }

        return (isCriticalHit, Mathf.FloorToInt(criticalHitDamage * effectiveness * stabMultiplier * contactMultiplier / 3));
    }

    private void ApplyStatusEffect(PokemonBase defender, Attack attack)
    {
        if (attack.statusEffect != PokemonBase.StatusCondition.None)
        {
            float randomChance = Random.Range(0f, 100f);
            if (randomChance <= attack.statusEffectChance)
            {
                defender.ApplyStatusCondition(attack.statusEffect, 3); // Ejemplo: duración de 3 turnos
                Debug.Log($"{defender.pokemonName} is now {attack.statusEffect} due to {attack.name}!");
            }
            else
            {
                Debug.Log($"{defender.pokemonName} resisted the {attack.statusEffect} from {attack.name}.");
            }
        }
    }

    private int CalculateDynamaxDamage(Attack attack, PokemonBase defender)
    {
        // Lógica para calcular daño Dinamax (aumentar el poder del ataque, etc.)
        int baseDamage = attack.power * 2; // Ejemplo de aumento de daño
        return baseDamage;
    }

    private int CalculateMegaEvolveDamage(Attack attack, PokemonBase defender)
    {
        // Lógica para calcular daño de Mega Evolución
        int baseDamage = attack.power + 30; // Ejemplo de aumento de daño
        return baseDamage;
    }

    private IEnumerator WaitForUserClick(List<Vector3> attackPositions, PokemonBase attacker, string attackName, GameObject attackPrefab, int staminaCost)
    {
        //Debug.Log($"{attacker.pokemonName} is waiting for a target to attack with {attackName}...");
        int pokemonLayerMask = LayerMask.GetMask("pokemon"); // Asegúrate de que "pokemon" sea el nombre exacto de la capa.

        // Esperar hasta que el usuario haga clic en una de las posiciones
        while (true)
        {
            if (Input.GetMouseButtonDown(0)) // Clic izquierdo del mouse
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Asegúrate de que la z sea 0 para la detección de colisiones.

                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, pokemonLayerMask);
                if (hit.collider != null) // Verifica que se haya detectado un collider
                {

                    // Usar hit.collider para acceder al GameObject
                    if (hit.collider.CompareTag("pokemon"))
                    {
                        PokemonBase defender = hit.collider.GetComponent<PokemonBase>(); // Cambié hit.transform a hit.collider

                        // Comprobar si el clic es en un Pokémon y no en el mismo atacante
                        if (defender != null && defender != attacker)
                        {
                            // Comprobar si el clic está dentro de las posiciones de ataque
                            foreach (var position in attackPositions)
                            {
                                if (Vector3.Distance(hit.transform.position, position) < 0.5f) // Comprobar si está dentro de un rango cercano
                                {
                                    DestroyAttackTiles();

                                    // Indicar que este Pokémon ha sido seleccionado para el ataque
                                    defender.isSelectedForAttack = true;

                                    // Guardar las posiciones originales
                                    Vector3 originalAttackerPosition = attacker.transform.position;
                                    Vector3 originalDefenderPosition = defender.transform.position;

                                    if(GetAttackByName(attackName).makesContact == true)
                                    {
                                        // Mover el atacante hacia el defensor
                                        yield return StartCoroutine(MovePokemon(attacker, position));
                                
                                        // Regresar el atacante a su posición original
                                        yield return StartCoroutine(MovePokemon(attacker, originalAttackerPosition));
                                    }
                                    
                                    bool isCriticalHit = false;
                                    // Ejecutar el ataque
                                    isCriticalHit = ApplyAttack(attacker, defender, GetAttackByName(attackName));
                                    
                                    attacker.stamina = Mathf.Clamp(attacker.stamina - staminaCost, 0, 100);

                                        // Animar & Empujar al defensor
                                    yield return StartCoroutine(PushPokemon(defender, originalAttackerPosition, attackPrefab, isCriticalHit));
                                    
                                    
                                    attacker.Attack(GetAttackByName(attackName));
                                    
                                    defender.Defend(damage);

                                    this.CancelAttack(); // Cancela el modo de ataque
                                    yield break; // Salir del bucle
                                }
                            }                            
                            DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                            yield break; // Salir del bucle
                        }
                        else
                        {
                            DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                            yield break; // Salir del bucle
                        }
                    }
                    else
                    {
                        DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                        yield break; // Salir del bucle
                    }
                }
                else
                {
                    DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                    yield break; // Salir del bucle
                }
            }

            yield return null; // Esperar un cuadro antes de volver a comprobar
        }
    }

    private IEnumerator MovePokemon(PokemonBase pokemon, Vector3 targetPosition, float stopDistance = 0.75f)
    {
        float moveDuration = 0.5f; // Duración del movimiento
        float elapsedTime = 0f;

        Vector3 originalPosition = pokemon.transform.position;

        // Calcular la distancia entre la posición original y la posición objetivo
        float totalDistance = Vector3.Distance(originalPosition, targetPosition);

        // Calcular la nueva posición donde el Pokémon debe detenerse (parcialmente hacia el objetivo)
        Vector3 stopPosition = Vector3.Lerp(originalPosition, targetPosition, stopDistance / totalDistance);

        while (elapsedTime < moveDuration)
        {
            // Mover el Pokémon hacia la posición donde debe detenerse
            pokemon.transform.position = Vector3.Lerp(originalPosition, stopPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // Esperar el siguiente frame
        }

        // Asegurarse de que el Pokémon llegue a la posición de detención
        pokemon.transform.position = stopPosition;
    }

    private IEnumerator PushPokemon(PokemonBase defender, Vector3 attackerOriginalPosition, GameObject attackPrefab, bool isCriticalHit)
    {
        isCriticalHit = isCriticalHit;

        float pushDuration = 0.3f; // Duración del empuje
        float pushDistance = 1f; // Distancia a empujar
        Vector3 originalPosition = defender.transform.position;

        // Calcular la dirección opuesta al atacante
        Vector3 pushDirection = (defender.transform.position - attackerOriginalPosition).normalized;

        // Instanciar el prefab
        Vector3 animationPosition = (attackerOriginalPosition + defender.transform.position) / 2;
        GameObject animationInstance = Instantiate(attackPrefab, animationPosition, Quaternion.identity);

        // Destruir el objeto después de 2 segundos (ajusta el tiempo según la duración de la animación)
        Destroy(animationInstance, 2f);

        float elapsedTime = 0f;
        if (isCriticalHit)
        {
            while (elapsedTime < pushDuration)
        {
            defender.transform.position = Vector3.Lerp(originalPosition, originalPosition + pushDirection * pushDistance, (elapsedTime / pushDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // Esperar el siguiente frame
        }
        }
    }

    private bool IsWithinMapBounds(Vector3 position)
    {
        // Convertir la posición a enteros para acceder al índice del arreglo
        int xIndex = Mathf.FloorToInt(position.x);
        int yIndex = Mathf.FloorToInt(position.y);

        // Verificar si está dentro de los límites del mapa
        return xIndex >= 0 && xIndex < mapWidth && yIndex >= 0 && yIndex < mapHeight;
    }

    private bool IsTileBlocked(Vector3 position)
    {
        // Utiliza un área pequeña alrededor de la posición para verificar colisiones
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(tileSize, tileSize), 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("object")) // Cambiar el tag según corresponda
            {
                return true; // Hay un objeto bloqueando el tile
            }
        }
        return false; // No hay objetos bloqueando
    }

    private bool IsPokemonInTile(Vector3 position)
{
    // Utiliza un área pequeña alrededor de la posición para verificar colisiones
    Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(tileSize, tileSize), 0);
    foreach (Collider2D collider in colliders)
    {
        if (collider.CompareTag("pokemon")) // Verifica si el collider tiene el tag "pokemon"
        {
            return true; // Hay un Pokémon en este tile
        }
    }
    return false; // No hay Pokémon en este tile
}

    private void DestroyAttackTiles()
    {
        // Destruir todos los tiles instanciados
        foreach (var tile in instantiatedTiles.Values)
        {
            Destroy(tile.gameObject);
        }
        instantiatedTiles.Clear();
    }

    public static Attack GetAttackByName(string name)
    {
        // Utiliza LINQ para buscar más eficientemente si es posible
        return Instance.allAttacks.Find(attack => attack.name == name);
    }

    private static AttackCatalog _instance;
    public static AttackCatalog Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AttackCatalog>();
            }
            return _instance;
        }
    }

    public void TeachTM(PokemonBase pokemon, string tmName)
    {
        TM tm = tmCatalog.Find(t => t.tmName == tmName);
        if (tm != null)
        {
            pokemon.LearnTM(tm);
        }
        else
        {
            Debug.Log($"TM {tmName} not found.");
        }
    }

#region Tackle
public void Tackle(PokemonBase attacker)
{
    Debug.Log($"{attacker.pokemonName} use Tackle!");
    Vector3 attackerPosition = attacker.transform.position;

    // Definir los rangos de alcance (direcciones ortogonales, como en ajedrez con la torre)
    List<Vector3> directions = new List<Vector3>
    {
        new Vector3(-1, 0, 0), // Izquierda
        new Vector3(1, 0, 0),  // Derecha
        new Vector3(0, 1, 0),  // Arriba
        new Vector3(0, -1, 0)  // Abajo
    };

    // Verificar si el prefab está referenciado correctamente
    if (attackTilePrefab == null)
    {
        Debug.LogError("tilePrefab is not assigned!");
        return; // Salir si el prefab no está asignado
    }

    // Lista para guardar posiciones válidas de ataque
    List<Vector3> validAttackPositions = new List<Vector3>();

    // Iterar sobre cada dirección y verificar los tiles en esa dirección
    foreach (var direction in directions)
    {
        // Verificar el primer tile (distancia 1)
        Vector3 firstTile = attackerPosition + direction;
        
        // Comprobar si el primer tile está dentro del mapa
        if (IsWithinMapBounds(firstTile))
        {
            // Comprobar si hay un objeto o un Pokémon en el primer tile
            bool isTileBlockedByObject = IsTileBlocked(firstTile);  // Para objetos
            bool isTileBlockedByPokemon = IsPokemonInTile(firstTile); // Nueva función para detectar Pokémon

            if (!isTileBlockedByObject)
            {
                // Instanciar el tile de ataque si no está bloqueado por objeto (si hay Pokémon, sí se instancia)
                validAttackPositions.Add(firstTile);

                // Verificar el segundo tile (distancia 2), solo si el primer tile no está bloqueado por objeto
                if (!isTileBlockedByPokemon) // Solo continuamos si no hay un Pokémon
                {
                    Vector3 secondTile = attackerPosition + (direction * 2);
                    if (IsWithinMapBounds(secondTile) && !IsTileBlocked(secondTile))
                    {
                        validAttackPositions.Add(secondTile);
                    }
                }
            }
        }
    }

    // Instanciar el prefab en las posiciones de ataque válidas
    foreach (var position in validAttackPositions)
    {
        GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
        instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

        // Obtener el componente TileAttack para configurar propiedades adicionales si es necesario
        TileAttack tileAttack = tile.GetComponent<TileAttack>();
    }

    // Calcular el costo de stamina del ataque
    float staminaCost = attacker.mass / attacker.agility * attacker.t;
    staminaCost = Mathf.Clamp(staminaCost, 0, 100);
    int staminaCostInt = Mathf.RoundToInt(staminaCost);
    Debug.Log("Stamina to Consume: " + staminaCostInt);

    // Esperar la interacción del usuario
    StartCoroutine(WaitForUserClick(validAttackPositions, attacker, "Tackle", tacklePrefab, staminaCostInt));
}



#endregion

#region Vine Whip
public void VineWhip(PokemonBase attacker)
{
    Debug.Log($"{attacker.pokemonName} use Vine Whip!");
    Vector3 attackerPosition = attacker.transform.position;

    // Definir las direcciones de ataque y el rango (3 cuadros alrededor del atacante)
    Vector3[] directions = 
    {
        new Vector3(-1, 0, 0), // Izquierda
        new Vector3(1, 0, 0),  // Derecha
        new Vector3(0, 1, 0),  // Arriba
        new Vector3(0, -1, 0)  // Abajo
    };

    // Verificar si el prefab está referenciado correctamente
    if (attackTilePrefab == null)
    {
        Debug.LogError("tilePrefab is not assigned!");
        return; // Salir si el prefab no está asignado
    }

    // Recorrer cada dirección para un rango de 3 tiles
    foreach (var direction in directions)
    {
        for (int i = 1; i <= 3; i++) // Rango de 3 cuadros
        {
            Vector3 currentPos = attackerPosition + direction * i;

            // Comprobar si la posición está dentro de los límites del mapa
            if (!IsWithinMapBounds(currentPos))
            {
                Debug.LogWarning($"Position {currentPos} is out of map bounds, skipping instantiation.");
                break; // Detenerse si la posición está fuera de los límites
            }

            // Verificar si el tile está bloqueado por un objeto
            if (IsTileBlocked(currentPos))
            {
                Debug.LogWarning($"Position {currentPos} is blocked by an object, skipping subsequent tiles in this direction.");
                break; // Detenerse en esta dirección si está bloqueado por un objeto
            }

            // Instanciar el prefab del tile en la posición actual
            GameObject tile = Instantiate(attackTilePrefab, currentPos, Quaternion.identity);
            instantiatedTiles.Add(currentPos, tile.GetComponent<TileAttack>());

            // Verificar si hay un Pokémon en el tile
            if (IsPokemonInTile(currentPos))
            {
                Debug.Log($"Pokemon detected at {currentPos}, skipping subsequent tiles in this direction.");
                break; // Detenerse si hay un Pokémon, pero igual instanciar el tile
            }
        }
    }

    // Calcular el costo de stamina
    float staminaCost = attacker.mass / attacker.agility * attacker.t;
    staminaCost = Mathf.Clamp(staminaCost, 0, 100);
    int staminaCostInt = Mathf.RoundToInt(staminaCost);

    // Ahora, espera la interacción del usuario
    StartCoroutine(WaitForUserClick(new List<Vector3>(instantiatedTiles.Keys), attacker, "Vine Whip", vineWhipPrefab, staminaCostInt));
}


#endregion

#region Scratch
public void Scratch(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Scratch!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Scratch", scratchPrefab, staminaCostInt));
    }

#endregion

#region Ember
public void Ember(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Ember!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, 1, 0),  // Derecha
            attackerPosition + new Vector3(1, -1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Ember", emberPrefab, staminaCostInt));
    }

#endregion

#region Dragon Claw
public void DragonClaw(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Dragon Claw!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Dragon Claw", scratchPrefab, staminaCostInt));
    }

#endregion

#region Water Gun
public void WaterGun(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Water Gun!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-2, 0, 0), // Izquierda
            attackerPosition + new Vector3(2, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 2, 0),  // Arriba
            attackerPosition + new Vector3(0, -2, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Water Gun", waterGunPrefab, staminaCostInt));
    }

#endregion



#region Heat Wave
public void HeatWave(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Heat Wave!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, 1, 0),  // Derecha
            attackerPosition + new Vector3(1, -1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Heat Wave", emberPrefab, staminaCostInt));
    }

#endregion

#region Nuzzle
public void Nuzzle(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Nuzzle!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, 1, 0),  // Derecha
            attackerPosition + new Vector3(1, -1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Nuzzle", nuzzlePrefab, staminaCostInt));
    }

#endregion

#region Thunder Shock
public void ThunderShock(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Thunder Shock!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, 1, 0),  // Derecha
            attackerPosition + new Vector3(1, -1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 2, 0), // Izquierda
            attackerPosition + new Vector3(1, -2, 0),  // Derecha
            attackerPosition + new Vector3(1, 2, 0),  // Arriba
            attackerPosition + new Vector3(-1, -2, 0),  // Abajo

            attackerPosition + new Vector3(-2, 0, 0), // Izquierda
            attackerPosition + new Vector3(2, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 2, 0),  // Arriba
            attackerPosition + new Vector3(0, -2, 0),  // Abajo
            attackerPosition + new Vector3(-2, 1, 0), // Izquierda
            attackerPosition + new Vector3(2, 1, 0),  // Derecha
            attackerPosition + new Vector3(2, -1, 0),  // Arriba
            attackerPosition + new Vector3(-2, -1, 0),  // Abajo
            
            
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Thunder Shock", nuzzlePrefab, staminaCostInt));
    }
#endregion

#region Discharge
public void Discharge(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Discharge!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, 1, 0),  // Derecha
            attackerPosition + new Vector3(1, -1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 2, 0), // Izquierda
            attackerPosition + new Vector3(1, -2, 0),  // Derecha
            attackerPosition + new Vector3(1, 2, 0),  // Arriba
            attackerPosition + new Vector3(-1, -2, 0),  // Abajo

            attackerPosition + new Vector3(-2, 0, 0), // Izquierda
            attackerPosition + new Vector3(2, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 2, 0),  // Arriba
            attackerPosition + new Vector3(0, -2, 0),  // Abajo
            attackerPosition + new Vector3(-2, 1, 0), // Izquierda
            attackerPosition + new Vector3(2, 1, 0),  // Derecha
            attackerPosition + new Vector3(2, -1, 0),  // Arriba
            attackerPosition + new Vector3(-2, -1, 0),  // Abajo
            
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Discharge", nuzzlePrefab, staminaCostInt));
    }

#endregion

#region Spark
public void Spark(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Spark!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, -1, 0),  // Derecha
            attackerPosition + new Vector3(1, 1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0)  // Abajo
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Spark", nuzzlePrefab, staminaCostInt));
    }

#endregion

#region Thunder Bolt
public void ThunderBolt(PokemonBase attacker)
    {
        Debug.Log($"{attacker.pokemonName} use Thunder Bolt!");
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 1, 0), // Izquierda
            attackerPosition + new Vector3(1, -1, 0),  // Derecha
            attackerPosition + new Vector3(1, 1, 0),  // Arriba
            attackerPosition + new Vector3(-1, -1, 0),  // Abajo
            attackerPosition + new Vector3(-1, 2, 0), // Izquierda
            attackerPosition + new Vector3(1, -2, 0),  // Derecha
            attackerPosition + new Vector3(1, 2, 0),  // Arriba
            attackerPosition + new Vector3(-1, -2, 0),  // Abajo

            attackerPosition + new Vector3(-2, 0, 0), // Izquierda
            attackerPosition + new Vector3(2, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 2, 0),  // Arriba
            attackerPosition + new Vector3(0, -2, 0),  // Abajo
            attackerPosition + new Vector3(-2, 1, 0), // Izquierda
            attackerPosition + new Vector3(2, -1, 0),  // Derecha
            attackerPosition + new Vector3(2, 1, 0),  // Arriba
            attackerPosition + new Vector3(-2, -1, 0),  // Abajo
            
        };

        // Verificar si el prefab está referenciado correctamente
        if (attackTilePrefab == null)
        {
            Debug.LogError("tilePrefab is not assigned!");
            return; // Salir si el prefab no está asignado
        }

        // Instanciar el prefab en las posiciones de ataque
        foreach (var position in attackPositions)
        {
            // Comprobar si la posición está dentro de los límites del mapa o bloqueada
            if (!(IsWithinMapBounds(position) && !IsTileBlocked(position)))
            {
                Debug.LogWarning($"Position {position} is out of map bounds or blocked, skipping instantiation.");
                continue; // Saltar si está fuera de los límites
            }

            // Instancia el prefab del tile en la posición de ataque
            GameObject tile = Instantiate(attackTilePrefab, position, Quaternion.identity);
            instantiatedTiles.Add(position, tile.GetComponent<TileAttack>());

            // Obtener el componente TileHover para cambiar el color del tile
            TileAttack tileAttack = tile.GetComponent<TileAttack>();
        }

        float staminaCost = attacker.mass / attacker.agility * attacker.t;
        staminaCost = Mathf.Clamp(staminaCost, 0, 100);
        int staminaCostInt = Mathf.RoundToInt(staminaCost);

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Thunder Bolt", nuzzlePrefab, staminaCostInt));
    }

#endregion







}

public class BattleManager
{
    // Estructura para almacenar información de combate
    public class CombatTurn
    {
        public PokemonBase attacker;
        public PokemonBase defender;
        public Attack attack;
        public int speed;
        public int priority;

        public CombatTurn(PokemonBase attacker, PokemonBase defender, Attack attack)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attack = attack;
            this.priority = attacker.ModifyPriority(attack); // Considera la prioridad del ataque
            this.speed = attacker.GetEffectiveSpeed();
        }
    }
}