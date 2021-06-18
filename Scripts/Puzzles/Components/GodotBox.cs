using Godot;
using System;

public class GodotBox : KinematicBody2D{
	public override void _Ready()
		=> Visible = true;
	
	
	public override void _PhysicsProcess(float delta)
		=> MoveAndSlide(speed * direction() * delta);
	
	//* To know which direction of mouse cursor go
	private Vector2 direction()
		=> (GetGlobalMousePosition() -  GlobalPosition).Floor();
	
	
	private int speed = 250;
	
}
