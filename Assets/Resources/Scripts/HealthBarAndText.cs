using UnityEngine;
using UnityEngine.UI;

public class HealthBarAndText : MonoBehaviour
{
    public RectTransform healthbarRectTransform;           //Health bar prefab
    public RectTransform hitTextHolderTransform;            // Hit texts holder prefab
    public float yOffset, xOffset, zOffset;                           //Position offset according to world posiiton
    public bool keepSize = true;                    // Keep distance independent size
    public float scale = 1;                         // Scale of the healthbar
    public Vector2 sizeOffsets;                     // Use this to overwrite healthbar width and height values
    public AlphaSettings alphaSettings;             // Health bar alpha settings
    public Color healthBarDefaultColor;             // Health color
    public Color hitTextColor;                      // Hit text color
    public int hitFontSize = 20;                    // Hit text font
    public int hitTexts_yOffset = 20;               // Hit text  Y offset on the canvas


    private CanvasGroup canvasGroup;
    private Image healthVolume;         // Health bar health image
    private Image backGround;         //Health bar background image, "the blood effect"
    private Text[] hitTexts = new Text[6];      // Hit text texts
    private Vector2 healthbarSize;      // Health bar size
    private float defaultHealth, camDistance, dist, backGroundDecrease;
    private Camera cam;             // Which came will be used as a reference for scale and raycasts
    private GameObject healthBarParent;         // Health bar instantiated parent gameobject
    private IDamageable character;          // Damageable character
    private float matchRate;            // A rate to everything with screen resolution

    void Awake()
    {
        cam = Camera.main; // Which came will be used as a reference for scale and raycasts
        Canvas canvas = FindAndSetCanvas();
        SetHealthBarParent(canvas);
        SetHealthBar();
        SetHealthBarChildren();
        SetHitTexts();
        SetCanvasGroup();
        character = GetComponent<IDamageable>();
        SetMatchRate(canvas);
    }

    private void Start()
    {
        ReNewAllValues();
    }

    void OnEnable()
    {
        character.DamageableEvent += DamageEventHandler; // Add listener to the damage event of the character
        ReNewAllValues();
        SetCanvasGroup();
    }

    private void OnDisable()
    {
        character.DamageableEvent -= DamageEventHandler; // Remove listener from the damage event of the character
        if (healthbarRectTransform != null) // If health was used, remove it from the invisible area
            healthbarRectTransform.anchoredPosition = new Vector2(-1000, 1000);
    }



    void Update() // For more smooth movement of health bars, this logic can be used in OnGUI(). Especially when camera is on the move, the difference will be huge.
    {
        SetHealthBarToPosition();
        SetFillAmounts();

        if (!IsVisible())
            return;

        if (character.CurrentHealth <= 0)
        {
            if (alphaSettings.nullFadeSpeed > 0)
            {
                if (backGround.fillAmount <= 0)
                    canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.nullAlpha, alphaSettings.nullFadeSpeed);
            }
            else
            {
                canvasGroup.alpha = alphaSettings.nullAlpha;
            }
        }

