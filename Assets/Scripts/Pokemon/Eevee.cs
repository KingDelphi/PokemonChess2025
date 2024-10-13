using UnityEngine;
using System.Collections.Generic;

public class Eevee : PokemonBase
{
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        // Define Eevee's stats
        stats = new Stats
        {
            hp = 55,
            atk = 55,
            def = 50,
            spAtk = 45,
            spDef = 65,
            spd = 55,
            evYield = new Stats.EVYield // Inicializa el EV yield aquí
            {
                hp = 0,
                atk = 0,
                def = 0,
                spAtk = 3,
                spDef = 0,
                spd = 0
            }
        };

        // Define Pokemon-specific properties
        pokemonNumber = 133;
        pokemonName = "Eevee";
        type1 = "Normal";
        type2 = ""; // Solo tiene un tipo
        height = 0.3f; // en metros
        weight = 6.5f; // en kilogramos
        c = 5;
        k = 2;
        description = "Its genetic code is irregular. It may mutate if it is exposed to radiation from element stones.";
        abilities = new string[] { "Run Away", "Adaptability", "Anticipation" };
        catchRate = 45;
        expYield = 65;
        growthRate = "Medium Fast";
        eggGroup1 = "Field";
        eggGroup2 = "";
        genderRatio = 0.875f; // 87.5% male, 12.5% female
        eggCycles = 35;
        actionPoints = GetComponent<Eevee>().stats.spd; // Asegúrate de que esto esté correcto

        // Localización de Eevee
        locations = new List<string>
        {
            "Celadon City"
        };

        // Definir las posibles evoluciones de Eevee
        evolutions = new List<Evolution>
        {
            new Evolution("Vaporeon", 0, "Water Stone"), // Requiere Water Stone
            new Evolution("Jolteon", 0, "Thunder Stone"), // Requiere Thunder Stone
            new Evolution("Flareon", 0, "Fire Stone"), // Requiere Fire Stone
            new Evolution("Espeon", 20, null), // Evoluciona a Espeon si es nivel 20 y tiene alta amistad de día
            new Evolution("Umbreon", 20, null), // Evoluciona a Umbreon si es nivel 20 y tiene alta amistad de noche
            new Evolution("Leafeon", 0, "Leaf Stone"), // Evoluciona con Leaf Stone
            new Evolution("Glaceon", 0, "Ice Stone"), // Evoluciona con Ice Stone
            new Evolution("Sylveon", 20, null) // Evoluciona a Sylveon si tiene alta amistad y un ataque específico
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Covet"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tackle"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tail Whip"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sand Attack"), 5)); // Aprendido en nivel 5
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Quick Attack"), 10)); // Aprendido en nivel 10
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Baby-Doll Eyes"), 15)); // Aprendido en nivel 15
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 20)); // Aprendido en nivel 20
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bite"), 25)); // Aprendido en nivel 25
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Copycat"), 30)); // Aprendido en nivel 30
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Baton Pass"), 35)); // Aprendido en nivel 35
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 40)); // Aprendido en nivel 40
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 45)); // Aprendido en nivel 45
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 50)); // Aprendido en nivel 50
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Last Resort"), 55)); // Aprendido en nivel 55

        // Añadir movimientos por huevo
        AddEggMoves();

        // Añadir movimientos por TM
        AddTMMoves();

        // Inicializar amistad y determinar si es de día
        friendshipLevel = 50;  // Nivel de amistad
        isDaytime = true;       // Suponiendo que es de día para esta prueba

        // Comprobar evoluciones
        CheckForFriendshipEvolutions();

        isGigantamaxed = false; // Bandera para el estado Gigantamax
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Tackle(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.W) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.VineWhip(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.E) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Scratch(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.R) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Ember(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.T) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.DragonClaw(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.Y) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.WaterGun(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.U) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.HeatWave(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.I) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Nuzzle(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.O) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.ThunderShock(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.P) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Discharge(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.A) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.Spark(this); // Aquí this se refiere a la instancia de Eevee
        }

        if (Input.GetKeyDown(KeyCode.S) && (this == playerPokemon)) // Presionar la tecla T para atacar
        {
            AttackCatalog.Instance.ThunderBolt(this); // Aquí this se refiere a la instancia de Eevee
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Trailblaze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Stored Power"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Curse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Alluring Voice"), 0)); // TM
    }

    // Método para verificar evoluciones por amistad
    public void CheckForFriendshipEvolutions()
    {
        foreach (Evolution evolution in evolutions)
        {
            if (evolution.requiredLevel > 0 && friendshipLevel >= 220)
            {
                if (evolution.evolveTo == "Espeon" && isDaytime)
                {
                    Debug.Log($"{pokemonName} evolves into Espeon during the day with high friendship!");
                }
                else if (evolution.evolveTo == "Umbreon" && !isDaytime)
                {
                    Debug.Log($"{pokemonName} evolves into Umbreon during the night with high friendship!");
                }
                else if (evolution.evolveTo == "Sylveon" && KnowsAttackThatFulfillsCondition(attack => attack.type == "Fairy"))
                {
                    Debug.Log($"{pokemonName} evolves into Sylveon with high friendship and a fairy-type attack!");
                }
            }
        }
    }

    // Método para Gigantamax
    public override void Gigantamax()
    {
        if (!isGigantamaxed)
        {
            Debug.Log($"{pokemonName} has Gigantamaxed!");
            stats.hp += 50; // Aumentar HP como ejemplo
            // Aquí puedes añadir cambios adicionales a stats o habilidades
            isGigantamaxed = true; // Cambiar estado a Gigantamax
        }
        else
        {
            Debug.Log($"{pokemonName} is already Gigantamaxed!");
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
