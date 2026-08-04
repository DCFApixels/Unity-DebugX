# AGENTS.md

Usage rules for agents that use the product-ready `Unity-DebugX` package in a Unity project.

## Purpose

`Unity-DebugX` draws non-interactive debug gizmos from regular gameplay or editor code. Use it when you need visual diagnostics similar to `Debug.DrawLine`, but with richer shapes, text, dots, casts, and primitives.

The core usage pattern is:

```csharp
DebugX.Draw(duration, color).Line(start, end);
DebugX.Draw(color).WireSphere(position, radius);
```

Draw calls enqueue gizmo data into DebugX buffers. DebugX renders that data later during its render update. You can call drawing methods from ordinary code such as `Update`, systems, services, utility methods, and debugging helpers.

## Public API Only

Use only public API exposed by the package:

- `DCFApixels.DebugX`
- `DCFApixels.DebugX.DrawHandler`
- `DCFApixels.DebugXEvents`
- `DCFApixels.DebugXLine`
- `DCFApixels.DebugXTextSettings`
- `DCFApixels.DebugXTextSettingsExtensions`
- public extension methods available on `DebugX.DrawHandler`
- public static data marker types from `DCFApixels.DebugXCore`, such as `SphereMesh`, `CubeMesh`, `LitMat`, `UnlitMat`, `WireMat`, when needed for public generic mesh calls

Do not use anything from `Runtime/Internal`. That folder contains implementation details of the package and is not a supported API surface.

Do not rely on private, internal, nested, unsafe, renderer, buffer, pinned-array, type-code, command-buffer, or lifecycle classes. They can change without notice.

## Files Agents Should Ignore

- Do not inspect or depend on `Runtime/Internal/` for normal usage.
- Do not use `Samples/` as required project knowledge. Samples are for users. You may copy simple call patterns from them when you need examples, but do not treat sample scripts, scene setup, or sample utilities as API.
- Do not modify package source, `.asmdef`, `.meta`, `Runtime/Resources`, shaders, meshes, or materials unless the user explicitly asks to develop the package itself.

## Basic Drawing

Import the package namespace:

```csharp
using DCFApixels;
```

Use `DebugX.Draw(...)` to choose color and duration:

```csharp
DebugX.Draw(Color.red).Line(start, end);
DebugX.Draw(1f, Color.yellow).Cube(center, rotation, size);
DebugX.Draw(Color.cyan).WireSphere(center, radius);
```

Common duration forms:

- `DebugX.Draw(color)` - draw with default duration for the current context.
- `DebugX.Draw(duration)` - draw for a duration using the default color.
- `DebugX.Draw(duration, color)` - draw for the specified duration and color.
- `DebugX.Draw()` - default duration and default color.

Methods are chainable when useful:

```csharp
DebugX.Draw(0.5f, Color.green)
    .Line(a, b)
    .WireSphere(b, 0.25f)
    .Text(b, "hit");
```

## Gizmo Call Families

The examples below cover the public gizmo families exposed by the package, not only the calls used in `Samples/`. Many methods have overloads for `Ray`, `Transform`, `Quaternion`, `Vector2`/`Vector3` collections, `float` vs vector sizes, and generic material variants.

Lines, line lists, and line strips:

```csharp
DebugX.Draw(Color.white).Line(start, end);
DebugX.Draw(Color.white).Lines(points);
DebugX.Draw(Color.white).LineStrip(points);
DebugX.Draw(Color.green).LineArrow(start, end);
DebugX.Draw(Color.green).LineFade(start, end);
DebugX.Draw(Color.yellow).Line(start, end, DebugXLine.Arrow, DebugXLine.Fade);
DebugX.Draw(Color.cyan).WidthLine(start, end, 0.1f);
DebugX.Draw(Color.cyan).WidthOutLine(start, end, 0.1f);
DebugX.Draw(Color.magenta).ZigzagLine(start, end, 0.3f);
DebugX.Draw(Color.white).Distance(start, end);
```

Rays and ray-shaped helpers:

```csharp
DebugX.Draw(Color.white).Ray(origin, direction);
DebugX.Draw(Color.cyan).Ray(origin, direction, DebugXLine.Arrow);
DebugX.Draw(Color.green).RayArrow(origin, direction);
DebugX.Draw(Color.green).RayFade(origin, direction);
DebugX.Draw(Color.yellow).RayWireBox(origin, direction, rotation, size);
DebugX.Draw(Color.yellow).RayWireSphere(origin, direction, radius);
DebugX.Draw(Color.yellow).RayWireCapsule(origin, direction, rotation, radius, height);
```

