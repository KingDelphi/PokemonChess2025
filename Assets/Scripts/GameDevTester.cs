using UnityEngine;

public class GameDevTester : MonoBehaviour
{
    // Referencia al Pokémon seleccionado
    private PokemonBase selectedPokemon;

    // Método Update para detectar teclas presionadas
    private void Update()
    {
        // Detectar si se presiona la tecla "L"
        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUpUnit();
        }

        // Detectar si se presiona la tecla "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            GiveExpToUnit();
        }
    }

    // Método para subir 1 nivel al Pokémon seleccionado
public void LevelUpUnit()
{
    // Verifica si PokemonMovement.currentPokemon no es nulo
    if (PokemonMovement.currentPokemon != null) 
    {
        // Obtén el Pokémon actual y asegúrate de que sea del tipo PokemonBase
        PokemonBase currentPokemon = PokemonMovement.currentPokemon.GetComponent<PokemonBase>();

        if (currentPokemon != null)
        {
            currentPokemon.OnLevelUp(); // Llama al método LevelUp en PokemonBase
            Debug.Log($"{currentPokemon.pokemonName} ha subido de nivel! Nivel actual: {currentPokemon.stats.level}");
        }
        else
        {
            Debug.LogError("No se pudo obtener el componente PokemonBase del Pokémon actual.");
        }
    }
    else if (TrainerBase.currentTrainer != null) // Verifica si hay un entrenador seleccionado
    {
        TrainerBase currentTrainer = TrainerBase.currentTrainer; // Obtén el entrenador actual

        if (currentTrainer != null)
        {
            currentTrainer.OnLevelUp(); // Llama al método LevelUp en TrainerBase
            Debug.Log($"{currentTrainer.trainerName} ha subido de nivel! Nivel actual: {currentTrainer.stats.level}");
        }
        else
        {
            Debug.LogError("No se pudo obtener el entrenador actual.");
        }
    }
    else
    {
        Debug.LogError("No hay Pokémon ni entrenador seleccionado para subir de nivel.");
    }
}



    // Método para darle 50 puntos de experiencia al Pokémon seleccionado
public void GiveExpToUnit(int expAmount = 50)
{
    // Verifica si PokemonMovement.currentPokemon no es nulo
    if (PokemonMovement.currentPokemon != null) 
    {
        // Obtén el componente PokemonBase del Pokémon actual
        PokemonBase currentPokemon = PokemonMovement.currentPokemon.GetComponent<PokemonBase>();

        if (currentPokemon != null)
        {
            currentPokemon.AddExperience(expAmount); // Llama al método AddExperience en PokemonBase
            Debug.Log($"{currentPokemon.pokemonName} ha recibido {expAmount} de experiencia! Experiencia actual: {currentPokemon.currentExperience}");
        }
        else
        {
            Debug.LogError("No se pudo obtener el componente PokemonBase del Pokémon actual.");
        }
    }
    else if (TrainerBase.currentTrainer != null) // Verifica si hay un entrenador seleccionado
    {
        TrainerBase currentTrainer = TrainerBase.currentTrainer; // Obtén el entrenador actual

        if (currentTrainer != null)
        {
            currentTrainer.AddExperience(expAmount); // Llama al método AddExperience en TrainerBase
            Debug.Log($"{currentTrainer.trainerName} ha recibido {expAmount} de experiencia! Experiencia actual: {currentTrainer.currentExperience}");
        }
        else
        {
            Debug.LogError("No se pudo obtener el entrenador actual.");
        }
    }
    else
    {
        Debug.LogError("No hay Pokémon ni entrenador seleccionado para recibir experiencia.");
    }
}


}
