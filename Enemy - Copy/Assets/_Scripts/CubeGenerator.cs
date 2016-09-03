using UnityEngine;
using System.Collections;

public class CubeGenerator : MonoBehaviour
{
    public Transform g_tilePrefab;
    public Vector2 g_mapSize;
    Color[] colors = new Color[3];
    int randomColour;

    [Range(0, 1)]
    public float g_outLinePercent;
    void Start()
    {
        colors[0] = Color.red;
        colors[1] = Color.white;
        colors[2] = Color.white;
        GenerateMap();
    }
    public void GenerateMap()
    {

        for (int x = 0; x < g_mapSize.x; x++)
        {
            
            for (int y = 0; y < g_mapSize.y; y++)
            {
                Vector3 _tilePosition = new Vector3(-g_mapSize.x / 2 + 1f + x, 0, -g_mapSize.y / 2 + 1f + y);
                Transform _newTile = Instantiate(g_tilePrefab, _tilePosition, Quaternion.Euler(Vector3.right)) as Transform;
                _newTile.localScale = new Vector3(( - g_outLinePercent),0.1f, ( - g_outLinePercent));
                randomColour = Random.Range(0, 3);
                _newTile.GetComponent<MeshRenderer>().material.color = colors[randomColour];
                _newTile.parent = transform;
            }
        }
    }
}
