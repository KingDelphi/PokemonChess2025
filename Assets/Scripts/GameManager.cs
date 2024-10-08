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
    public PokemonBase npcPokemonPrefab;
    public Vector3 position;

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
        SpawnPokemons();
        StartTurn();
    }

    private void SpawnPokemons()
    {
        // Asegúrate de que pokemonPrefab está asignado correctamente
            if (playerPokemonPrefab != null && npcPokemonPrefab != null)
            {
                Instantiate(playerPokemonPrefab, new Vector3 (3.5f, 0.5f, -1), Quaternion.identity); 
                Instantiate(npcPokemonPrefab, new Vector3 (3.5f, 5.5f, -1), Quaternion.identity);
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
            Debug.Log("Es el turno del jugador.");
            // Aquí puedes activar la lógica del turno del jugador
            // Ejemplo: permitir que el jugador seleccione un Pokémon y realice una acción
        }
        else
        {
            Debug.Log("Es el turno del enemigo.");
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
