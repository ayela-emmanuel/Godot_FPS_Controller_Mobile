using Godot;
using System;

public partial class Character_Movment : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	// Called when input is Given
	public Vector2 fingerPosition = Vector2.Zero;
	public int TouchIndex;
	public float TouchSensitivity = .01f;
	private Camera3D camera;
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventScreenTouch input){
			TouchIndex = input.Index;
			fingerPosition = input.Position;
		}
		if(@event is InputEventScreenDrag Input){
			if(TouchIndex == Input.Index ){
				var inputValue = (Input.Position - fingerPosition)*TouchSensitivity;
				RotateY(inputValue.X*-.5f);
				camera.RotateX(inputValue.Y*-.5f);
				fingerPosition = Input.Position;
			}
			
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("./Camera3D");
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
