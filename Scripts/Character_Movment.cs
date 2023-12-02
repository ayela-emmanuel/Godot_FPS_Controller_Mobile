using Godot;
using System;

public partial class Character_Movment : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float CrouchSpeed = 3.0f;
	public const float JumpVelocity = 4.5f;
	public bool IsCrouched = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	// Called when input is Given
	public Vector2 fingerPosition = Vector2.Zero;
	public int TouchIndex;
	public float TouchSensitivity = .01f;
	private Camera3D camera;
	private AnimationPlayer AnimPlayer;
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


				// camera.RotateX(inputValue.Y*-.5f);
				var tmpXrot = camera.RotationDegrees.X+inputValue.Y*-30;
				camera.RotationDegrees = new Vector3(Math.Clamp(tmpXrot,-90,90),camera.RotationDegrees.Y,camera.RotationDegrees.Z);
				fingerPosition = Input.Position;
			}
			
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("./Camera3D");
		AnimPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
	}

	public bool ToggleCrouch(){
		if(IsCrouched){
			var spaceState = GetWorld3D().DirectSpaceState;
			// use global coordinates, not local to node
			var query = PhysicsRayQueryParameters3D.Create(camera.GlobalPosition,camera.GlobalPosition+(Vector3.Up*1.1f));
			var result = spaceState.IntersectRay(query);
			if(result.Count>1){
				return false;
			}
		}
		AnimPlayer.Play("Crouch",-1,IsCrouched? CrouchSpeed*-1:CrouchSpeed,fromEnd:IsCrouched);
		IsCrouched = !IsCrouched;
		return true;
	}

	private Vector3 lastDirection =Vector3.Zero;
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;


		if (Input.IsActionJustPressed("ui_accept")&& IsOnFloor()){
			if(IsCrouched){
				if(ToggleCrouch()){
					velocity.Y = JumpVelocity;
				}
			}else{
				velocity.Y = JumpVelocity;
			}
			
		}
		if (Input.IsActionJustPressed("crouch")){
			ToggleCrouch();
		}
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if(IsOnFloor()){
			
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
			lastDirection = direction;
		}else{
			if (lastDirection != Vector3.Zero)
			{
				velocity.X = lastDirection.X * Speed;
				velocity.Z = lastDirection.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed/4);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed/4);
			}
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
