# Unity-DebugX
 
![image](https://github.com/user-attachments/assets/fb3edbce-9164-4ad7-a7a2-85748edf58e0)

Многофункциональная, расширяемая и высокопроизводительная утилита рисования Gizmos для Unity. Работает как в редакторе так и в билде, а вызывать методы отрисовки можно в Update.

Синтаксис рисования заготовленныех Gizmo: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

![image](https://github.com/user-attachments/assets/97d77716-145d-4357-bcb1-8601871d2fe0)

API для рисования кастомного меша и материала:
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
// Статический меш. 
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
    public int GetExecutuonOrder() => 100;
    public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```
Окно настроек "Tools -> DebugX -> Settings":

![image](https://github.com/user-attachments/assets/7dd981c1-1e00-4b7d-9a73-376638094689)



Утилита для загрузки статических ассетов: //TODO

Кастомная реализация Gizmo:
```c#
public readonly struct SomeGizmo : IGizmo<SomeGizmo>
{
    // Данные. 

    public SomeGizmo(/*...*/)
    {
        //... 
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
    public static DrawHandler SomeGizmo(this DrawHandler self, /*...*/) 
    {
        self.Gizmo(new SomeGizmo(/*...*/);
        return self;
    }
}
```
