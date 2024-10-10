using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;


public class PokemonBase : MonoBehaviour
{
    public int pokemonNumber;       // Pokémon number
    public string pokemonName;      // Pokémon name
    public Nature pokemonNature;
    public string type1;            // First type of the Pokémon
    public string type2;            // Second type (can be empty if only one type)
    public float height;            // Height of the Pokémon
    public float weight;            // Weight of the Pokémon
    public int c; // aprox. 5
    public int k; // aprox. 2, puede bajar hasta 0.5 (Caso de Zapdos), o hasta 0.05 (Caso de Snorlax)
    public string description;
    public string[] abilities;      // Pokémon abilities
    public int catchRate;           // Catch rate
    public int expYield;            // Experience yielded when defeated
    public string growthRate;       // Growth rate
    public string eggGroup1;         // Egg group
    public string eggGroup2;
    public float genderRatio;       // Gender ratio (e.g., 0.5 for 50% male, 50% female)
    public Gender gender;
    public int eggCycles;           // Egg cycles required for hatching
    public Stats stats;             // Stats object with the Pokémon's stats
    public List<LearnedAttack> attackList; // List of the Pokémon's attacks
    public List<Attack> tmMoves; // Lista de movimientos aprendidos por TM/HM
    public List<Attack> eggMoves; // Lista de movimientos aprendidos por huevo
    public float evasion;           // Evasion rate
    public string activeAbility;  // Habilidad activa del Pokémon
    public Nature nature;           // Naturaleza del Pokémon
    public List<Evolution> evolutions = new List<Evolution>(); // Lista de evoluciones
    public StatusEffect currentStatusEffect;  // Current status effect
    public Item heldItem; // Objeto que sostiene el Pokémon
    public float affinity; // Valor de afinidad del Pokémon
    public TransformationType currentTransformation = TransformationType.None;
    private int maxTransformations = 1; // Número de transformaciones permitidas por combate
    private int remainingTurns = 0;
    public List<PokemonForm> forms; // Lista de formas del Pokémon
    public int currentFormIndex; // Índice de la forma actual
    public int friendshipLevel;
    public bool isDaytime;
    protected List<string> locations;
    protected bool isGigantamaxed = false;
    public int actionPoints;
    public AttackCatalog attackCatalog; // Asignar en el Inspector
    public bool isSelectedForAttack = false;
    public bool isAttacking = false;
    public bool playerPokemon = false;
    public bool npcPokemon = false;

    public AudioSource audioSource;
    public AudioClip attackSound; // Asegúrate de asignar este clip en el inspector
    public AudioClip damageSound;
    public AudioClip healSound;
    public AudioClip levelUpSound;
    public AudioClip evolveSound;
    public AudioClip tauntSound;

    public GameObject damageTextPrefab; // Prefab del texto de daño, asignado en el Inspector
    public Sprite pokedexImage; // Imagen estática del Pokémon para el Pokédex

    private void PlaySound(AudioClip clip)
    {
        // Verificar si el AudioSource está correctamente inicializado
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is null! Make sure it is attached to the GameObject.");
        }

        // Verificar si el clip no es nulo
        if (clip == null)
        {
            Debug.LogError("AudioClip is null! Make sure it is assigned in the Inspector.");
        }

        // Verifica que ambos no sean nulos antes de reproducir el sonido
        if (clip != null && audioSource != null) 
        {
            Debug.Log("Playing sound: " + clip.name); // Log para ver si se está llamando
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Audio clip or AudioSource is null!");
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Genderless // Si quieres incluir la opción de Pokémon sin género
    }

    public void DetermineGender()
    {
        // Verificar si el género es genderless
        if (genderRatio < 0)
        {
            gender = Gender.Genderless;
            return;
        }

        // Generar un número aleatorio entre 0 y 1
        float randomValue = Random.Range(0f, 1f);

        // Comparar el valor aleatorio con el genderRatio para determinar el género
        if (randomValue <= genderRatio)
        {
            gender = Gender.Male; // Es masculino
        }
        else
        {
            gender = Gender.Female; // Es femenino
        }

        Debug.Log($"{this.name} is {gender}");
    }


