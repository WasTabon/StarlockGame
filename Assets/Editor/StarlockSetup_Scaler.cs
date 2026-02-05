using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StarlockSetup_Scaler : EditorWindow
{
    [MenuItem("Starlock/Add Gameplay Scaler")]
    public static void AddGameplayScaler()
    {
        GameplayScaler existingScaler = Object.FindFirstObjectByType<GameplayScaler>();
        if (existingScaler != null)
        {
            Debug.Log("GameplayScaler already exists, updating references...");
            SetupScalerReferences(existingScaler);
            return;
        }

        GameObject scalerObj = new GameObject("GameplayScaler");
        GameplayScaler scaler = scalerObj.AddComponent<GameplayScaler>();
        
        SetupScalerReferences(scaler);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        Debug.Log("GameplayScaler added and configured!");
        EditorUtility.DisplayDialog("Done", "GameplayScaler added!\n\nIt will automatically scale the game area to fit 70% of screen width.\n\nZ position of RotationPivot will be preserved.", "OK");
    }

    private static void SetupScalerReferences(GameplayScaler scaler)
    {
        SerializedObject so = new SerializedObject(scaler);

        GameObject rotationPivot = GameObject.Find("RotationPivot");
        if (rotationPivot != null)
        {
            so.FindProperty("rotationPivot").objectReferenceValue = rotationPivot.transform;
            so.FindProperty("pivotZPosition").floatValue = rotationPivot.transform.position.z;
        }
        else
        {
            Debug.LogWarning("RotationPivot not found! Run 'Starlock > Setup Gameplay Objects' first.");
        }

        CircleContainer circleContainer = Object.FindFirstObjectByType<CircleContainer>();
        if (circleContainer != null)
        {
            so.FindProperty("circleContainer").objectReferenceValue = circleContainer;
        }

        OuterZone outerZone = Object.FindFirstObjectByType<OuterZone>();
        if (outerZone != null)
        {
            so.FindProperty("outerZone").objectReferenceValue = outerZone;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            so.FindProperty("mainCamera").objectReferenceValue = mainCamera;
        }

        so.FindProperty("horizontalPadding").floatValue = 0.15f;
        so.FindProperty("baseCircleRadius").floatValue = 2f;
        so.FindProperty("baseOuterRadius").floatValue = 4f;
        so.FindProperty("baseInnerRadius").floatValue = 2.2f;

        so.ApplyModifiedProperties();
    }
}
