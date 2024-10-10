using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para usar TextMeshPro
using UnityEngine.UI;

public class PokedexUIController : MonoBehaviour
{
    // Referencias a los objetos de UI en el Pokedex
    public TextMeshProUGUI trainerNameTMP;
    public GameObject imagePanel; // Puede contener una imagen o sprite del Pokémon
    public GameObject attackTypePanel;
    public GameObject moveTypePanel;
    public TextMeshProUGUI attackNameTMP;
    public TextMeshProUGUI powerTMP;
    public TextMeshProUGUI accuracyTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI unitNameTMP;
    public TextMeshProUGUI unitNatureTMP;
    public TextMeshProUGUI unitStatsValuesTMP;

    // Clase Pokemon (puedes adaptarla o usar tu propia)
    private PokemonBase selectedPokemon;
    private TrainerBase selectedTrainer;


    // Este método será llamado cuando selecciones un Pokémon
    public void UpdatePokedex(object entity)
{
    if (entity is PokemonBase pokemon)
    {
        // Actualiza la información del Pokédex para el Pokémon
        selectedPokemon = pokemon;

        // Actualizar el nombre del Pokémon con el color basado en el género
        string color = selectedPokemon.gender == PokemonBase.Gender.Male ? "#0000FF" :  // Azul para masculino
                       selectedPokemon.gender == PokemonBase.Gender.Female ? "#FF69B4" : // Rosa para femenino
                       "#000000"; // Negro para genderless

        // Aplica el color al nombre del Pokémon
        unitNameTMP.text = $"<color={color}>{selectedPokemon.pokemonName}</color>";

        // Actualizar la naturaleza del Pokémon
        unitNatureTMP.text = selectedPokemon.pokemonNature?.name ?? "Sin Naturaleza"; // Muestra un mensaje si no hay naturaleza

        // Actualizar las estadísticas del Pokémon
        unitStatsValuesTMP.text = selectedPokemon.GetStatsString(); // Devuelve algo como "60, 50, 70"
        
        // Actualizar la imagen del Pokémon
        imagePanel.GetComponent<SpriteRenderer>().sprite = selectedPokemon.pokedexImage;
        imagePanel.transform.localScale = new Vector3(.2f, .2f, 1f); // Cambia la escala a 3x3
    }
    else if (entity is TrainerBase trainer)
    {
        // Actualiza la información del Pokédex para el Trainer
        selectedTrainer = trainer;

        // Actualizar el nombre del Trainer (puedes elegir si deseas cambiar el color o mantener uno fijo)
        unitNameTMP.text = $"<color=#FFA500>{selectedTrainer.trainerName}</color>"; // Naranja para el nombre del Trainer

        unitNatureTMP.text = "";

        // Actualizar los poderes o habilidades del Trainer
        // string powers = string.Join(", ", selectedTrainer.powers); // Muestra las habilidades del Trainer
        // trainerPowersTMP.text = !string.IsNullOrEmpty(powers) ? powers : "Sin poderes";

        // Actualizar la vida del Trainer (HP)
        unitStatsValuesTMP.text = selectedTrainer.GetStatsString();

        // Mostrar la imagen del Trainer si es necesario (si tienes una)
        SpriteRenderer imageSpriteRenderer = imagePanel.GetComponent<SpriteRenderer>();
        imageSpriteRenderer.sprite = selectedTrainer.GetComponent<SpriteRenderer>().sprite;
        imagePanel.transform.localScale = new Vector3(2.8f, 2.8f, 1f); // Cambia la escala a 3x3
    }
    else
    {
        Debug.LogError("No se ha seleccionado ni un Pokémon ni un Trainer.");
    }
}


}