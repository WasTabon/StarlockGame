using UnityEngine;

[CreateAssetMenu(fileName = "RotationPreset", menuName = "Starlock/Rotation Preset")]
public class RotationPreset : ScriptableObject
{
    [Header("Rotation")]
    [Tooltip("Degrees per second")]
    [SerializeField] private float rotationSpeed = 60f;
    
    [Header("Info")]
    [SerializeField] private string presetName = "Medium";

    public float RotationSpeed => rotationSpeed;
    public string PresetName => presetName;
}
