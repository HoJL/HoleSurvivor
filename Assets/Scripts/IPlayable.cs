using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayable
{
    void UpdateMove(float radian, Vector3 dir);
    void FinishMove();
}
