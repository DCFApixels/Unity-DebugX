# Unity-DebugX
 
![image](https://github.com/user-attachments/assets/f75e20cd-9614-41e8-887d-943987f4855d)

Многофункциональная, расширяемая и производительная утилита рисования Gizmos для Unity. Работает как в редакторе так и в билде, а рисовать можно и в Update.

Синтаксис: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

![image](https://github.com/user-attachments/assets/97d77716-145d-4357-bcb1-8601871d2fe0)

## Оглавление
- [Установка](#установка)
- [API](#api)
- [Загрузка статических ассетов](#загрузка-статических-ассетов)
- [Настройки](#настройки)
- [Кастомный Gizmo](#кастомный-gizmo)
- [Define Symbols](#define-symbols)

# Установка
Семантика версионирования - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
### Unity-пакет
Поддерживается установка в виде Unity-пакета через добавление [в PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) или ручного добавления в `Packages/manifest.json` этого git-URL: 
```
https://github.com/DCFApixels/Unity-DebugX
```
### В виде исходников
Пакет так же может быть добавлен в проект в виде исходников.
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

Сначала создаем хранилище для ассетов. 
```c#
public readonly struct SomeAssets
{
    public readonly Mesh SomeMesh;
    public readonly Material SomeMaterial;
} 
```
Далее необходимо создать префаб со списком ассетов. Каждый дочерний GameObject префаба рассматривается как один ассет, а его имя должно совпадать с полем в которое будет загружаться ассет. Для загрузки мешей в GameObject необходимо добавить компонент MeshFilter с ссылкой на нужный меш. Для загрузки материала нужен любой компонент унаследованный от Renderer с заданным материалом. Сам префаб должен быть расположен в папке Resources. 

После подготовки хранилища и префаба-списка можно загружать 
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
+ DISABLE_DEBUGX_INBUILD - по умолчанию Gizmo будут рисовать в сборке проекта, этот дефайн отключает рисование. Включить или выключить можно так же в окне настроек DebugX.
