using Godot;
using System;

public partial class Debug : Control
{
	Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("./FpsCount");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		label.Text = "FPS: "+Engine.GetFramesPerSecond();
	}
}
