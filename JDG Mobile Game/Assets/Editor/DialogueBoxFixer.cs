using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System;

/// <summary>
/// Editor utility to fix the DialogueBox GameObject by adding missing components.
/// The DialogueBox needs DialogueUI, TypewriterEffect, and ResponseHandler components
/// to display tutorial dialogue text.
///
/// Also removes duplicate components that can cause race conditions:
/// - DialogueUI, TypewriterEffect, ResponseHandler on Canvas (parent) - these should only be on DialogueBox
/// - Duplicate TypewriterEffect on DialogueBox
///
/// Note: Uses reflection to add components because the scripts are in JDG.Legacy assembly
/// which has autoReferenced=false and cannot be directly referenced by Editor scripts.
/// </summary>
public static class DialogueBoxFixer
{
    // Type names for components in JDG.Legacy assembly
    // TypewriterEffect is in OnePlayer.DialogueBox namespace, others are in global namespace
    private const string TypewriterEffectTypeName = "OnePlayer.DialogueBox.TypewriterEffect, JDG.Legacy";
    private const string ResponseHandlerTypeName = "ResponseHandler, JDG.Legacy";
    private const string DialogueUITypeName = "DialogueUI, JDG.Legacy";

    [MenuItem("Tools/Fix Dialogue Box")]
    public static void FixDialogueBox()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("DialogueBoxFixer: No active scene");
            return;
        }

        // Find DialogueBox in scene
        var dialogueBox = GameObject.Find("DialogueBox");
        if (dialogueBox == null)
        {
            Debug.LogError("DialogueBoxFixer: DialogueBox GameObject not found in scene. " +
                "Make sure you have the TutoPlayerGame scene open.");
            return;
        }

        Debug.Log($"DialogueBoxFixer: Found DialogueBox at '{GetFullPath(dialogueBox)}'");

        bool modified = false;

        // Get types via reflection
        var typewriterEffectType = Type.GetType(TypewriterEffectTypeName);
        var responseHandlerType = Type.GetType(ResponseHandlerTypeName);
        var dialogueUIType = Type.GetType(DialogueUITypeName);

        if (typewriterEffectType == null || responseHandlerType == null || dialogueUIType == null)
        {
            Debug.LogError("DialogueBoxFixer: Could not find required types. Make sure the JDG.Legacy assembly is compiled.");
            Debug.LogError($"  TypewriterEffect: {(typewriterEffectType != null ? "Found" : "NOT FOUND")}");
            Debug.LogError($"  ResponseHandler: {(responseHandlerType != null ? "Found" : "NOT FOUND")}");
            Debug.LogError($"  DialogueUI: {(dialogueUIType != null ? "Found" : "NOT FOUND")}");
            return;
        }

        // STEP 1: Remove duplicate components from Canvas (parent of DialogueBox)
        // These cause race conditions where two DialogueUI instances both try to write to the same text label
        var canvas = dialogueBox.transform.parent?.gameObject;
        if (canvas != null && canvas.name == "Canvas")
        {
            modified |= RemoveDuplicateDialogueComponents(canvas, dialogueUIType, typewriterEffectType, responseHandlerType);
        }

        // STEP 2: Remove duplicate TypewriterEffect from DialogueBox
        // Keep only one TypewriterEffect to prevent race conditions
        modified |= RemoveDuplicateTypewriterEffects(dialogueBox, typewriterEffectType);

        // STEP 3: Add TypewriterEffect if missing
        var typewriterEffect = dialogueBox.GetComponent(typewriterEffectType);
        if (typewriterEffect == null)
        {
            Undo.RegisterCompleteObjectUndo(dialogueBox, "Add TypewriterEffect");
            typewriterEffect = dialogueBox.AddComponent(typewriterEffectType);
            Debug.Log("DialogueBoxFixer: Added TypewriterEffect component");
            modified = true;
        }

        // Add ResponseHandler if missing
        var responseHandler = dialogueBox.GetComponent(responseHandlerType);
        if (responseHandler == null)
        {
            Undo.RegisterCompleteObjectUndo(dialogueBox, "Add ResponseHandler");
            responseHandler = dialogueBox.AddComponent(responseHandlerType);
            Debug.Log("DialogueBoxFixer: Added ResponseHandler component");
            modified = true;
        }

        // Add DialogueUI if missing
        var dialogueUI = dialogueBox.GetComponent(dialogueUIType);
        if (dialogueUI == null)
        {
            Undo.RegisterCompleteObjectUndo(dialogueBox, "Add DialogueUI");
            dialogueUI = dialogueBox.AddComponent(dialogueUIType);
            Debug.Log("DialogueBoxFixer: Added DialogueUI component");
            modified = true;
        }

        // Configure DialogueUI serialized fields
        var serializedObj = new SerializedObject(dialogueUI);

        // Set dialogueBox reference
        var dialogueBoxField = serializedObj.FindProperty("dialogueBox");
        if (dialogueBoxField != null && dialogueBoxField.objectReferenceValue == null)
        {
            dialogueBoxField.objectReferenceValue = dialogueBox;
            Debug.Log("DialogueBoxFixer: Set dialogueBox reference");
            modified = true;
        }

        // Find and set textLabel (TMP_Text child)
        var textLabelField = serializedObj.FindProperty("textLabel");
        TMP_Text tmpText = null;
        if (textLabelField != null && textLabelField.objectReferenceValue == null)
        {
            tmpText = dialogueBox.GetComponentInChildren<TMP_Text>(true);
            if (tmpText != null)
            {
                textLabelField.objectReferenceValue = tmpText;
                Debug.Log($"DialogueBoxFixer: Set textLabel to '{tmpText.gameObject.name}'");
                modified = true;
            }
            else
            {
                Debug.LogWarning("DialogueBoxFixer: Could not find TMP_Text component in DialogueBox children");
            }
        }
        else if (textLabelField != null && textLabelField.objectReferenceValue != null)
        {
            tmpText = textLabelField.objectReferenceValue as TMP_Text;
        }

        // Fix text and background colors for proper contrast
        modified |= FixDialogueColors(dialogueBox, tmpText);


        // Find and set testDialogue (DialogueObject asset) - search by type name since we can't reference it
        var testDialogueField = serializedObj.FindProperty("testDialogue");
        if (testDialogueField != null && testDialogueField.objectReferenceValue == null)
        {
            // Try to find Tutorial.asset by searching for ScriptableObject assets named Tutorial
            var guids = AssetDatabase.FindAssets("Tutorial t:ScriptableObject");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("DialogueData") || path.Contains("Dialogue"))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    if (asset != null && asset.GetType().Name == "DialogueObject")
                    {
                        testDialogueField.objectReferenceValue = asset;
                        Debug.Log($"DialogueBoxFixer: Set testDialogue to '{asset.name}' at '{path}'");
                        modified = true;
                        break;
                    }
                }
            }

            if (testDialogueField.objectReferenceValue == null)
            {
                // Try direct path
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/DialogueData/Tutorial.asset");
                if (asset != null)
                {
                    testDialogueField.objectReferenceValue = asset;
                    Debug.Log($"DialogueBoxFixer: Set testDialogue to '{asset.name}'");
                    modified = true;
                }
                else
                {
                    Debug.LogWarning("DialogueBoxFixer: Could not find Tutorial DialogueObject asset. " +
                        "Please assign it manually in the Inspector.");
                }
            }
        }

        serializedObj.ApplyModifiedProperties();

        // Configure ResponseHandler if needed
        ConfigureResponseHandler(responseHandler, dialogueBox);

        if (modified)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("DialogueBoxFixer: Scene marked dirty. Remember to save the scene!");
        }
        else
        {
            Debug.Log("DialogueBoxFixer: DialogueBox was already properly configured.");
        }
    }

    private static void ConfigureResponseHandler(Component responseHandler, GameObject dialogueBox)
    {
        if (responseHandler == null) return;

        var serializedObj = new SerializedObject(responseHandler);

        // Look for ResponseBox child
        var responseBoxField = serializedObj.FindProperty("responseBox");
        if (responseBoxField != null && responseBoxField.objectReferenceValue == null)
        {
            var responseBoxTransform = dialogueBox.transform.Find("ResponseBox");
            if (responseBoxTransform != null)
            {
                responseBoxField.objectReferenceValue = responseBoxTransform;
                Debug.Log("DialogueBoxFixer: Set ResponseHandler.responseBox");
            }
        }

        // Look for ResponseContainer child
        var responseContainerField = serializedObj.FindProperty("responseContainer");
        if (responseContainerField != null && responseContainerField.objectReferenceValue == null)
        {
            var responseContainerTransform = dialogueBox.transform.Find("ResponseBox/ResponseContainer");
            if (responseContainerTransform != null)
            {
                responseContainerField.objectReferenceValue = responseContainerTransform;
                Debug.Log("DialogueBoxFixer: Set ResponseHandler.responseContainer");
            }
        }

        serializedObj.ApplyModifiedProperties();
    }

    [MenuItem("Tools/Fix Dialogue Box", true)]
    private static bool ValidateFixDialogueBox()
    {
        // Only enable if a scene is loaded
        return EditorSceneManager.GetActiveScene().IsValid();
    }

    private static string GetFullPath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform parent = gameObject.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }

    /// <summary>
    /// Fixes text and background colors for proper contrast.
    /// If background is white/light and text is white, changes text to dark.
    /// If background is dark and text is dark, changes text to white.
    /// </summary>
    private static bool FixDialogueColors(GameObject dialogueBox, TMP_Text tmpText)
    {
        bool modified = false;

        // Find the background Image (first child of DialogueBox or on DialogueBox itself)
        Image backgroundImage = null;

        // Check children first
        foreach (Transform child in dialogueBox.transform)
        {
            backgroundImage = child.GetComponent<Image>();
            if (backgroundImage != null) break;
        }

        // If not found in children, check DialogueBox itself
        if (backgroundImage == null)
        {
            backgroundImage = dialogueBox.GetComponent<Image>();
        }

        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            float bgBrightness = (bgColor.r + bgColor.g + bgColor.b) / 3f;

            // If background is white/light (brightness > 0.7), make it dark
            if (bgBrightness > 0.7f)
            {
                Undo.RecordObject(backgroundImage, "Fix Background Color");
                backgroundImage.color = new Color(0.15f, 0.15f, 0.2f, 0.9f); // Dark blue-gray with slight transparency
                Debug.Log($"DialogueBoxFixer: Changed background from light (brightness={bgBrightness:F2}) to dark for text visibility");
                modified = true;
            }
        }

        // Fix text color based on background
        if (tmpText != null)
        {
            Color textColor = tmpText.color;
            float textBrightness = (textColor.r + textColor.g + textColor.b) / 3f;

            // If text is white/light, make sure background is dark (or change text to dark if background is light)
            if (textBrightness > 0.8f)
            {
                // Text is light - this should be fine with dark background
                // But we changed background to dark, so white text should now be visible
                Debug.Log($"DialogueBoxFixer: Text is light (brightness={textBrightness:F2}), should be visible on dark background");
            }
            else if (textBrightness < 0.3f && backgroundImage != null)
            {
                // Text is dark - check if background is also dark
                Color bgColor = backgroundImage.color;
                float bgBrightness = (bgColor.r + bgColor.g + bgColor.b) / 3f;

                if (bgBrightness < 0.4f)
                {
                    // Both are dark - change text to white
                    Undo.RecordObject(tmpText, "Fix Text Color");
                    tmpText.color = Color.white;
                    Debug.Log("DialogueBoxFixer: Changed text color from dark to white for visibility on dark background");
                    modified = true;
                }
            }
        }

        return modified;
    }

    /// <summary>
    /// Removes DialogueUI, TypewriterEffect, and ResponseHandler components from a GameObject.
    /// These components should only be on DialogueBox, not on its parent Canvas.
    /// Having them on both causes race conditions where both instances try to write to the same text label.
    /// </summary>
    private static bool RemoveDuplicateDialogueComponents(GameObject target, Type dialogueUIType, Type typewriterEffectType, Type responseHandlerType)
    {
        bool modified = false;

        // Remove DialogueUI from target (should only be on DialogueBox)
        var dialogueUI = target.GetComponent(dialogueUIType);
        if (dialogueUI != null)
        {
            Debug.Log($"DialogueBoxFixer: Removing duplicate DialogueUI from '{target.name}' (should only be on DialogueBox)");
            Undo.DestroyObjectImmediate(dialogueUI);
            modified = true;
        }

        // Remove TypewriterEffect from target
        var typewriterEffect = target.GetComponent(typewriterEffectType);
        if (typewriterEffect != null)
        {
            Debug.Log($"DialogueBoxFixer: Removing duplicate TypewriterEffect from '{target.name}' (should only be on DialogueBox)");
            Undo.DestroyObjectImmediate(typewriterEffect);
            modified = true;
        }

        // Remove ResponseHandler from target
        var responseHandler = target.GetComponent(responseHandlerType);
        if (responseHandler != null)
        {
            Debug.Log($"DialogueBoxFixer: Removing duplicate ResponseHandler from '{target.name}' (should only be on DialogueBox)");
            Undo.DestroyObjectImmediate(responseHandler);
            modified = true;
        }

        return modified;
    }

    /// <summary>
    /// Removes duplicate TypewriterEffect components from a GameObject, keeping only the first one.
    /// Multiple TypewriterEffect components can cause race conditions during text animation.
    /// </summary>
    private static bool RemoveDuplicateTypewriterEffects(GameObject target, Type typewriterEffectType)
    {
        var typewriterEffects = target.GetComponents(typewriterEffectType);
        if (typewriterEffects.Length <= 1)
        {
            return false;
        }

        Debug.Log($"DialogueBoxFixer: Found {typewriterEffects.Length} TypewriterEffect components on '{target.name}', removing {typewriterEffects.Length - 1} duplicate(s)");

        // Keep the first one, remove the rest
        for (int i = 1; i < typewriterEffects.Length; i++)
        {
            Undo.DestroyObjectImmediate(typewriterEffects[i]);
        }

        return true;
    }
}
