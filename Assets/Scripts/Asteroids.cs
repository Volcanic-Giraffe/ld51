using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public int Amount = 30;
    public Sprite[] sprites;
    public List<(GameObject, float)> _asteroids = new List<(GameObject, float)>();

    void Start()
    {
        for (var i = 0; i < Amount; i++)
        {
            var a = new GameObject();
            a.transform.SetParent(this.transform);
            var sr = a.AddComponent<SpriteRenderer>();
            sr.sprite = sprites.PickRandom();
            var size = sr.sprite.rect.width * sr.sprite.rect.height / 1000;
            sr.sortingOrder = Mathf.RoundToInt(-10000 / size); // smaller = farther
            a.transform.Rotate(new Vector3(0, 0, Random.Range(-10, 10)));
            Vector2 pos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            a.transform.position = new Vector3(pos.x, pos.y, 0);
            _asteroids.Add((a, size * 0.05f + Random.Range(-0.5f, 0.5f)));
            
        }
    }

    void Update()
    {
        foreach (var a in _asteroids)
        {
            a.Item1.transform.position += Vector3.right * a.Item2 * Time.deltaTime;
            a.Item1.transform.Rotate(new Vector3(0, 0, a.Item2 * Time.deltaTime));
            if (Camera.main.WorldToViewportPoint(a.Item1.transform.position).x > 1.2f)
            {
                a.Item1.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.4f, Random.Range(0, 1f), Camera.main.farClipPlane * 0.9f));
            }
        }
    }
}
