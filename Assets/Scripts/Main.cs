using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [Min(0f)] public float Radius = 10;
    [Min(1)] public int Count = 10;
    public Connectable Prefab;
    public Line LinePrefab;

    [Header("Materials")]
    public Material DefaultMaterial;
    public Material ActiveMaterial;
    public Material InactiveMaterial;

    private Connectable[] connectables;

    private Connectable.ObjectType GetObject(out Connectable obj, out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            foreach (var connectable in connectables)
                if (hit.collider == connectable.Sphere)
                {
                    obj = connectable;
                    hitPoint = hit.point;
                    return Connectable.ObjectType.Sphere;
                }
                else if (hit.collider == connectable.Platform)
                {
                    obj = connectable;
                    hitPoint = hit.point;
                    return Connectable.ObjectType.Platform;
                }
        obj = null;
        hitPoint = Vector3.zero;
        return Connectable.ObjectType.None;
    }

    private IEnumerator MoveObject(Connectable obj, Vector3 hitPoint)
    {
        Vector3 startPos = obj.transform.position;
        yield return null;

        while (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 delta = ray.origin + (hitPoint.y - ray.origin.y) / ray.direction.y * ray.direction - hitPoint;
            Vector3 pos = startPos + delta;
            if (pos.x * pos.x + pos.z * pos.z > Radius * Radius)
                pos = pos.normalized * Radius;
            obj.transform.position = pos;
            yield return null;
        }
    }

    private IEnumerator ConnectObjects(Connectable first)
    {
        foreach (var obj in connectables)
            obj.SetMaterial(obj == first ? ActiveMaterial : InactiveMaterial);
        yield return null;

        Line line = Instantiate(LinePrefab);
        line.SphereToMouse(first);

        Connectable second = null, current = null;

        while (!Input.GetMouseButtonUp(0))
        {
            if (GetObject(out current, out _) != Connectable.ObjectType.Sphere)
                current = null;
            if (current != second)
            {
                if (second)
                    second.SetMaterial(InactiveMaterial);
                second = current != first ? current : null;
                if (second)
                {
                    line.SphereToSphere(first, second);
                    second.SetMaterial(ActiveMaterial);
                }
                else
                    line.SphereToMouse(first);
            }
            yield return null;
        }

        if (!current)
        {
            foreach (var obj in connectables)
                obj.SetMaterial(DefaultMaterial);
            DestroyImmediate(line.gameObject);
            yield break;
        }

        if (!second)
        {
            line.Hide();
            while (!second)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GetObject(out current, out _) != Connectable.ObjectType.Sphere)
                        current = null;
                    if (current == first || !current)
                    {
                        foreach (var obj in connectables)
                            obj.SetMaterial(DefaultMaterial);
                        DestroyImmediate(line.gameObject);
                        yield break;
                    }
                    else
                        second = current;
                }
                yield return null;
            }
        }

        line.SphereToSphere(first, second);
        foreach (var obj in connectables)
            obj.SetMaterial(DefaultMaterial);
    }

    private IEnumerator Start()
    {
        if (!Prefab)
            yield break;

        Camera.main.transform.position = new Vector3(0f, (Radius * 2f + 2f) / Mathf.Tan(60f * Mathf.Deg2Rad) + 2f, -1f - Radius);
        Camera.main.transform.rotation = Quaternion.Euler(60f, 0f, 0f);

        connectables = new Connectable[Count];

        for (int i = 0; i < Count; i++)
        {
            float r = Mathf.Sqrt(Random.Range(0f, Radius * Radius));
            float a = Random.Range(0f, Mathf.PI * 2f);
            Vector3 position = new Vector3(r * Mathf.Cos(a), 0f, r * Mathf.Sin(a));
            connectables[i] = Instantiate(Prefab, position, Quaternion.identity);
        }

        yield return null;
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (GetObject(out Connectable obj, out Vector3 hitPoint))
                {
                    case Connectable.ObjectType.None:
                        yield return null;
                        break;
                    case Connectable.ObjectType.Sphere:
                        yield return ConnectObjects(obj);
                        break;
                    case Connectable.ObjectType.Platform:
                        yield return MoveObject(obj, hitPoint);
                        break;
                }
            }
            else
                yield return null;
        }
    }
}
