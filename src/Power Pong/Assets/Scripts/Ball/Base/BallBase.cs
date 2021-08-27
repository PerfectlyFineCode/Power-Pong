using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallBase : MonoBehaviour, IBall
{
	private Camera cam;
	private Rigidbody2D rb;
	private SpriteRenderer SpriteRenderer;

	[field: SerializeField] private Vector2 Velocity { get; set; } = Vector2.right;

	private void Awake()
	{
		cam = Camera.main;
		TryGetComponent(out SpriteRenderer);
		TryGetComponent(out rb);
		InitializeBounds();
	}

	protected void InitializeVelocity()
	{
		rb.velocity = Velocity;
	}


	[field: SerializeField] public float Speed { get; set; } = 5f;

	public Bounds BallBounds { get; set; } = new Bounds(
		new Vector3(0, 0),
		new Vector3(19.75f, 11f));

	protected void UpdateVelocity()
	{
		rb.velocity = Velocity.normalized * Speed;
	}

	public virtual void Move()
	{
		var spriteBounds = SpriteRenderer.bounds;

		var pos = rb.position;

		var newV = CheckCollisions(pos);

		pos.y = Mathf.Clamp(pos.y,
			BallBounds.min.y + spriteBounds.extents.y,
			BallBounds.max.y - spriteBounds.extents.y);

		pos.x = Mathf.Clamp(pos.x,
			BallBounds.min.x + spriteBounds.extents.x,
			BallBounds.max.x - spriteBounds.extents.y);

		rb.position = pos;
		Velocity    = newV;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		var point = other.GetContact(0);

		Debug.Log(point.point);

		var v = Vector2.Reflect(point.point * -1, point.normal);

		Velocity = v;
	}

	public virtual void DoFixedUpdate()
	{
		Move();
	}

	private void InitializeBounds()
	{
		const float screenAspect = 16f / 9f;
		var         cameraHeight = cam.orthographicSize * 2;

		BallBounds = new Bounds(cam.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
	}

	private Vector2 CheckCollisions(in Vector2 pos)
	{
		var v        = rb.velocity;
		var deltaPos = (pos - rb.position).normalized;

		Debug.DrawLine(pos, pos + deltaPos * 5, Color.blue, 2f);
		var spriteBounds = SpriteRenderer.bounds;

		// Right
		if (pos.x - spriteBounds.size.x >= BallBounds.max.x) v = Vector2.Reflect(deltaPos, Vector2.left);

		// Left
		if (pos.x + spriteBounds.size.x <= BallBounds.min.x) v = Vector2.Reflect(deltaPos, Vector2.right);

		// Top
		if (pos.y + spriteBounds.size.y >= BallBounds.max.y) v = Vector2.Reflect(deltaPos, Vector2.down);

		// Bottom
		if (pos.y + spriteBounds.size.y <= BallBounds.min.y) v = Vector2.Reflect(deltaPos, Vector2.up);

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