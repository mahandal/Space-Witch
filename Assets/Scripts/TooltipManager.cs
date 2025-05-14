using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Use your existing player cursor position
        Vector2 cursorPosition = GM.I.player.cursorPosition;
        
        // Cast at cursor position
        Collider2D hit = Physics2D.OverlapPoint(cursorPosition);
        
        if (hit != null)
        {
            Tooltip tooltip = hit.GetComponent<Tooltip>();
            if (tooltip != null)
            {
                //GM.I.ui.LoadTooltip(tooltip);
                tooltip.LoadTooltip();
            }
            else
            {
                HideTooltip();
            }
        }
        else
        {
            HideTooltip();
        }
    }

    public void HideTooltip()
    {
        GM.I.ui.tooltip.SetActive(false);
    }
}
