using UnityEngine;
using System.Collections.Generic;

public class Venusaur : PokemonBase
{
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        stats = new Stats
        {
            hp = 80,
            atk = 82,
            def = 83,
            spAtk = 100,
            spDef = 100,
            spd = 80,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 2,
                spDef = 1,
                spd = 0
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 3;
        pokemonName = "Venusaur";
        type1 = "Grass";
        type2 = "Poison"; // Solo tiene un tipo
        height = 2.0f; // en metros
        weight = 100.0f; // en kilogramos
        c = 5;
        k = 2;
        description = "The plant blooms when it is absorbing solar energy. It stays on the move to seek sunlight.";
        abilities = new string[] { "Overgrow", "Chlorophyll" };
        catchRate = 45;
        expYield = 236;
        growthRate = "Medium Slow";
        eggGroup1 = "Grass";
        eggGroup2 = "Monster";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 20;
        actionPoints = GetComponent<Venusaur>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Mega Venusaur", 0, "Venusaurite"),
            new Evolution("GMax Venusaur", 0)
        };

        affinity = 50; // Valor inicial de afinidad (puede ser aleatorio o basado en condiciones)

        // Inicialización y asignación de la forma inicial
        forms = new List<PokemonForm>();
        // Ejemplo: Agregar formas (deberías hacer esto con los datos específicos de tu Pokémon)
        DetermineGender();
        DetermineMass();
        AssignRandomNature(); // Asignar una naturaleza aleatoria al inicializar
        ApplyNature(); // Aplicar la naturaleza a las estadísticas
        AssignRandomIVs();
        DetermineAgility();
        DisplayNatureInfo(); // Mostrar información de la naturaleza
    }
    
    void Start()
    {
        base.Start();

        // Asignar ataques
        attackList = new List<LearnedAttack>();

        // Supongamos que tenemos una referencia al catálogo de ataques
        AttackCatalog attackCatalog = FindObjectOfType<AttackCatalog>();
        attackCatalog.InitializeAttacks();

        // Inicialización de ataques
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growth"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Petal Dance"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Vine Whip"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Leech Seed"), 9));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Razor Leaf"), 12));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Poison Powder"), 15));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Powder"), 15));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Seed Bomb"), 20));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 25));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sweet Scent"), 30));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Synthesis"), 37));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Worry Seed"), 44));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Power Whip"), 51));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Solar Beam"), 58));

        // Añadir movimientos por huevo
        AddEggMoves();

        // Añadir movimientos por TM
        AddTMMoves();

        // Inicializar amistad y determinar si es de día
        friendshipLevel = 50;  // Nivel de amistad
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Tackle(this); // Aquí this se refiere a la instancia de Eevee
        }
    }

    private void AddEggMoves()
    {
        
    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scary Face"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Acid Spray"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Trailblaze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bulldoze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Magical Leaf"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Venoshock"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bullet Seed"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("False Swipe"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Seed Bomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grass Knot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Poison Jab"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Stomping Tantrum"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swords Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Giga Drain"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Energy Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Amnesia"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Earth Power"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grassy Terrain"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grass Pledge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sludge Bomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Earthquake"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Giga Impact"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Frenzy Plant"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Leaf Storm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hyper beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Solar Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Toxic"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Knock Off"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grassy Glide"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Petal Blizzard"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Curse"), 0)); // TM


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
        if (megaStone == "Venusaurite")
        {
            stats.atk += 30; // Aumentar ataque como ejemplo
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
        case "Venusaurite":
            return "Mega Venusaur";
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

    // Método para usar Z-Move
    public override void UseZMove(Attack zMove)
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

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Debug.Log($"Stats: HP {stats.hp}, ATK {stats.atk}, DEF {stats.def}, SP.ATK {stats.spAtk}, SP.DEF {stats.spDef}, SPD {stats.spd}");
    }
}
