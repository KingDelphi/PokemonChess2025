using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMovement : MonoBehaviour
{
    public GameObject pokemon; // Referencia al Pokémon (Eevee)
    public float moveSpeed = 1f; // Velocidad de movimiento
    public int actionPoints; // Puntos de acción
    public float weight; // Peso del Pokémon
    private List<Vector3> moveableTiles = new List<Vector3>(); // Tiles a los que se puede mover
    private MapGenerator mapGenerator; // Referencia al MapGenerator

    public float tileSize = 1f; // Tamaño del tile
    public int mapWidth = 7; // Ancho del mapa
    public int mapHeight = 7; // Alto del mapa
    public GameObject[,] tileMap; // Suponiendo que es un arreglo 2D de tiles

    public Material grayMaterial;
    public Material originalMaterial;
    public GameObject tilePrefab;
    List<GameObject> instantiatedTiles = new List<GameObject>(); // Lista para almacenar los tiles instanciados
    private bool tilesHighlighted = false;
    private bool tilesAreActive = false; // Estado inicial, sin tiles activos

    private Vector3 initialPosition;
    private GameObject initialPositionTile; // Tile instanciado en la posición inicial de Eevee
    private Vector3 currentPokemonPosition;
    private bool areTilesVisible = false; // Para rastrear la visibilidad de los tiles
    private Vector3 lastClickedPosition;

    public bool isMoving;

    void Start()
    {
        instantiatedTiles = new List<GameObject>(); // Inicializa la lista
        StartCoroutine(InitializePokemonMovement());
        originalMaterial = tilePrefab.GetComponent<Renderer>().sharedMaterial; // Cambiado a sharedMaterial
    }

    private IEnumerator InitializePokemonMovement()
    {
        actionPoints = GetComponent<PokemonBase>().stats.spd; // Asumiendo que SPD está en el script PokemonBase
        mapGenerator = FindObjectOfType<MapGenerator>(); // Encuentra el MapGenerator en la escena

        // Verifica si mapGenerator se ha inicializado correctamente
        if (mapGenerator == null)
        {
            Debug.LogError("MapGenerator no encontrado en la escena. Asegúrate de que haya un objeto MapGenerator.");
            yield break; // Sale de la coroutine si mapGenerator es null
        }

        // Espera un milisegundo
        yield return new WaitForSeconds(0.001f);

        tileMap = mapGenerator.tileMap; // Inicializa tileMap

        // Verifica que tileMap esté correctamente inicializado
        if (tileMap == null || tileMap.GetLength(0) == 0 || tileMap.GetLength(1) == 0)
        {
            Debug.LogError("tileMap no está correctamente inicializado en MapGenerator.");
            yield break; // Sale de la coroutine si tileMap no es válido
        }

        grayMaterial = new Material(Shader.Find("Unlit/Color"));
        grayMaterial.color = new Color(0.6f, 0.6f, 0.6f); // Gris claro

        // Inicializa los tiles en la escena y agrégales a la lista
        foreach (var tile in instantiatedTiles)
        {
            Renderer tileRenderer = tile.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                // Cambia el material a su material original al iniciar
                tileRenderer.sharedMaterial = originalMaterial;
            }
        }
    }

    void Update()
{
    // Detecta el click del mouse
    if (Input.GetMouseButtonDown(0)) 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Asegura que Z sea 0

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            // Verifica si se ha hecho clic en el Pokémon
            if (hit.collider.CompareTag("pokemon")) 
            {
                OnPokemonClick();
            }
            // Verifica si se ha hecho clic en un tile
            else if (hit.collider.CompareTag("tile")) 
            {
                Vector3 targetTile = hit.collider.transform.position;

                // Mover al Pokémon si el tile está en moveableTiles
                if (moveableTiles.Contains(targetTile)) 
                {
                    // Determinar el número de movimientos (en casillas) requeridos
                    Vector3 initialPosition = pokemon.transform.position; // Posición inicial del Pokémon
                    int horizontalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.x) - Mathf.FloorToInt(initialPosition.x));
                    int verticalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.y) - Mathf.FloorToInt(initialPosition.y));
                    int totalMoves = horizontalMoves + verticalMoves; // Total de movimientos

                    int moveCost = CalculateMoveCost(totalMoves, weight); // Calcular el costo del movimiento
                    if (moveCost <= actionPoints) // Verificar si hay suficientes puntos de acción
                    {
                        MoveToTile(targetTile); // Mover al Pokémon
                        //actionPoints -= moveCost; // Resta el costo de movimiento de los puntos de acción
                    }
                    else
                    {
                        Debug.Log("No hay suficientes puntos de acción para mover.");
                    }
                }
            }
        }
    }

    // Lógica para mostrar la trayectoria al hacer hover sobre un tile
    if (Input.GetMouseButton(0))
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Asegura que Z sea 0 para hover

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            // Verifica si se ha hecho clic en un tile movible
            if (hit.collider.CompareTag("tile") && moveableTiles.Contains(hit.collider.transform.position))
            {
                ShowTrajectory(hit.collider.transform.position); // Mostrar la trayectoria hacia el tile
            }
        }
    }
}




    private void MoveToTile(Vector3 targetTile)
{
    initialPosition = pokemon.transform.position;

    // Determinar el número de movimientos
    int horizontalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.x) - Mathf.FloorToInt(initialPosition.x));
    int verticalMoves = Mathf.Abs(Mathf.FloorToInt(targetTile.y) - Mathf.FloorToInt(initialPosition.y));
    int totalMoves = horizontalMoves + verticalMoves;

    // Calcula el costo total de movimiento
    int moveCost = CalculateMoveCost(totalMoves, weight);

    // Almacena los puntos de acción iniciales
    int initialActionPoints = actionPoints;

    Debug.Log($"Total Moves: {totalMoves}, Move Cost: {moveCost}, Initial Action Points: {initialActionPoints}, Action Points Available: {actionPoints}");

    // Verifica si hay suficientes puntos de acción para mover
    if (actionPoints >= moveCost)
    {
        DestroyMoveableTiles();
        StartCoroutine(MovePokemon(initialPosition, targetTile, moveCost));
    }
    else
    {
        Debug.Log("No hay suficientes puntos de acción para mover.");
    }
}





