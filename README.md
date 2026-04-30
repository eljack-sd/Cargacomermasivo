# CargaComerMasivo 2.0
### Carga Masiva de Documentos para CONTPAQi Comercial Premium

---

## REQUISITOS ANTES DE ABRIR EN VISUAL STUDIO

1. **Visual Studio 2019 o 2022** (Community es gratis)
2. **.NET Framework 4.8** (ya viene en Windows 10/11)
3. **CONTPAQi Comercial Premium** instalado en:
   `C:\Program Files (x86)\Compac\COMERCIAL\`

---

## PASOS PARA COMPILAR

### 1. Instalar EPPlus (lector de Excel)
Abre el proyecto en Visual Studio, luego en el menú:
```
Herramientas → Administrador de paquetes NuGet → Consola
```
Escribe:
```
Install-Package EPPlus -Version 4.5.3.3
```

### 2. Configurar plataforma x86
**MUY IMPORTANTE** — el SDK de CONTPAQi es 32 bits.

En Visual Studio:
- Menú Build → Configuration Manager
- En Platform: selecciona **x86**
- Si no aparece x86, haz clic en "New" y agrégalo

### 3. Compilar y ejecutar
- Presiona F5 o Ctrl+F5

---

## USO DE LA APLICACIÓN

### Pantalla de Conexión
1. Ingresa la ruta de tu empresa (ej: `C:\Compac\Empresas\TUEMPRESA\`)
2. Haz clic en **"Abrir Empresa"**

### Pantalla Principal
1. **Cargar Excel**: haz clic en "Examinar" y selecciona tu archivo
   - Layout esperado (columnas en orden):
     | A | B | C | D | E | F |
     |---|---|---|---|---|---|
     | Código Producto | Descripción | Cantidad | Precio | Almacén | Referencia |

2. **Llenar el Encabezado**: selecciona Concepto, Fecha, Cliente, etc.

3. **Siguiente Folio**: obtiene automáticamente el siguiente folio de CONTPAQi

4. **Carga Masiva**: impacta todos los documentos del Excel a CONTPAQi

---

## AJUSTAR EL LAYOUT DEL EXCEL

Si tus columnas del Excel están en diferente orden, edita estas constantes
en el archivo `FrmPrincipal.cs`:

```csharp
private const int COL_CODIGO_PRODUCTO = 0;  // columna A
private const int COL_DESCRIPCION     = 1;  // columna B
private const int COL_UNIDADES        = 2;  // columna C
private const int COL_PRECIO          = 3;  // columna D
private const int COL_ALMACEN         = 4;  // columna E
private const int COL_REFERENCIA      = 5;  // columna F
```

---

## ERRORES COMUNES

| Error | Solución |
|-------|----------|
| "No se puede cargar MGW_SDK.dll" | Verifica que CONTPAQi esté instalado y compilas en x86 |
| fAbreEmpresa regresa error | Cierra CONTPAQi primero, verifica la ruta de empresa |
| fNuevoDocumento regresa negativo | Verifica que el concepto y cliente sean válidos |
| Error al agregar movimiento | El código del producto no existe en CONTPAQi |

---

Desarrollado con base en el proyecto original VB6 CargaComerMasivoMM
