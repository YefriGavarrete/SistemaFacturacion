# Sistema_GestionFacturacion — Guía de colaboración

Resumen
- Aplicación Windows Forms (.NET Framework 4.7.2) para gestión de facturación: pedidos, detalles de pedido, facturación, productos, categorías, descuentos, empleados, cargos, usuarios y roles.
- Punto de entrada: `Program.cs` — la app inicia con `FormUsuariosLogin`.
- Objetivo de esta guía: explicar cómo colaborar, reglas de acceso a la base de datos y buenas prácticas para evitar errores comunes.

Requisitos (para contribuir)
- Visual Studio 2022
- .NET Framework 4.7.2 workload instalado
- Microsoft SQL Server (LocalDB, Express o instancia)
- Configurar la `ConnectionString` en `App.config` con la clave `ConexionDB`
  - Ejemplo: `<add name="ConexionDB" connectionString="Server=.;Database=SistemaGestionFacturacion;Trusted_Connection=True;" providerName="System.Data.SqlClient" />`

SQL / Esquema de base de datos
- Scripts sugeridos (ejecutar en SQL Server Management Studio):
  - `Scripts\CreateRolesAndUsuarios.sql` (roles y usuarios con `Clave`/`Sal`/`Iteraciones`)
  - `Scripts\CreatePedidosEmpleadosCargos.sql` (cargos, empleados, pedidos)
  - `Scripts\CreateProductosCategoriasDescuentos.sql` (productos, categorias, descuentos)
  - `Scripts\CreateDetallesPedidosAndFactura.sql` (detalles de pedido y factura)
- Recomendación: ejecutar los scripts en un entorno de pruebas antes de producción.

Reglas obligatorias para consultas y acceso a BD
- Usar únicamente la clase `ConsultasSQL` desde forms y capas de UI para operaciones CRUD:
  - Métodos disponibles: `Buscar`, `Guardar`, `update`, `Eliminar`, `EjecutarConsulta`, `EjecutarComando`.
  - Para consultas parametrizadas y columnas binarios (`VARBINARY`) crear un `SqlCommand`, añadir parámetros y llamar `consulta.EjecutarComando(cmd)`.
- No colocar código de acceso a BD (creación de `SqlConnection` / `SqlCommand` / SQL inline) directamente dentro de event handlers (`btn_Click`) o de la UI.
  - Motivo: evita duplicación, fugas de conexión, errores de DataReader abiertos y facilita revisiones.
- No modificar `Clases\Conexion.cs`. Está establecido para la gestión de la conexión en el proyecto.

Contraseñas seguras
- Las contraseñas deben guardarse como hash PBKDF2 + salt:
  - Columnas recomendadas: `Clave VARBINARY(...)`, `Sal VARBINARY(...)`, `Iteraciones INT`.
  - Para insertar/actualizar hashes usar `SqlDbType.VarBinary` con `EjecutarComando`.
  - En login: leer `Clave`/`Sal` como `byte[]` y verificar con PBKDF2.

DataGridView y columnas binarios
- Nunca bindear columnas `Clave`, `Sal` o similares a DataGridView.
- Excluir esas columnas en el SELECT o eliminar las columnas del DataTable antes de `DataSource = dt`.
- Añadir un manejador `dgv.DataError` para suprimir el diálogo predeterminado y manejar errores de formato.

Buenas prácticas de implementación
- UI: validar entrada y delegar toda consulta a `ConsultasSQL`.
- Para operaciones de actualización, incluir siempre el `Id` (PK) en el SELECT y usar `Id` en la cláusula WHERE.
- Evitar concatenación de SQL con valores directos; preferir comandos parametrizados.
- Usar `using` para `IDisposable` (en los helpers de `ConsultasSQL` ya está).
- Manejar los estados (Activo/Inactivo) en tablas maestras y preferir soft-delete (`Estado`), en vez de borrar físicamente.

Rol y permisos en UI
- El login autentica y devuelve `Rol` (p. ej. `Administrador`, `Empleado`).
- `FormMenu` recibe `rol`, `nombre`, `apellido` y:
  - Muestra mensaje: "Bienvenido <Nombre> <Apellido>" en `lblEmpleado` (o `lblWelcome`).
  - Habilita/Deshabilita botones según rol:
    - Administrador: accesos a todos los formularios (Pedidos, DetallesPedidos, Facturas, Cargos, Empleados, Productos, Categorias, Descuento, Usuarios, Roles).
    - Empleado: solo botones de Pedidos, DetallesPedidos y Facturas.
  - `FormMenu` funciona como MDI container; usar `OpenChildForm(Form child)` para evitar abrir duplicados.

Cómo ejecutar la solución localmente
1. Clona el repositorio y abre la solución en Visual Studio 2022.
2. Actualiza `App.config` > `connectionStrings` > `ConexionDB`.
3. Ejecuta los scripts SQL mencionados en la BD.
4. Compila y ejecuta (F5). La app inicia en `FormUsuariosLogin` (ver `Program.cs`).

Pruebas básicas después de levantar el sistema
- Crear roles (Administrador, Empleado) si no existen.
- Crear un usuario Administrador (registro en `FormUsuariosLogin` o mediante script).
- Probar login con Administrador: abrir `sFormMenu`, verificar botones habilitados.
- Probar login con Empleado: abrir `FormMenu`, verificar buttons restringidos.
- Probar crear producto/categoria/descuento, crear pedido con detalles y generar factura.

Flujo de trabajo Git / Pull Requests
- Rama por tarea: `feature/<short-description>` o `bugfix/<id>-<short>`.
- Commits claros:
  - `feat(users): add password hashing when creating user`
  - `fix(cargos): correct update condition to use IdCargo`
- Push y abrir PR:
  - Crear branch local: `git checkout -b feature/nombre`
  - Añadir y commit: `git add . && git commit -m "feat: ..."`
  - Push: `git push origin feature/nombre`
  - Abrir Pull Request en GitHub describiendo cambios y pasos para probar.
- En el PR incluir:
  - Qué se cambió y por qué
  - SQL migrations (si aplica)
  - Pasos para probar localmente

Notas de mantenimiento
- Centralizar cambios en `ConsultasSQL` cuando necesites logging o manejo de transacciones.
- Si se necesita MARS (MultipleActiveResultSets) por diseño, agregar `MultipleActiveResultSets=True` en la cadena de conexión; preferible refactorizar para usar conexiones locales por operación.
- Para mayor seguridad, considerar cifrar la cadena de conexión en `App.config`.

Contacto / revisión
- Al enviar PR etiqueta a los revisores y añade un checklist breve: DB scripts, pruebas locales y formularios afectados.

---

Archivo inicial sugerido: crear `README.md` en la raíz del proyecto con este contenido.

