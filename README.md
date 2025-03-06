
![image](https://github.com/user-attachments/assets/a3b34566-2423-4418-8a74-1369b0c268f2)


<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/Unity-DebugX?style=for-the-badge&color=ff3600">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/Unity-DebugX?color=ff3600&style=for-the-badge">
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
      <a href="https://github.com/DCFApixels/DragonECS/blob/main/README-RU.md">
        <img src="https://github.com/user-attachments/assets/7bc29394-46d6-44a3-bace-0a3bae65d755"></br>
        <span>Русский</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/DragonECS">
        <img src="https://github.com/user-attachments/assets/3c699094-f8e6-471d-a7c1-6d2e9530e721"></br>
        <span>English</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/DragonECS/blob/main/README-ZH.md">
        <img src="https://github.com/user-attachments/assets/8e598a9a-826c-4a1f-b842-0c56301d2927"></br>
        <span>中文</span>
      </a>  
    </td>
  </tr>
</table>

</br>

A multifunctional, extensible, and high-performance Gizmos drawing utility for Unity. It works both in the editor and in the build, and drawing can be done both in OnDrawGizmos and in Update. HDRP, URP, and BRP are supported, but drawing in the OnDrawGizmos callbacks is not supported in BRP.

Syntax: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```


![image](https://github.com/user-attachments/assets/af09b0e3-8710-4461-99ce-a5f868b25260)




## Table of Contents
- [Installation](#Installation)
- [API](#api)
- [Loading Static Assets](#Loading-Static-Assets)
- [Settings](#Settings)
- [Custom Gizmo](#Custom-gizmo)
- [Define Symbols](#define-symbols)

# Установка
Versioning semantics - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
### Unity Package
Поддерживается установка в виде Unity-пакета через добавление [в PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) или ручного добавления в `Packages/manifest.json` этого git-URL: 
```
https://github.com/DCFApixels/Unity-DebugX
```
### Source Code
Пакет так же может быть напрямую скопирован в папку проекта.
</br>


# API

Синтаксис рисования заготовленных Gizmo: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

Рисование кастомных меша и материала:
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

Для оптимизации отрисовки используются статические данные:
```c#
// Статический меш. Обязателен для отрисовки с GPU instancing. 
public struct SomeMesh : IStaticMesh
{
    public Mesh GetMesh() => StaticStorage.SomeMesh;
}
```
```c#
// Статический материал. 
public struct SomeMesh : IStaticMesh
{
    // Контроль порядка выполнения рендереров. 
    public int GetExecuteOrder() => 100;
    public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```

# Загрузка статических ассетов 
Для загрузки имеется утилита `DebugXUtility.LoadStaticData(...);`. 

1) Сначала создаем хранилище для ассетов. 
```c#
public readonly struct SomeAssets
{
    public readonly Mesh SomeMesh;
    public readonly Material SomeMaterial;
} 
```
2) Далее необходимо создать префаб со списком ассетов. Каждый дочерний GameObject префаба рассматривается как один ассет, а его имя должно совпадать с полем в которое будет загружаться ассет. Для загрузки мешей в GameObject необходимо добавить компонент MeshFilter с ссылкой на нужный меш. Для загрузки материала нужен любой компонент унаследованный от Renderer с заданным материалом. Сам префаб должен быть расположен в папке Resources. 
 
![image](https://github.com/user-attachments/assets/191dd337-81d5-43ff-b92e-e8b0927841f9)

3) После подготовки хранилища и префаба-списка можно загружать 
```c#
SomeAssets assets = DebugXUtility.LoadStaticData(new SomeAssets(), "SomeAssets");
// Готово. 
```
> Пример как с этой утилитой работать можно посмотреть в исходинках в файле `DebugXAssets.cs`.

# Настройки
Окно настроек "Tools -> DebugX -> Settings":

![image](https://github.com/user-attachments/assets/7dd981c1-1e00-4b7d-9a73-376638094689)

# Кастомный Gizmo

Кастомная реализация Gizmo:
```c#
public readonly struct SomeGizmo : IGizmo<SomeGizmo>
{
    // Данные. 

    public SomeGizmo(/*...*/)
    {
        // Заполнение данных.
    } 
    
    public IGizmoRenderer<SomeGizmo> RegisterNewRenderer() => new Renderer();
    private class Renderer : IGizmoRenderer<SomeGizmo>
    {
        // Контроль порядка выполнения рендереров. 
        public int ExecuteOrder => 0; //можно использовать default(SomeMat).GetExecutuonOrder();
        // Флаг системе о способе оптимизации.
        // Если метод рисовки или подготовки зависят от текущей камеры, то false，иначе true.
        // Если не уверены то выбирайте false. 
        public bool IsStaticRender => false;

        // Подготовка данных перед рендером, тут можно выполнить дополнительные расчеты или запланировать Job. 
        public void Prepare(Camera camera, GizmosList<SomeGizmo> list) 
        {
            foreach (var item in list)
            {
                //... 
            }
        } 

        // Рендер, тут можно напрямую воспользоваться графическим API или добавить команду в CommandBuffer. 
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
// Создание метода расширения. 
public static class SomeGizmoExtensions
{
    public static DebugX.DrawHandler SomeGizmo(this DebugX.DrawHandler self, /*...*/) 
    {
        self.Gizmo(new SomeGizmo(/*...*/);
        return self;
    }
}
```


# Define Symbols
+ `DISABLE_DEBUGX_INBUILD` - по умолчанию Gizmo будут рисовать в сборке проекта, этот дефайн отключает рисование. Включить или выключить можно так же в окне настроек DebugX.
