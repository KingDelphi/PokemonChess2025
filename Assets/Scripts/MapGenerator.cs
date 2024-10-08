using UnityEngine;

public class MapGenerator : MonoBehaviour
{    
    public GameObject tilePrefab; // Prefab del tile
    public GameObject oldBookcasePrefab; // Prefab del Old Bookcase
    public GameObject tableWithComputerPrefab; // Prefab de la Table with Computer
    public GameObject chairPrefab; // Prefab de la Chair
    public GameObject longTablePrefab; // Prefab de la Long Table
    public GameObject tapestryPrefab; // Prefab de la Tapestry
    public GameObject bookcasePrefab; // Prefab del Bookcase
    public GameObject mainTablePrefab; // Prefab de la Table 46x35
    public GameObject pokeMachinePrefab; // Prefab de la PokeMachine
    public GameObject wallPrefab; // Prefab de las paredes

    public Material backgroundMaterial; // Material del background
    public GameObject backgroundPrefab; // Prefab del objeto Background (debe tener un Sprite Renderer)


    public int mapWidth = 7; // Ajusta el ancho del mapa
    public int mapHeight = 7; // Ajusta la altura del mapa
    public float tileSize = 1f; // Tamaño del tile ajustado a la unidad del tile

    private Camera mainCamera;
    private bool showGrid = true; // Controla la visibilidad de la cuadrícula
    private Material grayMaterial; // Material gris
    public GameObject[,] tileMap; // Array para almacenar los tiles generados
    private Material[,] originalMaterials; // Array para almacenar los materiales originales de los tiles

    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    public GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("No se encontró un GameManager en la escena.");
        }

        
        mainCamera = Camera.main;
        ConfigureCamera();
        CreateGrayMaterial(); // Crear el material gris
        GenerateBackground(); // Llamar a la función para generar el fondo
        GenerateMap();
        GenerateThinBorder(); // Llamar a la función para generar el borde delgado
        AddBackgroundMusic();
        showGrid = true;
    }

    void Update()
    {
        // Detecta si se presiona la tecla "G"
        if (Input.GetKeyDown(KeyCode.G))
        {
            showGrid = !showGrid; // Alterna la visibilidad de la cuadrícula
            //ToggleTileColor(); // Alterna entre el material original y el gris
        }
    }

    // Nueva función para configurar la música de fondo
    void AddBackgroundMusic()
    {
        // Añade el componente AudioSource al objeto que tiene este script
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // Para que la música se repita en bucle
        audioSource.playOnAwake = true; // Que empiece a sonar al iniciar el juego
        audioSource.volume = 0.5f; // Ajusta el volumen a tu preferencia
        audioSource.Play(); // Inicia la reproducción
    }

    void GenerateBackground()
    {
        GameObject background = new GameObject("Background");
        
        // Calcular la escala necesaria
        float scaleX = mapWidth/10f * 2.5f; // Ajustar en base a Pixels Per Unit
        float scaleY = mapHeight/10f * 2.5f; // Ajustar en base a Pixels Per Unit

        background.transform.localScale = new Vector3(scaleX, scaleY, 1); // Escala para cubrir todo el mapa

        SpriteRenderer spriteRenderer = background.AddComponent<SpriteRenderer>();

        // Asigna el material del fondo
        spriteRenderer.material = backgroundMaterial;

        // Asegúrate de que la textura del material esté asignada y conviértela en un sprite
        if (backgroundMaterial.mainTexture is Texture2D texture)
        {
            // Crear un sprite a partir de la textura
            spriteRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        // Posiciona el fondo en el centro
        background.transform.position = new Vector3(mapWidth / 2f, mapHeight / 2f, 10); // z = 10.1; Asegúrate de que esté detrás del mapa
    }

    void GenerateThinBorder()
    {
        float borderThickness = 0.1f; // Grosor del borde (puedes ajustarlo según lo necesites)

        // Borde superior
        PlaceThinBorder(new Vector3(mapWidth / 2f, mapHeight + borderThickness / 2f, 0), new Vector3(mapWidth, borderThickness, 1));
        
        // Borde inferior
        PlaceThinBorder(new Vector3(mapWidth / 2f, -borderThickness / 2f, 0), new Vector3(mapWidth, borderThickness, 1));

        // Borde izquierdo
        PlaceThinBorder(new Vector3(-borderThickness / 2f, mapHeight / 2f, 0), new Vector3(borderThickness, mapHeight, 1));

        // Borde derecho
        PlaceThinBorder(new Vector3(mapWidth + borderThickness / 2f, mapHeight / 2f, 0), new Vector3(borderThickness, mapHeight, 1));

        // Esquinas:
        float cornerSize = borderThickness; // Tamaño del cuadrado de la esquina

        // Esquina superior izquierda
        PlaceThinBorder(new Vector3(-borderThickness / 2f, mapHeight + borderThickness / 2f, 0), new Vector3(cornerSize, cornerSize, 1));
        
        // Esquina superior derecha
        PlaceThinBorder(new Vector3(mapWidth + borderThickness / 2f, mapHeight + borderThickness / 2f, 0), new Vector3(cornerSize, cornerSize, 1));
        
        // Esquina inferior izquierda
        PlaceThinBorder(new Vector3(-borderThickness / 2f, -borderThickness / 2f, 0), new Vector3(cornerSize, cornerSize, 1));
        
        // Esquina inferior derecha
        PlaceThinBorder(new Vector3(mapWidth + borderThickness / 2f, -borderThickness / 2f, 0), new Vector3(cornerSize, cornerSize, 1));
    }

    void PlaceThinBorder(Vector3 position, Vector3 scale)
    {
        if (wallPrefab != null)
        {
            GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
            wall.transform.parent = this.transform;
            wall.transform.localScale = scale; // Ajusta el tamaño del borde
            wall.GetComponent<Renderer>().material.color = new Color(0f, 1f, 1f); // Cambia el color a celeste
        }
    }

    // Crea un material gris en tiempo real
    void CreateGrayMaterial()
    {
        grayMaterial = new Material(Shader.Find("Unlit/Color")); // Usar un shader sin iluminación
        grayMaterial.color = new Color(0.6f, 0.6f, 0.6f); // Gris claro
    }

    void ConfigureCamera()
    {
        mainCamera.transform.position = new Vector3(mapWidth / 2f - 0.5f + 0.5f, mapHeight / 2f - 0.5f + 0.5f, -10);
        mainCamera.orthographicSize = mapHeight / 1.5f;
    }

    void GenerateMap()
    {
        tileMap = new GameObject[mapWidth, mapHeight]; // Inicializa el array de tiles
        originalMaterials = new Material[mapWidth, mapHeight]; // Inicializa el array de materiales originales

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Genera los tiles del fondo
                GameObject tile = PlaceTile(tilePrefab, new Vector3(x * tileSize + 0.5f, y * tileSize + 0.5f, 0), true);
                tile.tag = "walkable"; // Asigna el tag "walkable" a los tiles
                tileMap[x, y] = tile; // Almacena el tile en el array
                originalMaterials[x, y] = tile.GetComponent<Renderer>().material; // Almacena el material original
            }
        }

        // Coloca los objetos (sin tag "walkable")
        PlaceObject(oldBookcasePrefab, new Vector3(0 * tileSize + 0.5f, 6 * tileSize + 0.5f, 0), false);
        PlaceObject(tableWithComputerPrefab, new Vector3(1 * tileSize + 0.5f, 6 * tileSize + 0.5f, 0), false);
        PlaceObject(chairPrefab, new Vector3(1 * tileSize + 0.5f, 5 * tileSize + 0.5f, 0), true); // chair is walkable
        PlaceObject(longTablePrefab, new Vector3(3 * tileSize + 0.5f, 6 * tileSize + 0.5f, 0), false);
        PlaceObject(tapestryPrefab, new Vector3(3 * tileSize + 0.5f, 0 * tileSize + 0.5f, 0), true); // tapestry is walkable
        PlaceObject(bookcasePrefab, new Vector3(0 * tileSize + 0.5f, 2 * tileSize + 0.5f, 0), false);
        PlaceObject(bookcasePrefab, new Vector3(1 * tileSize + 0.5f, 2 * tileSize + 0.5f, 0), false);
        PlaceObject(bookcasePrefab, new Vector3(5 * tileSize + 0.5f, 2 * tileSize + 0.5f, 0), false);
        PlaceObject(bookcasePrefab, new Vector3(6 * tileSize + 0.5f, 2 * tileSize + 0.5f, 0), false);
        PlaceObject(mainTablePrefab, new Vector3(5 * tileSize + 0.5f, 5 * tileSize + 0.5f, 0), false);
        PlaceObject(pokeMachinePrefab, new Vector3(3 * tileSize + 0.5f, 3 * tileSize + 0.5f, 0), false);
    }

    GameObject PlaceTile(GameObject prefab, Vector3 position, bool isTile = true)
    {
        if (prefab != null)
        {
            float zPosition = isTile ? 1f : 0f; 
            Vector3 newPosition = new Vector3(position.x, position.y, zPosition);
            GameObject newTile = Instantiate(prefab, newPosition, Quaternion.identity);
            newTile.transform.parent = this.transform;
            return newTile;
        }
        return null;
    }

    void PlaceObject(GameObject prefab, Vector3 position, bool isWalkable)
    {
        GameObject obj = PlaceTile(prefab, position, false);
        if (obj != null)
        {
            // Encuentra el tile en la posición donde se colocó el objeto
            // Usa la posición original para calcular el índice, sin ajustar
            int x = Mathf.FloorToInt(position.x / tileSize);
            int y = Mathf.FloorToInt(position.y / tileSize);

            // Cambia el tag del tile según si es walkable o no
            if (tileMap[x, y] != null)
            {
                tileMap[x, y].tag = isWalkable ? "walkable" : "object"; // Cambia el tag del tile
            }
        }
    }

    void ToggleTileColor()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject tile = tileMap[x, y];
                if (tile != null)
                {
                    Renderer renderer = tile.GetComponent<Renderer>();

                    // Comprueba si el tile tiene un objeto encima
                    if (tile.CompareTag("object"))
                    {
                        continue; // Si el tile tiene un objeto, no cambiar el color
                    }

                    // Si no tiene un objeto, alternar el color
                    if (showGrid)
                    {
                        renderer.material = grayMaterial; // Asigna el material gris
                    }
                    else
                    {
                        renderer.material = originalMaterials[x, y]; // Vuelve al material original
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showGrid)
        {
            Gizmos.color = Color.white; // Color de la cuadrícula
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Dibuja un cuadro para cada celda
                    Gizmos.DrawWireCube(new Vector3(x * tileSize + tileSize / 2, y * tileSize + tileSize / 2, 0), new Vector3(tileSize, tileSize, 0));
                }
            }
        }
    }
}