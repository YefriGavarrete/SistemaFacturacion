# Sistema_GestionFacturacion � Gu�a de colaboraci�n
Resumen
- Aplicaci�n Windows Forms (.NET Framework 4.7.2) para gesti�n de facturaci�n: pedidos, detalles de pedido, facturaci�n, productos, categor�as, descuentos, empleados, cargos, usuarios y roles.
- Punto de entrada: `Program.cs` � la app inicia con `FormUsuariosLogin`.
- Objetivo de esta gu�a: explicar c�mo colaborar, reglas de acceso a la base de datos y buenas pr�cticas para evitar errores comunes.

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
- Recomendaci�n: ejecutar los scripts en un entorno de pruebas antes de producci�n.

Reglas obligatorias para consultas y acceso a BD
- Usar �nicamente la clase `ConsultasSQL` desde forms y capas de UI para operaciones CRUD:
  - M�todos disponibles: `Buscar`, `Guardar`, `update`, `Eliminar`, `EjecutarConsulta`, `EjecutarComando`.
  - Para consultas parametrizadas y columnas binarios (`VARBINARY`) crear un `SqlCommand`, a�adir par�metros y llamar `consulta.EjecutarComando(cmd)`.
- No colocar c�digo de acceso a BD (creaci�n de `SqlConnection` / `SqlCommand` / SQL inline) directamente dentro de event handlers (`btn_Click`) o de la UI.
  - Motivo: evita duplicaci�n, fugas de conexi�n, errores de DataReader abiertos y facilita revisiones.
- No modificar `Clases\Conexion.cs`. Est� establecido para la gesti�n de la conexi�n en el proyecto.

Contrase�as seguras
- Las contrase�as deben guardarse como hash PBKDF2 + salt:
  - Columnas recomendadas: `Clave VARBINARY(...)`, `Sal VARBINARY(...)`, `Iteraciones INT`.
  - Para insertar/actualizar hashes usar `SqlDbType.VarBinary` con `EjecutarComando`.
  - En login: leer `Clave`/`Sal` como `byte[]` y verificar con PBKDF2.

DataGridView y columnas binarios
- Nunca bindear columnas `Clave`, `Sal` o similares a DataGridView.
- Excluir esas columnas en el SELECT o eliminar las columnas del DataTable antes de `DataSource = dt`.

Buenas pr�cticas de implementaci�n
- UI: validar entrada y delegar toda consulta a `ConsultasSQL`.
- Para operaciones de actualizaci�n, incluir siempre el `Id` (PK) en el SELECT y usar `Id` en la cl�usula WHERE.
- Evitar concatenaci�n de SQL con valores directos; preferir comandos parametrizados.
- Usar `using` para `IDisposable` (en los helpers de `ConsultasSQL` ya est�).
- Manejar los estados (Activo/Inactivo) en tablas maestras y preferir soft-delete (`Estado`), en vez de borrar f�sicamente.

Rol y permisos en UI
- El login autentica y devuelve `Rol` (p. ej. `Administrador`, `Empleado`).
- `FormMenu` recibe `rol`, `nombre`, `apellido` y:
  - Muestra mensaje: "Bienvenido <Nombre> <Apellido>" en `lblEmpleado` (o `lblWelcome`).
  - Habilita/Deshabilita botones seg�n rol:
    - Administrador: accesos a todos los formularios (Pedidos, DetallesPedidos, Facturas, Cargos, Empleados, Productos, Categorias, Descuento, Usuarios, Roles).
    - Empleado: solo botones de Pedidos, DetallesPedidos y Facturas.
  - `FormMenu` funciona como MDI container; usar `OpenChildForm(Form child)` para evitar abrir duplicados.

C�mo ejecutar la soluci�n localmente
1. Clona el repositorio y abre la soluci�n en Visual Studio 2022.
2. Actualiza `App.config` > `connectionStrings` > `ConexionDB`.
3. Ejecuta los scripts SQL mencionados en la BD.
4. Compila y ejecuta (F5). La app inicia en `FormUsuariosLogin` (ver `Program.cs`).

Pruebas b�sicas despu�s de levantar el sistema
- Crear roles (Administrador, Empleado) si no existen.
- Crear un usuario Administrador (registro en `FormUsuariosLogin` o mediante script).
- Probar login con Administrador: abrir `FormMenu`, verificar botones habilitados.
- Probar login con Empleado: abrir `FormMenu`, verificar buttons restringidos.
- Probar crear producto/categoria/descuento, crear pedido con detalles y generar factura.
- Verificar que las operaciones CRUD funcionan correctamente y que los datos se reflejan en la BD.
- Hacer formularios de productos que es es el que contendra los productos almacenar.

Flujo de trabajo Git / Pull Requests
- Rama por tarea: `feature/<short-description>` o `bugfix/<id>-<short>`.
- Commits claros:
  - `feat(users): add password hashing when creating user`
  - `fix(cargos): correct update condition to use IdCargo`
- Push y abrir PR:
  - Crear branch local: `git checkout -b feature/nombre`
  - A�adir y commit: `git add . && git commit -m "feat: ..."`
  - Push: `git push origin feature/nombre`
  - Abrir Pull Request en GitHub describiendo cambios y pasos para probar.
- En el PR incluir:
  - Qu� se cambi� y por qu�
  - SQL migrations (si aplica)
  - Pasos para probar localmente

Notas de mantenimiento
- Centralizar cambios en `ConsultasSQL` cuando necesites logging o manejo de transacciones.
- Si se necesita MARS (MultipleActiveResultSets) por dise�o, agregar `MultipleActiveResultSets=True` en la cadena de conexi�n; preferible refactorizar para usar conexiones locales por operaci�n.
- Para mayor seguridad, considerar cifrar la cadena de conexi�n en `App.config`.

Contacto / revisi�n
- Al enviar PR etiqueta a los revisores y a�ade un checklist breve: DB scripts, pruebas locales y formularios afectados.

