using UnityEngine;

public class PokemonValuator : MonoBehaviour
{
    // private bool isEvaluating = false; // Estado para controlar la evaluación

    void Update()
    {
        // Obtener la referencia a este Pokémon
        PokemonBase thisPokemon = GetComponent<PokemonBase>();

        // Verificar si el currentPokemon es nulo
        if (PokemonMovement.currentPokemon == null || PokemonMovement.currentPokemon.gameObject != gameObject)
        {
            return; // No hacer nada si no hay Pokémon seleccionado
        }

        // Si se presiona la tecla "V", y este Pokémon es el currentPokemon, y no está en evaluación
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log($"{thisPokemon.name} está siendo evaluado."); // Mensaje de depuración para la evaluación
            
            // Ejecutar la valuación solo para este Pokémon
            TestValuation(thisPokemon);
        }
    }






    // Fórmula para el cálculo de precio basado en naturaleza (IMPROVE)
    public float GetNatureBoost(string nature, PokemonBase.Stats stats)
{
    // Agregar este Debug.Log al inicio de la función GetNatureBoost
    Debug.Log("Nature: " + nature + 
            "\nIVs - HP: " + stats.hpIV + 
            ", Attack: " + stats.atkIV + 
            ", Defense: " + stats.defIV + 
            ", Special Attack: " + stats.spAtkIV + 
            ", Special Defense: " + stats.spDefIV + 
            ", Speed: " + stats.spdIV);

    
    // Calcular el efecto de IVs de manera más específica según la naturaleza
    float natureBoost = 1.0f;

    switch (nature)
    {
        case "Adamant":
            // Aumenta Ataque, disminuye Ataque Especial
            natureBoost = (0.1f * stats.atkIV) - (0.1f * stats.spAtkIV);
            break;
        case "Bashful":
            // Neutra
            natureBoost = 0.0f;
            break;
        case "Bold":
            // Aumenta Defensa, disminuye Velocidad
            natureBoost = (0.1f * stats.defIV) - (0.1f * stats.spdIV);
            break;
        case "Brave":
            // Aumenta Ataque, disminuye Velocidad
            natureBoost = (0.1f * stats.atkIV) - (0.1f * stats.spdIV);
            break;
        case "Calm":
            // Aumenta Defensa Especial, disminuye Velocidad
            natureBoost = (0.1f * stats.spDefIV) - (0.1f * stats.spdIV);
            break;
        case "Careful":
            // Aumenta Defensa Especial, disminuye Ataque Especial
            natureBoost = (0.1f * stats.spDefIV) - (0.1f * stats.spAtkIV);
            break;
        case "Docile":
            // Neutra
            natureBoost = 0.0f;
            break;
        case "Gentle":
            // Aumenta Defensa Especial, disminuye Defensa
            natureBoost = (0.1f * stats.spDefIV) - (0.1f * stats.defIV);
            break;
        case "Hardy":
            // Neutra
            natureBoost = 0.0f;
            break;
        case "Hasty":
            // Aumenta Velocidad, disminuye Defensa
            natureBoost = (0.1f * stats.spdIV) - (0.1f * stats.defIV);
            break;
        case "Impish":
            // Aumenta Defensa, disminuye Ataque Especial
            natureBoost = (0.1f * stats.defIV) - (0.1f * stats.spAtkIV);
            break;
        case "Jolly":
            // Aumenta Velocidad, disminuye Ataque Especial
            natureBoost = (0.1f * stats.spdIV) - (0.1f * stats.spAtkIV);
            break;
        case "Lax":
            // Aumenta Defensa, disminuye Defensa Especial
            natureBoost = (0.1f * stats.defIV) - (0.1f * stats.spDefIV);
            break;
        case "Lonely":
            // Aumenta Ataque, disminuye Defensa
            natureBoost = (0.1f * stats.atkIV) - (0.1f * stats.defIV);
            break;
        case "Mild":
            // Aumenta Ataque Especial, disminuye Defensa
            natureBoost = (0.1f * stats.spAtkIV) - (0.1f * stats.defIV);
            break;
        case "Modest":
            // Aumenta Ataque Especial, disminuye Ataque
            natureBoost = (0.1f * stats.spAtkIV) - (0.1f * stats.atkIV);
            break;
        case "Naive":
            // Aumenta Defensa Especial, disminuye Velocidad
            natureBoost = (0.1f * stats.spDefIV) - (0.1f * stats.spdIV);
            break;
        case "Naughty":
            // Aumenta Ataque, disminuye Defensa Especial
            natureBoost = (0.1f * stats.atkIV) - (0.1f * stats.spDefIV);
            break;
        case "Quiet":
            // Aumenta Ataque Especial, disminuye Velocidad
            natureBoost = (0.1f * stats.spAtkIV) - (0.1f * stats.spdIV);
            break;
        case "Quirky":
            // Neutra
            natureBoost = 0.0f;
            break;
        case "Rash":
            // Aumenta Ataque Especial, disminuye Defensa Especial
            natureBoost = (0.1f * stats.spAtkIV) - (0.1f * stats.spDefIV);
            break;
        case "Relaxed":
            // Aumenta Defensa, disminuye Velocidad
            natureBoost = (0.1f * stats.defIV) - (0.1f * stats.spdIV);
            break;
        case "Sassy":
            // Aumenta Defensa Especial, disminuye Velocidad
            natureBoost = (0.1f * stats.spDefIV) - (0.1f * stats.spdIV);
            break;
        case "Serious":
            // Neutra
            natureBoost = 0.0f;
            break;
        case "Timid":
            // Aumenta Velocidad, disminuye Ataque
            natureBoost = (0.1f * stats.spdIV) - (0.1f * stats.atkIV);
            break;
        default:
            // Neutra, solo el efecto de IVs
            natureBoost = 0.0f;
            break;
    }

    return natureBoost + 1.0f; // Retornar el efecto final
}


    // Fórmula para el cálculo de IVs
    public float GetIVEffect(PokemonBase.Stats stats)
    {
        // Mostrar las estadísticas base que se están usando para el cálculo
        Debug.Log("Base Stats - HP: " + stats.hp + 
                ", Attack: " + stats.atk + 
                ", Defense: " + stats.def + 
                ", Special Attack: " + stats.spAtk + 
                ", Special Defense: " + stats.spDef + 
                ", Speed: " + stats.spd);

        // Calcula el total de IVs multiplicado por sus respectivos stats base
        float totalIVs = (stats.hpIV * stats.hp) +
                        (stats.atkIV * stats.atk) +
                        (stats.defIV * stats.def) +
                        (stats.spAtkIV * stats.spAtk) +
                        (stats.spDefIV * stats.spDef) +
                        (stats.spdIV * stats.spd);

        // Sumar los stats base para obtener el maximo total posible
        float maxIVs = (31 * stats.hp) + (31 * stats.atk) + (31 * stats.def) + 
                    (31 * stats.spAtk) + (31 * stats.spDef) + (31 * stats.spd); 

        return (totalIVs / maxIVs) + 0.5f; // Entre 0-1
    }


    // Fórmula para calcular masa
    public float GetMassEffect(PokemonBase pokemon)
    {
        // Masa real del Pokémon en evaluación
        float realMass = pokemon.realHeight * pokemon.realWeight;

        // Masa estándar del Pokémon
        float baseHeight = pokemon.height; // Altura estándar del Pokémon
        float baseWeight = pokemon.weight; // Peso estándar del Pokémon
        float minHeight = baseHeight * 0.7f;
        float maxHeight = baseHeight * 1.1f;
        float minWeight = baseWeight * 0.9f;
        float maxWeight = baseWeight * 1.2f;

        float minMass = minHeight * minWeight;
        float maxMass = maxHeight * maxWeight;

        // Retorna el efecto de la masa basado en la masa real del Pokémon comparada con los límites estándar
        return ((realMass - minMass) / (maxMass - minMass)) + 0.5f; // Valor entre 0 y 1
    }

    // Cálculo de agilidad
    public float GetAgilityEffect(PokemonBase.Stats stats, float height, float mass)
{
    float rawAgility = (height / mass) * stats.spd;

    // Aplicar logaritmo para reducir el impacto de valores altos
    float scaledAgility = Mathf.Log10(rawAgility + 1); // "+1" para evitar log(0)

    return (Mathf.Clamp01(scaledAgility));
}


    // Cálculo del valor del ratio de género
    public float GetGenderRatioEffect(PokemonBase.Gender gender)
    {
        switch (gender)
        {
            case PokemonBase.Gender.Male:
                return 0.125f; // Macho
            case PokemonBase.Gender.Female:
                return 0.875f; // Hembra
            default:
                return 0.5f; // Neutro o no especificado
        }
    }

    // Fórmula para calcular el valor total del Rattata
    public float EvaluatePokemon(int totalPokemonInGame)
    {
        // Obtener el Pokémon adjunto al GameObject
        PokemonBase pokemon = GetComponent<PokemonBase>();

        // Verificar si es shiny
        bool isShiny = pokemon.isShiny;
        Debug.Log("Shiny status: " + isShiny);

        // Factores del mercado
        float rarityEffect = (float)(totalPokemonInGame - pokemon.CountInGame) / totalPokemonInGame;
        Debug.Log("Rarity effect: " + rarityEffect);

        // Factores de stats
        float ivEffect = GetIVEffect(pokemon.stats);
        Debug.Log("IV effect: " + ivEffect);

        float natureEffect = GetNatureBoost(pokemon.pokemonNature.name, pokemon.stats);
        Debug.Log("Nature effect: " + natureEffect);

        float massEffect = GetMassEffect(pokemon);
        Debug.Log("Mass effect: " + massEffect);

        float agilityEffect = GetAgilityEffect(pokemon.stats, pokemon.height, pokemon.mass);
        Debug.Log("Agility effect: " + agilityEffect);

        float genderEffect = GetGenderRatioEffect(pokemon.gender);
        Debug.Log("Gender effect: " + genderEffect);

        // Peso total de factores
        float finalEffect = (rarityEffect + ivEffect + natureEffect + massEffect + agilityEffect + genderEffect) / 6;
        Debug.Log("Final effect: " + finalEffect);

        // Precio final sin depender de un rango predefinido
        float basePrice = isShiny ? 10000f : 100f; // Definir un precio base para shiny o normal
        float finalPrice = basePrice * finalEffect; // Escalar según los efectos

        Debug.Log("Final price: " + finalPrice);

        return finalPrice * 10/4;
    }


    // Aquí puedes aplicar las fórmulas con tus ejemplos
    public void TestValuation(PokemonBase pokemon)
    {
        Debug.Log("Llamando a TestValuation para: " + pokemon.pokemonName);
        float pokemonPrice = EvaluatePokemon(pokemon.CountInGame);
        Debug.Log("Precio del Pokémon " + pokemon.pokemonName + ": " + pokemonPrice);
    }

}
