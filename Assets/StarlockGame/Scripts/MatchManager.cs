using UnityEngine;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CircleContainer circleContainer;

    [Header("Settings")]
    [SerializeField] private float matchCheckDelay = 0.1f;
    [SerializeField] private int pointsPerMatch = 100;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private List<Shape> shapesInsideCircle = new List<Shape>();
    private bool isCheckingMatches = false;

    public System.Action<Shape, Shape, int> OnMatchFound;
    public System.Action<int> OnScoreChanged;

    private int currentScore = 0;

    public int CurrentScore => currentScore;

    private void Start()
    {
        if (circleContainer != null)
        {
            circleContainer.OnShapeCountChanged += OnCircleShapeCountChanged;
        }
    }

    private void OnDestroy()
    {
        if (circleContainer != null)
        {
            circleContainer.OnShapeCountChanged -= OnCircleShapeCountChanged;
        }
    }

    private void OnCircleShapeCountChanged(int count)
    {
        if (!isCheckingMatches)
        {
            Invoke(nameof(CheckForMatches), matchCheckDelay);
        }
    }

    public void RegisterShapeInside(Shape shape)
    {
        if (shape == null) return;
        if (shapesInsideCircle.Contains(shape)) return;

        shapesInsideCircle.Add(shape);
        shape.OnMatched += OnShapeMatched;

        if (showDebugInfo) Debug.Log($"Shape registered inside: {shape.Type} {shape.Color}. Total: {shapesInsideCircle.Count}");

        if (!isCheckingMatches)
        {
            Invoke(nameof(CheckForMatches), matchCheckDelay);
        }
    }

    public void UnregisterShape(Shape shape)
    {
        if (shape == null) return;

        shapesInsideCircle.Remove(shape);
        shape.OnMatched -= OnShapeMatched;
    }

    private void OnShapeMatched(Shape shape)
    {
        shapesInsideCircle.Remove(shape);

        if (circleContainer != null)
        {
            circleContainer.RemoveShapeInside(shape.gameObject);
        }
    }

    private void CheckForMatches()
    {
        isCheckingMatches = true;

        shapesInsideCircle.RemoveAll(s => s == null || s.State == ShapeState.Matched);

        bool foundMatch = false;

        for (int i = 0; i < shapesInsideCircle.Count; i++)
        {
            for (int j = i + 1; j < shapesInsideCircle.Count; j++)
            {
                Shape shape1 = shapesInsideCircle[i];
                Shape shape2 = shapesInsideCircle[j];

                if (shape1 == null || shape2 == null) continue;
                if (shape1.State != ShapeState.Inside || shape2.State != ShapeState.Inside) continue;

                if (shape1.Matches(shape2))
                {
                    if (showDebugInfo) Debug.Log($"Match found! {shape1.Type} {shape1.Color}");
                    
                    ProcessMatch(shape1, shape2);
                    foundMatch = true;
                    break;
                }
            }

            if (foundMatch) break;
        }

        isCheckingMatches = false;

        if (foundMatch)
        {
            Invoke(nameof(CheckForMatches), matchCheckDelay * 2f);
        }
    }

    private void ProcessMatch(Shape shape1, Shape shape2)
    {
        shapesInsideCircle.Remove(shape1);
        shapesInsideCircle.Remove(shape2);

        shape1.OnMatched -= OnShapeMatched;
        shape2.OnMatched -= OnShapeMatched;

        currentScore += pointsPerMatch;
        OnScoreChanged?.Invoke(currentScore);
        OnMatchFound?.Invoke(shape1, shape2, pointsPerMatch);

        shape1.SetState(ShapeState.Matched);
        shape2.SetState(ShapeState.Matched);
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void ClearAll()
    {
        foreach (Shape shape in shapesInsideCircle)
        {
            if (shape != null)
            {
                shape.OnMatched -= OnShapeMatched;
            }
        }
        shapesInsideCircle.Clear();
    }

    public int GetShapesInsideCount()
    {
        shapesInsideCircle.RemoveAll(s => s == null);
        return shapesInsideCircle.Count;
    }

    public List<Shape> GetShapesInside()
    {
        shapesInsideCircle.RemoveAll(s => s == null);
        return new List<Shape>(shapesInsideCircle);
    }
}
