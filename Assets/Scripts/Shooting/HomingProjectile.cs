using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
   private Rigidbody2D rb;
   private Transform target;

   private float speed;
   private float rotateSpeed = 400;

   private float horizontal;
   private float vertical;
   private float time;
   private int random;

   // Whether or not the bullet should home in on target or not
   private bool active = false;

   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
      target = GameObject.FindGameObjectWithTag("Player").transform;
   }

   private void OnEnable()
   {
      speed = 14;
      time = 0;
      active = false;

      // Move in a random direction downwards
      horizontal = Random.Range(-8f, 8f);
      vertical = Mathf.Sqrt(speed * speed - horizontal * horizontal) * -1;
      random = Random.Range(0, 2);
   }

   private void FixedUpdate()
   {
      time += Time.fixedDeltaTime;
      if (active)
      {
         // speed += .8f;
         FollowTarget();
      }
      else
      {
         rb.velocity = new Vector2(horizontal, vertical);

         float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

         if (random == 0)
         {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
         }
         else
         {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
         }

         if (time >= 0.15f)
         {
            active = true;
         }
      }
   }

   private void FollowTarget()
   {
      Vector2 direction = (Vector2)target.position - rb.position;
      Vector2 vel = rb.velocity;

      // Decreases speed if velocity is near perpendicular to direction to turn to avoid orbital behavior
      /*
      if ((ang > 85 && ang < 95) || (ang > -95 && ang < -85))
      {
         speed = 6;
      }
      */

      direction.Normalize();

      float rotateAmount = Vector3.Cross(direction, transform.right).z;
      rb.angularVelocity = -rotateAmount * rotateSpeed;
      rb.velocity = transform.right * speed;
   }
}