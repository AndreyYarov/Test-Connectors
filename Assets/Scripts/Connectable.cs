using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectable : MonoBehaviour
{
    public enum ObjectType { None, Sphere, Platform };

    public Collider Sphere;
    public Collider Platform;
}
