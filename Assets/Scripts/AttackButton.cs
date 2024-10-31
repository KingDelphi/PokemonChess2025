using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour
{
    private Button button;
    private string attackName;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (!string.IsNullOrEmpty(attackName))
        {
            // Convertimos el nombre del ataque para llamar al método adecuado en AttackCatalog
            string methodName = attackName.Replace(" ", ""); // Quita los espacios en blanco
            var method = typeof(AttackCatalog).GetMethod(methodName);

            if (method != null)
            {
                method.Invoke(AttackCatalog.Instance, new object[] { this });
            }
            else
            {
                Debug.LogWarning($"Método {methodName} no encontrado en AttackCatalog.");
            }
        }
    }
}
