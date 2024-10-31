using UnityEngine;

public class AttackButton : MonoBehaviour
{
    private AttackSelectionUI attackSelectionUI;

    private void Start()
    {
        attackSelectionUI = FindObjectOfType<AttackSelectionUI>();
    }

    public void OnAttackButtonClicked()
{
    // Busca AttackSelectionUI
    attackSelectionUI = FindObjectOfType<AttackSelectionUI>();

    if (attackSelectionUI != null)
    {
        // Alterna la visibilidad del AttackSelectionUI
        attackSelectionUI.gameObject.SetActive(!attackSelectionUI.gameObject.activeSelf);

        // Si se activa, actualiza la información de ataques
        if (attackSelectionUI.gameObject.activeSelf)
        {
            // Aquí asumimos que Eevee es el Pokémon seleccionado
            Eevee eevee = FindObjectOfType<Eevee>();
            if (eevee != null)
            {
                // Actualiza la información de ataques del Eevee
                attackSelectionUI.UpdateAttackInfo(eevee.attackList);
            }
            else
            {
                Debug.LogWarning("Eevee no encontrado en la escena.");
            }
        }
    }
    else
    {
        Debug.LogWarning("AttackSelectionUI no encontrado.");
    }
}

}
