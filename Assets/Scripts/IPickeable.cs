using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickeable
{
    GameObject Picked(PonshotEntity owner, Transform socket);
    void Throwed(float force, float axis, Transform direction);
    bool CheckPreconditionsToPicked();
}
