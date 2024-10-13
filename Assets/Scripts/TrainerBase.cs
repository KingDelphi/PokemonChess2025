using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerBase : MonoBehaviour
{
    public string trainerName;       // Trainer's name
    public Stats stats;
    public int movementRange = 2;    // How far the Trainer can move in tiles (2 por defecto)
    public int currentHP;            // Current health of the Trainer
    public int maxHP;                // Maximum health
    public List<string> powers;      // List of powers the Trainer can use

    public bool playerTrainer = false;
    public bool npcTrainer = false;

    public List<Vector3> moveableTiles = new List<Vector3>(); // Public para depuración
    private Dictionary<Vector3, TileHover> instantiatedTiles = new Dictionary<Vector3, TileHover>(); // Lista para almacenar los tiles instanciados

    private Vector3 currentTrainerPosition;  // Actual posición del entrenador
    public GameObject tilePrefab;
    public float tileSize = 1f;   // Tamaño del tile
    public int mapWidth = 7;      // Ancho del mapa
    public int mapHeight = 7;     // Alto del mapa

    public static TrainerBase currentTrainer;   // Referencia al Trainer actual
    public float moveSpeed = 1f; // Velocidad de movimiento

    private PokedexUIController pokedexUIController;
    public Sprite trainerImage;

    private bool areTilesVisible = false;
    private Vector3 lastClickedPosition;

    public int currentExperience;
    public int experienceToNextLevel = 100;

    void Awake()
    {
        stats.level = 1;
        stats = new Stats
        {
            hp = 50,
            atk = 50,
            def = 50,
            spAtk = 50,
            spDef = 50,
            spd = 50
        };
    }

    // Método para subir de nivel
    public void OnLevelUp()
    {
        stats.level++;
        stats.currentExperience = 0; // Resetea la experiencia al subir de nivel
        // O puedes implementar la lógica para aumentar la experiencia a un nuevo nivel
    }

    // Método para agregar experiencia
    public void AddExperience(int amount)
    {
        stats.currentExperience += amount;
        while (stats.currentExperience >= experienceToNextLevel)
        {
            OnLevelUp(); // Llama a LevelUp si alcanza el umbral
        }
    }


    void Start()
    {
        pokedexUIController = FindObjectOfType<PokedexUIController>();
        // Inicializa las estadísticas del Trainer
        currentHP = maxHP;
        currentTrainerPosition = transform.position;  // Guarda la posición inicial
    }

    void Update()
    {
        // Detecta la posición del mouse en cada frame
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Asegura que Z sea 0
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // Detecta el click del mouse
        if (Input.GetMouseButtonDown(0)) 
        {
            if (hit.collider != null)
            {
                // Verifica si se ha hecho clic en el Trainer
                if (hit.collider.CompareTag("trainer") && hit.collider.GetComponent<TrainerBase>() == this)
                {
                    TrainerBase clickedTrainer = hit.collider.GetComponent<TrainerBase>();

                    // Ejecuta OnTrainerClick para el Trainer clicado
                    clickedTrainer.OnTrainerClick();

                    // Actualiza currentTrainer para llevar un registro
                    currentTrainer = clickedTrainer;
                }
                // Verifica si se ha hecho clic en un tile
                else if (hit.collider.CompareTag("tile")) 
                {
                    // El resto del código para manejar el clic en un tile permanece igual
                    currentTrainer = null; // Resetea el Trainer actual si es necesario

                    Vector3 targetTile = hit.collider.transform.position;

                    // Mover al Trainer si el tile está en moveableTiles
                    if (moveableTiles.Contains(targetTile)) 
                    {
                        // Determinar el número de movimientos (en casillas) requeridos
                        Vector3 initialPosition = transform.position; // Posición inicial del Trainer
                        int horizontalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.x) - Mathf.FloorToInt(initialPosition.x));
                        int verticalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.y) - Mathf.FloorToInt(initialPosition.y));
                        int totalMoves = horizontalMoves + verticalMoves; // Total de movimientos

                        int moveCost = CalculateMoveCost(totalMoves); // Calcular el costo del movimiento, puedes agregar otros parámetros si es necesario
                        
                        MoveToTile(targetTile); // Mover al Trainer
                    }
                    else
                    {
                        //Debug.Log("El tile seleccionado está fuera del rango de movimiento.");
                    }
                }
            }
        }
    }

    public string GetStatsString()
    {
        // Devuelve las estadísticas como una cadena separada por comas
        return $"{stats.hp}\n\n{stats.atk}\n\n{stats.def}\n\n{stats.spAtk}\n\n{stats.spDef}\n\n{stats.spd}\n";
    }

    private void OnTrainerClick()
{
    TrainerBase trainer = gameObject.GetComponent<TrainerBase>();

    if (trainer != null && pokedexUIController != null)
    {
        // Llama al método UpdatePokedex en la instancia del controlador del Pokédex para mostrar la información del Trainer
        pokedexUIController.UpdatePokedex(trainer);
    }
    else
    {
        Debug.LogError("No se pudo encontrar el componente TrainerBase o PokedexUIController.");
    }

    // Acceder a currentPokemon estático en la clase PokemonMovement
    PokemonMovement.currentPokemon = null;  // Acceso directo a la variable estática

    // Obtén la posición actual del Trainer
    currentTrainerPosition = transform.position;

    // Comprobar si se hizo clic en el mismo Trainer
    if (areTilesVisible && lastClickedPosition == currentTrainerPosition)
    {
        // Si los tiles ya están visibles y se hizo clic en el mismo Trainer, los destruimos
        DestroyMoveableTiles();
        areTilesVisible = false; // Actualiza el estado de visibilidad
    }
    else
    {
        // Ahora calcula los tiles movibles con los valores de este Trainer
        CalculateTrainerMoveableTiles(); // Asume que tienes un método para calcular los tiles movibles del Trainer
        areTilesVisible = true; // Actualiza el estado de visibilidad
        lastClickedPosition = currentTrainerPosition; // Actualiza la última posición clicada
        currentTrainer = this; // Actualiza currentTrainer
    }
}




    // Función que calcula los tiles movibles para el entrenador
    private void CalculateTrainerMoveableTiles()
    {
        Debug.Log("CalculateTrainerMoveableTiles called.");
        moveableTiles.Clear();
        instantiatedTiles.Clear();

        Queue<Vector3> queue = new Queue<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        queue.Enqueue(currentTrainerPosition);  // Empieza desde la posición del Trainer
        visited.Add(currentTrainerPosition);

        int movesMade = 0;

        while (queue.Count > 0 && movesMade < movementRange)
        {
            int tilesInCurrentLevel = queue.Count;

            for (int i = 0; i < tilesInCurrentLevel; i++)
            {
                Vector3 currentTile = queue.Dequeue();

                foreach (Vector3 direction in new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down })
                {
                    Vector3 neighborTile = currentTile + direction;

                    if (IsWithinMapBounds(neighborTile) && !visited.Contains(neighborTile))
                    {
                        if (!IsTileBlocked(neighborTile))
                        {
                            visited.Add(neighborTile);
                            queue.Enqueue(neighborTile);

                            // Instanciar tile visible
                            GameObject tile = Instantiate(tilePrefab, neighborTile, Quaternion.identity);
                            tile.name = neighborTile.ToString();
                            instantiatedTiles.Add(neighborTile, tile.GetComponent<TileHover>());
                            moveableTiles.Add(neighborTile);
                        }
                    }
                }
            }

            movesMade++;
        }
    }

    private bool IsWithinMapBounds(Vector3 position)
    {
        // Convertir la posición a enteros para acceder al índice del arreglo
        int xIndex = Mathf.FloorToInt(position.x);
        int yIndex = Mathf.FloorToInt(position.y);

        // Verificar si está dentro de los límites del mapa
        return xIndex >= 0 && xIndex < mapWidth && yIndex >= 0 && yIndex < mapHeight;
    }

    private bool IsTileBlocked(Vector3 position)
    {
        // Utiliza un área pequeña alrededor de la posición para verificar colisiones
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(tileSize, tileSize), 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("object") || collider.CompareTag("pokemon") || collider.CompareTag("trainer")) // Cambiar el tag según corresponda
            {
                return true; // Hay un objeto bloqueando el tile
            }
        }
        return false; // No hay objetos bloqueando
    }

    public void TileEnter(Vector3 tile)
    {
        ClearPathBlue();
        List<Vector3> path = CalculatePath(currentTrainerPosition, tile);
        foreach (Vector3 t in path)
        {
            TileHover tileHover = GetTileAtPosition(t);
            tileHover.ChangeColorToBlue();
        }
    }

    void ClearPathBlue()
{
    // Recorre todos los tiles y restablece el material original en los tiles pintados
    foreach (Vector3 tilePos in moveableTiles)
    {
        TileHover tile = GetTileAtPosition(tilePos); 
        tile.ChangeColorToGray(); // Cambia al material por defecto
    }
}

