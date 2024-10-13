using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PokemonBase> playerPokemons; // Lista de Pokémon del jugador
    public List<PokemonBase> enemyPokemons;  // Lista de Pokémon del enemigo
    private int currentTurn; // Turno actual
    private bool isPlayerTurn; // Indica si es el turno del jugador
    public PokemonBase playerPokemonPrefab;
    public Vector3 position;
    public TrainerBase playerTrainerPrefab;
    public TrainerBase npcTrainerPrefab;

    public PokemonBase npc1PokemonPrefab;
    public PokemonBase npc2PokemonPrefab;
    public PokemonBase npc3PokemonPrefab;


    private void Awake()
    {
        // Asegurar que haya solo una instancia del GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentTurn = 0;
        isPlayerTurn = true;
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Inicializar Pokémon y otras configuraciones del juego
        // Ejemplo: Instanciar Pokémon en el tablero
        SpawnTrainers();
        SpawnPokemons();
        StartTurn();
    }

    private void SpawnTrainers()
    {
        // Asegúrate de que pokemonPrefab está asignado correctamente
            if (playerTrainerPrefab != null && npcTrainerPrefab != null)
            {
                // Instanciar Pokemon del jugador
                TrainerBase playerTrainer = Instantiate(playerTrainerPrefab, new Vector3(4.5f, 0.5f, -1), Quaternion.identity);
                playerTrainer.GetComponent<TrainerBase>().playerTrainer = true; // Activar el bool para el Pokémon del jugador

                // Instanciar Pokemon del NPC
                TrainerBase npcTrainer = Instantiate(npcTrainerPrefab, new Vector3(2.5f, 4.5f, -1), Quaternion.identity);
                npcTrainer.GetComponent<TrainerBase>().npcTrainer = true; // Activar el bool para el Pokémon del NPC
            }
            else
            {
                Debug.LogError("¡No se ha asignado el prefab del Entrenador!");
            }    
    }
    
    private void SpawnPokemons()
    {
        // Asegúrate de que pokemonPrefab está asignado correctamente
            if (playerPokemonPrefab != null && npc1PokemonPrefab != null
                                            && npc2PokemonPrefab != null
                                            && npc3PokemonPrefab != null)
            {
                // Instanciar Pokemon del jugador
                PokemonBase playerPokemon = Instantiate(playerPokemonPrefab, new Vector3(3.5f, 0.5f, -1), Quaternion.identity);
                playerPokemon.GetComponent<PokemonBase>().playerPokemon = true; // Activar el bool para el Pokémon del jugador

                // Instanciar Pokemon del NPC
                PokemonBase npc1Pokemon = Instantiate(npc1PokemonPrefab, new Vector3(2.5f, 1.5f, -1), Quaternion.identity);
                npc1Pokemon.GetComponent<PokemonBase>().npcPokemon = true; // Activar el bool para el Pokémon del NPC

                PokemonBase npc2Pokemon = Instantiate(npc2PokemonPrefab, new Vector3(5.5f, 4.5f, -1), Quaternion.identity);
                npc2Pokemon.GetComponent<PokemonBase>().npcPokemon = true; // Activar el bool para el Pokémon del NPC

                PokemonBase npc3Pokemon = Instantiate(npc3PokemonPrefab, new Vector3(2.5f, 5.5f, -1), Quaternion.identity);
                npc3Pokemon.GetComponent<PokemonBase>().npcPokemon = true; // Activar el bool para el Pokémon del NPC
            }
            else
            {
                Debug.LogError("¡No se ha asignado el prefab del Pokémon!");
            }    
    }

    private void StartTurn()
    {
        if (isPlayerTurn)
        {
            // Aquí puedes activar la lógica del turno del jugador
            // Ejemplo: permitir que el jugador seleccione un Pokémon y realice una acción
        }
        else
        {
            // Aquí puedes activar la lógica del turno del enemigo
            // Ejemplo: IA del enemigo para seleccionar un Pokémon y realizar una acción
        }
    }

    public void EndTurn()
    {
        // Cambiar el turno
        isPlayerTurn = !isPlayerTurn;
        currentTurn++;
        StartTurn(); // Comenzar el siguiente turno
    }
}
