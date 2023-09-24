using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    public float gravity = -9.81f;
    public float friction = 0.1f; // Tilpass
    public float mass = 1f;
    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private Vector3 angularVelocity = Vector3.zero;
    [SerializeField] private Vector3 acceleration = Vector3.zero;
    private Vector3 triangleNormal;
    [SerializeField] private int TriangleNumber = 1;

    public GameObject ball;
    public GameObject trianglePlane;
    public List<Vector3> trianglePoints;

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
    }

    // Fixed update instead? Updates at a lower speed
    // Update is called once per frame
    void FixedUpdate()
    {
        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Friction
        Vector3 frictionForce = -velocity.normalized * friction;
        velocity += frictionForce * Time.deltaTime;

        // Direction of the ball's velocity in the plane
        Vector3 planeDirection = Vector3.ProjectOnPlane(velocity, triangleNormal).normalized;
        // Ball's angular velocity when rolling
        angularVelocity = Vector3.Cross(triangleNormal, planeDirection) * 2.0f;

        // Let ball fall based on velocity
        transform.position += velocity * Time.deltaTime;

        // Rotate ball based on angular velocity
        transform.Rotate(angularVelocity * Time.deltaTime, Space.World);

        Vector3 ballPosition = ball.transform.position;
        Vector3 trianglePlanePosition = trianglePlane.transform.position;

        float distanceBallToPlane = Vector3.Distance(ballPosition, trianglePlanePosition);
        // Debug.Log("Distance between ball and plane: " + distanceBallToPlane);

        
        // Barycentric coordinates calculations

            // Running through the list from drawMesh.cs
        for (int i = 0; i < trianglePoints.Count; i += 3)
        {
            Vector3 v0 = trianglePoints[i];
            Vector3 v1 = trianglePoints[i + 1];
            Vector3 v2 = trianglePoints[i + 2];

            if (TriangleNumber < trianglePoints.Count / 3) 
            {
                //Debug.Log("Triangle " + TriangleNumber);
                TriangleNumber++;
            }


            Vector3 v0v1 = v1 - v0;
            Vector3 v0v2 = v2 - v0;
            Vector3 v0bp = ballPosition - v0;

            float dot00 = Vector3.Dot(v0v1, v0v1);
            float dot01 = Vector3.Dot(v0v1, v0v2);
            float dot02 = Vector3.Dot(v0v1, v0bp);
            float dot11 = Vector3.Dot(v0v2, v0v2);
            float dot12 = Vector3.Dot(v0v2, v0bp);

            float denom = dot00 * dot11 - dot01 * dot01;

            float u = (dot11 * dot02 - dot01 * dot12) / denom;
            float v = (dot00 * dot12 - dot01 * dot02) / denom;
            float w = 1 - u - v;

            bool isInsideTriangle = (u >= 0f) && (v >= 0f) && (w >= 0f) && (u + v <= 1f);

            if (isInsideTriangle)
            {
                Debug.Log("Ball is inside triangle");

                triangleNormal = Vector3.Cross(v1 - v0, v2 - v0);
                //float triangleNormalScalar = triangleNormal.magnitude;

                //Debug.Log("triangleNormal: " + triangleNormal);
                //Debug.Log("triangleNormalScalar: " + triangleNormalScalar);
                //Debug.Log("acc1: " + acceleration);

                acceleration = Vector3.Scale(new Vector3(0.0f, gravity, 0.0f), new Vector3(triangleNormal.x * triangleNormal.y, triangleNormal.y * triangleNormal.y - 1f, triangleNormal.z * triangleNormal.y));
                //var acceleration = gravity * new Vector3(triangleNormal.x * triangleNormal.z, triangleNormal.y * triangleNormal.z, triangleNormal.z * triangleNormal.z - 1);
                //acceleration = gravity * new Vector3(triangleNormal.x * triangleNormal.y, triangleNormal.z * triangleNormal.y, triangleNormal.y * triangleNormal.y - 1f);
                //Debug.Log("acc2: " + acceleration);

                var newVelocity = velocity + acceleration * Time.fixedDeltaTime;
                velocity = newVelocity;

                // nyPosisjon = gammelposisjon + oldVelocity * time.fixeddeltatime
                // gammelposisjon = nyposisjon
                // transformposition = nyPosisjon

                var newPosition = ballPosition + velocity * Time.fixedDeltaTime;
                ballPosition = newPosition;
                ball.transform.position = ballPosition;

                if (ballPosition.y < -1f)
                {
                    //acceleration = Vector3.zero;
                    //ball.transform.position.y= -1f; 
                }

            }
            else 
            {
                //Debug.Log("Ball is not inside triangle"); 
                //Vector3 gravity3D = { 0.0f, -9.81f, 0.0f };
                Vector3 force = mass * new Vector3(0f, gravity, 0f);
                Vector3 newPos = transform.position + velocity * Time.fixedDeltaTime;
                velocity += force * Time.fixedDeltaTime;

                transform.position = newPos;
            }

            

        }
    }
}
