using UnityEngine;

public class TileHover : MonoBehaviour
{
    public Material grayMaterial;   // Material gris original
    public Material blueMaterial;   // Material azul
    private Renderer tileRenderer;   // Referencia al Renderer del tile

    void Start()
    {
        tileRenderer = GetComponent<Renderer>(); // Obtener el Renderer del tile
        tileRenderer.material = grayMaterial; // Inicializar con el material gris
    }

    private void OnMouseEnter() // Método que se llama cuando el mouse entra en el collider del tile
    {
        PokemonMovement.currentPokemon.TileEnter(transform.position);
    }

    private void OnMouseExit() // Método que se llama cuando el mouse sale del collider del tile
    {
        PokemonMovement.currentPokemon.TileExit(transform.position);
    }

    public void ChangeColorToBlue()
    {
        tileRenderer.material = blueMaterial;
    }

    public void ChangeColorToGray()
    {
        tileRenderer.material = grayMaterial;
    }
}