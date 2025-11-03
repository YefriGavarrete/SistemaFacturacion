# Sistema_GestionFacturacion — Guía de Colaboración

Propósito
- Este repositorio es un sistema de facturación de Windows Forms (.NET Framework 4.7.2).

- Funcionalidades principales: pedidos, detalles de pedidos, generación de facturas, gestión de productos (categorías, descuentos), gestión de usuarios/roles.

Objetivo de este documento
- Definir cómo debe colaborar el equipo y cómo deben implementarse las consultas a la base de datos.

- Evitar errores comunes (insertar lógica SQL/DB dentro de los controladores de botones, duplicar la lógica de conexión, modificar el comportamiento establecido de `Conexion`).

Requisitos previos (para colaboradores)

- Visual Studio 2022

- .NET Framework 4.7.2

- Microsoft SQL Server (Express/LocalDB o instancia completa)

- Una cadena de conexión válida en `App.config` bajo el nombre `ConexionDB`

- Ejemplo: `Server=.;Database=SistemaGestionFacturacion;Trusted_Connection=True;`

- Asegúrese de que exista el esquema de la base de datos (roles, usuarios, productos, categorías, descuentos, pedidos, detalles, factura). Ejecute los scripts SQL proporcionados, si los hay.

NO modifique
- `Sistema_GestionFacturacion\Clases\Conexion.cs` — esta clase y su semántica de ciclo de vida forman parte de la infraestructura establecida del proyecto. Todo acceso a la base de datos debe respetarla o utilizar los wrappers de `ConsultasSQL`.

Regla principal para el acceso a la base de datos

- Utilice únicamente la clase `ConsultasSQL` para las operaciones de base de datos desde formularios y lógica de negocio.

- Funciones auxiliares disponibles: `Buscar`, `Guardar`, `update`, `Eliminar`, `EjecutarConsulta`, `EjecutarComando`.

- No inserte cadenas SQL sin procesar ni código de `SqlConnection`/`SqlCommand` directamente en los controladores de clic de botones o de eventos de la interfaz de usuario.

- Para comandos parametrizados (especialmente para columnas binarias como `Clave`/`Sal`), cree un `SqlCommand`, añada parámetros y llame a `consulta.EjecutarComando(cmd)`.

¿Por qué?

- Centralizar el acceso a la base de datos simplifica la gestión de conexiones, el registro y el manejo de errores.

- Evita código duplicado y errores de concurrencia (lectores abiertos/estado de conexión).

- Facilita la revisión y las pruebas de código.


Patrones recomendados

- Capa de interfaz de usuario (formularios): lógica mínima:

- Validar la entrada de la interfaz de usuario.

- Llamar a un método específico de la aplicación que utilice `ConsultasSQL`.

- No coloque lógica SQL directamente en `btnClick`.

Ejemplo de patrón (pseudocódigo)

- En la forma:

- Cree un método que llame a `ConsultasSQL`:

- `void SaveUser() { /* validar */ consulta.Guardar(tabla, cols, vals); }`

- `btnSave_Click` solo debe llamar a `SaveUser()`.

Columnas binarias de la base de datos (hash de contraseña)

- Si `Usuarios.Clave`/`Usuarios.Sal` son `VARBINARY`, use `SqlDbType.VarBinary` y `EjecutarComando` (no `Guardar` con cadenas en línea).

- Si necesita almacenar una cadena Base64 en `NVARCHAR`, use `Buscar`/`Guardar` según corresponda; sin embargo, se recomienda almacenar bytes como VARBINARY con comandos parametrizados.

Notas sobre DataGridView

- No vincule las columnas de contraseña o sal a `DataGridView`.

- Elimine `Clave`, `Sal` e `Iteraciones` de la DataTable antes de `DataSource = dt`, o exclúyalas en la consulta SELECT.

- Agregue un controlador `dgv.DataError` para evitar el cuadro de diálogo de error predeterminado de DataGridView.

Convenciones de codificación (breve)

- Utilice siempre `using` para los recursos `IDisposable` (los métodos de `ConsultasSQL` ya lo hacen).