        SetBarSizeDelta();

    }

    /// <summary>Set health bar size delta.</summary>
    private void SetBarSizeDelta()
    {
        camDistance = Vector3.Dot(transform.position - cam.transform.position, cam.transform.forward);
        dist = keepSize ? camDistance / scale : scale;
        healthbarRectTransform.sizeDelta = new Vector2(healthbarSize.x / (dist - sizeOffsets.x / 100), healthbarSize.y / (dist - sizeOffsets.y / 100));
    }

    /// <summary>Disactivate health bar parent.</summary>
    public void DisactivateHealthBar() => healthbarRectTransform.gameObject.SetActive(false);

    /// <summary> Set fill amount value of background of the health bar and helath bar when getting hit and
    /// blood effect of the background is trying to catch current health volume bar.</summary>
    private void SetFillAmounts()
    {
        healthVolume.fillAmount = character.CurrentHealth / defaultHealth;

        if (backGround.fillAmount > healthVolume.fillAmount)
        {
            backGroundDecrease = backGround.fillAmount - healthVolume.fillAmount < 0.2f ? (defaultHealth / 2000) : backGround.fillAmount - healthVolume.fillAmount;
            backGround.fillAmount -= backGroundDecrease * Time.deltaTime;
        }

    }

    /// <summary>
    /// Set health bar position onto the canvas with matchRate to make sure it is true for every screen resolution.
    /// Use offset values. If object is behind of the camera then do not show it.
    /// </summary>
    private void SetHealthBarToPosition()
    {
        Vector3 camScreen = cam.WorldToScreenPoint(transform.position + Vector3.up * yOffset + Vector3.forward * xOffset + Vector3.right * zOffset);
        if (camScreen.z < 0)
        {
            healthbarRectTransform.anchoredPosition = new Vector2(-1000, 1000);
            return;
        }
        healthbarRectTransform.anchoredPosition = camScreen * matchRate;
    }

    /// <summary>Check if health bar parent objects is in the visible region of the canvas.</summary><returns></returns>
    private bool IsVisible()
    {
        return healthbarRectTransform.anchoredPosition.x < matchRate * cam.scaledPixelWidth && healthbarRectTransform.anchoredPosition.x > 0 &&
            healthbarRectTransform.anchoredPosition.y < matchRate * cam.scaledPixelHeight && healthbarRectTransform.anchoredPosition.y > 0;
    }


    #region SetUp 

    /// <summary>A rate to make sure everything is scaled with the screen resolution for various devices.</summary>
    /// <param name="canvas"></param>
    private void SetMatchRate(Canvas canvas)
    {
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        matchRate = ((1 - canvasScaler.matchWidthOrHeight) * canvasScaler.referenceResolution.x / cam.scaledPixelWidth)
            + (canvasScaler.matchWidthOrHeight * canvasScaler.referenceResolution.y / cam.scaledPixelHeight);
    }

    /// <summary>Set alpha value of canvas group, interactable and block raycast values.</summary>
    private void SetCanvasGroup()
    {
        canvasGroup.alpha = alphaSettings.fullAplpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>Instantiate and set parent and size delta values for healt hbar.</summary>
    private void SetHealthBar()
    {
        healthbarRectTransform = Instantiate(healthbarRectTransform, new Vector2(-1000, -1000), Quaternion.identity);
        healthbarRectTransform.SetParent(healthBarParent.transform, false);
        healthbarSize = healthbarRectTransform.sizeDelta;
        SetBarSizeDelta();
    }

    /// <summary>
    /// Find or instantita health bar parent object.
    /// </summary>
    /// <param name="canvas"></param>
    private void SetHealthBarParent(Canvas canvas)
    {
        healthBarParent = GameObject.Find("HealthBarParent");
        if (healthBarParent == null)
        {
            healthBarParent = new GameObject();
            RectTransform rectT = healthBarParent.AddComponent<RectTransform>();
            rectT.transform.position = Vector3.zero;
            rectT.SetParent(canvas.transform, false);
            rectT.sizeDelta = Vector2.zero;
            rectT.anchorMax = Vector2.zero;
            rectT.anchorMin = Vector2.zero;
            healthBarParent.name = "HealthBarParent";
        }
    }

    /// <summary>Set health bar children objects.</summary>
    private void SetHealthBarChildren()
    {
        healthVolume = healthbarRectTransform.transform.GetChild(1).GetComponent<Image>();
        healthVolume.color = healthBarDefaultColor;
        backGround = healthbarRectTransform.transform.GetChild(0).GetComponent<Image>();
        canvasGroup = healthbarRectTransform.GetComponent<CanvasGroup>();
    }

    /// <summary>Find canvas game object and returen canvas component of it.</summary><returns>Canvas</returns>
    private Canvas FindAndSetCanvas()
    {
        GameObject canvasGO = GameObject.Find("Canvas"); // Assign it to another canvas to reduce redraw element number at every SetDirty call.
        if (canvasGO == null)
        {
            Debug.LogError("No Canvas in the scene!");
        }
        return canvasGO.GetComponent<Canvas>();
    }

    /// <summary>Renews default health and set background fill amount to max.</summary>
    public void ReNewAllValues()
    {
        defaultHealth = character.CurrentHealth;
        backGround.fillAmount = 1;
    }

    #endregion

    #region Hit And Heal Text

    /// <summary>
    /// Set hit texts of the health bar. Put them in the list and set their color.
    /// </summary>
    private void SetHitTexts()
    {
        if (hitTextHolderTransform == null)
            return;
        hitTextHolderTransform = Instantiate(hitTextHolderTransform, healthbarRectTransform);
        hitTextHolderTransform.anchoredPosition += Vector2.up * hitTexts_yOffset;
        hitTexts = hitTextHolderTransform.GetComponentsInChildren<Text>(true);
        foreach (Text text in hitTexts)
            text.fontSize = hitFontSize;
    }

    /// <summary>Damage event listen handler.</summary>
    /// <param name="damagetType"></param>
    /// <param name="hitAmount"></param>
    private void DamageEventHandler(DamageType damagetType, int? hitAmount)
    {
        switch (damagetType)
        {
            case DamageType.Hit:
                ShowHitText((int)hitAmount);
                break;
            case DamageType.Bomb:
                ShowHitText();
                break;
            case DamageType.Dodge:
                ShowDodgedText();
                break;
            case DamageType.Heal:
                healthVolume.fillAmount = character.CurrentHealth / defaultHealth;
                backGround.fillAmount = healthVolume.fillAmount;
                ShowHealText((int)hitAmount);
                break;
        }
    }

    /// <summary>Finds available text and shows the amount got hit.</summary>
    /// <param name="hitAmount"></param> The given hit amount.
    public void ShowHitText(int hitAmount)
    {
        hitAmount = CalculateHitAmountWillBeShown(hitAmount);
        foreach (Text text in hitTexts)
        {
            if (text.gameObject.activeSelf)
                continue;
            text.text = hitAmount.ToString();
            text.color = hitTextColor;
            text.gameObject.SetActive(true);
            return;
        }
    }

    /// <summary>Calculates hit amount that will be shown to the player.</summary>
    /// <param name="hitAmount"></param>
    /// <returns>The Amount will be shown</returns>
    private int CalculateHitAmountWillBeShown(int hitAmount) => hitAmount;

    /// <summary>Finds available text and shows the amount got healed.</summary>
    /// <param name="healAmount"></param>
    public void ShowHealText(int healAmount)
    {
        healAmount = CalculateHitAmountWillBeShown(healAmount);
        foreach (Text text in hitTexts)
        {
            if (text.gameObject.activeSelf)
                continue;
            text.text = healAmount.ToString();
            text.color = Color.green;
            text.gameObject.SetActive(true);
            return;
        }
    }

    /// <summary>Finds available text and shows the bomb text.</summary>
    public void ShowHitText()
    {
        foreach (Text text in hitTexts)
        {
            if (text.gameObject.activeSelf)
                continue;
            text.text = "Booom!";
            text.color = hitTextColor;
            text.gameObject.SetActive(true);
            return;
        }
    }

    /// <summary>Finds available text and shows the dodge text.</summary>
    public void ShowDodgedText()
    {
        foreach (Text text in hitTexts)
        {
            if (text.gameObject.activeSelf)
                continue;
            text.text = "Nahh!";
            text.color = Color.yellow;
            text.gameObject.SetActive(true);
            return;
        }
    }

    #endregion

}

[System.Serializable]
public class AlphaSettings
{
    public float fullAplpha = 1.0F;             //Healthbar alpha when health is full;
    public float nullAlpha = 0.0F;              //Healthbar alpha when health is zero or less;
    public float nullFadeSpeed = 0.1F;
}

