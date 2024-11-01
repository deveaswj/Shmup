using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveConfigSO))]
public class WaveConfigEditor : Editor
{
    private SerializedObject serializedWave;

    // SerializedProperties for each field
    private SerializedProperty isBossWave;
    private SerializedProperty enemyPrefabs;
    private SerializedProperty pathPrefab;
    private SerializedProperty flipX;
    private SerializedProperty flipY;
    private SerializedProperty rotationAngle;
    private SerializedProperty moveSpeed;
    private SerializedProperty reverseOrder;
    private SerializedProperty timeBetweenSpawns;
    private SerializedProperty spawnTimeVariance;
    private SerializedProperty minimumSpawnTime;
    private SerializedProperty loops;

    private void OnEnable()
    {
        // Initialize the serialized object and properties
        serializedWave = new SerializedObject(target);

        isBossWave = serializedWave.FindProperty("_isBossWave");
        enemyPrefabs = serializedWave.FindProperty("enemyPrefabs");
        pathPrefab = serializedWave.FindProperty("pathPrefab");
        flipX = serializedWave.FindProperty("flipX");
        flipY = serializedWave.FindProperty("flipY");
        rotationAngle = serializedWave.FindProperty("rotationAngle");
        moveSpeed = serializedWave.FindProperty("moveSpeed");
        reverseOrder = serializedWave.FindProperty("reverseOrder");
        timeBetweenSpawns = serializedWave.FindProperty("timeBetweenSpawns");
        spawnTimeVariance = serializedWave.FindProperty("spawnTimeVariance");
        minimumSpawnTime = serializedWave.FindProperty("minimumSpawnTime");
        loops = serializedWave.FindProperty("_loops");
    }

    public override void OnInspectorGUI()
    {
        // Update serialized object representation
        serializedWave.Update();

        // Draw each property with headers
        EditorGUILayout.LabelField(".Enemies", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isBossWave);
        EditorGUILayout.PropertyField(enemyPrefabs);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(".Path", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(pathPrefab);
        EditorGUILayout.PropertyField(flipX);
        EditorGUILayout.PropertyField(flipY);

        // Draw rotationAngle as a slider with the specified range
        EditorGUILayout.Slider(rotationAngle, 0f, 180f, new GUIContent("Rotation Angle"));
        // EditorGUILayout.PropertyField(rotationAngle);

        EditorGUILayout.PropertyField(reverseOrder);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(".Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(moveSpeed);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(".Spawning", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(timeBetweenSpawns);
        EditorGUILayout.PropertyField(spawnTimeVariance);
        EditorGUILayout.PropertyField(minimumSpawnTime);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(".Looping", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(loops);

        // Check if pathPrefab is assigned
        if (pathPrefab.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Assign a pathPrefab to calculate wave times.", MessageType.Warning);
        }
        else
        {
            // Button to recalculate waypoints when necessary
            if (GUILayout.Button("Recalculate Waypoints"))
            {
                ((WaveConfigSO)target).CreateWaypoints();
            }

            // Display calculated wave timing range
            var enemyWave = (WaveConfigSO)target;
            var (minTime, maxTime, avgTime) = enemyWave.CalculateWaveTimeRange();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave Timing Range", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Minimum Time", $"{minTime:F2} seconds");
            EditorGUILayout.LabelField("Maximum Time", $"{maxTime:F2} seconds");
            EditorGUILayout.LabelField("Average Time", $"{avgTime:F2} seconds");
        }

        // Apply changes to serialized properties
        serializedWave.ApplyModifiedProperties();
    }
}