Line-to-shape helpers:

```csharp
DebugX.Draw(Color.yellow).LineWireBox(start, end, rotation, size);
DebugX.Draw(Color.yellow).LineWireSphere(start, end, radius);
DebugX.Draw(Color.yellow).LineWireCapsule(start, end, rotation, radius, height);
```

3D primitives:

```csharp
DebugX.Draw(Color.yellow).Cube(position, rotation, size);
DebugX.Draw(Color.yellow).WireCube(position, rotation, size);
DebugX.Draw(Color.yellow).CubePoints(position, rotation, size);
DebugX.Draw(Color.yellow).CubeGrid(position, rotation, size, Vector3Int.one * 3);
DebugX.Draw(Color.cyan).Sphere(position, radius);
DebugX.Draw(Color.cyan).WireSphere(position, radius);
DebugX.Draw(Color.blue).Cylinder(position, rotation, radius, height);
DebugX.Draw(Color.blue).WireCylinder(position, rotation, radius, height);
DebugX.Draw(Color.green).Capsule(position, rotation, radius, height);
DebugX.Draw(Color.green).Capsule(point1, point2, radius);
DebugX.Draw(Color.green).WireCapsule(position, rotation, radius, height);
DebugX.Draw(Color.red).Cone(position, rotation, radius, height);
DebugX.Draw(Color.red).WireCone(position, rotation, radius, height);
```

Planar and 2D-style primitives:

```csharp
DebugX.Draw(Color.yellow).Quad(position, rotation, size);
DebugX.Draw(Color.yellow).WireQuad(position, rotation, size);
DebugX.Draw(Color.yellow).QuadPoints(position, rotation, size);
DebugX.Draw(Color.yellow).QuadGrid(position, rotation, size, Vector2Int.one * 3);
DebugX.Draw(Color.red).Triangle(position, rotation, size);
DebugX.Draw(Color.red).WireTriangle(position, rotation, size);
DebugX.Draw(Color.cyan).Circle(position, rotation, radius);
DebugX.Draw(Color.cyan).Circle(position, normal, radius);
DebugX.Draw(Color.cyan).WireCircle(position, rotation, radius);
DebugX.Draw(Color.green).FlatCapsule(position, rotation, radius, height);
DebugX.Draw(Color.green).WireFlatCapsule(position, rotation, radius, height);
```

Billboard and dot markers:

```csharp
DebugX.Draw(Color.white).BillboardCircle(position, radius);
DebugX.Draw(Color.white).BillboardCross(position, size);
DebugX.Draw(Color.white).Dot(position);
DebugX.Draw(Color.white).WireDot(position);
DebugX.Draw(Color.white).DotQuad(position);
DebugX.Draw(Color.white).WireDotQuad(position);
DebugX.Draw(Color.white).DotDiamond(position);
DebugX.Draw(Color.white).WireDotDiamond(position);
DebugX.Draw(Color.yellow).DotCross(position);
```

Text labels:

```csharp
DebugX.Draw(Color.cyan).Text(position, "Debug label");
DebugX.Draw(Color.cyan).Text(
    position,
    "World label",
    DebugXTextSettings.WorldSpace.Size(22).Anchor(TextAnchor.MiddleCenter));
```

Meshes:

```csharp
DebugX.Draw(Color.white).Mesh(mesh, position, rotation, size);
DebugX.Draw(Color.white).UnlitMesh(mesh, position, rotation, size);
DebugX.Draw(Color.white).WireMesh(mesh, position, rotation, size);
DebugX.Draw(Color.white).Mesh(mesh, matrix);
DebugX.Draw(Color.white).WireMesh<SphereMesh>(position, rotation, size);
DebugX.Draw(Color.white).Mesh<SphereMesh, UnlitMat>(position, rotation, size);
```

For generic mesh/material marker calls, also import:

```csharp
using DCFApixels.DebugXCore;
```

Bounds, colliders, frustums, projection, and arcs:

```csharp
DebugX.Draw(Color.yellow).Bounds(renderer);
DebugX.Draw(Color.yellow).Frustum(camera);
DebugX.Draw(Color.yellow).Frustum(center, rotation, fov, farClipPlane, nearClipPlane, aspect);
DebugX.Draw(Color.cyan).Projection(plane, point, circleRadius);
DebugX.Draw(Color.cyan).WireArc(center, normal, from, angle, radius);
```