// Coroutine para mover al Pokémon
private IEnumerator MovePokemon(Vector3 initialPosition, Vector3 targetTile, int moveCost)
{
    // Verifica si la coroutine ya está en ejecución
    if (isMoving) {
    Debug.Log("Ya se está moviendo, no se puede realizar otro movimiento.");
        yield break; // Sale si ya está en movimiento
    }
    
    isMoving = true; // Marca como en movimiento
    
    // Mover horizontalmente primero
    if (targetTile.x != pokemon.transform.position.x)
    {
        Vector3 horizontalTarget = new Vector3(targetTile.x, pokemon.transform.position.y, pokemon.transform.position.z);
        while (Vector3.Distance(pokemon.transform.position, horizontalTarget) > 0.1f)
        {
            pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, horizontalTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Ahora mover verticalmente
    if (targetTile.y != pokemon.transform.position.y)
    {
        Vector3 verticalTarget = new Vector3(pokemon.transform.position.x, targetTile.y, pokemon.transform.position.z);
        while (Vector3.Distance(pokemon.transform.position, verticalTarget) > 0.1f)
        {
            pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, verticalTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    pokemon.transform.position = targetTile; // Asegura que el Pokémon esté exactamente en el tile

Debug.Log($"Reducción de Action Points antes: {actionPoints}");
    actionPoints -= moveCost;
Debug.Log($"Reducción de Action Points después: {actionPoints}");

    Debug.Log($"Movimiento realizado. Action Points después del movimiento: {actionPoints}");
    isMoving = false;
}

private IEnumerator MoveHorizontally(Vector3 targetTile)
{
    // Mover en la dirección horizontal
    Vector3 horizontalTarget = new Vector3(targetTile.x, pokemon.transform.position.y, pokemon.transform.position.z);

    while (Vector3.Distance(pokemon.transform.position, horizontalTarget) > 0.1f)
    {
        pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, horizontalTarget, moveSpeed * Time.deltaTime);
        yield return null;
    }

    // Ahora mover en la dirección vertical
    while (Vector3.Distance(pokemon.transform.position, targetTile) > 0.1f)
    {
        pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, targetTile, moveSpeed * Time.deltaTime);
        yield return null;
    }

    pokemon.transform.position = targetTile; // Asegura que el Pokémon esté exactamente en el tile
}

private IEnumerator MoveVertically(Vector3 targetTile)
{
    // Mover en la dirección vertical
    Vector3 verticalTarget = new Vector3(pokemon.transform.position.x, targetTile.y, pokemon.transform.position.z);

    while (Vector3.Distance(pokemon.transform.position, verticalTarget) > 0.1f)
    {
        pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, verticalTarget, moveSpeed * Time.deltaTime);
        yield return null;
    }

    // Ahora mover en la dirección horizontal
    while (Vector3.Distance(pokemon.transform.position, targetTile) > 0.1f)
    {
        pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, targetTile, moveSpeed * Time.deltaTime);
        yield return null;
    }

    pokemon.transform.position = targetTile; // Asegura que el Pokémon esté exactamente en el tile
}


    System.Collections.IEnumerator MoveTowards(Vector3 targetTile)
    {
        while (Vector3.Distance(pokemon.transform.position, targetTile) > 0.1f)
        {
            pokemon.transform.position = Vector3.MoveTowards(pokemon.transform.position, targetTile, moveSpeed * Time.deltaTime);
            yield return null;
        }
        pokemon.transform.position = targetTile; // Asegura que el Pokémon esté exactamente en el tile
    }

    // Método para calcular el costo de movimiento
    int CalculateMoveCost(int totalMoves, float weight)
    {
        return (int)(totalMoves * (5 * weight)); // Costo total de movimiento
    }



    void ShowTrajectory(Vector3 targetTile)
    {
        // Dibuja una línea desde el Pokémon hasta el tile objetivo
        Debug.DrawLine(pokemon.transform.position, targetTile, Color.red, 1.0f); // Dibuja una línea roja
    }

    // Lógica para calcular los tiles a los que se puede mover usando BFS
