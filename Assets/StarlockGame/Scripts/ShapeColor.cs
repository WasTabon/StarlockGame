using UnityEngine;

public enum ShapeColor
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}

public static class ShapeColorExtensions
{
    public static Color ToColor(this ShapeColor shapeColor)
    {
        switch (shapeColor)
        {
            case ShapeColor.Red:
                return new Color(1f, 0.42f, 0.42f);
            case ShapeColor.Blue:
                return new Color(0.31f, 0.8f, 0.77f);
            case ShapeColor.Green:
                return new Color(0.66f, 0.9f, 0.81f);
            case ShapeColor.Yellow:
                return new Color(1f, 0.9f, 0.43f);
            case ShapeColor.Purple:
                return new Color(0.77f, 0.3f, 1f);
            default:
                return Color.white;
        }
    }
}
