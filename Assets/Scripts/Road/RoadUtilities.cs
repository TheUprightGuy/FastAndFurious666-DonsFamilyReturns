using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadUtilities : MonoBehaviour
{
    public LineRenderer lineRenderer;

    [HideInInspector]
    public LineRenderer LR => (lineRenderer == null) ? (GetComponent<LineRenderer>()) : (lineRenderer);

    /// <summary>
    /// Gets the total world length of the line
    /// </summary>
    /// <returns>The length of the line</returns>
    public float GetLengthOfLine()
    {
        float retLength = 0.0f;
        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        for (int i = 0; i < points.Length - 1; i++)
        {
            retLength += Vector3.Distance(points[i], points[i + 1]);
        }

        return (retLength);
    }

    /// <summary>
    /// Gets a points at <paramref name="_percentageAlongLine"/> percent along line
    /// </summary>
    /// <param name="_percentageAlongLine">The percentage along line, from 0.0f to 1.0f</param>
    /// <returns>The point along the line, Vector3.zero will be returned with an invalid percentage</returns>
    public Vector3 GetPointAlongLine(float _percentageAlongLine)
    {
        float distanceAim = GetLengthOfLine() * _percentageAlongLine;

        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        float retLength = 0.0f;
        for (int i = 0; i < points.Length - 1; i++)
        {
            float newLength = retLength + Vector3.Distance(points[i], points[i + 1]);

            if (newLength >= distanceAim) //Then the desired point is between these two points
            {
                float amountAlongLine = distanceAim - newLength;
                Vector3 dir = (points[i + 1] - points[i]).normalized;
                return points[i] + (dir * amountAlongLine);
            }
            else
            {
                retLength = newLength;
            }
            
        }

        return (Vector3.zero);
    }

    public List<Vector3> GetPointsAtAngle(float _angle)
    {
        List<Vector3> retList = new List<Vector3>();

        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        for (int i = 1; i < points.Length - 1; i++)
        {
            Vector3 dirA = (points[i - 1] - points[i]).normalized;
            Vector3 dirB = (points[i + 1] - points[i]).normalized;

            float angleBetween = 180.0f - Vector3.Angle(dirA, dirB);

            if (angleBetween > _angle)
            {
                retList.Add(points[i]);
            }
        }
        return retList;
    }

    public List<int> GetIndexesAtAngle(float _angle)
    {
        List<int> retList = new List<int>();

        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        for (int i = 1; i < points.Length - 1; i++)
        {
            Vector3 dirA = (points[i - 1] - points[i]).normalized;
            Vector3 dirB = (points[i + 1] - points[i]).normalized;

            float angleBetween = 180.0f - Vector3.Angle(dirA, dirB);

            if (angleBetween > _angle)
            {
                retList.Add(i);
            }
        }
        return retList;
    }
    /// <summary>
    /// Checks if the closest distance to <paramref name="_point"/> is within the width of the line
    /// </summary>
    /// <param name="_point">The point to check</param>
    /// <returns>True if within the line width</returns>
    public bool IsOnLine(Vector3 _point)
    {
        return (GetDistanceToLine(_point) < (LR.startWidth / 2));
    }

    /// <summary>
    /// Gets the distance from <paramref name="_point"/> to the line
    /// </summary>
    /// <param name="_point">Point to get distance to</param>
    /// <returns>The distance to <paramref name="_point"/></returns>
    public float GetDistanceToLine(Vector3 _point)
    {
        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        Vector2 point2D = new Vector2(_point.x, _point.z);

        float closestDist = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;
        for (int i = 0; i < points.Length - 1; i++) //Avoid overflow with plus one
        {
            Vector2 pointA = new Vector2(points[i].x, points[i].z);
            Vector2 pointB = new Vector2(points[i + 1].x, points[i + 1].z);

            Vector2 point = FindNearestPointOnLine(pointA, pointB, point2D);

            float dist = Vector2.Distance(point2D, point);
            if (dist < closestDist) //if closer
            {
                closestDist = dist;
                closestPoint = point;
            }
        }

        return closestDist;
    }

    /// <summary>
    /// Returns the closes point on the line render to <paramref name="_point"/>
    /// </summary>
    /// <param name="_point">The point to get closest to</param>
    /// <returns>Returns the closest point on line, with a flattened Y value</returns>
    public Vector3 GetClosestPointOnLine(Vector3 _point)
    {
        Vector3[] points = new Vector3[LR.positionCount];
        LR.GetPositions(points);

        Vector2 point2D = new Vector2(_point.x, _point.z);

        float closestDist = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;
        for (int i = 0; i < points.Length - 1; i++) //Avoid overflow with plus one
        {
            Vector2 pointA = new Vector2(points[i].x, points[i].z);
            Vector2 pointB = new Vector2(points[i + 1].x, points[i + 1].z);

            Vector2 point = FindNearestPointOnLine(pointA, pointB, point2D);

            float dist = Vector2.Distance(point2D, point);
            if (dist < closestDist) //if closer
            {
                closestDist = dist;
                closestPoint = point;
            }
        }

        return new Vector3(closestPoint.x, 0.0f, closestPoint.y);
    }

    Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }


}