    public virtual void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>(); // Asegúrate de tener un AudioSource en el GameObject

        stats.level = 1;

        affinity = 50; // Valor inicial de afinidad (puede ser aleatorio o basado en condiciones)

        // Inicialización y asignación de la forma inicial
        forms = new List<PokemonForm>();
        // Ejemplo: Agregar formas (deberías hacer esto con los datos específicos de tu Pokémon)
        
        DetermineGender();
        AssignRandomNature(); // Asignar una naturaleza aleatoria al inicializar
        ApplyNature(); // Aplicar la naturaleza a las estadísticas
        AssignRandomIVs();
        DisplayNatureInfo(); // Mostrar información de la naturaleza
    }

    void Start()
    {        
        
    }

#region Nature Methods

// Método para asignar una naturaleza aleatoria
public void AssignRandomNature()
{
    int index = Random.Range(0, Nature.natures.Count);
    pokemonNature = Nature.natures[index];
}

public void ApplyNature()
{
    if (nature != null)
    {
        ApplyNatureModifiers(nature);
        Debug.Log($"{this.pokemonName} has applied the {pokemonNature.name} nature modifiers!");
    }
    else
    {
        Debug.LogWarning($"{this.pokemonName} has no nature assigned!"); // Avísame si la naturaleza es null
    }
}


public void DisplayNatureInfo()
{
    Debug.Log($"Nature: {pokemonNature.name}, Attack Modifier: {pokemonNature.attackModifier}, Defense Modifier: {pokemonNature.defenseModifier}, Special Attack Modifier: {pokemonNature.specialAttackModifier}, Special Defense Modifier: {pokemonNature.specialDefenseModifier}, Speed Modifier: {pokemonNature.speedModifier}");
}

public string GetNatureInfo()
{
    return $"Nature: {pokemonNature.name}, Attack Mod: {pokemonNature.attackModifier}, Defense Mod: {pokemonNature.defenseModifier}, " +
            $"Special Attack Mod: {pokemonNature.specialAttackModifier}, Special Defense Mod: {pokemonNature.specialDefenseModifier}, " +
            $"Speed Mod: {pokemonNature.speedModifier}";
}

#endregion

#region IV/EV Methods

public void AssignRandomIVs()
{
    stats.hpIV = Random.Range(0, 32);
    stats.atkIV = Random.Range(0, 32);
    stats.defIV = Random.Range(0, 32);
    stats.spAtkIV = Random.Range(0, 32);
    stats.spDefIV = Random.Range(0, 32);
    stats.spdIV = Random.Range(0, 32);

    Debug.Log($"IVs asignados a {pokemonName}: HP {stats.hpIV}, ATK {stats.atkIV}, DEF {stats.defIV}, SP. ATK {stats.spAtkIV}, SP. DEF {stats.spDefIV}, SPD {stats.spdIV}");
}

public void GainEVs(PokemonBase defeatedPokemon)
{
    stats.hpEV += defeatedPokemon.stats.evYield.hp;
    stats.atkEV += defeatedPokemon.stats.evYield.atk;
    stats.defEV += defeatedPokemon.stats.evYield.def;
    stats.spAtkEV += defeatedPokemon.stats.evYield.spAtk;
    stats.spDefEV += defeatedPokemon.stats.evYield.spDef;
    stats.spdEV += defeatedPokemon.stats.evYield.spd;

    // Asegurarse de no sobrepasar el límite de 510 EVs totales y 252 por estadística
    LimitEVs();
}

