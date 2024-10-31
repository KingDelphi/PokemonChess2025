using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AttackSelectionUI : MonoBehaviour
{
    public PokemonBase currentPokemon; // Agrega este campo

    public Sprite normalSprite;
    public Sprite fightingSprite;
    public Sprite flyingSprite;
    public Sprite poisonSprite;
    public Sprite groundSprite;
    public Sprite rockSprite;
    public Sprite bugSprite;
    public Sprite ghostSprite;
    public Sprite steelSprite;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite electricSprite;
    public Sprite psychicSprite;
    public Sprite iceSprite;
    public Sprite dragonSprite;
    public Sprite darkSprite;
    public Sprite fairySprite;

    public Dictionary<string, Sprite> typeSprites = new Dictionary<string, Sprite>();

    public Sprite physicalSprite;
    public Sprite specialSprite;
    public Sprite statusSprite;

    private Dictionary<AttackCategory, Sprite> categorySprites = new Dictionary<AttackCategory, Sprite>();


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

    public void Awake()
{
    // Inicializar sprites
    InitializeTypeSprites();
    InitializeCategorySprites(); // Inicializar sprites de categorías de ataque

}

    public void Start()
    {
        gameObject.SetActive(false);
    }

// Método para inicializar los sprites de tipo
private void InitializeTypeSprites()
{
    typeSprites.Add("Normal", normalSprite);
    typeSprites.Add("Fighting", fightingSprite);
    typeSprites.Add("Flying", flyingSprite);
    typeSprites.Add("Poison", poisonSprite);
    typeSprites.Add("Ground", groundSprite);
    typeSprites.Add("Rock", rockSprite);
    typeSprites.Add("Bug", bugSprite);
    typeSprites.Add("Ghost", ghostSprite);
    typeSprites.Add("Steel", steelSprite);
    typeSprites.Add("Fire", fireSprite);
    typeSprites.Add("Water", waterSprite);
    typeSprites.Add("Grass", grassSprite);
    typeSprites.Add("Electric", electricSprite);
    typeSprites.Add("Psychic", psychicSprite);
    typeSprites.Add("Ice", iceSprite);
    typeSprites.Add("Dragon", dragonSprite);
    typeSprites.Add("Dark", darkSprite);
    typeSprites.Add("Fairy", fairySprite);
}

private void InitializeCategorySprites()
    {
        categorySprites.Add(AttackCategory.Physical, physicalSprite);
        categorySprites.Add(AttackCategory.Special, specialSprite);
        categorySprites.Add(AttackCategory.Status, statusSprite);
    }

    public void UpdateAttackInfo(List<PokemonBase.LearnedAttack> attacks)
{
    Debug.Log($"Cantidad de ataques: {attacks.Count}, Cantidad de paneles: {attackPanels.Count}");

    int numAttacksToDisplay = Mathf.Min(attacks.Count, attackPanels.Count);
    for (int i = 0; i < numAttacksToDisplay; i++)
    {
        var attack = attacks[i];
        var attackData = attack.attack;

        // Accedemos al SpriteRenderer incluso cuando el panel está desactivado
        if (attackPanels[i].attackTypePanel.TryGetComponent<SpriteRenderer>(out var spriteRenderer) &&
            typeSprites.TryGetValue(attackData.type, out var typeSprite))
        {
            bool wasActive = spriteRenderer.enabled;  // Guarda el estado original
            spriteRenderer.enabled = true;            // Activa temporalmente el SpriteRenderer
            spriteRenderer.sprite = typeSprite;       // Cambia el sprite
            spriteRenderer.enabled = wasActive;       // Restaura el estado original
        }
        else
        {
            Debug.LogWarning($"No se pudo actualizar el sprite para el ataque '{attackData.name}'. " +
                             $"SpriteRenderer: {(spriteRenderer == null ? "No encontrado" : "Encontrado")}, " +
                             $"Tipo: {attackData.type}");
        }

        if (attackPanels[i].moveTypePanel.TryGetComponent<SpriteRenderer>(out var categorySpriteRenderer) &&
                categorySprites.TryGetValue(attackData.category, out var categorySprite))
            {
                bool wasActive = categorySpriteRenderer.enabled;
                categorySpriteRenderer.enabled = true;
                categorySpriteRenderer.sprite = categorySprite;
                categorySpriteRenderer.enabled = wasActive;
            }

        // Actualizar textos y configuración de paneles
        attackPanels[i].attackName.text = attackData.name;
        attackPanels[i].power.text = "Power = " + attackData.power.ToString();
        attackPanels[i].accuracy.text = "Accuracy = " + attackData.accuracy.ToString();
        attackPanels[i].description.text = attackData.description;

        // Configuración de botones de ataque
        Button attackButton = attackPanels[i].panel.GetComponent<Button>();
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(() =>
            {
                string attackNameFormatted = FormatAttackName(attackData.name);
                CallAttackMethod(attackNameFormatted);
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
            gameObject.SetActive(false);
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