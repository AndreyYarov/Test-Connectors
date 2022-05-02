using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [Min(0f)] public float Radius = 10;
    [Min(1)] public int Count = 10;
    public Connectable Prefab;

    private Connectable[] connectables;

    private Connectable.ObjectType GetObject(out Connectable obj)
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
                foreach (var connectable in connectables)
                    if (hit.collider == connectable.Sphere)
                    {
                        obj = connectable;
                        return Connectable.ObjectType.Sphere;
                    }
                    else if (hit.collider == connectable.Platform)
                    {
                        obj = connectable;
                        return Connectable.ObjectType.Platform;
                    }
        }
        obj = null;
        return Connectable.ObjectType.None;
    }

    private IEnumerator MoveObject(Connectable obj)
    {
        yield return null;
    }

    private IEnumerator ConnectObjects(Connectable first)
    {
        yield return null;
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
            switch (GetObject(out var obj))
            {
                case Connectable.ObjectType.None:
                    yield return null;
                    break;
                case Connectable.ObjectType.Sphere:
                    yield return ConnectObjects(obj);
                    break;
                case Connectable.ObjectType.Platform:
                    yield return MoveObject(obj);
                    break;
            }
        }
    }
}
