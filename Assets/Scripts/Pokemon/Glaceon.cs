using UnityEngine;
using System.Collections.Generic;

public class Glaceon : PokemonBase
{
    public Glaceon() : base(false, 0, 0, Gender.Male, 0, 0, 0, 0, 0, 0, 0, 0)
    {
        // Lógica adicional de inicialización si es necesario
    }
    
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        // Define Eevee's stats
        stats = new Stats
        {
            hp = 65,
            atk = 60,
            def = 110,
            spAtk = 130,
            spDef = 95,
            spd = 65,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 2,
                spDef = 0,
                spd = 0
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 471;
        pokemonName = "Glaceon";
        type1 = "Ice";
        type2 = ""; // Solo tiene un tipo
        height = 0.8f; // en metros
        weight = 25.9f; // en kilogramos
        c = 5;
        k = 2;
        description = "As a protective technique, it can completely freeze its fur to make its hairs stand like needles.";
        abilities = new string[] { "Snow Cloak", "Ice Body" };
        catchRate = 45;
        expYield = 184;
        growthRate = "Medium Fast";
        eggGroup1 = "Field";
        eggGroup2 = "";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 35;
        actionPoints = GetComponent<Glaceon>().stats.spd; // Asegúrate de que esto esté correcto

        // Localización de Eevee
        locations = new List<string>
        {
            
        };

        // Definir las posibles evoluciones de Eevee
        evolutions = new List<Evolution>
        {
            
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

        // Asignar ataques a Eevee
        attackList = new List<LearnedAttack>();

        // Supongamos que tenemos una referencia al catálogo de ataques
        AttackCatalog attackCatalog = FindObjectOfType<AttackCatalog>();
        attackCatalog.InitializeAttacks();

        // Inicialización de ataques de Eevee
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Baton Pass"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Copycat"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Covet"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1)); // Aprendido en nivel 5
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 1)); // Aprendido en nivel 10
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 1)); // Aprendido en nivel 15
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1)); // Aprendido en nivel 20
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tail Whip"), 1)); // Aprendido en nivel 25
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 1)); // Aprendido en nivel 30
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sand Attack"), 5)); // Aprendido en nivel 35
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Quick Attack"), 10)); // Aprendido en nivel 40
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Baby-Doll Eyes"), 15)); // Aprendido en nivel 45
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Shard"), 20)); // Aprendido en nivel 50
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 25)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Fang"), 30)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Snowscape"), 35)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Freeze-Dry"), 40)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Mirror Coat"), 45)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Blizzard"), 50)); // Aprendido en nivel 55
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Last Resort"), 55)); // Aprendido en nivel 55

        // Añadir movimientos por huevo
        AddEggMoves();

        // Añadir movimientos por TM
        AddTMMoves();

        // Inicializar amistad y determinar si es de día
        friendshipLevel = 35;  // Nivel de amistad
        isDaytime = true;       // Suponiendo que es de día para esta prueba

        isGigantamaxed = false; // Bandera para el estado Gigantamax
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Curse"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Detect"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double Kick"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flail"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Mud-Slap"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tickle"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Wish"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Yawn"), 0)); // Movimientos por huevo
    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fake Tears"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Mud-Slap"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Fang"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Water Pulse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Trailblaze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Chilling Water"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Icy Wind"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Mud Shot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Stored Power"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Avalanche"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Snowscape"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shadow Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hyper Voice"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Calm Mind"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Baton Pass"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ice Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Blizzard"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Giga Impact"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hyper Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Haze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Gravity"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Icicle Spear"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Triple Axel"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Curse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Alluring Voice"), 0)); // TM
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
