using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Player/CameraFollow")]
public class CameraFollow : Ability
{
    public override void CooldownActivation(Entity self)
    {
        Vector3 pos = Camera.main.transform.position;
        pos.x = self.transform.position.x;
        pos.y = self.transform.position.y;
        Camera.main.transform.position = pos;
    }
}
