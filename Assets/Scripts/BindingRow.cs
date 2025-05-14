using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BindingRow : MonoBehaviour
{
    public TextMeshProUGUI actionText;
    public TextMeshProUGUI bindingText;
    public Button rebindButton;
    public string bindingName;
    public int rowIndex;
    public InputAction action;
}
