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
    public Material blueMaterial; // Nuevo material azul

    public Material originalMaterial;
    public GameObject tilePrefab;
    Dictionary<Vector3, TileHover> instantiatedTiles = new Dictionary<Vector3, TileHover>(); // Lista para almacenar los tiles instanciados
    private Vector3 initialPosition;
    private Vector3 currentPokemonPosition;
    private bool areTilesVisible = false; // Para rastrear la visibilidad de los tiles
    private Vector3 lastClickedPosition;
    private Vector3? lastHoveredTile = null;

    public static PokemonMovement currentPokemon;


    public bool isMoving;

    void Start()
    {
        instantiatedTiles = new Dictionary<Vector3, TileHover>(); // Inicializa la lista
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
        foreach (var tile in instantiatedTiles.Values)
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
        // Detecta la posición del mouse en cada frame
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0; // Asegura que Z sea 0
    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

    // // Verifica si el mouse está sobre un tile
    // if (hit.collider != null && hit.collider.CompareTag("tile") && !hit.collider.CompareTag("pokemon"))
    // {
    //     Vector3 targetTile = hit.collider.transform.position;

    //     // Si el tile es diferente al último tile hovereado
    //     if (lastHoveredTile == null || targetTile != lastHoveredTile)
    //     {
    //         // Limpiar la trayectoria anterior si existía un último tile hovereado
    //         if (lastHoveredTile != null)
    //         {
    //             ClearPathBlue(); // Función que limpia la trayectoria pintada
    //         }

    //         // Si el tile actual está en moveableTiles, calcular y pintar el camino
    //         if (moveableTiles.Contains(targetTile))
    //         {
    //             Vector3 initialPosition = pokemon.transform.position; // Posición inicial del Pokémon
                
    //             // Calcula el camino desde la posición inicial al tile objetivo
    //             List<Vector3> path = CalculatePath(initialPosition, targetTile); // Obtiene el camino
                
    //             // Verifica si se encontró un camino antes de pintar
    //             if (path != null && path.Count > 0)
    //             {
    //                 PaintPathBlue(path); // Pinta la nueva trayectoria en azul
    //             }
    //             lastHoveredTile = targetTile; // Actualiza el último tile hovereado
    //         }
    //     }
    // }
    // else
    // {
    //     // Si el mouse no está sobre ningún tile, limpiar la trayectoria anterior
    //     if (lastHoveredTile != null)
    //     {
    //         ClearPathBlue(); // Limpiar la trayectoria cuando el mouse sale de los tiles
    //         lastHoveredTile = null;
    //     }
    // }



        // Detecta el click del mouse
        if (Input.GetMouseButtonDown(0)) 
        {
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
                        }
                        else
                        {
                            Debug.Log("No hay suficientes puntos de acción para mover.");
                        }
                    }
                }
            }
        }
    }

    public void TileEnter(Vector3 tile)
    {
        ClearPathBlue();
        List<Vector3> path = CalculatePath(currentPokemonPosition, tile);
        foreach (Vector3 t in path)
        {
            TileHover tileHover = GetTileAtPosition(t);
            tileHover.ChangeColorToBlue();
        }
    }

    public void TileExit(Vector3 tile)
    {
        ClearPathBlue();
    }




// Función para limpiar los tiles pintados de azul
void ClearPathBlue()
{
    // Recorre todos los tiles y restablece el material original en los tiles pintados
    foreach (Vector3 tilePos in moveableTiles)
    {
        TileHover tile = GetTileAtPosition(tilePos); 
        tile.ChangeColorToGray(); // Cambia al material por defecto
    }
}


// Función para pintar el camino en azul
// void PaintPathBlue(List<Vector3> path)
// {
//     // Verifica si la lista de path no está vacía
//     if (path == null || path.Count == 0)
//     {
//         Debug.Log("Path is null or empty, nothing to paint.");
//         return;
//     }

//     Debug.Log($"Painting path with {path.Count} tiles."); // Muestra cuántos tiles se van a pintar

