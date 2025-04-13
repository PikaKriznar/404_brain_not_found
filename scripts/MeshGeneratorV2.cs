
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorV2 : MonoBehaviour
{
    Mesh mesh;
    public int MESH_SCALE = 1;
    public GameObject[] objects;
    public GameObject[] objectsToPlace;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] public ObjectSpawnNotifier notifier;
    private Vector3[] vertices;
    private int[] triangles;
    
    private Color[] colors;
    [SerializeField] private Gradient gradient;
    
    private float minTerrainheight;
    private float maxTerrainheight;

    public int xSize;
    public int zSize;

    public float scale; 
    public int octaves;
    public float lacunarity;

    public int seed;

    private float lastNoiseHeight;

    public NavMeshSurface navm;
    public int rand = 2;
    public float hight;

    int points = 0;
    string environmentState = "Neutral";

    public TMP_InputField profession;

    private Coroutine colorTransitionCoroutine;

    public Material skyboxmat;

    public Texture origtex;
    public Texture helltex;
    public Material grass;
    public Material hell;

    public GameObject gm;

    void Start()
    {
        // Use this method if you havn't filled out the properties in the inspector
        // SetNullProperties(); 

        mesh = new Mesh();
        GetComponent<MeshRenderer>().material = grass;

        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
        navm.BuildNavMesh();
        skyboxmat.mainTexture = origtex;
    }

    private void SetNullProperties() 
    {
        if (xSize <= 0) xSize = 50;
        if (zSize <= 0) zSize = 50;
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 2;
        if (scale <= 0) scale = 50;
    } 

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        ColorTerrain();
        UpdateMesh();
    }

    private void ColorTerrain()
    {
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
        }
        mesh.colors = colors;
    }

    private void CreateMeshShape ()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0) scale = 0.0001f;
            
        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                SetMinMaxHeights(noiseHeight);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        seed = Random.Range(0, 1000);
        
        // changes area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
                    
        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 20;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            //The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }


    private void CreateTriangles() 
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        // Go to next row
        for (int z = 0; z < zSize; z++)
        {
            // fill row
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void MapEmbellishments() 
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > hight)
                {
                    // Chance to generate
                    if (Random.Range(1, rand) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight * 2;
                        Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.Euler(0,Random.Range(0,360),0));
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    public void updateState() {
        if (points <= -100) {
            gm.SetActive(true);
            notifier.NotifyGodAction(environmentState, "You failed. Give a review of your horrible actions when placing a bunch of zombie bunnies to kill the character. Not too long no most than 6 sentences.", points, profession.text);
        }
        else if (points <= -50) {
            environmentState = "Hell";
            TransitionToColor(Color.red, 2f);
            skyboxmat.mainTexture = helltex;
            GetComponent<MeshRenderer>().material = hell;
        } else if (points >= 50) {
            environmentState = "Heaven";
        } else {
            environmentState = "Neutral";
            TransitionToGradient(gradient, 2f);
        }
    }

    public void PlaceBurger(){
        PlaceObject(0);
        points += 10;
        updateState();
        Debug.Log("" + profession.text);
        notifier.NotifyGodAction(environmentState, "A juicy burger appears in front of you.", points, profession.text);
    }

    public void PlaceRedbull(){
        PlaceObject(1);
        points += 5;
        updateState();
        notifier.NotifyGodAction(environmentState, "A chilled can of Redbull snaps into existence.", points, profession.text);
    }

    public void PlaceZombie(){
        PlaceObject(2);
        points -= 10;
        updateState();
        notifier.NotifyGodAction(environmentState, "A zombie rises from the ground and starts chasing you!", points, profession.text);
    }

    public void PlaceMushroom(){
        PlaceObject(3);
        if (Random.Range(0,2) == 0){
            points -= 10;
            updateState();
            notifier.NotifyGodAction(environmentState, "A funky mushroom spawns it looks really wierd.", points, profession.text);
        }
        else{
            points += 10;
            updateState();
            notifier.NotifyGodAction(environmentState, "A tasty looking mushroom with unpredictable effects spawns.", points, profession.text);
        }
    }

    private void PlaceObject(int id) 
    {
        int r = Random.Range(1000,6000);
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > hight)
                {
                    // Chance to generate
                    if (Random.Range(1, r) == 1)
                    {
                        GameObject objectToSpawn = objectsToPlace[id];
                        var spawnAboveTerrainBy = noiseHeight * 2;
                        Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity);
                        return;
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);

        MapEmbellishments();
    }

    private IEnumerator LerpMeshColors(Color[] startColors, Color[] endColors, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.Lerp(startColors[i], endColors[i], t);
            }

            mesh.colors = colors;
            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = endColors[i];
        }
        mesh.colors = colors;
    }

    private void TransitionToColor(Color targetColor, float duration = 2f)
    {
        Color[] startColors = mesh.colors;
        Color[] endColors = new Color[startColors.Length];
        for (int i = 0; i < endColors.Length; i++)
        {
            endColors[i] = targetColor;
        }

        if (colorTransitionCoroutine != null)
            StopCoroutine(colorTransitionCoroutine);

        colorTransitionCoroutine = StartCoroutine(LerpMeshColors(startColors, endColors, duration));
    }

    private void TransitionToGradient(Gradient targetGradient, float duration = 2f)
    {
        Color[] startColors = mesh.colors;
        Color[] endColors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            endColors[i] = targetGradient.Evaluate(height);
        }

        if (colorTransitionCoroutine != null)
            StopCoroutine(colorTransitionCoroutine);

        colorTransitionCoroutine = StartCoroutine(LerpMeshColors(startColors, endColors, duration));
    }

    public void TryAgain(){
        SceneManager.LoadScene(0);
    }
}