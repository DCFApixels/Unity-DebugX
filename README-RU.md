
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
        <img src="https://github.com/user-attachments/assets/3c699094-f8e6-471d-a7c1-6d2e9530e721"></br>
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
        <img src="https://github.com/user-attachments/assets/8e598a9a-826c-4a1f-b842-0c56301d2927"></br>
        <span>中文</span>
      </a>  
    </td>
  </tr>
</table>

</br>

Многофункциональная, расширяемая и производительная утилита рисования Gizmos для Unity. Работает как в редакторе так и в сборке, а рисовать можно как в `OnDrawGizmos` так и в `Update`. Поддерживается HDRP, URP и BRP, но в BRP не поддерживается отрисовка в `OnDrawGizmos`.

Синтаксис:
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

![image](https://github.com/user-attachments/assets/af09b0e3-8710-4461-99ce-a5f868b25260)

<br>

## Оглавление
- [Установка](#установка)
- [Базовый API](#api)
- [Настройки](#настройки)
- [Расширение API](#кастомный-gizmo)
- [Загрузка статических ассетов](#загрузка-статических-ассетов)
- [Define Symbols](#define-symbols)

<br>

# Установка
Семантика версионирования - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
### Unity-Package
Поддерживается установка в виде Unity-модуля, достаточно добавить [в PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html)  этот Git-URL :
```
https://github.com/DCFApixels/Unity-DebugX.git
```
Или скопировать в `Packages/manifest.json` эту строчку:
```
"com.dcfa_pixels.debugx": "https://github.com/DCFApixels/Unity-DebugX.git"
```
### В виде исходников
Пакет так же может быть напрямую скопирован в папку проекта.

</br>

# Базовый API

Общий синтаксис рисования заготовленных Gizmo:
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```
</br>

Среди заготовленных Gizmo есть разные вариации примитивов, линий, точек и текст. Пример некоторых Gizmo:
```c#
// Рисует обычную линию аналогично Debug.DrawLine(...).
// Показываться линия будет в течении секунды и красного цвета.
DebugX.Draw(1, Color.red).Line(startPoint, endPoint);
```
```c#
// Рисует куб, но всего один кадр и желтого цвета.
DebugX.Draw(Color.yellow).Cube(center, rotation, size);
```
```c#
// Рисует сферу.
DebugX.Draw(Color.yellow).Cube(center, radius);
```
```c#
// Рисует точку, точка имеет screen space размер. 
DebugX.Draw(Color.yellow).Dot(startPoint, endPoint);
```
```c#
// Рисует текст. Текст так же может показываться в течении заданного времени.
DebugX.Draw(1, Color.red).Text(center, text);
// Для расширенной настройки отображения используется DebugXTextSettings.
DebugX.Draw(Color.yellow).Text(center, text, DebugXTextSettings.Default.SetBackgroundColor(Color.black));
```
</br>


На случай если не хватает заготовленных примитивов есть методы для рисования кастомных меша и материала:
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
    // Контроль порядка выполнения отрисовки. 
    public int GetExecuteOrder() => 100;
    public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```
> В примере StaticStorage это условная реализация статического хранилища ассетов. Так как в Unity статические данные нельзя заполнять через инспектор, решение этой проблемы описано в главе [Загрузка статических ассетов](#загрузка-статических-ассетов).

<br>

# Настройки
Окно настроек "Tools -> DebugX -> Settings":

![image](https://github.com/user-attachments/assets/dee3954a-1c8a-442c-a4bb-d2c710e065a4)

<br>

# Расширение API
Самый простой вариант это создание метода расширения который комбинирует заготовленные гизмо, например:
```c#
public static class SomeGizmoExtensions
{
    public static DebugX.DrawHandler Distance(this DebugX.DrawHandler self, Vector3 start, Vector3 end)
    {
        // Рисуем линию.
        self.Line(start, end);
        // Рисуем текст посреди линии который показывает длину линии.
        self.Text(Vector3.Lerp(start, end, 0.5f), Vector3.Distance(start, end), DebugXTextSettings.Default.SetAnchor(TextAnchor.UpperCenter));
        // для поддержки Method Chaining синтаксиса.
        return self;
    }
}
```
> Так же можно использовать методы `Mesh` для создания методов рисования других примитивов.

Расширенная реализация Gizmo, на случай нехватки встроенных методов отрисовки:
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

<br>

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
> Пример как с этой утилитой работать можно посмотреть в исходниках в файле `DebugXAssets.cs`.

<br>

# Define Symbols
Все Define Symbols можно менять в окне настроек.
+ `DEBUGX_DISABLE_INBUILD` - По умолчанию Gizmo будут рисовать в сборке проекта, этот дефайн отключает рисование.
+ `DEBUGX_ENABLE_PHYSICS2D` - Включает Gizmos для Physics2D.
+ `DEBUGX_ENABLE_PHYSICS3D` - Включает Gizmos для Physics3D.