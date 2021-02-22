using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct WebcamStream : NetworkMessage
{
    public byte[] webcamStream;
}

public class WebcamSender : NetworkBehaviour
{
    WebCamTexture webcamTex;
    MeshRenderer meshToApply;
    Texture2D targetTex;
    [SerializeField] string receivedText;
    Material mat;


    private void Awake() => mat = this.gameObject.GetComponent<MeshRenderer>().material;

    void Start()
    {
        if (!NetworkClient.active) return;

        NetworkClient.ReplaceHandler<WebcamStream>(OnWebcamStream);

        SetupWebcamDevice();
        mat.mainTexture = webcamTex;

        targetTex = new Texture2D(webcamTex.width, webcamTex.height);       
    }

    private void Update()
    {
        if (!NetworkClient.active) return;

        if (webcamTex.didUpdateThisFrame)
        {
            Texture2D tex = WebCamTextureToTexture2d(webcamTex);
            SendWebcamStream(tex.EncodeToJPG());
        }
    }

    public void SendWebcamStream(byte[] webcamPixels)
    {
        WebcamStream msg = new WebcamStream()
        {
            webcamStream = webcamPixels
        };

        NetworkServer.SendToAll(msg);
    }

    private void OnWebcamStream(NetworkConnection conn, WebcamStream msg)
    {
        //if (netId == conn.identity.netId) {return;}
        
        Texture2D tex = new Texture2D(webcamTex.width, webcamTex.height);
        tex.LoadImage(msg.webcamStream);
        conn.identity.gameObject.GetComponent<MeshRenderer>().material.mainTexture = tex;
        
    }

    void SetupWebcamDevice()
    {
        webcamTex = new WebCamTexture(512, 512, 12);
        webcamTex.Play();
    }

    public Texture2D WebCamTextureToTexture2d(WebCamTexture _webCamTexture)
    {
        Texture2D _texture2D = new Texture2D(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGB24, false);
        _texture2D.SetPixels32(_webCamTexture.GetPixels32());
        return _texture2D;
    }
}
