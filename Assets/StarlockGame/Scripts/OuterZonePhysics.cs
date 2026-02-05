using UnityEngine;
using System.Collections.Generic;

public class OuterZonePhysics : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RotationController rotationController;
    [SerializeField] private OuterZone outerZone;
    [SerializeField] private CircleContainer circleContainer;

    [Header("Force Settings")]
    [SerializeField] private float tangentialForceMultiplier = 2f;
    [SerializeField] private float centrifugalForceMultiplier = 0.5f;
    [SerializeField] private float maxForce = 10f;

    [Header("Damping")]
    [SerializeField] private float velocityDamping = 0.98f;
    [SerializeField] private float maxVelocity = 8f;

    private List<Shape> shapesInZone = new List<Shape>();

    private void FixedUpdate()
    {
        if (rotationController == null) return;

        float rotationSpeed = rotationController.CurrentSpeed;
        if (Mathf.Abs(rotationSpeed) < 0.01f) return;

        ApplyForcesToShapes(rotationSpeed);
    }

    private void ApplyForcesToShapes(float rotationSpeed)
    {
        shapesInZone.RemoveAll(s => s == null);

        foreach (Shape shape in shapesInZone)
        {
            if (shape == null) continue;
            if (shape.State != ShapeState.Outside) continue;

            Rigidbody2D rb = shape.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            Vector2 shapePos = shape.transform.localPosition;
            float distanceFromCenter = shapePos.magnitude;

            if (distanceFromCenter < 0.1f) continue;

            Vector2 directionFromCenter = shapePos.normalized;

            Vector2 tangentDirection;
            if (rotationSpeed > 0)
            {
                tangentDirection = new Vector2(-directionFromCenter.y, directionFromCenter.x);
            }
            else
            {
                tangentDirection = new Vector2(directionFromCenter.y, -directionFromCenter.x);
            }

            float speedFactor = Mathf.Abs(rotationSpeed) / 60f;
            
            Vector2 tangentialForce = tangentDirection * tangentialForceMultiplier * speedFactor;
            Vector2 centrifugalForce = directionFromCenter * centrifugalForceMultiplier * speedFactor * distanceFromCenter;

            Vector2 totalForce = tangentialForce + centrifugalForce;

            if (totalForce.magnitude > maxForce)
            {
                totalForce = totalForce.normalized * maxForce;
            }

            rb.AddForce(totalForce, ForceMode2D.Force);

            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }

            rb.velocity *= velocityDamping;
        }
    }

    public void RegisterShape(Shape shape)
    {
        if (shape != null && !shapesInZone.Contains(shape))
        {
            shapesInZone.Add(shape);
        }
    }

    public void UnregisterShape(Shape shape)
    {
        if (shape != null)
        {
            shapesInZone.Remove(shape);
        }
    }

    public void ClearAllShapes()
    {
        shapesInZone.Clear();
    }

    public void SetForceMultipliers(float tangential, float centrifugal)
    {
        tangentialForceMultiplier = tangential;
        centrifugalForceMultiplier = centrifugal;
    }
}
