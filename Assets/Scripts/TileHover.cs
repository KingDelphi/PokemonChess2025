using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHover : MonoBehaviour
{
    [Header("Materials")]
    public Material grayMaterial;   // Material gris original
    public Material blueMaterial;   // Material azul
    private Renderer tileRenderer;   // Referencia al Renderer del tile

    void Start()
    {
        tileRenderer = GetComponent<Renderer>(); // Obtener el Renderer del tile
        if (tileRenderer != null)
        {
            tileRenderer.material = grayMaterial; // Inicializar con el material gris
        }
    }

    private void OnMouseEnter() // Método que se llama cuando el mouse entra en el collider del tile
    {
        if (tileRenderer != null)
        {
            tileRenderer.material = blueMaterial; // Cambiar a material azul
        }
    }

    private void OnMouseExit() // Método que se llama cuando el mouse sale del collider del tile
    {
        if (tileRenderer != null)
        {
            tileRenderer.material = grayMaterial; // Regresar a material gris
        }
    }
}
