using EcoGarden.Utilities;
using UnityEngine;

namespace EcoGarden.AI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ButterflyMovementController : MonoBehaviour
    {
        public enum MovementPattern
        {
            Loop,
            Hover
        }

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private MovementPattern pattern = MovementPattern.Loop;
        [SerializeField] private Color butterflyColor = new Color(1f, 0.72f, 0.36f, 1f);
        [SerializeField] private Vector3 center = Vector3.zero;
        [SerializeField] private Vector2 loopRadius = new Vector2(4.6f, 2.2f);
        [SerializeField] private Vector2 hoverRadius = new Vector2(0.38f, 0.26f);
        [SerializeField] private float speed = 0.42f;
        [SerializeField] private float phaseOffset;
        [SerializeField] private float wingPulseScale = 0.08f;

        private Vector3 baseScale;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = PlaceholderSpriteFactory.ButterflySprite;
            spriteRenderer.color = butterflyColor;
            spriteRenderer.sortingOrder = 4;
            baseScale = transform.localScale == Vector3.one
                ? new Vector3(0.18f, 0.12f, 1f)
                : transform.localScale;
        }

        private void Update()
        {
            float t = Time.time * speed + phaseOffset;
            if (pattern == MovementPattern.Loop)
            {
                UpdateLoop(t);
            }
            else
            {
                UpdateHover(t);
            }

            float pulse = 1f + Mathf.Sin(Time.time * speed * 22f + phaseOffset) * wingPulseScale;
            transform.localScale = new Vector3(baseScale.x * pulse, baseScale.y, baseScale.z);
        }

        public void ConfigureLoop(Vector3 loopCenter, Vector2 radius, float movementSpeed, float phase, Color color)
        {
            pattern = MovementPattern.Loop;
            center = loopCenter;
            loopRadius = radius;
            speed = movementSpeed;
            phaseOffset = phase;
            butterflyColor = color;
            ApplyColor();
        }

        public void ConfigureHover(Vector3 hoverCenter, Vector2 radius, float movementSpeed, float phase, Color color)
        {
            pattern = MovementPattern.Hover;
            center = hoverCenter;
            hoverRadius = radius;
            speed = movementSpeed;
            phaseOffset = phase;
            butterflyColor = color;
            ApplyColor();
        }

        private void UpdateLoop(float t)
        {
            Vector3 position = center + new Vector3(
                Mathf.Cos(t) * loopRadius.x,
                Mathf.Sin(t * 1.7f) * loopRadius.y,
                -0.2f);

            FaceMovement(position);
            transform.position = position;
        }

        private void UpdateHover(float t)
        {
            Vector3 position = center + new Vector3(
                Mathf.Sin(t * 2.1f) * hoverRadius.x,
                Mathf.Sin(t * 3.4f) * hoverRadius.y,
                -0.2f);

            FaceMovement(position);
            transform.position = position;
        }

        private void FaceMovement(Vector3 nextPosition)
        {
            float deltaX = nextPosition.x - transform.position.x;
            if (Mathf.Abs(deltaX) > 0.001f)
            {
                float direction = deltaX >= 0f ? 1f : -1f;
                baseScale.x = Mathf.Abs(baseScale.x) * direction;
            }
        }

        private void ApplyColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = butterflyColor;
            }
        }
    }
}
