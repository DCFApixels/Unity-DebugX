
![image](https://github.com/user-attachments/assets/a3b34566-2423-4418-8a74-1369b0c268f2)

<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/Unity-DebugX?style=for-the-badge&color=ff3530">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/Unity-DebugX?color=ff3530&style=for-the-badge">
</p>


# Unity-DebugX

> [!WARNING]
> The translation of the Readme is not completely finished


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
        <img src="https://github.com/user-attachments/assets/30528cb5-f38e-49f0-b23e-d001844ae930"></br>
        <span>English</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/Unity-DebugX/blob/main/README-ZH.md">
        <img src="https://github.com/user-attachments/assets/3c699094-f8e6-471d-a7c1-6d2e9530e721"></br>
        <span>中文</span>
      </a>  
    </td>
  </tr>
</table>

</br>

一个多功能、可扩展且高性能的 Unity Gizmos 绘图工具。 它既可以在编辑器中运行，也可以在 Build 中运行，绘制可以在 `OnDrawGizmos` 和 `Update` 中进行. 支持 HDRP、URP 和 BRP，但在 BRP 中不支持在 `OnDrawGizmos` 回调中进行绘制。

语法：
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

![image](https://github.com/user-attachments/assets/af09b0e3-8710-4461-99ce-a5f868b25260)

<br>

## 目录
- [安装](#安装)
- [基础API](#基础-api)
- [设置](#设置)
- [API Extension](#API-Extension)
- [Loading Static Assets](#Loading-Static-Assets)
- [Define Symbols](#define-symbols)

<br>

# Installation
版本的语义  - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
### Unity-软件包
支持以 Unity 模块的形式安装，只需将 Git-URL 复制到 [PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) 或  `Packages/manifest.json`中. 复制此 Git-URL 以安装最新的工作版本:
```
https://github.com/DCFApixels/Unity-DebugX
```
### 作为源代码
该包也可以直接复制到项目文件夹中。

</br>

# 基础 API

绘制预定义 Gizmo 的通用语法：
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```
</br>

预定义的 Gizmo 包括各种图元、线条、点和文本。以下是一些 Gizmo 的示例：
```c#
// 绘制一条普通线条，类似于 Debug.DrawLine(...)。
// 线条将显示一秒钟，并且为红色。
DebugX.Draw(1, Color.red).Line(startPoint, endPoint);
```
```c#
// 绘制一个立方体，但仅显示一帧，并且为黄色。
DebugX.Draw(Color.yellow).Cube(center, rotation, size);
```
```c#
// 绘制一个球体。
DebugX.Draw(Color.yellow).Cube(center, radius);
```
```c#
// 绘制一个点，点的大小为屏幕空间大小。
DebugX.Draw(Color.yellow).Dot(startPoint, endPoint);
```
```c#
// 绘制文本。文本也可以显示指定的时间。
DebugX.Draw(1, Color.red).Text(center, text);
// 使用 DebugXTextSettings 进行高级显示设置。
DebugX.Draw(Color.yellow).Text(center, text, DebugXTextSettings.Default.SetBackgroundColor(Color.black));
```
</br>


如果预定义的图元不够用，可以使用以下方法绘制自定义网格和材质：
```c#
// 使用 lit 材质绘制任何网格。不使用 GPU 实例化。
DebugX.Draw(...).Mesh(mesh, pos, rot, sc);
// UnlitMesh - 使用 unlit 材质的网格
// WireMesh - 使用线框材质的网格
```
```c#
// 使用 lit 材质绘制静态网格。使用 GPU 实例化。
DebugX.Draw(...).Mesh<IStaticMesh>(pos, rot, sc);
// UnlitMesh<IStaticMesh> - 使用 unlit 材质的网格
// WireMesh<IStaticMesh> - 使用线框材质的网格
```
```c#
// 使用静态材质绘制静态网格。使用 GPU 实例化。
DebugX.Draw(...).Mesh<IStaticMesh, IStaticMat>(pos, rot, sc);
```
</br>

为了优化绘制，使用静态数据：
```c#
// 静态网格。使用 GPU 实例化绘制时必须。
public struct SomeMesh : IStaticMesh
{
    public Mesh GetMesh() => StaticStorage.SomeMesh;
}
```
```c#
// 静态材质。
public struct SomeMesh : IStaticMesh
{
    // 控制渲染顺序。
    public int GetExecuteOrder() => 100;
    public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```
> In the example, StaticStorage is a conditional implementation of a static asset storage. Since in Unity static data cannot be filled through the inspector, the solution to this problem is described in the section [Loading Static Assets](#Loading-Static-Assets).

<br>

# 设置
设置窗口位于 "Tools -> DebugX -> Settings"：

![image](https://github.com/user-attachments/assets/7dd981c1-1e00-4b7d-9a73-376638094689)

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