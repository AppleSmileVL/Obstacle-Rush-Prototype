using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/Horizontal Gradient")]
public class UIHorizontalGradient : BaseMeshEffect
{
    public Color colorTop = Color.white;
    public Color colorBottom = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex v = vertices[i];
            
            v.color = Color.Lerp(colorBottom, colorTop, (v.position.y - GetMinY(vertices)) / GetHeight(vertices));
            vertices[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }

    private float GetMinY(List<UIVertex> vertices)
    {
        float min = vertices[0].position.y;
        for (int i = 1; i < vertices.Count; i++) if (vertices[i].position.y < min) min = vertices[i].position.y;
        return min;
    }

    private float GetHeight(List<UIVertex> vertices)
    {
        float min = GetMinY(vertices);
        float max = vertices[0].position.y;
        for (int i = 1; i < vertices.Count; i++) if (vertices[i].position.y > max) max = vertices[i].position.y;
        return max - min;
    }
}