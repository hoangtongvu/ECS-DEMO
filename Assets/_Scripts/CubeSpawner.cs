using System.Collections;
using System.Collections.Generic;
using Unity.Scenes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeSpawner : MonoBehaviour
{
    
    [SerializeField] private Transform prefabToSpawn;
    [SerializeField] private List<Transform> spawnedObj;
    [SerializeField] private SubScene subScene;
    [SerializeField] private int spawnCount = 100;
    [SerializeField] private float spacing = 3f;


    [ContextMenu("Spawn")]
    public void SpawnCubes()
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(spawnCount));
        float offset = spacing * 0.5f; // Adjust for center alignment

        for (int i = 0; i < spawnCount; i++)
        {
            int row = i / rows;
            int col = i % rows;

            Vector3 position = transform.position + new Vector3(
                col * spacing - offset,
                0,
                row * spacing - offset
            );

            Transform prefab = Instantiate(prefabToSpawn, position, Quaternion.identity);
            prefab.gameObject.SetActive(true);
            //this.spawnedObj.Add(prefab);

            //UnityEditor.SceneManagement.EditorSceneManager.OpenScene(m_subScene.EditableScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
            EditorSceneManager.MoveGameObjectToScene(prefab.gameObject, this.subScene.EditingScene);
        }
        EditorSceneManager.SaveScene(subScene.EditingScene);
    }

    [ContextMenu("Destroy All Cubes")]
    public void DestroyAllCubes()
    {
        this.spawnedObj.ForEach(c => DestroyImmediate(c.gameObject));
    }

}
