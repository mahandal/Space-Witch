using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour
{

    [Header("Tooltip")]
    public string myName;
    public string description;

    [Header("Special")]
    public Image specialImage;
    public string specialImageFileName = "";

    [Header("Automated machinery")]
    public Sprite sprite;

    void Awake()
    {
        // Try and get our sprite?
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            sprite = spriteRenderer.sprite;
        
    }

    public void LoadTooltip()
    {
        // activate
        GM.I.ui.tooltip.SetActive(true);

        // load name
        GM.I.ui.tooltipName.text = myName;

        // load description
        GM.I.ui.tooltipDescription.text = description;

        // Check if we should use our special tooltip
        if (specialImage != null && GM.I.player.isCalm)
        {
            // Set sprite directly to our special image's sprite
            GM.I.ui.tooltipImage.sprite = specialImage.sprite;
        }
        else if (specialImageFileName != "" && GM.I.player.isCalm)
        {
            // Load image by name
            Utility.LoadImage(GM.I.ui.tooltipImage, specialImageFileName);
        }
        else
        {
            // Set sprite directly
            GM.I.ui.tooltipImage.sprite = sprite;
        }
    }
}
