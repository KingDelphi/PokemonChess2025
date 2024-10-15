using UnityEngine;
using System.Collections.Generic;

public class Rattata : PokemonBase
{   
    public Rattata() : base(false, 0, 0, Gender.Male, 0, 0, 0, 0, 0, 0, 0, 0)
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
            hp = 30,
            atk = 56,
            def = 35,
            spAtk = 25,
            spDef = 35,
            spd = 72,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 0,
                spDef = 0,
                spd = 1
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 19;
        pokemonName = "Rattata";
        type1 = "Normal";
        type2 = ""; // Solo tiene un tipo
        height = 0.3f; // en metros
        weight = 3.5f; // en kilogramos
        c = 5;
        k = 2;
        description = "Bites anything when it attacks. Small and very quick, it is a common sight in many places.";
        abilities = new string[] { "Run Away", "Guts", "Hustle" };
        catchRate = 255;
        expYield = 51;
        growthRate = "Medium Fast";
        eggGroup1 = "Field";
        eggGroup2 = "";
        genderRatio = 0.5f; // 50% male, 50% female
        eggCycles = 15;
        actionPoints = GetComponent<Rattata>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            "Route 1"
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Raticate", 20)
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tail Whip"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Quick Attack"), 4));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Energy"), 7));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 10));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Laser Focus"), 13));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 16));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Assurance"), 19));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Crunch"), 22));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sucker Punch"), 25));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Super Fang"), 28));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 31));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endeavor"), 34));

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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Counter"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Final Gambit"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flame Wheel"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fury Swipes"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Last Resort"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Revenge"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Reversal"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Screech"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Uproar"), 0)); // Movimientos por huevo

    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Work Up"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Taunt"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Blizzard"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Iron Tail"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunderbolt"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shadow Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double Team"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shock Wave"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sludge Bomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Attract"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thief"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charge Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Wave"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grass Knot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swagger"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Pluck"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Cut"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rock Smash"), 0)); // TM
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
