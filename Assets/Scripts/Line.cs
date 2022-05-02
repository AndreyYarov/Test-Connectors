using System.Collections;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private LineRenderer lineRenderer
    {
        get
        {
            if (!_lineRenderer)
                _lineRenderer = GetComponent<LineRenderer>();
            return _lineRenderer;
        }
    }

    public void Hide()
    {
        StopAllCoroutines();
        lineRenderer.enabled = false;
    }

    public void SphereToMouse(Connectable first)
    {
        StopAllCoroutines();
        lineRenderer.enabled = true;
        StartCoroutine(FollowMouse(first));
    }

    private IEnumerator FollowMouse(Connectable first)
    {
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mouse = ray.origin + (1.5f - ray.origin.y) / ray.direction.y * ray.direction;
            lineRenderer.SetPositions(new Vector3[]
                {
                first.transform.position + Vector3.up * 1.5f,
                mouse
                });
            yield return null;
        }
    }

    public void SphereToSphere(Connectable first, Connectable second)
    {
        StopAllCoroutines();
        lineRenderer.enabled = true;
        StartCoroutine(ConnectSpheres(first, second));
    }

    private IEnumerator ConnectSpheres(Connectable first, Connectable second)
    {
        while (true)
        {
            lineRenderer.SetPositions(new Vector3[]
            {
                first.transform.position + Vector3.up * 1.5f,
                second.transform.position + Vector3.up * 1.5f
            });
            yield return null;
        }
    }
}
