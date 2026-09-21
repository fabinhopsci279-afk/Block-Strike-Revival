using UnityEngine;

public class vp_PlayerEventHandler : vp_StateEventHandler
{
	public vp_Value<float> Health;

	public vp_Value<Vector3> Position;

	public vp_Value<Vector2> Rotation;

	public vp_Value<Vector3> Forward;

	public vp_Value<Vector3> MotorThrottle;

	public vp_Value<bool> MotorJumpDone;

	public vp_Activity Dead;

	public vp_Activity Run;

	public vp_Activity Jump;

	public vp_Activity Crouch;

	public vp_Activity Zoom;

	public vp_Activity Attack;

	public vp_Activity Reload;

	public vp_Activity Climb;

	public vp_Activity Interact;

	public vp_Activity<int> SetWeapon;

	public vp_Message<string, int> GetItemCount;

	public vp_Message<Vector3> Move;

	public vp_Value<Vector3> Velocity;

	public vp_Value<float> SlopeLimit;

	public vp_Value<float> StepOffset;

	public vp_Value<float> Radius;

	public vp_Value<float> Height;

	public vp_Value<float> FallSpeed;

	public vp_Message<float> FallImpact;

	public vp_Message<float> HeadImpact;

	public vp_Message<Vector3> ForceImpact;

	public vp_Message Stop;

	public vp_Value<Transform> Platform;

	public vp_Value<Texture> GroundTexture;

	public vp_Value<vp_SurfaceIdentifier> SurfaceType;

	protected override void Awake()
	{
		base.Awake();
		BindStateToActivity(Run);
		BindStateToActivity(Jump);
		BindStateToActivity(Crouch);
		BindStateToActivity(Zoom);
		BindStateToActivity(Reload);
		BindStateToActivity(Dead);
		BindStateToActivity(Climb);
		BindStateToActivityOnStart(Attack);
		SetWeapon.AutoDuration = 1f;
		Reload.AutoDuration = 1f;
		Zoom.MinDuration = 0.2f;
		Crouch.MinDuration = 0.5f;
		Jump.MinPause = 0f;
		SetWeapon.MinPause = 0.2f;
	}
}
