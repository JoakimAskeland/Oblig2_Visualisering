using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public string fileName = "triangleData.txt";
    public Material material;
    public List<Vector3> vertices = new List<Vector3>();

    private void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

           

            foreach (string line in lines)
            {
                string[] coords = line.Split(' '); // This is if we use 'space' to seperate vertices in the txt file

                if (coords.Length == 3) 
                {
                    float x = float.Parse(coords[0]);
                    float y = float.Parse(coords[1]);
                    float z = float.Parse(coords[2]);
                    vertices.Add(new Vector3(x, y, z));
                }
            }

            if (vertices.Count % 3 == 0) 
            {
                Mesh mesh = new Mesh();
                mesh.vertices = vertices.ToArray();
                int[] triangles = new int[vertices.Count];

                for (int i = 0; i < vertices.Count; i++) 
                {
                    triangles[i] = i;
                }

                mesh.triangles = triangles;

                GameObject meshObject = new GameObject("CustomMesh");
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();   
                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                mesh.RecalculateNormals();
            }
            else
            {
                Debug.LogError("Vertex count is not a multiple of 3.");
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }



    //Mesh m;
    //MeshFilter mf;

    //Mesh b;
    //MeshFilter bf;



    //// Start is called before the first frame update
    //void Start()
    //{
    //    mf = GetComponent<MeshFilter>();
    //    m = new Mesh();
    //    mf.mesh = m;

    //    bf = GetComponent<MeshFilter>();
    //    b = new Mesh();
    //    bf.mesh = b;

    //    drawTriangle();
    //}

    //void drawTriangle()
    //{
    //    // Holds the vertices
    //    Vector3[] VerticesArray = new Vector3[3];
    //    // Holds the triangles
    //    int[] trianglesArray = new int[3];

    //    // 3 vertices in 3D space
    //    VerticesArray[0] = new Vector3(0, 1, 0);
    //    VerticesArray[1] = new Vector3(-1, 0, 0);
    //    VerticesArray[2] = new Vector3(1, 0, 0);

    //    // Defining the order in which the vertices in VerticesArray should be used
    //    trianglesArray[0] = 0;
    //    trianglesArray[1] = 1;
    //    trianglesArray[2] = 2;

    //    // Adding these two triangles to the mesh
    //    m.vertices = VerticesArray;
    //    m.triangles = trianglesArray;


    /*----- Makes a giant triangle -----*/

    // Back side of triangle
    //Vector3[] VerticesArray2 = new Vector3[3];
    //int[] trianglesArray2 = new int[3];

    //VerticesArray2[0] = new Vector3(0, 1, 0);
    //VerticesArray2[1] = new Vector3(-1, 0, 0);
    //VerticesArray2[2] = new Vector3(1, 0, 0);

    //trianglesArray2[0] = 2;
    //trianglesArray2[1] = 1;
    //trianglesArray2[2] = 0;

    //b.vertices = VerticesArray2;
    //b.triangles = trianglesArray2;
}

