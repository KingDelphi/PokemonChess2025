using UnityEngine;

public class TileAttack : MonoBehaviour
{
    public Material redMaterial;
    public Material orangeMaterial;
    private Renderer tileRenderer;   // Referencia al Renderer del tile

    void Start()
{
    tileRenderer = GetComponent<Renderer>(); // Obtener el Renderer del tile
    if (tileRenderer == null)
    {
        Debug.LogError("TileRenderer is null in TileHover!");
        return; // Salir si el renderer no está disponible
    }
    tileRenderer.material = orangeMaterial; // Inicializar con el material gris
}

    private void OnMouseEnter() // Método que se llama cuando el mouse entra en el collider del tile
    {
        ChangeColorToRed();
    }

    private void OnMouseExit() // Método que se llama cuando el mouse sale del collider del tile
    {
        ChangeColorToOrange();
    }

    public void ChangeColorToOrange()
    {
        tileRenderer.material = orangeMaterial;
    }

    public void ChangeColorToRed()
    {
        tileRenderer.material = redMaterial;
    }
}