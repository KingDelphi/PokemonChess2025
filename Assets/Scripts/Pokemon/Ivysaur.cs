using UnityEngine;
using System.Collections.Generic;

public class Ivysaur : PokemonBase
{
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        stats = new Stats
        {
            hp = 60,
            atk = 62,
            def = 63,
            spAtk = 80,
            spDef = 80,
            spd = 60,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 1,
                spDef = 1,
                spd = 0
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 2;
        pokemonName = "Ivysaur";
        type1 = "Grass";
        type2 = "Poison"; // Solo tiene un tipo
        height = 1.0f; // en metros
        weight = 13.0f; // en kilogramos
        c = 5;
        k = 2;
        description = "When the bulb on its back grows large, it appears to lose the ability to stand on its hind legs.";
        abilities = new string[] { "Overgrow", "Chlorophyll" };
        catchRate = 45;
        expYield = 142;
        growthRate = "Medium Slow";
        eggGroup1 = "Grass";
        eggGroup2 = "Monster";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 20;
        actionPoints = GetComponent<Ivysaur>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Venusaur", 32)
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Vine Whip"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Leech Seed"), 9));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Razor Leaf"), 12));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Poison Powder"), 15));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Powder"), 15));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Seed Bomb"), 20));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 25));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sweet Scent"), 30));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Synthesis"), 35));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Worry Seed"), 40));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Power Whip"), 45));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Solar Beam"), 50));

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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Acid Spray"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Trailblaze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swords Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Giga Drain"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Energy Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grassy Terrain"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grass Pledge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sludge Bomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Leaf Storm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Solar Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Toxic"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Knock Off"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grassy Glide"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Curse"), 0)); // TM
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