private void LimitEVs()
{
    // Limitar los EVs de cada estadística a 252
    stats.hpEV = Mathf.Min(stats.hpEV, 252);
    stats.atkEV = Mathf.Min(stats.atkEV, 252);
    stats.defEV = Mathf.Min(stats.defEV, 252);
    stats.spAtkEV = Mathf.Min(stats.spAtkEV, 252);
    stats.spDefEV = Mathf.Min(stats.spDefEV, 252);
    stats.spdEV = Mathf.Min(stats.spdEV, 252);

    // Limitar el total de EVs a 510
    int totalEVs = stats.hpEV + stats.atkEV + stats.defEV + stats.spAtkEV + stats.spDefEV + stats.spdEV;
    if (totalEVs > 510)
    {
        // Aquí podrías restar los puntos extra de forma equitativa o ajustar según sea necesario.
    }
}

#endregion

#region Level/Evolution Methods

void OnLevelUp()
{
    stats.level++;
    CalculateFinalStats(stats.level);
    ApplyNature();  // Aplicar nuevamente los efectos de la naturaleza tras el nivel
    PlaySound(levelUpSound);
}

public void CheckEvolution(int currentLevel, string heldItem = null)
{
    foreach (Evolution evolution in evolutions)
    {
        if (currentLevel >= evolution.requiredLevel && (heldItem == evolution.requiredItem || evolution.requiredItem == null))
        {
            Evolve(evolution.evolveTo);
            break;
        }
    }
}

private void Evolve(string newForm)
{
    Debug.Log($"{pokemonName} is evolving into {newForm}!");
    pokemonName = newForm;  // Change the Pokémon's name to its new form
    PlaySound(evolveSound);

    // Aquí podrías añadir lógica adicional, como cambiar estadísticas o aprender nuevos ataques.
}

#endregion

#region Stats Methods

public int hp;
public int maxHP;

// Método para obtener las estadísticas en formato de texto
    public string GetStatsString()
    {
        // Devuelve las estadísticas como una cadena separada por comas
        return $"{stats.hp}\n\n{stats.atk}\n\n{stats.def}\n\n{stats.spAtk}\n\n{stats.spDef}\n\n{stats.spd}\n";
    }

public void ApplyNatureModifiers(Nature nature)
{
    stats.atk = Mathf.FloorToInt(stats.atk * (1 + nature.attackModifier));
    stats.def = Mathf.FloorToInt(stats.def * (1 + nature.defenseModifier));
    stats.spAtk = Mathf.FloorToInt(stats.spAtk * (1 + nature.specialAttackModifier));
    stats.spDef = Mathf.FloorToInt(stats.spDef * (1 + nature.specialDefenseModifier));
    stats.spd = Mathf.FloorToInt(stats.spd * (1 + nature.speedModifier));
}

public void CalculateFinalStats(int level)
{
    stats.hp = (stats.maxHP + stats.hpIV + (stats.hpEV / 4)) * level / 100 + 10;
    stats.atk = (stats.atk + stats.atkIV + (stats.atkEV / 4)) * level / 100 + 5;
    stats.def = (stats.def + stats.defIV + (stats.defEV / 4)) * level / 100 + 5;
    stats.spAtk = (stats.spAtk + stats.spAtkIV + (stats.spAtkEV / 4)) * level / 100 + 5;
    stats.spDef = (stats.spDef + stats.spDefIV + (stats.spDefEV / 4)) * level / 100 + 5;
    stats.spd = (stats.spd + stats.spdIV + (stats.spdEV / 4)) * level / 100 + 5;
}

#endregion

#region Attack Management

public int ModifyPriority(Attack attack)
{
    // Prioridad modificada por la habilidad "Prankster"
    if (activeAbility == "Prankster" && attack.power == 0)  // Prankster aumenta la prioridad de movimientos sin poder de ataque
    {
        return attack.priority + 1;
    }

    // Prioridad modificada por "Gale Wings" (requiere que el ataque sea de tipo Volador y HP esté lleno)
    if (activeAbility == "Gale Wings" && attack.type == "Flying" && stats.hp == stats.total)
    {
        return attack.priority + 1;
    }

    return attack.priority;  // No modificar si no aplica la habilidad
}

