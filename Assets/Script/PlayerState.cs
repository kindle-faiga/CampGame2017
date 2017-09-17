using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour 
{
	//ジャンプの強さ
	[SerializeField]
	private float jumpForce = 100.0f;
	//横軸の最高スピード
	[SerializeField]
	private float maxSpeed = 5.0f;
	//横軸の加速度
	[SerializeField]
	private float acceleration = 0.5f;
	//横軸の減衰速度
	[SerializeField]
	private float deceleration = 0.05f;

    public float GetJumpForce() { return jumpForce; }
    public float GetMaxSpeed() { return maxSpeed; }
    public float GetAcceleration() { return acceleration; }
    public float GetDeceleration() { return deceleration; }
}
