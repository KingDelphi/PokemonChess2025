using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;
    public ItemType itemType;
    public int effectValue; // Valor del efecto, puede ser usado para curaciones, etc.

    // Método para usar el ítem
    public void Use(PokemonBase targetPokemon)
    {
        switch (itemType)
        {
            case ItemType.Potion:
                UsePotion(targetPokemon, effectValue);
                break;
            case ItemType.Berry:
                UseBerry(targetPokemon, effectValue);
                break;
            case ItemType.Revive:
                UseRevive(targetPokemon);
                break;
            case ItemType.StatusHealer:
                UseStatusHeal(targetPokemon);
                break;
            case ItemType.BattleItem:
                UseBattleItem(targetPokemon);
                break;
            // Agregar más tipos de ítems aquí si es necesario
            default:
                Debug.Log($"Cannot use item: {itemName}.");
                break;
        }
    }

    private void UsePotion(PokemonBase targetPokemon, int healAmount)
    {
        targetPokemon.hp = Mathf.Min(targetPokemon.maxHP, targetPokemon.hp + healAmount);
        Debug.Log($"{targetPokemon.pokemonName} used a Potion and healed for {healAmount} HP!");
    }

    private void UseBerry(PokemonBase targetPokemon, int healPercentage)
    {
        int healAmount = (int)(targetPokemon.maxHP * (healPercentage / 100f));
        targetPokemon.hp = Mathf.Min(targetPokemon.maxHP, targetPokemon.hp + healAmount);
        Debug.Log($"{targetPokemon.pokemonName} used a Berry and healed for {healAmount} HP!");
    }

    private void UseRevive(PokemonBase targetPokemon)
    {
        // Lógica para revivir al Pokémon
        targetPokemon.hp = targetPokemon.maxHP / 2; // Revive con 50% de HP
        Debug.Log($"{targetPokemon.pokemonName} has been revived!");
    }

    private void UseStatusHeal(PokemonBase targetPokemon)
    {
        // Lógica para curar estado del Pokémon
        Debug.Log($"{targetPokemon.pokemonName}'s status has been healed!");
    }

    private void UseBattleItem(PokemonBase targetPokemon)
    {
        // Lógica para usar un ítem en batalla
        Debug.Log($"{targetPokemon.pokemonName} used a battle item!");
    }
}

public enum ItemType
{
    EvolutionStone, // Piedras de evolución
    MegaStone,      // Orbe para megaevolución
    Potion,         // Pociones de salud
    Berry,          // Berries
    Revive,         // Revivir
    StatusHealer,   // Curaciones de estado
    BattleItem,     // Items usados en combate
    Held             // Para los ítems que se sostienen
}

public class ItemCatalog : MonoBehaviour
{
    private List<Item> itemCatalog;

    void Start()
    {
        // Inicializar el catálogo de ítems
        itemCatalog = new List<Item>
        {
            new Item { itemName = "Evolution Stone", itemType = ItemType.EvolutionStone },
            new Item { itemName = "Mega Stone", itemType = ItemType.MegaStone },
            new Item { itemName = "Potion", itemType = ItemType.Potion, effectValue = 50 }, // Cura 50 HP
            new Item { itemName = "Berry", itemType = ItemType.Berry, effectValue = 25 }, // Cura 25% HP
            new Item { itemName = "Revive", itemType = ItemType.Revive }, // Revive a un Pokémon
            new Item { itemName = "Status Heal", itemType = ItemType.StatusHealer }, // Cura estado
            new Item { itemName = "Battle Item", itemType = ItemType.BattleItem } // Usado en batalla
        };
    }

    public void Use(Item item, PokemonBase targetPokemon)
    {
        // Verificar si el Pokémon puede usar el ítem
        if (item != null)
        {
            item.Use(targetPokemon); // Usar el ítem
            // Aquí puedes añadir lógica para consumir el ítem después de usarlo, si es necesario.
        }
        else
        {
            Debug.Log("No item to use.");
        }
    }
}
