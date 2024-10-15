using UnityEngine;

public class PokemonValuator : MonoBehaviour
{
    // Rango de precios
    private float normalMinPrice = 1f;
    private float normalMaxPrice = 100f;
    private float shinyMinPrice = 100f;
    private float shinyMaxPrice = 10000f;

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
        // Calcular el efecto de IVs
        float ivEffect = (stats.hpIV + stats.atkIV + stats.defIV + stats.spAtkIV + stats.spDefIV + stats.spdIV) / 186f; // Normalizar IVs si es necesario.

        // Definir un valor de efecto por defecto
        float natureBoost = 0f;

        // Lógica para calcular el efecto según la naturaleza
        switch (nature)
        {
            case "Adamant":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque, disminuye Ataque Especial
                break;
            case "Bashful":
                natureBoost = 0.0f + ivEffect; // Neutra
                break;
            case "Bold":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa, disminuye Velocidad
                break;
            case "Brave":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque, disminuye Velocidad
                break;
            case "Calm":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa Especial, disminuye Velocidad
                break;
            case "Careful":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa Especial, disminuye Ataque Especial
                break;
            case "Docile":
                natureBoost = 0.0f + ivEffect; // Neutra
                break;
            case "Gentle":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa Especial, disminuye Defensa
                break;
            case "Hardy":
                natureBoost = 0.0f + ivEffect; // Neutra
                break;
            case "Hasty":
                natureBoost = 0.1f + ivEffect; // Aumenta Velocidad, disminuye Defensa
                break;
            case "Impish":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa, disminuye Ataque Especial
                break;
            case "Jolly":
                natureBoost = 0.1f + ivEffect; // Aumenta Velocidad, disminuye Ataque Especial
                break;
            case "Lax":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa, disminuye Defensa Especial
                break;
            case "Lonely":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque, disminuye Defensa
                break;
            case "Mild":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque Especial, disminuye Defensa
                break;
            case "Modest":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque Especial, disminuye Ataque
                break;
            case "Naive":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa Especial, disminuye Velocidad
                break;
            case "Naughty":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque, disminuye Defensa Especial
                break;
            case "Quiet":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque Especial, disminuye Velocidad
                break;
            case "Quirky":
                natureBoost = 0.0f + ivEffect; // Neutra
                break;
            case "Rash":
                natureBoost = 0.1f + ivEffect; // Aumenta Ataque Especial, disminuye Defensa Especial
                break;
            case "Relaxed":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa, disminuye Velocidad
                break;
            case "Sassy":
                natureBoost = 0.1f + ivEffect; // Aumenta Defensa Especial, disminuye Velocidad
                break;
            case "Serious":
                natureBoost = 0.0f + ivEffect; // Neutra
                break;
            case "Timid":
                natureBoost = 0.1f + ivEffect; // Aumenta Velocidad, disminuye Ataque
                break;
            default:
                natureBoost = ivEffect; // Neutro, solo el efecto de IVs
                break;
        }

        return natureBoost; // Retornar el efecto de la naturaleza
    }

    // Fórmula para el cálculo de IVs
    public float GetIVEffect(PokemonBase.Stats stats)
    {
        // Calcula el total de IVs usando las variables individuales
        float totalIVs = stats.hpIV + stats.atkIV + stats.defIV + stats.spAtkIV + stats.spDefIV + stats.spdIV;
        float maxIVs = 186f; // 31 IVs * 6 stats (31 * 6)

        return totalIVs / maxIVs; // Entre 0-1
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
        return (realMass - minMass) / (maxMass - minMass); // Valor entre 0 y 1
    }

    // Cálculo de agilidad
    public float GetAgilityEffect(PokemonBase.Stats stats, float height, float mass)
    {
        return (height / mass) * stats.spd;
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

        // Factores iniciales
        float minPrice = isShiny ? shinyMinPrice : normalMinPrice;
        float maxPrice = isShiny ? shinyMaxPrice : normalMaxPrice;
        Debug.Log("Min price: " + minPrice);
        Debug.Log("Max price: " + maxPrice);

        // Factores del mercado
        float rarityEffect = (float)(totalPokemonInGame - pokemon.CountInGame) / totalPokemonInGame;
        Debug.Log("Rarity effect: " + rarityEffect);

        // Factores de stats
        float ivEffect = GetIVEffect(pokemon.stats);
        Debug.Log("IV effect: " + ivEffect);

        float natureEffect = GetNatureBoost(pokemon.nature.name, pokemon.stats);
        Debug.Log("Nature effect: " + natureEffect);

        float massEffect = GetMassEffect(pokemon);
        Debug.Log("Mass effect: " + massEffect);

        float agilityEffect = GetAgilityEffect(pokemon.stats, pokemon.height, pokemon.mass);
        Debug.Log("Agility effect: " + agilityEffect);

        float genderEffect = GetGenderRatioEffect(pokemon.DetermineGender());
        Debug.Log("Gender effect: " + genderEffect);

        // Peso total de factores
        float finalEffect = (rarityEffect + ivEffect + natureEffect + massEffect + agilityEffect + genderEffect) / 6;
        Debug.Log("Final effect: " + finalEffect);

        // Precio final basado en todos los factores
        float finalPrice = Mathf.Lerp(minPrice, maxPrice, finalEffect);
        Debug.Log("Final price: " + finalPrice);

        return finalPrice;
    }

    // Aquí puedes aplicar las fórmulas con tus ejemplos
    public void TestValuation(PokemonBase pokemon)
    {
        Debug.Log("Llamando a TestValuation para: " + pokemon.pokemonName);
        float pokemonPrice = EvaluatePokemon(pokemon.CountInGame);
        Debug.Log("Precio del Pokémon " + pokemon.pokemonName + ": " + pokemonPrice);
    }

}
