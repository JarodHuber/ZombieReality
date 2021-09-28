using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{
    public Transform lineStart, lineEnd, objectToProject, projectionExample;

    private void Update()
    {
        float u = Vector3.Dot(lineStart.position - objectToProject.position, lineStart.position - lineEnd.position);
             u /= Vector3.Dot(lineStart.position - lineEnd.position,       lineStart.position - lineEnd.position);

        u = Mathf.Clamp(u, 0.0f, 1.0f);

        print(u);
        print(lineEnd.position + (u * (lineStart.position - lineEnd.position)));
        projectionExample.position = lineStart.position + (u * (lineEnd.position - lineStart.position));
    }
}