private List<Vector3> CalculatePath(Vector3 start, Vector3 target)
{
    Queue<Vector3> queue = new Queue<Vector3>();
    Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
    HashSet<Vector3> visited = new HashSet<Vector3>();

    queue.Enqueue(start);
    visited.Add(start);
    cameFrom[start] = start;

    while (queue.Count > 0)
    {
        Vector3 currentTile = queue.Dequeue();

        if (currentTile == target)
        {
            // Si hemos llegado al objetivo, reconstruimos el camino
            List<Vector3> path = ReconstructPath(cameFrom, start, target);
            path.RemoveAt(0);
            return path;
        }

        // Solo agregar tiles en direcciones ortogonales
        foreach (Vector3 direction in new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down })
        {
            Vector3 neighborTile = currentTile + direction;

            if (IsWithinMapBounds(neighborTile) && !visited.Contains(neighborTile) && !IsTileBlocked(neighborTile))
            {
                queue.Enqueue(neighborTile);
                visited.Add(neighborTile);
                cameFrom[neighborTile] = currentTile;
            }
        }
    }

    return null;
}

private TileHover GetTileAtPosition(Vector3 position)
{
    // Asegúrate de que la posición esté dentro de los límites del mapa
    if (IsWithinMapBounds(position))
    {
        TileHover tile = instantiatedTiles[position];
        return tile;
    }

    Debug.Log("Position is out of map bounds."); // Mensaje si la posición está fuera de los límites del mapa
    return null;
}

