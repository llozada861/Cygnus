# ClipBoardCygnus (.NET Framework 4.7.2 · Escritorio Local + SQLite Portable)

Configuración del ensamblado y proyecto:
- **Nombre del proyecto / ensamblado**: `ClipBoardCygnus`
- **Espacio de nombres predeterminado**: `ClipBoardCygnus`
- **Plataforma de destino**: `.NET Framework 4.7.2`
- **Tipo de salida**: `Aplicación Windows` (`WinExe`)
- **Base de datos local**: `SQLite` (`.\CygnusData\ClipBoardCygnus.db` en modo portable)
- **Icono**: `icons8-código-fuente-64.ico`

## Plantillas de código con variables bind
- Crea, edita y elimina plantillas en SQLite (`code_templates`) desde el botón **Plantillas** o desde la bandeja del sistema.
- Usa sintaxis `{{variable}}` (por ejemplo `{{tabla}}`, `{{filtro}}`) y valores por defecto opcionales.
- Al usar una plantilla, asigna los valores a cada variable con vista previa en vivo y copia o pega directamente en la aplicación activa.

## Instalación de SQLite en Visual Studio
1. Abre `ClipBoardCygnus.csproj` en Visual Studio (o añade los archivos `.cs` a tu solución).
2. Restaura el paquete NuGet **`System.Data.SQLite.Core`** (v1.0.118).
3. Asegúrate de tener referenciados los ensamblados estándar: `System.Runtime.Serialization`, `System.Drawing`, `System.Windows.Forms`.

## Integración en 1 línea (si lo llamas desde otro formulario)
```csharp
ClipBoardCygnus.ClipboardManager.Initialize(this);
```
