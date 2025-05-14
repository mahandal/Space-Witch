using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class RebindManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject rebindingOverlay;
    [SerializeField] private TextMeshProUGUI overlayText;
    
    [Header("UI Elements")]
    [SerializeField] private Transform bindingRowsParent;
    [SerializeField] private GameObject bindingRowPrefab;
    
    // Internal tracking
    public List<BindingRow> bindingRows = new List<BindingRow>();
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Awake()
    {
        // Hide overlay initially
        rebindingOverlay.SetActive(false);
    }

    void Start()
    {
        // Initialize all binding rows
        InitializeBindingRows();
        
        // Load any saved bindings
        LoadBindings();
        
        // Update the UI to show current bindings
        UpdateAllBindingTexts();
    }

    private void InitializeBindingRows()
    {
        for (int i = 0; i < bindingRows.Count; i++)
        {
            BindingRow row = bindingRows[i];
            row.action = InputSystem.actions.FindAction(row.bindingName);
            row.rebindButton.onClick.AddListener(() => StartRebinding(row.rowIndex));
        }
    }

    private void UpdateAllBindingTexts()
    {
        foreach (var row in bindingRows)
        {
            UpdateBindingText(row);
        }
    }

    private void UpdateBindingText(BindingRow row)
    {
        // Get the display string for the binding
        string displayString = row.action.GetBindingDisplayString();
        row.bindingText.text = displayString;
    }

    public void StartRebinding(int rowIndex)
    {
        BindingRow row = bindingRows[rowIndex];
        
        // Disable input actions while rebinding
        inputActions.Disable();
        
        // Show the overlay
        rebindingOverlay.SetActive(true);
        overlayText.text = $"Press a key for {row.bindingName}...";
        
        // Start the rebinding operation
        rebindOperation = row.action.PerformInteractiveRebinding()
            //.WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(rowIndex))
            .OnCancel(operation => RebindCancelled())
            .Start();
    }

    private void RebindComplete(int rowIndex)
    {
        // Clean up the operation
        rebindOperation.Dispose();
        rebindOperation = null;
        
        // Hide the overlay
        rebindingOverlay.SetActive(false);
        
        // Update the UI
        UpdateBindingText(bindingRows[rowIndex]);
        
        // Save the new bindings
        SaveBindings();
        
        // Re-enable input actions
        inputActions.Enable();
    }
    
    public void RebindCancelled()
    {
        // Clean up the operation
        rebindOperation.Dispose();
        rebindOperation = null;
        
        // Hide the overlay
        rebindingOverlay.SetActive(false);
        
        // Re-enable input actions
        inputActions.Enable();
    }

    private void SaveBindings()
    {
        // Save to PlayerPrefs as JSON
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputBindings", rebinds);
        PlayerPrefs.Save();
    }

    private void LoadBindings()
    {
        // Load from PlayerPrefs
        if (PlayerPrefs.HasKey("InputBindings"))
        {
            string rebinds = PlayerPrefs.GetString("InputBindings");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void OnDestroy()
    {
        // Make sure we clean up any ongoing rebinding operation
        if (rebindOperation != null)
        {
            rebindOperation.Dispose();
            rebindOperation = null;
        }
    }
}