# Unity-DebugX
 
![image](https://github.com/user-attachments/assets/fb3edbce-9164-4ad7-a7a2-85748edf58e0)

Многофункциональный, расширяемый и высоко производительный инструмент для рисования Gizmos для Unity.

API для рисования заготовленные Gizmo: 
```c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
```

API для рисования кастомного меша и материала:
```c#
//Рисования любого меша lit материалом. Без GPU instancing. 
DebugX.Draw(...).Mesh(mesh, pos, rot, sc);
//UnlitMesh - меш с unlit материалом
//WireMesh - меш с wireframe материалом

//Рисования статического меша lit материалом. В режиме GPU instancing. 
DebugX.Draw(...).Mesh<IStaticMesh>(pos, rot, sc);

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

// Статический материал. 
public struct SomeMesh : IStaticMesh
{
	public int GetExecutuonOrder() => 100;
	public Mesh GetMaterial() => StaticStorage.SomeMaterial;
} 
```
Утилита для загрузки мешей: //TODO

Кастомная реализация Gizmo:
```c#
public readonly struct SomeGizmo : IGizmo<SomeGizmo>
{
    // data... 

    public SomeGizmo(/*...*/)
    {
        //... 
    } 
    
    public IGizmoRenderer<SomeGizmo> RegisterNewRenderer() => new Renderer();
    private class Renderer : IGizmoRenderer<SomeGizmo>
    {
        // Контроль порядка выполнения рендереров. 
        public int ExecuteOrder => //...
        // Флаг системе о способе оптимизации. Если методы рисовки зависят от текущей камеры, то false，иначе true. Если не уверены то выбирайте false. 
        public bool IsStaticRender => //...

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