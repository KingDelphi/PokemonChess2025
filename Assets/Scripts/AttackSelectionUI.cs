using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AttackSelectionUI : MonoBehaviour
{
    public PokemonBase currentPokemon; // Agrega este campo


    [System.Serializable]
    public class AttackInfoUI
    {
        public GameObject panel; // Este es el panel que muestra la información del ataque
        public GameObject attackTypePanel; // Cambiado de Image a GameObject
        public GameObject moveTypePanel; // Cambiado de Image a GameObject
        public TMP_Text attackName;
        public TMP_Text power;
        public TMP_Text accuracy;
        public TMP_Text description;
    }

    public List<AttackInfoUI> attackPanels = new List<AttackInfoUI>(); // 4 paneles, uno por cada ataque

    public void UpdateAttackInfo(List<PokemonBase.LearnedAttack> attacks)
    {
        Debug.Log($"Cantidad de ataques: {attacks.Count}, Cantidad de paneles: {attackPanels.Count}");

        int numAttacksToDisplay = Mathf.Min(attacks.Count, attackPanels.Count);
        for (int i = 0; i < numAttacksToDisplay; i++)
        {
            var attack = attacks[i];
            var attackData = attack.attack;

            attackPanels[i].attackTypePanel.SetActive(true);
            attackPanels[i].moveTypePanel.SetActive(true);

            attackPanels[i].attackName.text = attackData.name;
            attackPanels[i].power.text = "Power = " + attackData.power.ToString();
            attackPanels[i].accuracy.text = "Accuracy = " + attackData.accuracy.ToString();
            attackPanels[i].description.text = attackData.description;

            attackPanels[i].panel.SetActive(true);

            int index = i; 

            Button attackButton = attackPanels[i].panel.GetComponent<Button>();
            if (attackButton != null)
            {
                attackButton.onClick.RemoveAllListeners();
                attackButton.onClick.AddListener(() =>
                {
                    string attackNameFormatted = FormatAttackName(attackData.name);
                    CallAttackMethod(attackNameFormatted); // Aquí llamamos a la función
                });
            }
        }
    }

    private void CallAttackMethod(string attackName)
    {
        var method = typeof(AttackCatalog).GetMethod(attackName);
        if (method != null)
        {
            method.Invoke(AttackCatalog.Instance, new object[] { currentPokemon }); // Cambia 'this' por 'currentPokemon'
        }
        else
        {
            Debug.LogWarning($"Método {attackName} no encontrado en AttackCatalog.");
        }
    }

    private string FormatAttackName(string attackName)
    {
        return attackName.Replace(" ", ""); // Quita los espacios
    }







    private Sprite GetAttackTypeIcon(string type)
    {
        // Implementa la lógica para retornar el ícono correspondiente al tipo de ataque
        return null;
    }

    private Sprite GetMoveTypeIcon(string moveCategory)
    {
        // Implementa la lógica para retornar el ícono correspondiente a la categoría del movimiento
        return null;
    }
}
