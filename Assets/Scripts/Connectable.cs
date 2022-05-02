using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectable : MonoBehaviour
{
    public enum ObjectType { None, Sphere, Platform };

    public Collider Sphere;
    public Collider Platform;

    private Renderer rnd;

    private void Start()
    {
        rnd = Sphere.GetComponent<Renderer>();
    }

    public void SetMaterial(Material material)
    {
        rnd.material = material;
    }
}