public void LearnAttack(Attack newAttack, int level)
{
    // Verifica si el Pokémon ya conoce el ataque
    if (attackList.Exists(a => a.attack.name == newAttack.name))
    {
        Debug.Log($"{pokemonName} ya conoce {newAttack.name}.");
        return;
    }

    // Agrega el nuevo ataque a la lista
    attackList.Add(new LearnedAttack(newAttack, level));
    Debug.Log($"{pokemonName} ha aprendido {newAttack.name}!");
}

public void LearnTM(TM tm)
{
    if (!KnowsAttackThatFulfillsCondition(a => a.name == tm.attack.name)) // Verificar si ya lo conoce
    {
        attackList.Add(new LearnedAttack(tm.attack, 0));
        Debug.Log($"{pokemonName} learned {tm.attack.name} from TM {tm.tmName}!");
    }
    else
    {
        Debug.Log($"{pokemonName} already knows {tm.attack.name}.");
    }
}

public void LearnEggMove(EggMove eggMove)
{
    if (!KnowsAttackThatFulfillsCondition(a => a.name == eggMove.attack.name)) // Verificar si ya lo conoce
    {
        attackList.Add(new LearnedAttack(eggMove.attack, 0)); // Crea un nuevo LearnedAttack a partir de eggMove.attack
        Debug.Log($"{pokemonName} learned {eggMove.attack.name} as an egg move!");
    }
    else
    {
        Debug.Log($"{pokemonName} already knows {eggMove.attack.name}.");
    }
}

public bool KnowsAttackThatFulfillsCondition(System.Func<Attack, bool> condition)
{
    foreach (LearnedAttack learnedAttack in attackList)
    {
        if (condition(learnedAttack.attack))
        {
            return true;
        }
    }
    return false;
}

// Method to check and apply evasión
    public bool TryEvadeAttack()
    {
        float evasionCheck = Random.Range(0f, 100f);
        return evasionCheck < evasion;
    }

#endregion

#region Status Effect Management

// Class for representing status effects with duration
[System.Serializable]
public class StatusEffect
{
    public PokemonBase.StatusCondition condition;
    public float duration;

    public StatusEffect(PokemonBase.StatusCondition condition, float duration)
    {
        this.condition = condition;
        this.duration = duration;
    }
}

// Enum for altered statuses
    public enum StatusCondition
    {
        None,
        Paralysis,
        Freeze,
        Burn,
        Poison,
        Sleep
    }

public void ApplyStatusCondition(StatusCondition condition, float duration)
{
    if (HasImmunityTo(condition))
    {
        Debug.Log($"{pokemonName} is immune to {condition}.");
        return;
    }

    currentStatusEffect = new StatusEffect(condition, duration);
    Debug.Log($"{pokemonName} is now {condition} for {duration} turns.");
    ApplyStatusEffects();
}

public void UpdateStatusEffectDuration()
{
    if (currentStatusEffect != null && currentStatusEffect.duration > 0)
    {
        currentStatusEffect.duration--;
        if (currentStatusEffect.duration == 0)
        {
            ClearStatus();
        }
    }
}

private void ClearStatus()
{
    Debug.Log($"{pokemonName} has recovered from {currentStatusEffect.condition}!");
    currentStatusEffect = null; // Clear the status effect
    stats.spd = Mathf.Max(1, stats.spd * 2); // Restore speed
}

public bool IsParalyzed()
{
    return currentStatusEffect != null && currentStatusEffect.condition == StatusCondition.Paralysis;
}

public int GetEffectiveSpeed()
{
    // Si está paralizado, reducir la velocidad a la mitad
    if (IsParalyzed())
    {
        return Mathf.Max(1, stats.spd / 2);
    }
    return stats.spd;
}

