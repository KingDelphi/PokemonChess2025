using UnityEngine;
using System.Collections.Generic;

public class Pidgeotto : PokemonBase
{   
    public Pidgeotto() : base(false, 0, 0, Gender.Male, 0, 0, 0, 0, 0, 0, 0, 0)
    {
        // Lógica adicional de inicialización si es necesario
    }
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        stats = new Stats
        {
            hp = 63,
            atk = 60,
            def = 55,
            spAtk = 50,
            spDef = 50,
            spd = 71,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 0,
                spDef = 0,
                spd = 2
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 17;
        pokemonName = "Pidgeotto";
        type1 = "Normal";
        type2 = "Flying"; // Solo tiene un tipo
        height = 1.1f; // en metros
        weight = 30.0f; // en kilogramos
        c = 5;
        k = 2;
        description = "Very protective of its sprawling territorial area, this Pokemon will fiercely peck at any intruder.";
        abilities = new string[] { "Keen Eye", "Tangled Feet", "Big Pecks" };
        catchRate = 120;
        expYield = 122;
        growthRate = "Medium Slow";
        eggGroup1 = "Flying";
        eggGroup2 = "";
        genderRatio = 0.5f; // 50% male, 50% female
        eggCycles = 15;
        actionPoints = GetComponent<Pidgeotto>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Pidgeot", 36)
        };

        affinity = 50; // Valor inicial de afinidad (puede ser aleatorio o basado en condiciones)

        // Inicialización y asignación de la forma inicial
        forms = new List<PokemonForm>();
        // Ejemplo: Agregar formas (deberías hacer esto con los datos específicos de tu Pokémon)
        DetermineGender();
        AssignRandomNature(); // Asignar una naturaleza aleatoria al inicializar
        ApplyNature(); // Aplicar la naturaleza a las estadísticas
        AssignRandomIVs();
        DisplayNatureInfo(); // Mostrar información de la naturaleza
        isShiny = GenerateIsShiny(); 
        realHeight = GenerateRandomHeight(height); 
        realWeight = GenerateRandomWeight(weight);
        DetermineMass();
        DetermineAgility();
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Gust"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sand Attack"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sand Attack"), 5));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Gust"), 9));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Quick Attack"), 13));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Whirlwind"), 17));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Twister"), 22));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Feather Dance"), 27));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Agility"), 32));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Wing Attack"), 37));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roost"), 42));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tailwind"), 47));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Aerial Ace"), 52));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Air Slash"), 57));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hurricane"), 62));


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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Air Cutter"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Air Slash"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Brave Bird"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Uproar"), 0)); // Movimientos por huevo
    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Work Up"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double Team"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Aerial Ace"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Attract"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thief"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Steel Wing"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roost"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swagger"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Pluck"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("U-turn"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fly"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Defog"), 0)); // TM
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
