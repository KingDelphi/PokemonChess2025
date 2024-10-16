using UnityEngine;
using System.Collections.Generic;

public class Charmander : PokemonBase
{
    public Charmander() : base(false, 0, 0, Gender.Male, 0, 0, 0, 0, 0, 0, 0, 0)
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
            hp = 39,
            atk = 52,
            def = 43,
            spAtk = 60,
            spDef = 50,
            spd = 65,
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
        pokemonNumber = 4;
        pokemonName = "Charmander";
        type1 = "Fire";
        type2 = ""; // Solo tiene un tipo
        height = 0.6f; // en metros
        weight = 8.5f; // en kilogramos
        c = 5;
        k = 2;
        description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.";
        abilities = new string[] { "Blaze", "Solar Power" };
        catchRate = 45;
        expYield = 62;
        growthRate = "Medium Slow";
        eggGroup1 = "Dragon";
        eggGroup2 = "Monster";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 20;
        actionPoints = GetComponent<Charmander>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            "Pallet Town"
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Charmeleon", 16)
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scratch"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ember"), 4));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Smokescreen"), 8));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Breath"), 12));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Fang"), 17));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Slash"), 20));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flamethrower"), 24));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scary Face"), 28));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Spin"), 32));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Inferno"), 36));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flare Blitz"), 40));

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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ancient Power"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Belly Drum"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Counter"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Rush"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Tail"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Iron Tail"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Metal Claw"), 0)); // Movimientos por huevo
    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scary Face"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Fang"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Spin"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Metal Claw"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rock Tomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flame Charge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fling"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Tail"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("False Swipe"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Brick Break"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shadow Claw"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Claw"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rock Slide"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swords Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Will-O-Wisp"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Crunch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Pulse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Heat Wave"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flamethrower"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Pledge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Outrage"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Overheat"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flare Blitz"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Temper Flare"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Breaking Swipe"), 0)); // TM
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
