using UnityEngine;

public class TileEffect : MonoBehaviour
{
    // Tipos de efectos que pueden activarse
    public enum TileType { None, Vines, Fire, Water, Static }
    public TileType currentEffect = TileType.None;

    // Variables para diferentes efectos
    public bool isOccupied;
    public float fireDamage = 5f;
    public float movementCostMultiplier = 1f; // Multiplicador de costo de movimiento
    public bool canEnsnare = false;
    public bool canBurn = false;

    // Prefabs de animaciones que se instanciarán en el tile
    public  GameObject vinesEffectPrefab;
    public  GameObject fireEffectPrefab;
    public  GameObject waterEffectPrefab;
    public  GameObject staticEffectPrefab;

    private GameObject activeEffectInstance;

    // Activar un tipo de efecto y la animación correspondiente
    public void ActivateEffect(TileType effectType)
{
    // Asignar el tipo de efecto al tile
    currentEffect = effectType; // 'currentEffect' es una variable dentro de TileEffect que almacena el efecto activo

    // Aquí puedes agregar la lógica para cambiar la visual o el comportamiento del tile, por ejemplo:
    switch (effectType)
    {
        case TileType.Vines:
            ApplyVinesEffect();  // Cambia el estado visual o propiedades del tile
            break;
        case TileType.Fire:
            ApplyFireEffect();
            break;
        case TileType.Water:
            ApplyWaterEffect();
            break;
        case TileType.Static:
            ApplyStaticEffect();
            break;
        default:
            ClearEffects(); // Elimina cualquier efecto si no hay uno válido
            break;
    }
}

// Ejemplo de cómo podrías manejar cada efecto
private void ApplyVinesEffect()
{
    // Cambia el estado o visuales para el efecto de enredaderas
    Debug.Log("Aplicando efecto de enredaderas.");
    // Puedes cambiar el color del tile, cambiar propiedades físicas, etc.
}

private void ApplyFireEffect()
{
    Debug.Log("Aplicando efecto de fuego.");
    // Lógica específica para el efecto de fuego
}

private void ApplyWaterEffect()
{
    Debug.Log("Aplicando efecto de agua.");
    // Lógica específica para el efecto de agua
}

private void ApplyStaticEffect()
{
    Debug.Log("Aplicando efecto estático (eléctrico).");
    // Lógica específica para el efecto estático
}

private void ClearEffects()
{
    Debug.Log("Limpiando efectos.");
    // Lógica para limpiar los efectos aplicados al tile
}


    // Resetear los efectos del tile y eliminar cualquier animación activa
    public void ResetEffects()
    {
        currentEffect = TileType.None;
        canEnsnare = false;
        canBurn = false;
        fireDamage = 0f;
        movementCostMultiplier = 1f;

        // Destruir la instancia de cualquier efecto visual activo
        if (activeEffectInstance != null)
        {
            Destroy(activeEffectInstance);
            activeEffectInstance = null;
        }

        Debug.Log("Resetting effects on tile.");
    }

    // Si un Pokémon entra en el tile
    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return; // Solo aplicamos el efecto si el tile no está ocupado

        PokemonBase pokemon = other.GetComponent<PokemonBase>();
        if (pokemon != null)
        {
            switch (currentEffect)
            {
                case TileType.Vines:
                    if (canEnsnare)
                    {
                        pokemon.ApplyStatusCondition(PokemonBase.StatusCondition.Esnared, 3);
                        Debug.Log($"{pokemon.pokemonName} is ensnared by vines!");
                    }
                    break;
                case TileType.Fire:
                    if (canBurn)
                    {
                        //pokemon.TakeDamage(fireDamage);
                        pokemon.ApplyStatusCondition(PokemonBase.StatusCondition.Burn, 3);
                        Debug.Log($"{pokemon.pokemonName} is burned!");
                    }
                    break;
                case TileType.Water:
                    Debug.Log($"{pokemon.pokemonName} is soaked!");
                    break;
                case TileType.Static:
                    Debug.Log($"{pokemon.pokemonName} is affected by static electricity!");
                    break;
                case TileType.None:
                default:
                    break;
            }
        }
    }

    // Simular la salida de un Pokémon del tile (por si quieres limpiar efectos)
    private void OnTriggerExit(Collider other)
    {
        isOccupied = false;
    }
}
