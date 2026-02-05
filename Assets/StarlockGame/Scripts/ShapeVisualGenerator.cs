using UnityEngine;

public static class ShapeVisualGenerator
{
    private const int TextureSize = 128;
    private const float PixelsPerUnit = 64f;

    public static Texture2D GenerateShapeTexture(ShapeType shapeType)
    {
        Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        
        Color transparent = new Color(0, 0, 0, 0);
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                texture.SetPixel(x, y, transparent);
            }
        }

        switch (shapeType)
        {
            case ShapeType.Circle:
                DrawCircle(texture);
                break;
            case ShapeType.Square:
                DrawSquare(texture);
                break;
            case ShapeType.Triangle:
                DrawTriangle(texture);
                break;
            case ShapeType.Diamond:
                DrawDiamond(texture);
                break;
            case ShapeType.Hexagon:
                DrawHexagon(texture);
                break;
        }

        texture.Apply();
        return texture;
    }

    public static Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            PixelsPerUnit
        );
    }

    public static Vector2[] GetColliderPoints(ShapeType shapeType)
    {
        float size = 1.8f;

        switch (shapeType)
        {
            case ShapeType.Circle:
                return GenerateCirclePoints(16, size * 0.5f);
            case ShapeType.Square:
                return new Vector2[]
                {
                    new Vector2(-size * 0.5f, -size * 0.5f),
                    new Vector2(-size * 0.5f, size * 0.5f),
                    new Vector2(size * 0.5f, size * 0.5f),
                    new Vector2(size * 0.5f, -size * 0.5f)
                };
            case ShapeType.Triangle:
                float h = size * 0.866f;
                return new Vector2[]
                {
                    new Vector2(0, h * 0.5f),
                    new Vector2(-size * 0.5f, -h * 0.5f),
                    new Vector2(size * 0.5f, -h * 0.5f)
                };
            case ShapeType.Diamond:
                return new Vector2[]
                {
                    new Vector2(0, size * 0.5f),
                    new Vector2(-size * 0.5f, 0),
                    new Vector2(0, -size * 0.5f),
                    new Vector2(size * 0.5f, 0)
                };
            case ShapeType.Hexagon:
                return GenerateHexagonPoints(size * 0.5f);
            default:
                return GenerateCirclePoints(16, size * 0.5f);
        }
    }

    private static Vector2[] GenerateCirclePoints(int segments, float radius)
    {
        Vector2[] points = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            points[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }
        return points;
    }

    private static Vector2[] GenerateHexagonPoints(float radius)
    {
        Vector2[] points = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = (i / 6f) * Mathf.PI * 2f + Mathf.PI / 6f;
            points[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }
        return points;
    }

    private static void DrawCircle(Texture2D texture)
    {
        float center = TextureSize / 2f;
        float radius = TextureSize * 0.45f;

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radius)
                {
                    float alpha = Mathf.Clamp01((radius - dist) / 2f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
        }
    }

    private static void DrawSquare(Texture2D texture)
    {
        int margin = (int)(TextureSize * 0.1f);
        int size = TextureSize - margin * 2;

        for (int y = margin; y < margin + size; y++)
        {
            for (int x = margin; x < margin + size; x++)
            {
                float edgeDist = Mathf.Min(
                    Mathf.Min(x - margin, margin + size - 1 - x),
                    Mathf.Min(y - margin, margin + size - 1 - y)
                );
                float alpha = Mathf.Clamp01(edgeDist / 2f);
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }
    }

    private static void DrawTriangle(Texture2D texture)
    {
        float center = TextureSize / 2f;
        float size = TextureSize * 0.8f;
        
        Vector2 p1 = new Vector2(center, center + size * 0.4f);
        Vector2 p2 = new Vector2(center - size * 0.45f, center - size * 0.35f);
        Vector2 p3 = new Vector2(center + size * 0.45f, center - size * 0.35f);

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInTriangle(p, p1, p2, p3))
                {
                    float dist = DistanceToTriangleEdge(p, p1, p2, p3);
                    float alpha = Mathf.Clamp01(dist / 2f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
        }
    }

    private static void DrawDiamond(Texture2D texture)
    {
        float center = TextureSize / 2f;
        float size = TextureSize * 0.45f;

        Vector2 top = new Vector2(center, center + size);
        Vector2 bottom = new Vector2(center, center - size);
        Vector2 left = new Vector2(center - size, center);
        Vector2 right = new Vector2(center + size, center);

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInQuad(p, top, left, bottom, right))
                {
                    float dist = DistanceToQuadEdge(p, top, left, bottom, right);
                    float alpha = Mathf.Clamp01(dist / 2f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
        }
    }

    private static void DrawHexagon(Texture2D texture)
    {
        float center = TextureSize / 2f;
        float radius = TextureSize * 0.45f;

        Vector2[] vertices = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = (i / 6f) * Mathf.PI * 2f + Mathf.PI / 6f;
            vertices[i] = new Vector2(
                center + Mathf.Cos(angle) * radius,
                center + Mathf.Sin(angle) * radius
            );
        }

        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInPolygon(p, vertices))
                {
                    float dist = DistanceToPolygonEdge(p, vertices);
                    float alpha = Mathf.Clamp01(dist / 2f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = Sign(p, p1, p2);
        float d2 = Sign(p, p2, p3);
        float d3 = Sign(p, p3, p1);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    private static bool PointInQuad(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        return PointInTriangle(p, p1, p2, p3) || PointInTriangle(p, p1, p3, p4);
    }

    private static bool PointInPolygon(Vector2 p, Vector2[] vertices)
    {
        int n = vertices.Length;
        bool inside = false;
        
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            if (((vertices[i].y > p.y) != (vertices[j].y > p.y)) &&
                (p.x < (vertices[j].x - vertices[i].x) * (p.y - vertices[i].y) / (vertices[j].y - vertices[i].y) + vertices[i].x))
            {
                inside = !inside;
            }
        }
        
        return inside;
    }

    private static float DistanceToTriangleEdge(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = DistanceToLine(p, p1, p2);
        float d2 = DistanceToLine(p, p2, p3);
        float d3 = DistanceToLine(p, p3, p1);
        return Mathf.Min(d1, Mathf.Min(d2, d3));
    }

    private static float DistanceToQuadEdge(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = DistanceToLine(p, p1, p2);
        float d2 = DistanceToLine(p, p2, p3);
        float d3 = DistanceToLine(p, p3, p4);
        float d4 = DistanceToLine(p, p4, p1);
        return Mathf.Min(Mathf.Min(d1, d2), Mathf.Min(d3, d4));
    }

    private static float DistanceToPolygonEdge(Vector2 p, Vector2[] vertices)
    {
        float minDist = float.MaxValue;
        int n = vertices.Length;
        
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            float dist = DistanceToLine(p, vertices[i], vertices[j]);
            minDist = Mathf.Min(minDist, dist);
        }
        
        return minDist;
    }

    private static float DistanceToLine(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab));
        Vector2 closest = a + t * ab;
        return Vector2.Distance(p, closest);
    }
}
