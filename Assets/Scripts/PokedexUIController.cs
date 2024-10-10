using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para usar TextMeshPro
using UnityEngine.UI;

public class PokedexUIController : MonoBehaviour
{
    // Referencias a los objetos de UI en el Pokedex
    public TextMeshProUGUI trainerNameTMP;
    public GameObject pokemonPanel; // Puede contener una imagen o sprite del Pokémon
    public GameObject attackTypePanel;
    public GameObject moveTypePanel;
    public TextMeshProUGUI attackNameTMP;
    public TextMeshProUGUI powerTMP;
    public TextMeshProUGUI accuracyTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI pokemonNameTMP;
    public TextMeshProUGUI pokemonNatureTMP;
    public TextMeshProUGUI pokemonStatsValuesTMP;

    // Clase Pokemon (puedes adaptarla o usar tu propia)
    private PokemonBase selectedPokemon;

    // Este método será llamado cuando selecciones un Pokémon
    public void UpdatePokedex(PokemonBase pokemon)
    {
        selectedPokemon = pokemon;

        // Actualizar el nombre del Pokémon
        string color = selectedPokemon.gender == PokemonBase.Gender.Male ? "#0000FF" :  // Azul para masculino
               selectedPokemon.gender == PokemonBase.Gender.Female ? "#FF69B4" : // Rosa para femenino
               "#000000"; // Negro para genderless

        Debug.Log($"Color assigned: {color}"); // Verifica el color asignado


        // Aplica el color al nombre del Pokémon
        pokemonNameTMP.text = $"<color={color}>{selectedPokemon.pokemonName}</color>";


        // Actualizar la naturaleza del Pokémon
        pokemonNatureTMP.text = selectedPokemon.pokemonNature?.name ?? "Sin Naturaleza"; // Muestra un mensaje si no hay naturaleza

        // Actualizar las estadísticas del Pokémon
        pokemonStatsValuesTMP.text = selectedPokemon.GetStatsString(); // Devuelve algo como "60, 50, 70"

        // Actualizar ataque
        // attackNameTMP.text = selectedPokemon.GetAttackName();
        // powerTMP.text = selectedPokemon.GetAttackPower().ToString();
        // accuracyTMP.text = selectedPokemon.GetAttackAccuracy().ToString();
        // descriptionTMP.text = selectedPokemon.GetAttackDescription();

        pokemonPanel.GetComponent<SpriteRenderer>().sprite = selectedPokemon.pokedexImage;
        
        // attackTypePanel.GetComponent<Image>().sprite = selectedPokemon.attackTypeSprite;
        // moveTypePanel.GetComponent<Image>().sprite = selectedPokemon.moveTypeSprite;
    }
}