using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Paddle : PlayerBase
{
	public float Speed = 5f;
	private PaddleController Controller;
	private float MoveDelta;
	private Rigidbody2D rb;

	private void Awake()
	{
		TryGetComponent(out rb);
		Controller = PlayerBaseBuilder.Create<PaddleController>(this)
			.SetMove(x => x.Player.Move, Move)
			.Build();
	}

	private void FixedUpdate()
	{
		var pos = rb.position;
		var p = MoveDelta * Time.fixedDeltaTime * Speed;
		pos.y = Mathf.Clamp(pos.y + p, -5.6875f, 4.9375f);
		rb.position = pos;
	}

	public override void OnControllerEnabled(IInputActionCollection input)
	{
		switch (input)
		{
			case PaddleController _controller:
				break;
		}

		Debug.Log("Paddle activated");
	}

	private void Move(float delta)
	{
		MoveDelta = delta;
	}
}