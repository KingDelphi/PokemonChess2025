using UnityEngine;
using System.Collections.Generic;

public class Wartortle : PokemonBase
{   
    
    public Wartortle() : base(false, 0, 0, Gender.Male, 0, 0, 0, 0, 0, 0, 0, 0)
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
            hp = 59,
            atk = 63,
            def = 80,
            spAtk = 65,
            spDef = 80,
            spd = 58,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 1,
                spAtk = 0,
                spDef = 1,
                spd = 0
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 8;
        pokemonName = "Wartortle";
        type1 = "Water";
        type2 = ""; // Solo tiene un tipo
        height = 1.0f; // en metros
        weight = 22.5f; // en kilogramos
        c = 5;
        k = 2;
        description = "Often hides in water to stalk unwary prey. For swimming fast, it moves its ears to maintain balance.";
        abilities = new string[] { "Torrent", "Rain Dish" };
        catchRate = 45;
        expYield = 142;
        growthRate = "Medium Slow";
        eggGroup1 = "Monster";
        eggGroup2 = "Water 1";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 20;
        actionPoints = GetComponent<Wartortle>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Blastoise", 36)
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
        pokemonBody = new BodyParts(realHeight);
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tail Whip"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Water Gun"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Withdraw"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rapid Spin"), 9));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 12));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Water Pulse"), 15));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 20));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 25));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Aqua Tail"), 30));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shell Smash"), 35));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Iron Defense"), 40));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hydro Pump"), 45));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Wave Crash"), 50));

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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Water Pulse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Chilling Water"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Icy Wind"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Mud Shot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fling"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Brick Break"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Zen Headbutt"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Iron Defense"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Liquidation"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Surf"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Spinner"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hydro Pump"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Blizzard"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Water Pledge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Haze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Gyro Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flip Turn"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Whirlpool"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Muddy Water"), 0)); // TM
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
