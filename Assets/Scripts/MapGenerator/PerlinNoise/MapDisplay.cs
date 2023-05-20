using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Texture texture;
    public UnityEngine.UI.RawImage rawImage;

    [HideInInspector]public int width;
    [HideInInspector] public int height;

    int maxLength = 200;
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, texture.height, 1);

        rawImage.texture = texture;
        width = texture.width;
        height = texture.height;
        
        if (width > height)
        {
            height = maxLength * height / width;
            width = maxLength;
        }
        else
        {
            width = maxLength * width / height;
            height = maxLength;
        }
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }
    public void DrawTexture(Texture2D texture, int maxLength)
    {
        this.maxLength = maxLength;
        DrawTexture(texture);
    }

    //public void DrawMesh(MeshData meshData, Texture2D texture)
    //{
    //    meshFilter.sharedMesh = meshData.CreateMesh();
    //    meshRenderer.sharedMaterial.mainTexture = texture;
    //}
}
