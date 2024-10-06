using UnityEngine;
using System.Collections.Generic;

public class Charizard : PokemonBase
{
    public override void Awake()
    {
        // Define Charizard's stats
        stats = new Stats
        {
            hp = 78,
            atk = 84,
            def = 78,
            spAtk = 109,
            spDef = 85,
            spd = 100,
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
    }
    
    void Start()
    {        
        // Define Pokemon-specific properties
        pokemonNumber = 006;
        pokemonName = "Charizard";
        type1 = "Fire";
        type2 = "Flying"; // Charizard tiene dos tipos
        height = 1.7f; // en metros
        weight = 90.5f; // en kilogramos
        c = 5;
        k = 2;
        description = "Charizard flies around the sky in search of powerful opponents. It breathes fire of such great heat that it melts anything.";
        abilities = new string[] { "Blaze", "Solar Power" }; // Puede tener la habilidad Solar Power en su forma Mega Y
        catchRate = 45;
        expYield = 267;
        growthRate = "Medium Slow";
        eggGroup = "Monster, Dragon";
        genderRatio = 0.875f; // 87.5% masculino, 12.5% femenino
        eggCycles = 20;

        // Localización de Charizard
        locations = new List<string>
        {

        };

        // Definir las posibles evoluciones de Charizard
        evolutions = new List<Evolution>
        {
            new Evolution("Mega Charizard X", 0, "Charizardite X"), // Requiere Charizardite X
            new Evolution("Mega Charizard Y", 0, "Charizardite Y")  // Requiere Charizardite Y
        };

        // Asignar ataques a Charizard
        attackList = new List<LearnedAttack>();

        // Supongamos que tenemos una referencia al catálogo de ataques
        AttackCatalog attackCatalog = FindObjectOfType<AttackCatalog>();
        attackCatalog.InitializeAttacks();

        // Añadir movimientos a Charizard con el nivel en que se aprenden
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Claw"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ember"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Growl"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Heat Wave"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scratch"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Smokescreen"), 1)); // Aprendido en nivel 1
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Breath"), 12)); // Aprendido en nivel 12
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Fang"), 19)); // Aprendido en nivel 19
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Slash"), 24)); // Aprendido en nivel 24
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flamethrower"), 30)); // Aprendido en nivel 30
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scary Face"), 39)); // Aprendido en nivel 39
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Spin"), 46)); // Aprendido en nivel 46
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Inferno"), 54)); // Aprendido en nivel 54
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flare Blitz"), 62)); // Aprendido en nivel 62
        
        // Añadir movimientos por huevo
        AddEggMoves();

        // Añadir movimientos por TM
        AddTMMoves();

        // Inicializar amistad
        friendshipLevel = 50; // Ejemplo de nivel de amistad
        isDaytime = true;       // Suponiendo que es de día para esta prueba

        // Comprobar evoluciones
        CheckForMegaEvolutions("Charizardite X"); // Puedes cambiar a Y o a la piedra correspondiente según el caso
        
        isGigantamaxed = false; // Bandera para el estado Gigantamax
    }

    private void AddEggMoves()
    {
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Ancient Power"), 0)); // Movimientos por huevo
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Belly Drum"), 0)); // Movimientos por huevo
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Acrobatics"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Spin"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Facade"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Aerial Ace"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Bulldoze"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swift"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rock Tomb"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flame Charge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Air Cutter"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fling"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Tail"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Endure"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sunny Day"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sandstorm"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dig"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Brick Break"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Shadow Claw"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Air Slash"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Body Slam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fire Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Thunder Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Sleep Talk"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Claw"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rest"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Rock Slide"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Swords Dance"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Fly"), 0)); // TM
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
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Earthquake"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Giga Impact"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Blast Burn"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Outrage"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Overheat"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hurricane"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Hyper Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Flare Blitz"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Solar Beam"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Tera Blast"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Roar"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Heat Crash"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Focus Punch"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Weather Ball"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Double-Edge"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Temper Flare"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Scorching Sands"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Breaking Swipe"), 0)); // TM
        attackList.Add(new LearnedAttack(AttackCatalog.GetAttackByName("Dragon Cheer"), 0)); // TM
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
        Debug.Log($"Charizard Stats: HP {stats.hp}, ATK {stats.atk}, DEF {stats.def}, SP.ATK {stats.spAtk}, SP.DEF {stats.spDef}, SPD {stats.spd}");
    }
}
