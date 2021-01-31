using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class NPCCharacter : Controller
{
    private Mover mover;
        float currentVerticalSpeed = 0f;
        bool isGrounded;
        public float movementSpeed = 7f;
        public float jumpSpeed = 10f;
        public float gravity = 10f;

        public List<Vector3> waypoints;
        public List<int> visited;

        public int currentWaypoint = -1;
        
		Vector3 lastVelocity = Vector3.zero;

        Transform tr;

        // Use this for initialization
        void Start()
        {
            tr = transform;
            mover = GetComponent<Mover>();
            
            waypoints = new List<Vector3>();
            visited = new List<int>();
        }

        
        // Update is called once per frame
        void Update()
        {

        }
        
        void FixedUpdate()
        {
            //Run initial mover ground check;
            mover.CheckForGround();

            //If character was not grounded int the last frame and is now grounded, call 'OnGroundContactRegained' function;
            if(isGrounded == false && mover.IsGrounded() == true)
                OnGroundContactRegained(lastVelocity);

            //Check whether the character is grounded and store result;
            isGrounded = mover.IsGrounded();

            Vector3 _velocity = Vector3.zero;

            //Add player movement to velocity;
            _velocity += CalculateMovementDirection() * movementSpeed;
            
            //Handle gravity;
            if (!isGrounded)
            {
                currentVerticalSpeed -= gravity * Time.deltaTime;
            }
            else
            {
                if (currentVerticalSpeed <= 0f)
                    currentVerticalSpeed = 0f;
            }

            //Handle jumping;
            // if ((characterInput != null) && isGrounded && characterInput.IsJumpKeyPressed())
            // {
            //     OnJumpStart();
            //     currentVerticalSpeed = jumpSpeed;
            //     isGrounded = false;
            // }

            //Add vertical velocity;
            _velocity += tr.up * currentVerticalSpeed;

			//Save current velocity for next frame;
			lastVelocity = _velocity;

            mover.SetExtendSensorRange(isGrounded);
            mover.SetVelocity(_velocity);
        }

        private Vector3 CalculateMovementDirection()
        {
            Vector3 _direction = Vector3.zero;

            if (currentWaypoint == -1) currentWaypoint = GetClosestWaypoint();

            if (currentWaypoint != -1)
            {
                var waypoint = waypoints[currentWaypoint];
                _direction = (waypoint - transform.position).normalized;

                if (Vector3.Distance(transform.position, waypoints[currentWaypoint]) < 0.25f)
                {
                    ReachedWaypoint(currentWaypoint);
                }
            }
            
			// //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
			// if(cameraTransform == null)
			// {
			// 	_direction += tr.right * characterInput.GetHorizontalMovementInput();
			// 	_direction += tr.forward * characterInput.GetVerticalMovementInput();
			// }
			// else
			// {
			// 	//If a camera transform has been assigned, use the assigned transform's axes for movement direction;
			// 	//Project movement direction so movement stays parallel to the ground;
			// 	_direction += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * characterInput.GetHorizontalMovementInput();
			// 	_direction += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * characterInput.GetVerticalMovementInput();
			// }

			//If necessary, clamp movement vector to magnitude of 1f;
			if(_direction.magnitude > 1f)
				_direction.Normalize();

			return _direction;
        }

        //This function is called when the controller has landed on a surface after being in the air;
		void OnGroundContactRegained(Vector3 _collisionVelocity)
		{
			//Call 'OnLand' delegate function;
			if(OnLand != null)
				OnLand(_collisionVelocity);
		}

        //This function is called when the controller has started a jump;
        void OnJumpStart()
        {
            //Call 'OnJump' delegate function;
            if(OnJump != null)
                OnJump(lastVelocity);
        }

        //Return the current velocity of the character;
        public override Vector3 GetVelocity()
        {
            return lastVelocity;
        }

        //Return only the current movement velocity (without any vertical velocity);
        public override Vector3 GetMovementVelocity()
        {
            return lastVelocity;
        }

        //Return whether the character is currently grounded;
        public override bool IsGrounded()
        {
            return isGrounded;
        }
    

        #region WAYPOINTS
    public void SetWaypoints(List<Vector3> w)
    {
        waypoints.Clear();
        visited.Clear();
        waypoints = w;
        currentWaypoint = GetClosestWaypoint();
    }

    public int GetClosestWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;

        int closest = -1;
        float closestDistance = float.MaxValue;
        foreach (var waypoint in waypoints)
        {
            var id = waypoints.IndexOf(waypoint);
            if (visited.Contains(id)) return -1;
            var d = Vector3.Distance(waypoint, transform.position);
            if (d > maxDistance) continue;
            if (d > closestDistance) continue;
            if (d < minDistance) continue;
            closestDistance = d;
            closest = waypoints.IndexOf(waypoint);
        }

        return closest;
    }

    public int GetPreviousWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;
        if (currentWaypoint == -1) return GetClosestWaypoint(maxDistance);
        if (currentWaypoint - 1 < 0) return -1;
        if (visited.Contains(currentWaypoint - 1)) return -1;

        var d = Vector3.Distance(waypoints[currentWaypoint - 1], transform.position);
        if (d > maxDistance) return -1;
        if (d < minDistance) return -1;
        return currentWaypoint + 1;
    }

    public int GetNextWaypoint(float maxDistance = 5.0f, float minDistance = 0.25f)
    {
        if (waypoints == null || waypoints.Count == 0) return -1;
        if (currentWaypoint == -1) return GetClosestWaypoint(maxDistance);
        if (currentWaypoint + 1 >= waypoints.Count) return -1;
        if (visited.Contains(currentWaypoint + 1)) return -1;
        var d = Vector3.Distance(waypoints[currentWaypoint + 1], transform.position);
        if (d > maxDistance) return -1;
        if (d < minDistance) return -1;

        return currentWaypoint + 1;
    }

    public void ReachedWaypoint(int id)
    {
        visited.Add(id);
        var next = GetNextWaypoint();
        var previous = GetPreviousWaypoint();
        if (next >= 0) currentWaypoint = next;
        else if (previous >= 0) currentWaypoint = previous;
        else currentWaypoint = -1;
    }
    
    #endregion
}