private void ApplyStatusEffects()
{
    switch (currentStatusEffect.condition)
    {
        case StatusCondition.Paralysis:
            stats.spd = Mathf.Max(1, stats.spd / 2);
            Debug.Log($"{pokemonName} is paralyzed and its speed is reduced!");
            break;
        case StatusCondition.Freeze:
            Debug.Log($"{pokemonName} is frozen and cannot move!");
            break;
        case StatusCondition.Burn:
            stats.hp -= (int)(stats.hp * 0.1f); // Lose 10% of HP each turn
            Debug.Log($"{pokemonName} is burned and loses HP!");
            break;
        case StatusCondition.Poison:
            stats.hp -= (int)(stats.hp * 0.1f); // Lose 10% of HP each turn
            Debug.Log($"{pokemonName} is poisoned and loses HP!");
            break;
        case StatusCondition.Sleep:
            Debug.Log($"{pokemonName} is asleep and cannot move!");
            break;
        default:
            break;
    }
}

public bool HasImmunityTo(StatusCondition condition)
{
    if (condition == StatusCondition.Burn && (type1 == "Fire" || type2 == "Fire"))
        return true;
    if (condition == StatusCondition.Freeze && (type1 == "Ice" || type2 == "Ice"))
        return true;
    if (condition == StatusCondition.Paralysis && (type1 == "Electric" || type2 == "Electric"))
        return true;
    if (condition == StatusCondition.Poison && (type1 == "Poison" || type2 == "Poison"))
        return true;
    if (condition == StatusCondition.Sleep && (type1 == "Grass" || type2 == "Grass"))
        return true;

    return false;
}

#endregion

#region Megaevolutions, Gigantamax, Dinamax & Z-Moves Management

public enum TransformationType
{
    None,
    Dynamax,
    Gigantamax,
    MegaEvolve,
    ZMove
}

public void UseDynamax()
{
    if (currentTransformation == TransformationType.None)
    {
        currentTransformation = TransformationType.Dynamax;
        remainingTurns = 3; // Duración de la transformación
        Debug.Log($"{pokemonName} has Dynamaxed!");
    }
    else
    {
        Debug.Log($"{pokemonName} cannot Dynamax again until the effect wears off.");
    }
}

public virtual void Gigantamax()
{
    Debug.Log($"{pokemonName} has Gigantamaxed!");
    // Cambiar stats o atributos a los de la forma Gigantamax
    stats.hp += 50; // Aumentar HP como ejemplo
    // Aquí puedes añadir cambios adicionales a stats o habilidades
}

public void CheckForMegaEvolutions(string megaStone)
{
    // Verificar si se tiene el objeto necesario para megaevolucionar
    if (HasMegaStone(megaStone))
    {
        Debug.Log($"{pokemonName} megaevolves into {GetMegaEvolutionName(megaStone)}!");
        // Cambiar stats o atributos según el mega Pokémon
        if (megaStone == "Charizardite X")
        {
            stats.atk += 30; // Aumentar ataque como ejemplo
        }
        else if (megaStone == "Charizardite Y")
        {
            stats.spAtk += 30; // Aumentar ataque especial como ejemplo
        }
    }
}

protected bool HasMegaStone(string megaStone)
{
    // Aquí iría la lógica para verificar si tiene la mega piedra
    // Para el ejemplo, podemos suponer que siempre tiene una de ellas
    return true; // Cambiar esto por la lógica real en tu implementación
}

// Método para obtener el nombre de la mega evolución
private string GetMegaEvolutionName(string megaStone)
{
    switch (megaStone)
    {
        case "Charizardite X":
            return "Mega Charizard X";
        case "Charizardite Y":
            return "Mega Charizard Y";
        // Agrega más casos según las mega evoluciones que desees implementar
        default:
            return "Unknown Mega Evolution";
    }
}

public void UseMegaEvolution()
{
    if (currentTransformation == TransformationType.None)
    {
        currentTransformation = TransformationType.MegaEvolve;
        Debug.Log($"{pokemonName} has Mega Evolved!");
        // Aumentar stats o aplicar efectos especiales aquí si es necesario
    }
    else
    {
        Debug.Log($"{pokemonName} cannot Mega Evolve again until the effect wears off.");
    }
}

