using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using UI = UnityEngine.UI;
using System;
using UnityEngine.Events;

sealed class MnistScript : MonoBehaviour
{
    [SerializeField] private DrawLineScript drawLineScript;
    public NNModel _model;
    public RenderTexture tex;
    private Texture2D _image;
    public UI.RawImage _imageView;
    public UI.Text _textView;
    public UI.RawImage convertedImage;
    public Texture2D testTex;
    public void OnLineDrawn()
    {
        RunMNIST();
    }


    void Start()
    {
        _image = new Texture2D(240, 240, TextureFormat.RGB24, false);
        testTex = new Texture2D(28,  28, TextureFormat.RGB24, false);
    }

    void Update()
    {

    }

    void toTexture2D()
    {
        RenderTexture.active = tex;
        _image.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        _image.Apply();
    }

    void RunMNIST()
    {
        toTexture2D();
        using var input = new Tensor(1, 28, 28, 1);

        for (var y = 0; y < 28; y++)
        {
            for (var x = 0; x < 28; x++)
            {
                var tx = x * _image.width / 28;
                var ty = y * _image.height / 28;
                input[0, 27 - y, x, 0] = _image.GetPixel(tx, ty).grayscale;
                testTex.SetPixel(x, y, _image.GetPixel(tx, ty));
            }
        }


        // Run the MNIST model.
        using var worker =
          ModelLoader.Load(_model).CreateWorker(WorkerFactory.Device.CPU);
        
        worker.Execute(input);

        // Inspect the output tensor.
        var output = worker.PeekOutput();

        var scores = Enumerable.Range(0, 10).
                     Select(i => output[0, 0, 0, i]).SoftMax().ToArray();

        // Show the results on the UI.
        _imageView.texture = _image;
        testTex.Apply();
        convertedImage.texture = testTex;

        _textView.text = Enumerable.Range(0, 10).
                         Select(i => $"{i}: {scores[i]:0.00}").
                         Aggregate((t, s) => t + "\n" + s);

    }

}