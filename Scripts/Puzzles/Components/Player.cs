using Godot;
using System;

public class Player : KinematicBody2D{
	public override void _Ready(){
		Visible = false;
		acceleration = 500;
		maxSpeed = 10;
		friction = 500;
	}
	public override void _PhysicsProcess(float delta){
		Vector2 vector = GetLocalMousePosition();
		
		if(vector != Vector2.Zero)
			velocity = velocity.MoveToward(vector * maxSpeed, acceleration * delta);
		else velocity = velocity.MoveToward(Vector2.Zero, friction * delta);
		
		velocity = MoveAndSlide(velocity);
	}
	
	private Vector2 velocity { get; set; } = new Vector2();
	private int acceleration, maxSpeed, friction;
}