public virtual void UseZMove(Attack zMove)
{
    if (currentTransformation == TransformationType.None)
    {
        currentTransformation = TransformationType.ZMove;
        // Aplicar efectos del movimiento Z aquí
        Debug.Log($"{pokemonName} used a Z-Move: {zMove.name}!");
    }
    else
    {
        Debug.Log($"{pokemonName} cannot use a Z-Move until the effect wears off.");
    }
}

#endregion

#region Pokemon Multiple Forms Management

public void AddForms()
{
    // Ejemplo de cómo agregar formas, deberías hacerlo de acuerdo a tus datos
    forms.Add(new PokemonForm("Forma Normal", new Stats(), new List<LearnedAttack>()));
    forms.Add(new PokemonForm("Forma Alternativa", new Stats(), new List<LearnedAttack>()));
}

public void ChangeForm(int newFormIndex)
{
    if (newFormIndex < 0 || newFormIndex >= forms.Count)
    {
        Debug.LogError("Índice de forma no válido");
        return;
    }

    currentFormIndex = newFormIndex;
    ApplyFormStats(forms[currentFormIndex]);
    Debug.Log($"{pokemonName} ha cambiado a {forms[currentFormIndex].formName}!");
}

private void ApplyFormStats(PokemonForm form)
{
    stats = form.baseStats; // Aplicar estadísticas de la nueva forma
    attackList = form.formAttacks; // Aplicar ataques de la nueva forma
}

#endregion

#region In-match Actions Manager (Attack, ReceiveDamage, Heal, OnCharacterClick)

public void Attack()
    {
        // Lógica de ataque
        PlaySound(attackSound);
        ShowDamageText(0);
    }

    public void Defend(int damage)
    {
        //hp -= damage;
        PlaySound(damageSound);
        ShowDamageText(damage);
        // Aquí puedes agregar la lógica para mostrar el daño en pantalla, etc.
    }

    public void Heal(int healAmount)
    {
        hp = Mathf.Min(maxHP, hp + healAmount);
        PlaySound(healSound);
    }

    public void OnCharacterClick()
    {
        // Lógica al hacer clic en el personaje
        PlaySound(tauntSound); // Ejemplo de sonido al hacer clic
    }

    private void ShowDamageText(int damage)
{
    // Instanciar el prefab del texto de daño sobre el Pokémon
    GameObject textObject = Instantiate(damageTextPrefab, transform.position + new Vector3(0, .05f, 0), Quaternion.identity);

    // Obtener el componente TextMeshPro del objeto instanciado
    TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();

    // Asegurarte de que el componente TextMeshPro no sea nulo
    if (textMesh != null)
    {
        textMesh.text = damage.ToString(); // Asignar el texto del daño
        StartCoroutine(AnimateDamageText(textObject)); // Iniciar la animación
    }
    else
    {
        Debug.LogError("No se encontró el componente TextMeshPro en el prefab de daño.");
    }
}

private IEnumerator AnimateDamageText(GameObject textObject)
{
    float duration = 1.5f; // Duración de la animación
    Vector3 originalPosition = textObject.transform.position;
    Vector3 targetPosition = originalPosition + new Vector3(0, .05f, 0); // Mover hacia arriba

    float elapsedTime = 0f;
    while (elapsedTime < duration)
    {
        // Mover el texto hacia arriba
        textObject.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);

        // Escalar el texto (agrandarlo)
        textObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, elapsedTime / duration);

        elapsedTime += Time.deltaTime;
        yield return null; // Esperar al siguiente frame
    }

    // Después de la animación, destruir el texto
    Destroy(textObject);
}

#endregion

#region Affinity Management

public void IncreaseAffinity(float amount)
{
    affinity = Mathf.Clamp(affinity + amount, 0, 100); // Limitar la afinidad entre 0 y 100
    Debug.Log($"{pokemonName}'s affinity increased to {affinity}!");
}