Collider helpers require their matching physics define symbols:

```csharp
DebugX.Draw(Color.yellow).Bounds(collider);
DebugX.Draw(Color.yellow).Collider(boxCollider);
DebugX.Draw(Color.yellow).Collider(sphereCollider);
DebugX.Draw(Color.yellow).Collider(capsuleCollider);
DebugX.Draw(Color.yellow).Collider(characterController);
DebugX.Draw(Color.yellow).Collider(meshCollider);
DebugX.Draw(Color.yellow).Bounds(collider2D);
DebugX.Draw(Color.yellow).Collider(boxCollider2D);
DebugX.Draw(Color.yellow).Collider(circleCollider2D);
DebugX.Draw(Color.yellow).Collider(capsuleCollider2D);
```

Raycast and cast visualizers require their matching physics define symbols:

```csharp
DebugX.Draw(Color.green).RaycastHit(hit);
DebugX.Draw(Color.green).Raycast(ray, hit);
DebugX.Draw(Color.green).SphereCast(ray, radius, hit);
DebugX.Draw(Color.green).BoxCast(ray, rotation, size, hit);
DebugX.Draw(Color.green).CapsuleCast(origin, direction, rotation, radius, height, hit);
DebugX.Draw(Color.green).CapsuleCast(point1, point2, direction, radius, hit);
DebugX.Draw(Color.green).RaycastHit(hit2D);
DebugX.Draw(Color.green).Raycast2D(ray2D, hit2D);
DebugX.Draw(Color.green).CircleCast2D(ray2D, radius, hit2D);
DebugX.Draw(Color.green).BoxCast2D(ray2D, angle, size, hit2D);
DebugX.Draw(Color.green).CapsuleCast2D(ray2D, angle, size, CapsuleDirection2D.Vertical, hit2D);
```

## Define Symbols

Supported define symbols:

- `DEBUGX_DISABLE_INBUILD` - disables DebugX drawing in builds.
- `DEBUGX_ENABLE_PHYSICS2D` - enables Physics2D gizmo helpers.
- `DEBUGX_ENABLE_PHYSICS3D` - enables Physics3D gizmo helpers.
- `DISABLE_DEBUGX` - hard-disables DebugX through preprocessor blocks.

The settings window is available at:

```text
Tools -> DebugX -> Settings
```

Use it to adjust global display settings and package define symbols when working inside Unity.

## Usage Guidelines

- Prefer DebugX for temporary or diagnostic visualization, not gameplay visuals.
- Keep draw calls close to the code being diagnosed so they can be removed easily.
- Prefer short durations for frequently called code paths.
- Use one-frame/default drawing from `Update` or repeated systems when the value changes every frame.
- Use explicit duration such as `DebugX.Draw(1f, color)` for event-style diagnostics.
- Use `DebugX.ClearAllGizmos()` when a tool or workflow needs to wipe currently buffered gizmos.
- Use `DebugXEvents.OnDrawGizmo` only when you specifically need a draw callback during DebugX rendering.
- Do not add your own render loop, command buffers, or package lifecycle hooks for normal usage.
- Do not instantiate or manage package meshes/materials manually; call the public drawing methods instead.

## Text Guidelines

Use `DebugXTextSettings.ScreenSpace` for labels that should stay readable in screen space:

```csharp
DebugX.Draw(Color.white).Text(position, "speed", DebugXTextSettings.ScreenSpace);
```

Use `DebugXTextSettings.WorldSpace` for labels that should scale with world distance:

```csharp
DebugX.Draw(Color.white).Text(position, "spawn", DebugXTextSettings.WorldSpace);
```

Customize text with extension methods:

```csharp
DebugX.Draw(Color.white).Text(
    position,
    "target",
    DebugXTextSettings.WorldSpace
        .Size(18)
        .Anchor(TextAnchor.UpperCenter)
        .BackgroundColor(Color.black));
```

## Package Metadata

- Unity package id: `com.dcfa_pixels.debugx`
- Display name: `DebugX`
- Minimum Unity version in `package.json`: `2021.3`
- Runtime asmdef: `DCFApixels.DebugX`
- Editor asmdef: `DCFApixels.DebugX.Editor`
- Samples asmdef: `DCFApixels.DebugX.Samples`
