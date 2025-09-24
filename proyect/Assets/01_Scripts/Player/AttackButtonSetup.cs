using UnityEngine;
using UnityEngine.UI;

public class AttackButtonSetup : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject attackButtonPrefab;
    public Vector2 buttonPosition = new Vector2(200, 100);
    public Vector2 buttonSize = new Vector2(100, 100);
    
    private GameObject attackButton;
    private MovementTest playerMovement;
    
    void Start()
    {
        // Buscar el player
        playerMovement = FindObjectOfType<MovementTest>();
        
        if (playerMovement == null)
        {
            Debug.LogError("AttackButtonSetup: No se encontró MovementTest en la escena");
            return;
        }
        
        // Crear el botón de ataque
        CreateAttackButton();
    }
    
    void CreateAttackButton()
    {
        // Buscar el Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("AttackButtonSetup: No se encontró Canvas en la escena");
            return;
        }
        
        // Crear el botón
        attackButton = new GameObject("AttackButton");
        attackButton.transform.SetParent(canvas.transform, false);
        
        // Agregar componentes
        Image buttonImage = attackButton.AddComponent<Image>();
        Button button = attackButton.AddComponent<Button>();
        
        // Configurar la imagen
        buttonImage.color = new Color(1f, 0.2f, 0.2f, 0.8f); // Rojo semi-transparente
        
        // Configurar el RectTransform
        RectTransform rectTransform = attackButton.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.anchoredPosition = buttonPosition;
        rectTransform.sizeDelta = buttonSize;
        
        // Configurar el botón
        button.onClick.AddListener(() => {
            if (playerMovement != null)
            {
                playerMovement.OnAttackButtonPressed();
            }
        });
        
        // Agregar texto
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(attackButton.transform, false);
        
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = "ATAQUE";
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 20;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Debug.Log("AttackButtonSetup: Botón de ataque creado exitosamente");
    }
    
    // Método para mostrar/ocultar el botón
    public void SetButtonVisible(bool visible)
    {
        if (attackButton != null)
        {
            attackButton.SetActive(visible);
        }
    }
    
    // Método para cambiar la posición del botón
    public void SetButtonPosition(Vector2 position)
    {
        if (attackButton != null)
        {
            RectTransform rectTransform = attackButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
        }
    }
}