public float GetCriticalHitMultiplier()
{
    return 1.0f + (affinity / 100f * 0.1f); // Aumentar daño crítico basado en la afinidad (máx. 10%)
}

public float GetEvasionChance()
{
    return 0.1f * (affinity / 100f * 0.1f); // Aumentar evasión basado en la afinidad (máx. 10%)
}

// Lógica para usar objetos que aumentan la afinidad
public void UseHealingItem(Item item)
{
    Heal(item.effectValue);
    IncreaseAffinity(5); // Aumentar afinidad al usar un objeto de curación
}

// Método que se llamaría al no usar al Pokémon en un tiempo determinado
public void DecreaseAffinity(float amount)
{
    affinity = Mathf.Clamp(affinity - amount, 0, 100); // Disminuir la afinidad
    Debug.Log($"{pokemonName}'s affinity decreased to {affinity}.");
}

#endregion

#region Item Management

public void EquipItem(Item item)
{
    heldItem = item;
    Debug.Log($"{pokemonName} is now holding {item.itemName}.");
}

// Método para usar el objeto en combate
public void UseItem()
{
    if (heldItem != null)
    {
        // Usar el objeto
        heldItem.Use(this);
        heldItem = null; // Opcional: Descartar el objeto después de usarlo
    }
    else
    {
        Debug.Log($"{pokemonName} has no item to use.");
    }
}

#endregion

#region Interaction Methods

    public virtual void DisplayInfo()
    {
        Debug.Log($"{pokemonName} (# {pokemonNumber}) - Types: {type1}/{type2}");
    }

    #endregion

#region Nested Classes

    [System.Serializable]
    public class PokemonForm
    {
        public string formName; // Nombre de la forma
        public Stats baseStats; // Estadísticas base para esta forma
        public List<LearnedAttack> formAttacks; // Lista de ataques únicos para esta forma

        public PokemonForm(string name, Stats stats, List<LearnedAttack> attacks)
        {
            formName = name;
            baseStats = stats;
            formAttacks = attacks;
        }
    }

    // Structure for Pokémon stats
    [System.Serializable]
    public struct Stats
    {
        public int level;

        public int maxHP;
        public int hp;
        public int atk;
        public int def;
        public int spAtk;
        public int spDef;
        public int spd;

        public int hpIV;
        public int atkIV;
        public int defIV;
        public int spAtkIV;
        public int spDefIV;
        public int spdIV;

        public int hpEV;
        public int atkEV;
        public int defEV;
        public int spAtkEV;
        public int spDefEV;
        public int spdEV;

        public int total
        {
            get
            {
                return hp + atk + def + spAtk + spDef + spd;
            }
        }

        // Definición de los EV yields
        [System.Serializable]
        public struct EVYield
        {
            public int hp;
            public int atk;
            public int def;
            public int spAtk;
            public int spDef;
            public int spd;
        }

        public EVYield evYield;
    }

    // Class for learned attacks with level
    [System.Serializable]
    public class LearnedAttack
    {
        public Attack attack;
        public int level;

        public LearnedAttack(Attack attack, int level)
        {
            this.attack = attack;
            this.level = level;
        }
    }

    // Class for evolution details
    [System.Serializable]
    public class Evolution
    {
        public string evolveTo;    // Name of the Pokémon to evolve into
        public int requiredLevel;  // Required level for evolution
        public string requiredItem; // Required item for evolution (optional)

        public Evolution(string evolveTo, int requiredLevel, string requiredItem = null)
        {
            this.evolveTo = evolveTo;
            this.requiredLevel = requiredLevel;
            this.requiredItem = requiredItem;
        }
    }

