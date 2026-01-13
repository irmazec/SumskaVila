using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Vector2 bottomLeftCoords;
    public Vector2 topRightCoords;
    public Transform player;
    public RawImage playerIndicator;

    private float xRange;
    private float yRange;   // represents z range in world coords, y in canvas
    private float barWidth;
    private float barHeight;

    void Start()
    {
        xRange = Mathf.Abs(bottomLeftCoords.x - topRightCoords.x);
        barWidth = GetComponent<RectTransform>().rect.width - playerIndicator.rectTransform.rect.width;
        yRange = Mathf.Abs(bottomLeftCoords.y - topRightCoords.y);
        barHeight = GetComponent<RectTransform>().rect.height - playerIndicator.rectTransform.rect.height;
    }

    void FixedUpdate()
    {
        playerIndicator.rectTransform.anchoredPosition = new Vector2(
            (Mathf.Clamp(player.position.x, bottomLeftCoords.x, topRightCoords.x) - bottomLeftCoords.x) * barWidth / xRange,
            (Mathf.Clamp(player.position.z, bottomLeftCoords.y, topRightCoords.y) - bottomLeftCoords.y) * barHeight / yRange
        );
    }
}
