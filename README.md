## Ejercicio práctico

Developer: Danilo A.

### Instrucciones

- 

## Criterios de Diseño

Las siguientes deciciones las realicé tomando en cuenta que lo importante es demostrar que tengo habilidad de desarrollo de backend pero en el contexto de un ejercicio con tiempo limitado, y con cierto grado de libertad en los requerimientos. Podría hacerse tan complejo según situaciones hipotéticas del contexto de dominio o tan simple que solo cumpla lo básico. Por esta razón busqué un equilibrio entre tiempo del ejercicio, demostración de habilidad y complejidad de la solución. 

### Decisiones de dominio

- Los movimientos no deberían actualizarse porque representan algo que ya sucedió, sí ya se creó no tiene sentido modificarlo, en lugar de ello se debe crear otro movimiento.

- Los movimientos deben ser distintos a 0. El signo va conforme al tipo de movimiento, los depositos positivos, los retiros negativos. Al guardar un movimiento el saldo disponible de la cuenta

- Eliminar un cliente no debería borrar el registro físico, solo cambiaría su estado a false (pues tiene cuentas asociadas y su información es relevante para eliminarla). Será un soft delete. Haré que se puede reactivar un cliente cambiando su estado a true, sus cuentas quedarán en el mismo estado que tenían antes. 

- Para realizar movimientos en las cuentas de un cliente, tanto las cuentas como el cliente deben tener un estado activo (true) para poder realizar esta operación. Para esto se coloca una proyección de la entidad cliente (solo para leerla) en el microservicio de cuentas que se actualiza de manera asíncrona.

- Un cliente debe estar activo para crear cuentas. Las cuentas creadas deberían tener saldo positivo o cero.

- Varios campos serán inmutables, por ejemplo en el modelo Cuentas su número, el cliente, saldo inicial, o también la fecha de nacimiento en Persona (para calcular su edad). El tipo de cuenta haré que se pueda modificar pero solamente si la cuenta aún no tiene movimientos.

- Para el reporte, se limitara su rango de consulta a un máximo de 3 meses (podría cambiar según el contexto), las fecha inicial y final se incluirán. Para cada cuenta se incluirá su saldo disponible actual y su lista ordenada de movimientos en los cuales se incluirá su saldo despues del movimiento. Incluiré cuentas que no tengan movimientos el rango consultado. 

### Decisiones técnicas y arquitectura

- Utilizaré 2 microservicios, sin apigateway porque para efectos del ejercicios práctico no aplica. Cada microservicio se ejecuta en un puerto distinto. 

- Utilizaré clean architecture para cada microservicio con layers de dominio application infrastructure y api, y se codificará siguiendo clean code, solid, dry, design patterns relevantes

- Utilizaré .NET 10, Postgresql, Efcore, RabbitMQ, xUnit, Testcontainers, Postman, Docker, Git. No utilizaré algun pipeline CI/CD porque no se pide.

- Utilizaré minimal APIs por su simplicidad para codificar y es un mejor fit para microservicios, también porque consumen un poco menos recursos lo que ayuda en parte a la escalabilidad

- Cada microservicio tendrá su propia base de datos independiente, en Postgresql. El archivo BaseDatos.sql será creado utilizando los sql generados por migraciones de efcore y agregando manualmente las partes de creación de las base de datos.

- No utilizaré auto mappers porque no son necesarios al momento. Tampoco strongly typed ids porque añade configuración y necesito priorizar primero otras funcionalidades.

- No utilizaré un mecanismo de autenticación y autorización porque no se define exactamente el requerimiento para esto, pensaba solicitar contraseña para las acciones con cuentas y movimientos y usar hash fucntion para comprobar autorización pero considero que sale del alcance. 

- El modelo Persona tiene esta descripción "Debe manera su clave primaria (PK)" parace indicar que se requiere un id, no se usará su identificador porque es un dato del dominio.Cliente también debe tener un id y está en relación de herencia con Persona por lo que usaré TPT de Efcore para que tengan tablas separadas con su columna id pero compartirán el mismo Id.

- Para la proyección de cliente en el microservicio de cuentas con consistencia eventual, se usa RabbitMQ y el patrón outbox esto permitirá cubrir varios escenarios como caídas de RabbitMQ. Esto hace que a pesar de que el cliente este desactivado, el estado de las cuentas no se altere, si se desea que todas las cuentas estén en false al eliminar (desactivar) un cliente se debería usar una saga por ejemplo pero considero que sale del alcance.

- Los microservicios manejarán todos fechas en UTC al igual que las bases de datos, y se añadirá un tiempo local solo cuando se retorne data para efectos del ejercicio (puesto que no tenemos un frontend).

- Se utilizará paginación donde amerite excepto en el reporte donde se restringirá el rango a un valor determinado. 

- Para el endpoint del reporte las fechas deben estar separadas con una coma, y el cliente simplemente su id.

- Se utilizará optimistic concurrecy para evitar problemas con cuentas y movimientos, por ejemplo retiros sobre saldos desactualizados.

- No utilizaré códigos para idempotencia en los movimientos, porque añade complejidad innecesaria para el ejercicio y no se conoce todo el contexto. Estos códigos permitirían por ejemplo reintentar desde el frontend que un movimiento se concrete (cuando hubo un error) sin que se genere uno nuevo.
