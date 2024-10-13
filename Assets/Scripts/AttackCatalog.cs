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
            new Attack("Vine Whip", true, "Grass", 45, 100, "Whips the foe with slender vines.", AttackCategory.Physical), // Prioridad normal
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

    public void ApplyAttack(PokemonBase attacker, PokemonBase defender, Attack attack)
    {
        // Lógica para aplicar daño basada en la categoría del ataque

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

    private IEnumerator WaitForUserClick(List<Vector3> attackPositions, PokemonBase attacker, string attackName, GameObject attackPrefab)
    {
        Debug.Log("WaitForUserClick called.");
        int pokemonLayerMask = LayerMask.GetMask("pokemon"); // Asegúrate de que "pokemon" sea el nombre exacto de la capa.

        // Esperar hasta que el usuario haga clic en una de las posiciones
        while (true)
        {
            if (Input.GetMouseButtonDown(0)) // Clic izquierdo del mouse
            {
                Debug.Log("Mouse button clicked.");
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Asegúrate de que la z sea 0 para la detección de colisiones.

                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, pokemonLayerMask);
                if (hit.collider != null) // Verifica que se haya detectado un collider
                {
                    Debug.Log($"Hit detected: {hit.collider.name}");

                    // Usar hit.collider para acceder al GameObject
                    if (hit.collider.CompareTag("pokemon"))
                    {
                        PokemonBase defender = hit.collider.GetComponent<PokemonBase>(); // Cambié hit.transform a hit.collider

                        // Comprobar si el clic es en un Pokémon y no en el mismo atacante
                        if (defender != null && defender != attacker)
                        {
                            Debug.Log("Defender found and it's not the attacker.");

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
                                        Debug.Log("Moving attacker to defender position.");
                                        // Mover el atacante hacia el defensor
                                        yield return StartCoroutine(MovePokemon(attacker, position));
                                
                                        

                                        Debug.Log("Returning attacker to original position.");
                                        // Regresar el atacante a su posición original
                                        yield return StartCoroutine(MovePokemon(attacker, originalAttackerPosition));

                                        
                                    }

                                    Debug.Log("Pushing defender away.");
                                    // Empujar al defensor
                                    yield return StartCoroutine(PushPokemon(defender, originalAttackerPosition, attackPrefab));

                                    Debug.Log("Returning defender to original position.");
                                    // Regresar el defensor a su posición original
                                    defender.transform.position = originalDefenderPosition;

                                    Debug.Log("Executing attack.");
                                    // Ejecutar el ataque
                                    ApplyAttack(attacker, defender, GetAttackByName(attackName));

                                    Debug.Log("Playing attack sound.");
                                    attacker.Attack(GetAttackByName(attackName));

                                    Debug.Log("Playing defend sound.");
                                    defender.Defend(damage);

                                    this.CancelAttack(); // Cancela el modo de ataque
                                    yield break; // Salir del bucle
                                }
                            }
                            Debug.Log("Clicked position is not valid for attack.");
                            DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                            yield break; // Salir del bucle
                        }
                        else
                        {
                            Debug.Log("Clicked on the same attacker or no defender found.");
                            DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                            yield break; // Salir del bucle
                        }
                    }
                    else
                    {
                        Debug.Log("Hit object is not a Pokémon.");
                        DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                        yield break; // Salir del bucle
                    }
                }
                else
                {
                    Debug.Log("No collider hit.");
                    DestroyAttackTiles(); // Destruir los tiles si no se hizo clic en un Pokémon
                    yield break; // Salir del bucle
                }
            }

            yield return null; // Esperar un cuadro antes de volver a comprobar
        }
    }

    private IEnumerator MovePokemon(PokemonBase pokemon, Vector3 targetPosition, float stopDistance = 0.75f)
    {
        Debug.Log("MovePokemon called.");
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

    private IEnumerator PushPokemon(PokemonBase defender, Vector3 attackerOriginalPosition, GameObject attackPrefab)
    {
        Debug.Log("PushPokemon called.");
        float pushDuration = 0.3f; // Duración del empuje
        float pushDistance = 0.3f; // Distancia a empujar
        Vector3 originalPosition = defender.transform.position;

        // Calcular la dirección opuesta al atacante
        Vector3 pushDirection = (defender.transform.position - attackerOriginalPosition).normalized;

        // Instanciar el prefab
        Vector3 animationPosition = (attackerOriginalPosition + defender.transform.position) / 2;
        GameObject animationInstance = Instantiate(attackPrefab, animationPosition, Quaternion.identity);

        // Destruir el objeto después de 2 segundos (ajusta el tiempo según la duración de la animación)
        Destroy(animationInstance, 2f);

        float elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            defender.transform.position = Vector3.Lerp(originalPosition, originalPosition + pushDirection * pushDistance, (elapsedTime / pushDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // Esperar el siguiente frame
        }

        // Asegurarse de que el defensor regrese a su posición original
        defender.transform.position = originalPosition;
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
        Debug.Log("Tackle called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Tackle", tacklePrefab));
    }

#endregion

#region Vine Whip
public void VineWhip(PokemonBase attacker)
    {
        Debug.Log("Vine Whip called.");
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
            attackerPosition + new Vector3(0, -2, 0),  // Abajo
            attackerPosition + new Vector3(-3, 0, 0), // Izquierda
            attackerPosition + new Vector3(3, 0, 0),  // Derecha
            attackerPosition + new Vector3(0, 3, 0),  // Arriba
            attackerPosition + new Vector3(0, -3, 0)  // Abajo
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Vine Whip", vineWhipPrefab));
    }

#endregion

#region Scratch
public void Scratch(PokemonBase attacker)
    {
        Debug.Log("Scratch called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Scratch", scratchPrefab));
    }

#endregion

#region Ember
public void Ember(PokemonBase attacker)
    {
        Debug.Log("Ember called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Ember", emberPrefab));
    }

#endregion

#region Dragon Claw
public void DragonClaw(PokemonBase attacker)
    {
        Debug.Log("Dragon Claw called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Dragon Claw", scratchPrefab));
    }

#endregion

#region Water Gun
public void WaterGun(PokemonBase attacker)
    {
        Debug.Log("Water Gun called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Water Gun", waterGunPrefab));
    }

#endregion

#region Heat Wave
public void HeatWave(PokemonBase attacker)
    {
        Debug.Log("Heat Wave called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Heat Wave", emberPrefab));
    }

#endregion

#region Nuzzle
public void Nuzzle(PokemonBase attacker)
    {
        Debug.Log("Nuzzle called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Nuzzle", nuzzlePrefab));
    }

#endregion

#region Thunder Shock
public void ThunderShock(PokemonBase attacker)
    {
        Debug.Log("Thunder Shock called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Thunder Shock", nuzzlePrefab));
    }
#endregion

#region Discharge
public void Discharge(PokemonBase attacker)
    {
        Debug.Log("Discharge called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Discharge", nuzzlePrefab));
    }

#endregion

#region Spark
public void Spark(PokemonBase attacker)
    {
        Debug.Log("Spark called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Spark", nuzzlePrefab));
    }

#endregion

#region Thunder Bolt
public void ThunderBolt(PokemonBase attacker)
    {
        Debug.Log("Thunder Bolt called.");
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

        // Ahora, espera la interacción del usuario
        StartCoroutine(WaitForUserClick(attackPositions, attacker, "Thunder Bolt", nuzzlePrefab));
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