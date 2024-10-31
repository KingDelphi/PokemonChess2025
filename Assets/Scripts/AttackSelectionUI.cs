using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AttackSelectionUI : MonoBehaviour
{
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
    for (int i = 0; i < attackPanels.Count; i++)
    {
        var attack = attacks[i];
        var attackData = attack.attack; // Obtenemos el ataque de LearnedAttack

        // Asigna los paneles de ataque
        attackPanels[i].attackTypePanel.SetActive(true); // Activar panel del tipo de ataque
        attackPanels[i].moveTypePanel.SetActive(true); // Activar panel de la categoría de ataque

        attackPanels[i].attackName.text = attackData.name;
        attackPanels[i].power.text = "Power = " + attackData.power.ToString();
        attackPanels[i].accuracy.text = "Accuracy = " + attackData.accuracy.ToString();

        attackPanels[i].description.text = attackData.description;

        attackPanels[i].panel.SetActive(true); // Activar el panel del ataque
    }
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