// Estructura para las naturalezas
    [System.Serializable]
    public class Nature
    {
        public string name;        // Nombre de la naturaleza
        public float attackModifier; // Modificador de ataque
        public float defenseModifier; // Modificador de defensa
        public float specialAttackModifier; // Modificador de ataque especial
        public float specialDefenseModifier; // Modificador de defensa especial
        public float speedModifier; // Modificador de velocidad

        public Nature(string name, float attackMod, float defenseMod, float spAtkMod, float spDefMod, float speedMod)
        {
            this.name = name;
            this.attackModifier = attackMod;
            this.defenseModifier = defenseMod;
            this.specialAttackModifier = spAtkMod;
            this.specialDefenseModifier = spDefMod;
            this.speedModifier = speedMod;
        }

        // Lista de naturalezas
        public static List<Nature> natures = new List<Nature>
        {
            new Nature("Adamant", 0.10f, 0f, -0.10f, 0f, 0f),  // Aumenta Ataque, disminuye Ataque Especial
            new Nature("Bashful", 0f, 0f, 0f, 0f, 0f),         // Neutra
            new Nature("Bold", 0f, 0.10f, 0f, 0f, -0.10f),     // Aumenta Defensa, disminuye Velocidad
            new Nature("Brave", 0.10f, 0f, 0f, 0f, -0.10f),    // Aumenta Ataque, disminuye Velocidad
            new Nature("Calm", 0f, 0f, 0f, 0.10f, -0.10f),     // Aumenta Defensa Especial, disminuye Velocidad
            new Nature("Careful", 0f, 0f, -0.10f, 0.10f, 0f),  // Aumenta Defensa Especial, disminuye Ataque Especial
            new Nature("Docile", 0f, 0f, 0f, 0f, 0f),          // Neutra
            new Nature("Gentle", 0f, -0.10f, 0f, 0.10f, 0f),   // Aumenta Defensa Especial, disminuye Defensa
            new Nature("Hardy", 0f, 0f, 0f, 0f, 0f),           // Neutra
            new Nature("Hasty", 0f, -0.10f, 0f, 0f, 0.10f),    // Aumenta Velocidad, disminuye Defensa
            new Nature("Impish", 0f, 0.10f, -0.10f, 0f, 0f),   // Aumenta Defensa, disminuye Ataque Especial
            new Nature("Jolly", 0f, 0f, -0.10f, 0f, 0.10f),     // Aumenta Velocidad, disminuye Ataque Especial
            new Nature("Lax", 0f, 0.10f, 0f, -0.10f, 0f),      // Aumenta Defensa, disminuye Defensa Especial
            new Nature("Lonely", 0.10f, -0.10f, 0f, 0f, 0f),   // Aumenta Ataque, disminuye Defensa
            new Nature("Mild", 0f, -0.10f, 0.10f, 0f, 0f),     // Aumenta Ataque Especial, disminuye Defensa
            new Nature("Modest", -0.10f, 0f, 0.10f, 0f, 0f),   // Aumenta Ataque Especial, disminuye Ataque
            new Nature("Naive", 0f, 0f, -0.10f, 0.10f, 0f),    // Aumenta Defensa Especial, disminuye Velocidad
            new Nature("Naughty", 0.10f, 0f, -0.10f, 0f, 0f),  // Aumenta Ataque, disminuye Defensa Especial
            new Nature("Quiet", 0f, 0f, 0.10f, 0f, -0.10f),     // Aumenta Ataque Especial, disminuye Velocidad
            new Nature("Quirky", 0f, 0f, 0f, 0f, 0f),          // Neutra
            new Nature("Rash", 0f, 0f, 0.10f, -0.10f, 0f),     // Aumenta Ataque Especial, disminuye Defensa Especial
            new Nature("Relaxed", 0f, 0.10f, 0f, 0f, -0.10f),   // Aumenta Defensa, disminuye Velocidad
            new Nature("Sassy", 0f, 0f, 0f, 0.10f, -0.10f),     // Aumenta Defensa Especial, disminuye Velocidad
            new Nature("Serious", 0f, 0f, 0f, 0f, 0f),         // Neutra
            new Nature("Timid", 0f, -0.10f, 0f, 0f, 0.10f),    // Aumenta Velocidad, disminuye Ataque
        };
    }
}
#endregion