- Escapar los valores de cadena solo al crear SQL seguro para `Guardar`/`update`. Prefiera los comandos parametrizados para todas las entradas del usuario.

- Mantenga los métodos pequeños y con una sola responsabilidad: la interfaz de usuario debe ser sencilla; las llamadas a la base de datos se realizan en `ConsultasSQL` o en pequeños contenedores de repositorio.

Flujo de trabajo de desarrollo (Git / GitHub)

- Ramificación:

- Cree una rama por tarea: `feature/<descripción-breve>` o `bugfix/<id>-breve`.

- Mantén las ramas pequeñas y enfocadas.

- Confirmaciones:

- Usa mensajes descriptivos: `feat(users): agregar formulario de creación de usuario` o `fix(products): validación de precio`.

- Agrupa los cambios relacionados en una sola solicitud de extracción (PR) cuando sea posible.

- Solicitudes de extracción (PR):

- Abre una PR contra `main` (o la rama de integración designada).

- Incluye una breve descripción, los pasos para probar y cualquier cambio necesario en el script de la base de datos.

- Al menos un revisor debe aprobar antes de fusionar.

- Lista de verificación para la revisión de código:

- No uses SQL directamente en los controladores de eventos.

- Usa `ConsultasSQL` para acceder a la base de datos.

- La cadena de conexión no se modifica en `Connection.cs`.

- Las contraseñas se manejan de forma segura (PBKDF2 + salt) y se almacenan como VARBINARY cuando sea posible.

- Los DataGridView no enlazan columnas binarias.

- Incluye pasos para pruebas unitarias/manuales.

Cómo ejecutar localmente

1. Clonar el repositorio.

2. Abrir la solución en Visual Studio 2022.
3. Actualizar `App.config` > `connectionStrings` > `ConnectionDB` con la instancia de SQL Server y la base de datos.

4. Ejecutar los scripts de creación de la base de datos (consultar `Scripts/` o preguntar al responsable del mantenimiento si faltan).

5. Compilar y ejecutar la aplicación.

6. Usar un usuario administrador de prueba o crear usuarios mediante los formularios proporcionados.

Cómo enviar cambios para su revisión:

- Crear una rama de desarrollo.

- Realizar los cambios, ejecutar y probar localmente.

- Confirmar los cambios con mensajes claros y subir la rama al repositorio remoto.

- Abrir una solicitud de extracción en GitHub describiendo:

- Qué cambió y por qué

- Cómo probar (scripts de base de datos, datos de muestra)

- Pasos de migración
- Etiquetar a los revisores y esperar la revisión manual o de integración continua.

Descripción general de archivos y formularios (actual)

- Formularios:

- `Login` — autenticación; Abre `FormRoles` (administrador) o `FormPedidos` (empleado).

- `FormPedidos`: crea pedidos; solo requiere el `Nombre` y el `Apellido` del empleado.

- `FormUsuariosLogin`: creación y gestión de usuarios.

- `FormRoles`: gestión de roles.

- `FormUsuarios`: (variantes de gestión de productos/usuarios).

- `Logica_CRUD`: formulario auxiliar de orquestación (menú principal).

- Clases:

- `Clases\ConsultasSQL.cs`: auxiliar central de la base de datos (usar este).

- `Clases\Conexion.cs`: utilidad de conexión compartida (no modificar).

- `Clases\AlertasDelSistema.cs`: alertas de la interfaz de usuario.

Recordatorios finales:

- Centralizar el acceso a la base de datos mediante `ConsultasSQL`.

- Mantener los controladores de la interfaz de usuario pequeños y llamar a métodos que utilicen `ConsultasSQL`. - Nunca modifiques `Conexion.cs`.

- Usa `EjecutarComando` para los comandos `SqlCommand` parametrizados (columnas VARBINARY).

- Sigue el flujo de trabajo de Git: rama → commit → PR → revisión → fusión.

- Agregar un script SQL de ejemplo `Scripts/CreateSchema.sql` (roles, usuarios con columnas VARBINARY).
- Agregar un archivo `CONTRIBUTING.md` ligero con una plantilla para PR. Cómo ejecutar localmente


