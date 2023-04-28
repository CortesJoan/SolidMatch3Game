using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Drawing;
using System.Drawing; 
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Imaging; 
public class RenderTextureTOProcess : MonoBehaviour
{
    private RenderTexture renderTexture;
    [SerializeField] Material material;

    private void Start()
    {
        Process process = new Process();
        process.StartInfo.FileName = "notepad.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
 

//Bitmap stored compiler.StandardOutput.ReadToEnd()
         Bitmap bitmap  = new Bitmap(Screen.width, UnityEngine.Device.Screen.height); // Obtener el bitmap del proceso
        //MemoryStream stream = process.MainModule; // Crear un flujo de memoria
       // bitmap.Save(stream, ImageFormat.Png); // Guardar el bitmap en el flujo como PNG

        //byte[] buffer = stream.ToArray(); // Obtener el array de bytes del flujo

        Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height); // Crear una textura vacía
      
      // s texture.LoadImage(buffer); // Cargar la imagen desde el array de bytes
        texture.Apply();
        MemoryStream memoryStream = new MemoryStream();
        process.StandardOutput.BaseStream.CopyTo(memoryStream);
        byte[] byteArray = memoryStream.ToArray();
        memoryStream.Close();
        Texture2D tex = new Texture2D(Screen.width, UnityEngine.Device.Screen.height, TextureFormat.RG32, false);
     
        //Graphics.DrawTexture(byteArray.Length);
   //     tex.LoadRawTextureData(byteArray);
     //   tex.Apply();
        GetComponent<Renderer>().material.mainTexture = texture;

      
         //   renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
//      renderTexture=  Texture2D.CreateExternalTexture(byteArray, Screen.width, Screen.height, TextureFormat.ARGB32, false, false);
        //   renderTexture.Create();
        // Graphics.Blit(renderTexture, material);
    }
}