private void CalculateMoveableTiles()
{
    moveableTiles.Clear(); // Limpiar la lista de tiles movibles
    instantiatedTiles.Clear(); // Limpiar la lista de tiles instanciados

    Queue<Vector3> queue = new Queue<Vector3>(); // Cola para BFS
    HashSet<Vector3> visited = new HashSet<Vector3>(); // Para rastrear los tiles visitados

    // Agregar la posición inicial a la cola y a la lista de visitados
    queue.Enqueue(currentPokemonPosition);
    visited.Add(currentPokemonPosition);

    // Calcular el costo de movimiento para una casilla
    int moveCostPerTile = (int)CalculateMoveCost(1, weight); // Aquí puedes mantenerlo así
    int maxMoves = actionPoints / moveCostPerTile; // Calcular el rango máximo basado en puntos de acción

    int movesMade = 0;

    while (queue.Count > 0 && movesMade < maxMoves)
    {
        int tilesInCurrentLevel = queue.Count; // Número de tiles en el nivel actual

        for (int i = 0; i < tilesInCurrentLevel; i++)
        {
            Vector3 currentTile = queue.Dequeue();

            // Intenta agregar cada dirección (izquierda, derecha, arriba, abajo)
            foreach (Vector3 direction in new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down })
            {
                Vector3 neighborTile = currentTile + direction;

                // Verifica si la posición está dentro de los límites del mapa
                if (IsWithinMapBounds(neighborTile) && !visited.Contains(neighborTile))
                {
                    // Verifica si hay un objeto bloqueando el tile
                    if (!IsTileBlocked(neighborTile))
                    {
                        // Calcula el costo de movimiento para este tile
                        int moveCost = CalculateMoveCost(1, weight); // Usar 1 para indicar el costo de mover a un solo tile

                        // Verifica si hay suficientes puntos de acción
                        if (moveCost <= actionPoints)
                        {
                            visited.Add(neighborTile);
                            queue.Enqueue(neighborTile);

                            // Instancia el tile y agrégalo a la lista de tiles instanciados
                            GameObject tile = Instantiate(tilePrefab, neighborTile, Quaternion.identity);
                            instantiatedTiles.Add(tile);
                            moveableTiles.Add(neighborTile); // Añadir a la lista de tiles movibles
                        }
                    }
                    else
                    {
                        // Si hay un objeto, no instanciar ni agregar el tile
                    }
                }
            }
        }

        movesMade++; // Aumenta el número de movimientos realizados
    }
}



