using UnityEngine;

public class TileHover : MonoBehaviour
{
    public Material grayMaterial;   // Material gris original
    public Material blueMaterial;   // Material azul
    private Renderer tileRenderer;   // Referencia al Renderer del tile

    void Start()
{
    tileRenderer = GetComponent<Renderer>(); // Obtener el Renderer del tile
    if (tileRenderer == null)
    {
        Debug.LogError("TileRenderer is null in TileHover!");
        return; // Salir si el renderer no está disponible
    }
    tileRenderer.material = grayMaterial; // Inicializar con el material gris
}

    private void OnMouseEnter() // Método que se llama cuando el mouse entra en el collider del tile
    {
        if (PokemonMovement.currentPokemon != null) // Si hay un Pokémon seleccionado
        {
            PokemonMovement.currentPokemon.TileEnter(transform.position);
        }
        else if (TrainerBase.currentTrainer != null) // Si hay un Trainer seleccionado
        {
            TrainerBase.currentTrainer.TileEnter(transform.position);
        }
    }


    private void OnMouseExit() // Método que se llama cuando el mouse sale del collider del tile
    {
        if (PokemonMovement.currentPokemon != null) // Si hay un Pokémon seleccionado
        {
            PokemonMovement.currentPokemon.TileExit(transform.position);
        }
        else if (TrainerBase.currentTrainer != null) // Si hay un Trainer seleccionado
        {
            TrainerBase.currentTrainer.TileExit(transform.position);
        }
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