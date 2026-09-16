### Ejercicio práctico

Developer: Danilo A.


### Decisiones de dominio

- Los movimientos no deberían actualizarse porque representan algo que ya sucedió, sí ya se creó no tiene sentido modificarlo, en lugar de ello se debe crear otro movimiento.

- Eliminar un cliente no debería borrar el registro físico, solo cambiaría su estado a false (pues tiene cuentas asociadas y su información es relevante para eliminarla). Será un soft delete. Haré que se puede reactivar un cliente cambiando su estado a true, sus cuentas quedarán en el mismo estado que tenían antes. 

- Para realizar movimientos en las cuentas de un cliente, tanto las cuentas como el cliente deben tener un estado activo (true) para poder realizar esta operación. Para esto se coloca una proyección o mirror de la entidad cliente (solo para leerla) en el microservicio de cuentas que se actualiza de manera asíncrona.





### Decisiones técnicas y arquitectura

- Utilizaré 2 microservicios, sin apigateway porque para efectos del ejercicios práctico no aplica. Cada microservicio se ejecuta en un puerto distinto. 

- Utilizaré clean architecture para cada microservicio con layers de dominio application infrastructure y api, y se codificará siguiendo clean code, solid, dry, design patterns relevantes

- Utilizaré .NET 10, Postgresql, Efcore, RabbitMQ, xUnit, Postman, Docker, Git. No utilizaré algun pipeline CI/CD porque no se pide.

- Utilizaré minimal APIs por su simplicidad para codificar y es un mejor fit para microservicios, también porque consumen un poco menos recursos lo que ayuda en parte a la escalabilidad

- Cada microservicio tendrá su propia base de datos independiente, de Postgresql

- No utilizaré auto mappers porque no son necesarios al momento. Tampoco strongly typed ids porque añade configuración y necesito priorizar primero otras funcionalidades.

- No utilizaré un mecanismo de autenticación y autorización porque no se define exactamente el requerimiento para esto, pensaba solicitar contraseña para las acciones con cuentas y movimientos y usar hash fucntion para comprobar autorización pero considero que sale del alcance. 

- El modelo Persona tiene esta descripción "Debe manera su clave primaria (PK)" parace indicar que se requiere un id, no se usará su identificador porque es un dato del dominio.Cliente también debe tener un id y está en relación de herencia con Persona por lo que usaré TPT de Efcore para que tengan tablas separadas con su columna id pero compartirán el mismo Id.

- Para la proyección de cliente en el microservicio de cuentas, se usa RabbitMQ y el patrón outbox esto permitirá cubrir varios escenarios como caídas de RabbitMQ. Esto hace que a pesar de que el cliente este desactivado, el estado de las cuentas no se altere, si se desea que todas las cuentas estén en false al eliminar (desactivar) un cliente se debería usar una saga por ejemplo pero considero que sale del alcance.

- Se utilizará paginación donde amerite excepto en el reporte donde se restringirá el rango a un valor determinado (3 meses por ejemplo)
