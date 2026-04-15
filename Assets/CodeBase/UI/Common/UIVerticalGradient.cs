using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Vertical Gradient")]
public class UIVerticalGradient : BaseMeshEffect
{
    public Color colorLeft = Color.white;
    public Color colorRight = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        float minX = GetMinX(vertices);
        float width = GetWidth(vertices);

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex v = vertices[i];

            float t = (v.position.x - minX) / width;
            v.color = Color.Lerp(colorLeft, colorRight, t);
            vertices[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }

    private float GetMinX(List<UIVertex> vertices)
    {
        float min = vertices[0].position.x;
        for (int i = 1; i < vertices.Count; i++)
            if (vertices[i].position.x < min) min = vertices[i].position.x;
        return min;
    }

    private float GetWidth(List<UIVertex> vertices)
    {
        float min = GetMinX(vertices);
        float max = vertices[0].position.x;
        for (int i = 1; i < vertices.Count; i++)
            if (vertices[i].position.x > max) max = vertices[i].position.x;
        return max - min;
    }

}