// Método para verificar si el tile está bloqueado por un objeto
private bool IsTileBlocked(Vector3 position)
{
    // Utiliza un área pequeña alrededor de la posición para verificar colisiones
    Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(tileSize, tileSize), 0);
    foreach (Collider2D collider in colliders)
    {
        if (collider.CompareTag("object")) // Cambiar el tag según corresponda
        {
            return true; // Hay un objeto bloqueando el tile
        }
    }
    return false; // No hay objetos bloqueando
}



// Método para verificar si la posición está dentro de los límites del mapa
private bool IsWithinMapBounds(Vector3 position)
{
    // Convertir la posición a enteros para acceder al índice del arreglo
    int xIndex = Mathf.FloorToInt(position.x);
    int yIndex = Mathf.FloorToInt(position.y);

    // Verificar si está dentro de los límites del mapa
    return xIndex >= 0 && xIndex < mapWidth && yIndex >= 0 && yIndex < mapHeight;
}


    // Nuevo método para verificar si el tile está dentro de los límites
    private bool IsTileInBounds(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / tileSize);
        int y = Mathf.FloorToInt(position.y / tileSize);

        // Verifica que los índices estén dentro de los límites del mapa
        return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
    }


    private void DestroyMoveableTiles()
    {

        // Destruir todos los tiles instanciados
        foreach (GameObject tile in instantiatedTiles)
        {
            Destroy(tile);
        }
        instantiatedTiles.Clear();
        moveableTiles.Clear(); // Limpiar la lista de tiles movibles
    }

    private void OnPokemonClick()
{
    currentPokemonPosition = transform.position;

    // Comprobar si se hizo clic en la misma posición
    if (areTilesVisible && lastClickedPosition == currentPokemonPosition)
    {
        // Si los tiles ya están visibles y se hizo clic en la misma posición, los destruimos
        DestroyMoveableTiles();
        areTilesVisible = false; // Actualiza el estado de visibilidad
    }
    else
    {
        // Verifica si hay puntos de acción antes de calcular los tiles movibles
        if (actionPoints > 0)
        {
            // Obtén el peso del Pokémon en el que se hizo clic
            PokemonBase clickedPokemon = GetComponent<PokemonBase>();
            if (clickedPokemon != null)
            {
                weight = clickedPokemon.weight; // Asigna el peso a la variable de clase
                CalculateMoveableTiles();
                areTilesVisible = true; // Actualiza el estado de visibilidad
                lastClickedPosition = currentPokemonPosition; // Actualiza la última posición clicada
            }
            else
            {
                Debug.LogError("No se pudo obtener el peso del Pokémon.");
            }
        }
        else
        {
            Debug.Log("No hay puntos de acción disponibles para mover.");
        }
    }
}



    // Métodos auxiliares omitidos por brevedad...
}
