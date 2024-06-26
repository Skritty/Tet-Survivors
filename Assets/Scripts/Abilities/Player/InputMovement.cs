using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/InputMovement")]
public class InputMovement : Ability
{
    public KeyCode up, down, left, right;
    public override void CooldownActivation(Entity self)
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(up)) dir += Vector3.up;
        if (Input.GetKey(down)) dir += Vector3.down;
        if (Input.GetKey(left)) dir += Vector3.left;
        if (Input.GetKey(right)) dir += Vector3.right;

        dir = dir.normalized * self.stats.baseMovementSpeed * self.stats.movementScaling * self.SlowMulti * Time.fixedDeltaTime + self.Knockback;
        if (dir.sqrMagnitude != 0)
        {
            self.stats.facing = dir.normalized;
            Vector3 scale = self.animator.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(dir.x);
            self.animator.transform.localScale = scale;
            PlayAnimation(self);
            self.stats.distanceMovedWithoutStopping += dir.magnitude;
        }
        else
        {
            self.stats.distanceMovedWithoutStopping = 0;
        }
        
        self.transform.position += dir;
    }
}