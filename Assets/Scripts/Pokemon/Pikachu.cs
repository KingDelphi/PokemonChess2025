using UnityEngine;
using System.Collections.Generic;

public class Pikachu : PokemonBase
{   
    public override void Awake()
    {
        attackCatalog = FindObjectOfType<AttackCatalog>();
        audioSource = GetComponent<AudioSource>();
        
        stats.level = 1;
        stats = new Stats
        {
            hp = 35,
            atk = 55,
            def = 40,
            spAtk = 50,
            spDef = 50,
            spd = 90,
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
        pokemonNumber = 25;
        pokemonName = "Pikachu";
        type1 = "Electric";
        type2 = ""; // Solo tiene un tipo
        height = 0.4f; // en metros
        weight = 6.0f; // en kilogramos
        c = 5;
        k = 2;
        description = "When several of these Pokemon gather, their electricity could build and cause lightning storms.";
        abilities = new string[] { "Static", "Lightning Rod" };
        catchRate = 190;
        expYield = 112;
        growthRate = "Medium Fast";
        eggGroup1 = "Fairy";
        eggGroup2 = "Field";
        genderRatio = 0.5f; // 50% male, 50% female
        eggCycles = 10;
        actionPoints = GetComponent<Pikachu>().stats.spd; // Asegúrate de que esto esté correcto

        locations = new List<string>
        {
            "Pallet Town"
        };

        // Definir las posibles evoluciones
        evolutions = new List<Evolution>
        {
            new Evolution("Raichu", 0, "Thunder Stone") // Requiere Water Stone
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

        // Asignar ataques
        attackList = new List<LearnedAttack>();

        // Supongamos que tenemos una referencia al catálogo de ataques
        AttackCatalog attackCatalog = FindObjectOfType<AttackCatalog>();
        attackCatalog.InitializeAttacks();

        // Inicialización de ataques
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Nasty Plot"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Nuzzle"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Play Nice"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Quick Attack"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sweet Kiss"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tail Whip"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Shock"), 1));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Wave"), 4));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double Team"), 8));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Electro Ball"), 12));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Feint"), 16));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Spark"), 20));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Agility"), 24));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Iron Tail"), 28));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Discharge"), 32));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunderbolt"), 36));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Light Screen"), 40));
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder"), 44));

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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charge"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Disarming Voice"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fake Out"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flail"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Present"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tickle"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Wish"), 0)); // Movimientos por huevo
    }

    private void AddTMMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Take Down"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fake Tears"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Agility"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Protect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thief"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Disarming Voice"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Trailblaze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charge Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Draining Kiss"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fling"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Volt Switch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rain Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Brick Break"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Electro Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Reflect"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Light Screen"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Grass Knot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Wave"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Eeerie Impulse"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Substitute"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Encore"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Surf"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunderbolt"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Play Rough"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Helping Hand"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Reversal"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Electric Terrain"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Nasty Plot"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Wild Charge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Charge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Knock Off"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endeavor"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Electroweb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Alluring Voice"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Upper Hand"), 0)); // TM
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