//     // Guarda todos los tiles que se van a pintar
//     List<GameObject> paintedTiles = new List<GameObject>();

//     // Pinta cada tile en azul según el camino calculado
//     foreach (Vector3 tile in path)
//     {
//         // Obtiene el objeto tile y cambia su material a azul
//         GameObject tileObject = GetTileAtPosition(tile).gameObject;
        
//         if (tileObject != null)
//         {
//             Debug.Log($"Painting tile at position {tile} to blue."); // Mensaje de depuración para cada tile pintado
//             paintedTiles.Add(tileObject); // Agrega el tile pintado a la lista
//             tileObject.GetComponent<Renderer>().material = blueMaterial; // Cambia el material a azul
//         }
//         else
//         {
//             Debug.Log($"No tile found at position {tile}."); // Mensaje si no se encuentra el objeto tile
//         }
//     }
// }



void ResetTilesToOriginal()
{
    // Aquí puedes recorrer tu tileMap y restablecer todos los tiles a su material original
    for (int x = 0; x < tileMap.GetLength(0); x++)
    {
        for (int y = 0; y < tileMap.GetLength(1); y++)
        {
            GameObject tileObject = tileMap[x, y];
            if (tileObject != null)
            {
                Renderer tileRenderer = tileObject.GetComponent<Renderer>();
                if (tileRenderer != null)
                {
                    tileRenderer.material = originalMaterial; // Restablece al material original
                }
            }
        }
    }
}



// Resetea los materiales de los tiles a su color original
private void ResetTrajectory()
{
    foreach (var tile in instantiatedTiles.Values)
    {
        Renderer tileRenderer = tile.GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            // Cambiar el material de los tiles de vuelta a su estado original
            tileRenderer.sharedMaterial = originalMaterial;
        }
    }
}





    public void MoveToTile(Vector3 targetTilePosition)
{
    List<Vector3> path = CalculatePath(currentPokemonPosition, targetTilePosition);

    if (path == null || path.Count == 0)
    {
        Debug.LogError("No se pudo calcular un camino válido.");
        return;
    }

    // Determinar el costo del movimiento
    int totalMoves = path.Count; // Restamos 1 porque la posición inicial no cuenta como movimiento
    int moveCost = CalculateMoveCost(totalMoves, weight);

    if (actionPoints >= moveCost)
    {
        DestroyMoveableTiles();
        StartCoroutine(MoveAlongPath(path)); // Mueve a través del camino
        actionPoints -= moveCost;
    }
    else
    {
        Debug.Log("No hay suficientes puntos de acción para mover.");
    }
}


// Método para mover al Pokémon a lo largo del camino completo
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

private bool IsDiagonalMove(Vector3 from, Vector3 to)
{
    float deltaX = Mathf.Abs(to.x - from.x);
    float deltaY = Mathf.Abs(to.y - from.y);

    // Solo permitir movimiento en línea recta (horizontal o vertical), no en diagonal
    return deltaX > 0 && deltaY > 0;
}







// Método para reconstruir el camino desde el objetivo hasta el inicio
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




    // Método para calcular el costo de movimiento
    int CalculateMoveCost(int totalMoves, float weight)
    {
        return (int)(totalMoves * (5 * weight)); // Costo total de movimiento
    }

    // Función para obtener un tile basado en su posición en el mapa
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

    // Lógica para calcular los tiles a los que se puede mover usando BFS
private void CalculateMoveableTiles()
{
    moveableTiles.Clear();
    instantiatedTiles.Clear();

    Queue<Vector3> queue = new Queue<Vector3>();
    HashSet<Vector3> visited = new HashSet<Vector3>();

    queue.Enqueue(currentPokemonPosition);
    visited.Add(currentPokemonPosition);

    int moveCostPerTile = CalculateMoveCost(1, weight);
    int maxMoves = actionPoints / moveCostPerTile;

    int movesMade = 0;

    while (queue.Count > 0 && movesMade < maxMoves)
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

    private void OnPokemonClick()
{
    currentPokemon = this;
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
}