using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections.Generic;
using UnityTemplateProjects;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {
    public const float solidFactor = 0.75f;

    public const float blendFactor = 1f - solidFactor;
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshCollider meshCollider;
    List<Color> colors;
    void Awake () {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        meshCollider = gameObject.AddComponent<MeshCollider>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }
    public void Triangulate (HexCell[] cells) {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        hexMesh.colors = colors.ToArray();
    }

    void Triangulate(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }
  

    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        //AddTriangle(
        //    center,
        //    center + HexMetrics.GetFirstCorner(direction),
        //    center + HexMetrics.GetSecondCorner(direction)
        //);
        AddTriangle(
            center,
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );
        AddTriangleColor(cell.color);

        Vector3 v3 = center + HexMetrics.GetFirstCorner(direction);
        Vector3 v4 = center + HexMetrics.GetSecondCorner(direction);

        AddQuad(v1, v2, v3, v4);
       
        //AddTriangleColor(cell.color);
        HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;
        //AddTriangleColor(
        //    cell.color,
        //    (cell.color + prevNeighbor.color + neighbor.color) / 3f,
        //    (cell.color + neighbor.color + nextNeighbor.color) / 3f
        //);
        AddQuadColor(
           cell.color,
           cell.color,
           (cell.color + prevNeighbor.color + neighbor.color) / 3f,
           (cell.color + neighbor.color + nextNeighbor.color) / 3f
       );
    }
    //void Triangulate (HexCell cell) {
    //    Vector3 center = cell.transform.localPosition;
    //    // AddTriangle(
    //    //     center,
    //    //     center + HexMetrics.corners[0],
    //    //     center + HexMetrics.corners[1]
    //    // );
    //    // Vector3 center = cell.transform.localPosition;
    //    for (int i = 0; i < 6; i++) {
    //        AddTriangle(
    //            center,
    //            center + HexMetrics.corners[i],
    //            center + HexMetrics.corners[i + 1]
    //        );
    //        AddTriangleColor(cell.color);
    //    }
    //    meshCollider.sharedMesh = hexMesh;
    //}
    void AddTriangleColor (Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }
    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
}
