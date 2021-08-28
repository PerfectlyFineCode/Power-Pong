using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallBase : MonoBehaviour, IBall
{
	public LayerMask BounceMask;

	private Camera cam;
	private Rigidbody2D rb;
	private SpriteRenderer SpriteRenderer;

	[field: SerializeField]
	private Vector2 Velocity { get; set; } = Vector2.right;

	[field: SerializeField]
	private bool CanMove { get; set; } = true;

	private void Awake()
	{
		cam = Camera.main;
		TryGetComponent(out SpriteRenderer);
		TryGetComponent(out rb);
		InitializeBounds();
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!other.gameObject.TryGetComponent(out Paddle paddle) &&
		    !other.gameObject.TryGetComponent(out PaddleAI ai)) return;
		CanMove = false;

		var point = other.GetContact(0);

		var v = Vector2.Reflect(point.point, point.normal);

		Velocity = v;
		CanMove = true;
	}


	[field: SerializeField]
	public float Speed { get; set; } = 5f;

	public Bounds BallBounds { get; set; }

	public virtual void Move()
	{
		var spriteBounds = SpriteRenderer.bounds;
		var previousPos = rb.position;
		var pos = previousPos;
		var velocity = Velocity;

		var extentsX = spriteBounds.extents.x;
		var extentsY = spriteBounds.extents.y;

		var speed = Speed * Time.fixedDeltaTime;

		var newV = CheckCollisions(velocity, pos, pos + velocity * speed);

		pos.y = Mathf.Clamp(pos.y + newV.y * speed,
			BallBounds.min.y + extentsY,
			BallBounds.max.y - extentsY);

		pos.x = Mathf.Clamp(pos.x + newV.x * speed,
			BallBounds.min.x + extentsX,
			BallBounds.max.x - extentsX);

		if (CanMove && CheckBounce(previousPos, pos)) rb.position = pos;
		Velocity = newV;
	}

	public virtual void DoFixedUpdate()
	{
		Move();
	}

	private bool CheckBounce(in Vector2 previousPosition, in Vector2 pos)
	{
		var velocity = Velocity;

		var nextPos = (velocity * (Speed * Time.fixedDeltaTime)).magnitude;

		var size = SpriteRenderer.bounds.extents;

		var hit = Physics2D.BoxCast(SpriteRenderer.bounds.center,
			size, 180f,
			velocity, nextPos,
			BounceMask);
		// var ray = Physics2D.Raycast(transform.position, velocity, nextPos, BounceMask);

		if (!hit) return true;
		// Velocity = Vector2.Reflect(velocity, cast.normal);
		rb.position = hit.point - (Vector2) SpriteRenderer.bounds.size;
		Debug.Log(hit.collider.name);
		return false;
	}

	private void InitializeBounds()
	{
		const float screenAspect = 16f / 9f;
		var cameraHeight = cam.orthographicSize * 2;

		BallBounds = new Bounds(cam.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
	}

	private Vector2 CheckCollisions(in Vector2 velocity, in Vector2 pos, in Vector2 nextPos)
	{
		var v = velocity;
		var deltaPos = (nextPos - pos).normalized;

		Debug.DrawLine(pos, pos + deltaPos * 5, Color.blue, 2f);
		var spriteBounds = SpriteRenderer.bounds;

		var extentsX = spriteBounds.size.x;
		var extentsY = spriteBounds.size.y;

		// Right
		if (pos.x >= BallBounds.max.x - extentsX)
		{
			Debug.Log("RIGHT");
			v.x *= -1f;
		}

		// Left
		if (pos.x <= BallBounds.min.x + extentsX)
		{
			v.x *= -1f;
			Debug.Log("LEFT");
		}

		// Top
		if (pos.y >= BallBounds.max.y - extentsY)
		{
			v.y *= -1f;
			Debug.Log("TOP");
		}

		// Bottom
		if (pos.y <= BallBounds.min.y + extentsY)
		{
			Debug.Log("BOTTOM");
			v.y *= -1f;
		}


		return v.normalized;
	}
}

public interface IBall
{
	public float Speed { get; set; }
	public Bounds BallBounds { get; set; }
	public void Move();

	public void DoFixedUpdate();
}