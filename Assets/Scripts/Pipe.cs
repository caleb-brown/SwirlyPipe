using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

    public float pipeRadius = 1;
    public int pipeSegmentCount = 10;
    public float ringDistance = 1;
    public float minCurveRadius = 4, maxCurveRadius = 20;
    public int minCurveSegmentCount = 4, maxCurveSegmentCount = 10;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    float curveAngle, curveRadius;
    int curveSegmentCount;

    public float CurveRadius
    {
        get
        {
            return curveRadius;
        }
    }

    public float CurveAngle
    {
        get
        {
            return curveAngle;
        }
    }

    public Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p;
        float r = (curveRadius + pipeRadius * Mathf.Cos(v));

        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);

        return p;
    }

    public void AlignWith(Pipe pipe)
    {
        float relativeRotation = Random.Range(0.0f, curveSegmentCount) * 360.0f / pipeSegmentCount;

        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -pipe.curveAngle);
        transform.Translate(0.0f, pipe.curveRadius, 0.0f);
        transform.Rotate(relativeRotation, 0.0f, 0.0f);
        transform.Translate(0.0f, -curveRadius, 0.0f);
        transform.SetParent(pipe.transform.parent);
    }

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";

        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount);

        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
    }

    void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        float uStep = ringDistance / curveRadius;
        curveAngle = uStep * curveSegmentCount * (360.0f / (2.0f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    void SetTriangles()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];

        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }

    void CreateFirstQuadRing(float u)
    {
        float vStep = (2.0f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0.0f, 0.0f);
        Vector3 vertexB = GetPointOnTorus(u, 0.0f);

        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0.0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    void CreateQuadRing(float u, int i)
    {
        float vStep = (2.0f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0.0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    /*
    void OnDrawGizmos()
    {
        float uStep = (2.0f * Mathf.PI) / curveSegmentCount;
        float vStep = (2.0f * Mathf.PI) / pipeSegmentCount;

        for (int u = 0; u < curveSegmentCount; u++)
        {
            for (int v = 0; v < pipeSegmentCount; v++)
            {
                Vector3 point = GetPointOnTorus(u * uStep, v * vStep);
                Gizmos.color = new Color(
                            1.0f, 
                            (float)v / pipeSegmentCount, 
                            (float)u / curveSegmentCount);
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
    */
}
