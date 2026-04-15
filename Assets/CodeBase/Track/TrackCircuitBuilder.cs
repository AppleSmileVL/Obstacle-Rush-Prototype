using UnityEngine;

public static class TrackCircuitBuilder
{
    public static TrackPoint[] Build(Transform trackTransform, TrackType type)
    {
        TrackPoint[] points = new TrackPoint[trackTransform.childCount];

        ResetPoints(trackTransform, points);

        if (!ValidatePoints(points))
            return points; 

        MakeLinks(points, type);
        MarkPoint(points, type);

        return points;
    }

    private static void ResetPoints(Transform trackTransform, TrackPoint[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = trackTransform.GetChild(i).GetComponent<TrackPoint>();

            if (points[i] == null)
            {
                Debug.LogError($"TrackPoint component is missing on child '{trackTransform.GetChild(i).name}' at index {i}");
                return;
            }

            points[i].Reset();
        }
    }

    private static bool ValidatePoints(TrackPoint[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogError("Cannot build circuit: TrackPoint array contains null elements. Please check all child objects have TrackPoint component.");
                return false;
            }
        }
        return true;
    }

    private static void MakeLinks(TrackPoint[] points, TrackType type)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            points[i].Next = points[i + 1];
        }

        if (type == TrackType.Circular)
        {
            points[points.Length - 1].Next = points[0];
        }
    }

    private static void MarkPoint(TrackPoint[] points, TrackType type)
    {
        points[0].IsFirst = true;

        if (type == TrackType.Sprint)
        {
            points[points.Length - 1].IsLast = true;
        }
        else if (type == TrackType.Circular)
        {
            points[0].IsLast = true;
        }
    }
}
