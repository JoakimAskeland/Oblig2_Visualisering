using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMovement : MonoBehaviour
{
    public GameObject ball;
    public GameObject trianglePlane;
    public List<Vector3> trianglePoints;

    [SerializeField] private int TriangleNumber = 1;
    private Vector3 triangleNormal;
    [SerializeField] private Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public float mass = 1f;
    [SerializeField] private Vector3 acceleration = Vector3.zero;
    [SerializeField] private Vector3 velocity = new Vector3(40f, 1f, 40f);
    float radius = 0.15f;
    private Vector3 previousNormal;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Sphere");
        trianglePlane = GameObject.Find("TriangleFrom_txt");

        if (trianglePlane != null)
        {
            trianglePoints = trianglePlane.GetComponent<NewBehaviourScript>().vertices;
        }
        else { Debug.LogError("trianglePlane GameObject not found"); }

        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

        previousNormal = Vector3.up;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        barycentricCoords();

        // Visualization of some vectors on the ball
        Debug.DrawRay(transform.position, acceleration, Color.yellow);
        Debug.DrawRay(transform.position, velocity, Color.green);
    }

    void barycentricCoords()
    {
        Vector3 ballPosition = transform.position;

        for (int i = 0; i < trianglePoints.Count; i += 3)
        {
            Vector3 v0 = Vector3.zero;
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;


            v0 = trianglePoints[i];
            v1 = trianglePoints[i + 1];
            v2 = trianglePoints[i + 2];

            if (TriangleNumber < trianglePoints.Count / 3)
            {
                //Debug.Log("Triangle " + TriangleNumber);
                TriangleNumber++;
            }

            Debug.Log("v0: " + v0 + " | v1: " + v1 + " | v2: " + v2);
            //Debug.Log("i: " + i);

            Vector3 v0v1 = v1 - v0;
            Vector3 v0v2 = v2 - v0;
            Vector3 v0bp = ballPosition - v0;

            Debug.Log("v0v1: " + v0v1 + " | v0v2: " + v0v2);

            float dot00 = Vector3.Dot(v0v1, v0v1);
            float dot01 = Vector3.Dot(v0v1, v0v2);
            float dot02 = Vector3.Dot(v0v1, v0bp);
            float dot11 = Vector3.Dot(v0v2, v0v2);
            float dot12 = Vector3.Dot(v0v2, v0bp);

            float denom = dot00 * dot11 - dot01 * dot01;

            float u = (dot11 * dot02 - dot01 * dot12) / denom;
            float v = (dot00 * dot12 - dot01 * dot02) / denom;
            float w = 1 - u - v;

            //Debug.Log("u: " + u);
            //Debug.Log("v: " + v);
            //Debug.Log("w: " + w);

            Vector3 barycWorldCoord = new Vector3(u, v, w);
            float height = v0.y * w + v1.y * u + v2.y * v;
            //Debug.Log("height: " + height);

            bool isInsideTriangle = (u >= 0f) && (v >= 0f) && (w >= 0f) && (u + v <= 1f);
            
            if (isInsideTriangle)
            {
                //Debug.Log("Ball is inside triangle");

                triangleNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                var triangleNormalnonNormalized = Vector3.Cross(v1 - v0, v2 - v0); 

                // Calculating acceleration
                Vector3 gravityProjection = -Vector3.Dot(gravity, triangleNormal) * triangleNormal;
                Debug.Log("gravityp: " + gravityProjection);
                //Debug.Log("gravity: " + gravity);
                Debug.Log("TriangleNormal: " + triangleNormal);
                Debug.Log("TriangleNormalnonNormalized: " + triangleNormalnonNormalized);
                //Debug.Log("TriangleNumber: " + TriangleNumber);
                acceleration = gravityProjection + gravity;

                Debug.Log("Acceleration: " + acceleration + " | Velocity: " + velocity);
                //Debug.Log("velocity: " + velocity);

                var newVelocity = velocity + acceleration * Time.deltaTime;
                velocity = newVelocity;

                var newPosition = ballPosition + velocity * Time.deltaTime;
                ballPosition = newPosition;
                
                //transform.position = ballPosition;
                transform.position = new Vector3(newPosition.x, height + radius - 0.03f, newPosition.z);


                // New triangle
                if (!sameNormal(triangleNormal, previousNormal))
                {
                    Debug.Log("New triangle");

                    previousNormal = triangleNormal;


                }
            }
            else
            {
                //Debug.Log("Ball is outside triangle");

                transform.position += gravity * Time.deltaTime;
            }
            //isInsideTriangle = false;

        }
    }

    bool sameNormal(Vector3 normalA, Vector3 normalB)
    {
        float angleTolerance = 0.1f;
        float angle = Vector3.Angle(normalA, normalB);

        return angle <= angleTolerance;
    }
}
//x3.3, y0.7, z-1.8
//3.86, y0.827, -1.891
// -3.878, y1, z-1.882