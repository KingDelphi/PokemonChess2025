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
    public string type;
    public int power;
    public int accuracy;
    public string description;
    public int priority;  // Prioridad del movimiento
    public PokemonBase.StatusCondition statusEffect;  // Estado alterado que puede causar el ataque
    public float statusEffectChance;      // Probabilidad de causar el estado alterado (0-100%)
    public AttackCategory category; // Nueva propiedad para la categoría

    // Constructor modificado para incluir la categoría
    public Attack(string name, string type, int power, int accuracy, string description, AttackCategory category, PokemonBase.StatusCondition statusEffect = PokemonBase.StatusCondition.None, float statusEffectChance = 0, int priority = 0)
    {
        this.name = name;
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

    public void InitializeAttacks()
    {
        allAttacks = new List<Attack>
        {
            // Ejemplos de ataques básicos sin estado alterado
            new Attack("Tackle", "Normal", 40, 100, "A basic physical attack.", AttackCategory.Physical), // Prioridad normal
            new Attack("Vine Whip", "Grass", 45, 100, "A grass-type whip attack.", AttackCategory.Physical), // Prioridad normal

            // Ejemplos de ataques con efectos de estado alterado
            new Attack("Thunderbolt", "Electric", 90, 100, "A strong electric attack that may cause paralysis.", AttackCategory.Special, PokemonBase.StatusCondition.Paralysis, 10f), // Prioridad normal
            new Attack("Flamethrower", "Fire", 90, 100, "A fiery attack that may cause burns.", AttackCategory.Special, PokemonBase.StatusCondition.Burn, 10f), // Prioridad normal
            new Attack("Ice Beam", "Ice", 90, 100, "A freezing beam that may cause freezing.", AttackCategory.Special, PokemonBase.StatusCondition.Freeze, 10f), // Prioridad normal
            new Attack("Poison Fang", "Poison", 50, 100, "A fang attack that may poison the target.", AttackCategory.Physical, PokemonBase.StatusCondition.Poison, 30f), // Prioridad normal

            // Ataques de alta prioridad
            new Attack("Quick Attack", "Normal", 40, 100, "A fast attack that strikes first.", AttackCategory.Physical, PokemonBase.StatusCondition.None, 0, 1), // Alta prioridad
            new Attack("Extreme Speed", "Normal", 80, 100, "An extremely fast attack.", AttackCategory.Physical, PokemonBase.StatusCondition.None, 0, 2), // Alta prioridad

            // Ejemplos de ataques adicionales
            new Attack("Thunder Wave", "Electric", 0, 90, "A jolt of electricity that paralyzes the opponent.", AttackCategory.Status, PokemonBase.StatusCondition.Paralysis, 100f), // Prioridad normal
            new Attack("Will-O-Wisp", "Fire", 0, 85, "Engulfs the opponent in a damaging fire, causing a burn.", AttackCategory.Status, PokemonBase.StatusCondition.Burn, 85f) // Prioridad normal
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

    public void ApplyAttack(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        // Lógica para aplicar daño basada en la categoría del ataque
        int damage = 0;

        // Utilizar el método de cálculo de daño que considera las transformaciones
        damage = CalculateDamage(attack, attacker, defender);

        // Aplicar daño al defensor
        defender.stats.hp -= damage;
        Debug.Log($"{attacker.pokemonName} dealt {damage} damage to {defender.pokemonName} using {attack.name}!");

        // Verificar evasión
        if (defender.TryEvadeAttack())
        {
            Debug.Log($"{defender.pokemonName} evaded the attack!");
            return;
        }

        // Aplicar estado alterado con probabilidad
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

    private IEnumerator WaitForUserClick(List<Vector3> attackPositions, PokemonBase attacker)
{
    // Esperar hasta que el usuario haga clic en una de las posiciones
    while (true)
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo del mouse
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                PokemonBase defender = hit.transform.GetComponent<PokemonBase>();
                // Comprobar si el clic es en un Pokémon y no en el mismo atacante
                if (defender != null && defender != attacker)
                {
                    // Comprobar si el clic está dentro de las posiciones de ataque
                    foreach (var position in attackPositions)
                    {
                        if (Vector3.Distance(hit.transform.position, position) < 0.5f) // Comprobar si está dentro de un rango cercano
                        {
                            // Indicar que este Pokémon ha sido seleccionado para el ataque
                            defender.isSelectedForAttack = true;

                            // Ejecutar el ataque Tackle
                            ApplyAttack(attacker, defender, GetAttackByName("Tackle"));
                            this.CancelAttack(); // Cancela el modo de ataque
                            yield break; // Salir del bucle
                        }
                    }
                }
            }
        }

        yield return null; // Esperar un cuadro antes de volver a comprobar
    }
}

public void CancelAttack()
    {
        // Restablecer el estado del Pokémon
        isSelectedForAttack = false; // Asegúrate de que el Pokémon no esté seleccionado para el ataque

        // Si tienes alguna lógica específica que necesites cancelar o restablecer, puedes agregarla aquí
        // Por ejemplo, detener animaciones de ataque o restablecer la posición del cursor

        Debug.Log($"{this.name} ha cancelado su ataque.");
    }


private float GetTypeEffectiveness(string attackType, string defenderType)
    {
        // Tabla de efectividad de tipos
        Dictionary<string, Dictionary<string, float>> effectivenessTable = new Dictionary<string, Dictionary<string, float>>
        {
            { "Fire", new Dictionary<string, float>
                {
                    { "Grass", 2.0f }, // Doble daño
                    { "Water", 0.5f }, // Mitad de daño
                    { "Fire", 1.0f },  // Daño normal
                    { "Rock", 0.5f }   // Mitad de daño
                }
            },
            { "Grass", new Dictionary<string, float>
                {
                    { "Water", 2.0f }, // Doble daño
                    { "Fire", 0.5f },  // Mitad de daño
                    { "Grass", 1.0f },  // Daño normal
                    { "Rock", 2.0f }    // Doble daño
                }
            },
            { "Water", new Dictionary<string, float>
                {
                    { "Fire", 2.0f },  // Doble daño
                    { "Grass", 0.5f },  // Mitad de daño
                    { "Water", 1.0f },  // Daño normal
                    { "Electric", 0.5f } // Mitad de daño
                }
            },
            // Agregar más tipos aquí según sea necesario
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

public int CalculateDamage(Attack attack, PokemonBase attacker, PokemonBase defender)
    {
        int damage = 0;

        // Aquí se puede ajustar el daño basado en la transformación
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
            damage = CalculateNormalDamage(attacker, defender, attack);
        }

        return damage;
    }

    private int CalculateNormalDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        // Implementación original de daño
        // Aquí puedes reutilizar la lógica de daño físico y especial según sea necesario
        int damage = 0;

        // Ejemplo simple para llamar a los métodos de daño físico o especial
        if (attack.category == AttackCategory.Physical)
        {
            damage = CalculatePhysicalDamage(attacker, defender, attack);
        }
        else if (attack.category == AttackCategory.Special)
        {
            damage = CalculateSpecialDamage(attacker, defender, attack);
        }

        return damage;
    }

    private int CalculatePhysicalDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
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

        // Calcular daño crítico
        int criticalHitDamage = (Random.Range(0, 100) < 20) ? (int)(baseDamage * 1.5f * affinityMultiplier) : (int)(baseDamage * affinityMultiplier); // 20% de chance de golpe crítico

        return Mathf.FloorToInt(criticalHitDamage * effectiveness * stabMultiplier);
    }

    private int CalculateSpecialDamage(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
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

        // Calcular daño crítico
        int criticalHitDamage = (Random.Range(0, 100) < 20) ? (int)(baseDamage * 1.5f * affinityMultiplier) : (int)(baseDamage * affinityMultiplier); // 20% de chance de golpe crítico

        return Mathf.FloorToInt(criticalHitDamage * effectiveness * stabMultiplier);
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


    public void Tackle(PokemonBase attacker)
    {
        Vector3 attackerPosition = attacker.transform.position;

        // Definir los rangos de alcance (cuadrados alrededor del atacante)
        List<Vector3> attackPositions = new List<Vector3>
        {
            attackerPosition + new Vector3(-1, 0, 0), // Izquierda
            attackerPosition + new Vector3(1, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 1, 0),  // Arriba
            attackerPosition + new Vector3(0, -1, 0)  // Abajo
        };

        // Dibujar los rangos de ataque (esto se puede mejorar con un método de visualización)
        foreach (var pos in attackPositions)
        {
            // Aquí puedes usar Gizmos o un método de visualización adecuado
            Debug.DrawLine(attackerPosition, pos, Color.green, 2.0f); // Dibuja una línea verde para mostrar el rango
        }

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker));
    }

    // Método para obtener un ataque por su nombre
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

    // Método para aprender TM
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

    

    // Método para ejecutar el turno
    // public void ExecuteTurn(PokemonBase pokemon1, PokemonBase pokemon2, Attack attack1, Attack attack2)
    // {
    //     CombatTurn turn1 = new CombatTurn(pokemon1, pokemon2, attack1);
    //     CombatTurn turn2 = new CombatTurn(pokemon2, pokemon1, attack2);

    //     // Comparar la prioridad primero
    //     CombatTurn firstTurn, secondTurn;
    //     if (turn1.priority > turn2.priority)
    //     {
    //         firstTurn = turn1;
    //         secondTurn = turn2;
    //     }
    //     else if (turn2.priority > turn1.priority)
    //     {
    //         firstTurn = turn2;
    //         secondTurn = turn1;
    //     }
    //     else
    //     {
    //         // Si la prioridad es igual, decidir por velocidad
    //         if (turn1.speed > turn2.speed)
    //         {
    //             firstTurn = turn1;
    //             secondTurn = turn2;
    //         }
    //         else if (turn2.speed > turn1.speed)
    //         {
    //             firstTurn = turn2;
    //             secondTurn = turn1;
    //         }
    //         else
    //         {
    //             // Si la velocidad es igual, elegir aleatoriamente
    //             firstTurn = Random.Range(0, 2) == 0 ? turn1 : turn2;
    //             secondTurn = firstTurn == turn1 ? turn2 : turn1;
    //         }
    //     }

    //     // Ejecutar el primer turno
    //     ApplyAttack(firstTurn.attacker, firstTurn.defender, firstTurn.attack);

    //     // Comprobar si el defensor sigue vivo antes de ejecutar el segundo turno
    //     if (firstTurn.defender.stats.hp > 0)
    //     {
    //         ApplyAttack(secondTurn.attacker, secondTurn.defender, secondTurn.attack);
    //     }
    // }

    // Método en PokemonBase que calcula el daño basado en la transformación
}