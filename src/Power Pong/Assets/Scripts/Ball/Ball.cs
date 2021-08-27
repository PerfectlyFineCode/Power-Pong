public class Ball : BallBase
{
	// Start is called before the first frame update
	private void Start()
	{
		InitializeVelocity();
	}

	private void FixedUpdate()
	{
		DoFixedUpdate();
		UpdateVelocity();
	}
}