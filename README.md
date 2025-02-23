# Unity-DebugX
 
![image](https://github.com/user-attachments/assets/fb3edbce-9164-4ad7-a7a2-85748edf58e0)

Продвинутый Debug.DrawLine движка Unity. 
Многофункциональный, расширяемый и высоко производительный инструмент для рисования Gizmos.

API для рисования заготовленные Gizmo: 
'''c#
DebugX.Draw(duration, color).*Gizmo Function*(...);
'''

API для рисования кастомного меша и материала:
'''c#
//Рисования любого меша lit материалом. Без instansing. 
DebugX.Draw(...).Mesh(mesh, pos, rot, sc);
//UnlitMesh - меш с unlit материалом
//WireMesh - меш с wireframe материалом

//Рисования статического меша lit материалом. В режиме instansing. 
DebugX.Draw(...).Mesh<IStaticMesh>(pos, rot, sc);

//Рисования статического меша кастомным материалом. В режиме instansing. 
DebugX.Draw(...).Mesh<IStaticMesh, IStaticMat>(pos, rot, sc);
'''