int CalculateMoveCost(int totalMoves)
    {
        return (int)(totalMoves); // Costo total de movimiento
    }

public void MoveToTile(Vector3 targetTilePosition)
{
    List<Vector3> path = CalculatePath(currentTrainerPosition, targetTilePosition);

    if (path == null || path.Count == 0)
    {
        Debug.LogError("No se pudo calcular un camino válido.");
        return;
    }

    // Determinar el costo del movimiento
    int totalMoves = path.Count; // Restamos 1 porque la posición inicial no cuenta como movimiento
    int moveCost = CalculateMoveCost(totalMoves);
    DestroyMoveableTiles();
    StartCoroutine(MoveAlongPath(path)); // Mueve a través del camino
}

private void DestroyMoveableTiles()
    {

        // Destruir todos los tiles instanciados
        foreach (var tile in instantiatedTiles.Values)
        {
            Destroy(tile.gameObject);
        }
        instantiatedTiles.Clear();
        moveableTiles.Clear(); // Limpiar la lista de tiles movibles
    }

private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 start, Vector3 target)
{
    List<Vector3> path = new List<Vector3>();
    Vector3 currentTile = target;

    while (currentTile != start)
    {
        path.Add(currentTile);
        currentTile = cameFrom[currentTile];
    }

    path.Add(start); // Agregamos el inicio
    path.Reverse(); // Invertimos la lista para que vaya del inicio al objetivo

    return path;
}

private IEnumerator MoveAlongPath(List<Vector3> path)
{
    for (int i = 0; i < path.Count; i++)
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = path[i];

        // Comprobar si el movimiento es diagonal antes de moverse
        if (IsDiagonalMove(currentPosition, nextPosition))
        {
            Debug.LogError("Intentando mover en diagonal, no permitido.");
            yield break;  // Detener el movimiento si es diagonal
        }

        // Movimiento del Pokémon hacia la siguiente posición en el camino
        while (Vector3.Distance(transform.position, nextPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = nextPosition;
        yield return new WaitForSeconds(0.1f);  // Pequeña pausa entre movimientos
    }
}

private bool IsDiagonalMove(Vector3 from, Vector3 to)
{
    float deltaX = Mathf.Abs(to.x - from.x);
    float deltaY = Mathf.Abs(to.y - from.y);

    // Solo permitir movimiento en línea recta (horizontal o vertical), no en diagonal
    return deltaX > 0 && deltaY > 0;
}

public void TileExit(Vector3 tile)
    {
        ClearPathBlue();
    }

    // Method to use a power
    public void UsePower(string powerName)
    {
        switch (powerName)
        {
            case "Running Shoes":
                ActivateRunningShoes();
                break;
            case "Pokéball Throw":
                ThrowPokeball();
                break;
            case "Fishing":
                Fish();
                break;
            case "Item Use":
                UseItem();
                break;
            case "Escape Rope":
                EscapeRope();
                break;
            // Add other powers here
        }

    }
    

    // Example power: Running Shoes
    void ActivateRunningShoes()
    {
        movementRange += 2; // Temporarily increase movement range
        Debug.Log($"{trainerName} activated Running Shoes! Movement range increased.");
    }

    // Example power: Pokéball Throw
    void ThrowPokeball()
    {
        Debug.Log($"{trainerName} threw a Pokéball!");
        // Logic for catching or failing to catch a Pokémon
    }

    // Example power: Fishing
    void Fish()
    {
        Debug.Log($"{trainerName} is fishing...");
        // Logic for summoning a wild Pokémon
    }

    // Example power: Item Use
    void UseItem()
    {
        Debug.Log($"{trainerName} used an item!");
        // Logic for using an item on a Pokémon
    }

    // Example power: Item Use
    void EscapeRope()
    {
        Debug.Log($"{trainerName} used an Escape Rope!");
        // Logic for using an item on a Pokémon
    }

    [System.Serializable]
    public struct Stats
    {
        public int level;
        public int currentExperience;

        public int maxHP;
        public int hp;
        public int atk;
        public int def;
        public int spAtk;
        public int spDef;
        public int spd;

        public int hpIV;
        public int atkIV;
        public int defIV;
        public int spAtkIV;
        public int spDefIV;
        public int spdIV;

        public int hpEV;
        public int atkEV;
        public int defEV;
        public int spAtkEV;
        public int spDefEV;
        public int spdEV;

        public int total
        {
            get
            {
                return hp + atk + def + spAtk + spDef + spd;
            }
        }
    }
}
