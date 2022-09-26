using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = .5f;
    private float offset;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10f;

        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
