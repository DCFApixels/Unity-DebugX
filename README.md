
![image](https://github.com/user-attachments/assets/a3b34566-2423-4418-8a74-1369b0c268f2)

<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/Unity-DebugX?style=for-the-badge&color=ff3530">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/Unity-DebugX?color=ff3530&style=for-the-badge">
</p>


# Unity-DebugX

<table>
  <tr></tr>
  <tr>
    <td colspan="3">Readme Languages:</td>
  </tr>
  <tr></tr>
  <tr>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/Unity-DebugX/blob/main/README-RU.md">
        <img src="https://github.com/user-attachments/assets/7bc29394-46d6-44a3-bace-0a3bae65d755"></br>
        <span>Русский</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/Unity-DebugX">
        <img src="https://github.com/user-attachments/assets/3c699094-f8e6-471d-a7c1-6d2e9530e721"></br>
        <span>English</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/Unity-DebugX/blob/main/README-ZH.md">
        <img src="https://github.com/user-attachments/assets/8e598a9a-826c-4a1f-b842-0c56301d2927"></br>
        <span>中文</span>
      </a>  
    </td>
  </tr>
</table>

</br>

A multifunctional, extensible, and high-performance Gizmos drawing utility for Unity. It works both in the editor and in the build, and drawing can be done both in `OnDrawGizmos` and in `Update`. HDRP, URP, and BRP are supported, but drawing in the `OnDrawGizmos` callbacks is not supported in BRP.

Syntax: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

![image](https://github.com/user-attachments/assets/af09b0e3-8710-4461-99ce-a5f868b25260)

<br>

## Table of Contents
- [Installation](#Installation)
- [Basic API](#Basic-api)
- [Settings](#Settings)
- [API Extension](#API-Extension)
- [Loading Static Assets](#Loading-Static-Assets)
- [Define Symbols](#define-symbols)

<br>

# Installation
Versioning semantics - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
### Unity-Package
Supports installation as a Unity module. Copy the Git-URL [into PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) or into `Packages/manifest.json`. Copy this Git-URL to install the latest working version:
```
https://github.com/DCFApixels/Unity-DebugX
```
### Source Code
The Package can also be directly copied into the project folder.

</br>

# Basic API

The general syntax for drawing predefined Gizmos:
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```
</br>

Among the predefined Gizmos, there are various primitives, lines, points, and text. Examples of some Gizmos:
```c#
// Draws a regular line similar to Debug.DrawLine(...).
// The line will be displayed for one second and will be red.
DebugX.Draw(1, Color.red).Line(startPoint, endPoint);
```
```c#
// Draws a cube, but for just one frame and in yellow.
DebugX.Draw(Color.yellow).Cube(center, rotation, size);
```
```c#
// Draws a sphere.
DebugX.Draw(Color.yellow).Cube(center, radius);
```
```c#
// Draws a point. The point has a screen space size. 
DebugX.Draw(Color.yellow).Dot(startPoint, endPoint);
```
```c#
// Draws text. The text can also be displayed for the specified time.
DebugX.Draw(1, Color.red).Text(center, text);
// For advanced display settings, use DebugXTextSettings.
DebugX.Draw(Color.yellow).Text(center, text, DebugXTextSettings.Default.SetBackgroundColor(Color.black));
```
</br>


In case the predefined primitives are not enough, there are methods for drawing custom meshes and materials:
```c#
//Рисования любого меша lit материалом. Без GPU instancing. 
DebugX.Draw(...).Mesh(mesh, pos, rot, sc);
//UnlitMesh - меш с unlit материалом
//WireMesh - меш с wireframe материалом
```
```c#
//Рисования статического меша lit материалом. В режиме GPU instancing. 
DebugX.Draw(...).Mesh<IStaticMesh>(pos, rot, sc);
//UnlitMesh<IStaticMesh> - меш с unlit материалом
//WireMesh<IStaticMesh> - меш с wireframe материалом
```
```c#
//Рисования статического меша статическим материалом. В режиме GPU instancing. 
DebugX.Draw(...).Mesh<IStaticMesh, IStaticMat>(pos, rot, sc);
```
</br>

Static data is used to optimize the rendering:
```c#
// Static mesh. Required for drawing with GPU instancing.
public struct SomeMesh : IStaticMesh
{
    public Mesh GetMesh() => StaticStorage.SomeMesh;
}
```
```c#
// Static material.
public struct SomeMesh : IStaticMesh
{
    // Control the drawing order.
    public int GetExecuteOrder() => 100;
    public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```
> In the example, StaticStorage is a conditional implementation of a static asset storage. Since in Unity static data cannot be filled through the inspector, the solution to this problem is described in the section [Loading Static Assets](#Loading-Static-Assets).

<br>

# Settings
Settings window "Tools -> DebugX -> Settings":

![image](https://github.com/user-attachments/assets/dee3954a-1c8a-442c-a4bb-d2c710e065a4)

<br>

# API Extension
The simplest option is to create an extension method that combines predefined gizmos, for example:
```c#
public static class SomeGizmoExtensions
{
    public static DebugX.DrawHandler Distance(this DebugX.DrawHandler self, Vector3 start, Vector3 end)
    {
        // Draw a line.
        self.Line(start, end);
        // Draw text in the middle of the line showing the length of the line.
        self.Text(Vector3.Lerp(start, end, 0.5f), Vector3.Distance(start, end), DebugXTextSettings.Default.SetAnchor(TextAnchor.UpperCenter));
        // for support Method Chaining syntax.
        return self;
    }
}
```
> You can also use the `Mesh` methods to create drawing methods for other primitives.

Extended implementation of Gizmo, in case the built-in drawing methods are not enough:
```c#
public readonly struct SomeGizmo : IGizmo<SomeGizmo>
{
    // Gizmo Data.

    public SomeGizmo(/*...*/)
    {
        // Fill the data.
    } 
    
    public IGizmoRenderer<SomeGizmo> RegisterNewRenderer() => new Renderer();
    private class Renderer : IGizmoRenderer<SomeGizmo>
    {
        // Control the execution order of renderers.
        public int ExecuteOrder => 0; // can use default(SomeMat).GetExecutionOrder();
        // Flag for the system about optimization.
        // If the drawing or preparation method depends on the current camera, set to false, otherwise true.
        // If unsure, choose false.
        public bool IsStaticRender => false;

        // Prepare data before rendering, you can perform additional calculations or schedule a Job here.
        public void Prepare(Camera camera, GizmosList<SomeGizmo> list) 
        {
            foreach (var item in list)
            {
                //... 
            }
        } 

        // Render, you can directly use the graphics API or add a command to CommandBuffer here.
        public void Render(Camera camera, GizmosList<SomeGizmo> list, CommandBuffer cb)
        {
            foreach (var item in list)
            {
                //... 
            }
        }
    }
}
```
```c#
// Create an extension method. 
public static class SomeGizmoExtensions
{
    public static DebugX.DrawHandler SomeGizmo(this DebugX.DrawHandler self, /*...*/) 
    {
        self.Gizmo(new SomeGizmo(/*...*/);
        return self;
    }
}
```

<br>

# Loading Static Assets
Для загрузки имеется утилита `DebugXUtility.LoadStaticData(...);`. 

1) First, create a storage for the assets.
```c#
public readonly struct SomeAssets
{
    public readonly Mesh SomeMesh;
    public readonly Material SomeMaterial;
} 
```
2) Next, create a prefab with a list of assets. Each child GameObject of the prefab is treated as one asset, and its name must match the field where the asset will be loaded. To load meshes, add a MeshFilter component to the GameObject with a reference to the desired mesh. To load a material, add any component inherited from Renderer with the specified material. The prefab itself must be located in the Resources folder.
 
![image](https://github.com/user-attachments/assets/191dd337-81d5-43ff-b92e-e8b0927841f9)

3) Once the repository and prefab list are prepared, assets can be uploaded.
```c#
SomeAssets assets = DebugXUtility.LoadStaticData(new SomeAssets(), "SomeAssets");
// Done.
```
> An example of how to work with this utility can be found in the source code in the file `DebugXAssets.cs`.

<br>

# Define Symbols
+ `DISABLE_DEBUGX_INBUILD` - By default, Gizmos will be drawn in the project build. This define disables drawing. It can also be enabled or disabled in the DebugX settings window.
