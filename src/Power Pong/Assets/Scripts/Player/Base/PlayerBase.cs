using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour, IPlayerBase
{
	public virtual void OnControllerEnabled(IInputActionCollection input)
	{
	}
}


public interface IPlayerBase
{
	abstract void OnControllerEnabled(IInputActionCollection input);
}

public class PlayerBaseSource<T> where T : IInputActionCollection
{
	private readonly T controller;
	private readonly IPlayerBase PlayerBase;

	public PlayerBaseSource(T input, IPlayerBase player)
	{
		PlayerBase = player;
		controller = input;
	}

	public PlayerBaseSource<T> SetMove(Func<T, InputAction> action, Action<float> MoveAction)
	{
		var inputAction = action(controller);
		inputAction.performed += ctx => MoveAction(ctx.ReadValue<float>());
		inputAction.canceled += ctx => MoveAction(ctx.ReadValue<float>());
		return this;
	}

	public T Build()
	{
		controller.Enable();
		PlayerBase.OnControllerEnabled(controller);
		return controller;
	}
}

public static class PlayerBaseBuilder
{
	public static PlayerBaseSource<T> Create<T>(IPlayerBase playerBase) where T : IInputActionCollection
	{
		return new PlayerBaseSource<T>(Activator.CreateInstance<T>(), playerBase);
	}
}