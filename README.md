## Ejercicio práctico

Developer: Danilo A.

### Instrucciones

**Seguir secuencialmente:**

1. crear un .env utilizando .env.example
    - Los password son lo prioritario para cambiar.

2. docker compose -f compose.build.yaml build
    - Genera las imagenes de los microservicios.

3. docker compose -f compose.yaml -f compose.db.yaml run --rm db-init
    - Crea las bases de datos y las tablas.

4. docker compose up -d
    - Ejercuta los microservicios, postgresql y rabbitmq.

**Comandos adicionales:**

- docker compose -f compose.yaml -f compose.db.yaml run --rm db-reset
    - Elimina las bases de datos y su contenido.

## Criterios de Diseño

Considerando que el propósito es demostrar habilidades, que el tiempo es limitado y que existe cierto grado de libertad en los requerimientos (pudiendo ser muy complejo o básico) busqué un equilibrio que permita alcanzar un buen resultado.

Tomaré como referencia el lenguaje Gherkin para describir los comportamientos que decidí importantes para el dominio, y ADR para las deciciones de arquitectura importantes, y los demás temas técnicos en una lista al final. Código bien escrito que expresa el modelo de negocio es en parte una buena documentación, pero realicé este documento previo a programar para dejar claro los requerimientos que desarrollaré. 


## Decisiones de Dominio

### F1 - Generación CRUD y CRU

**Feature: Gestión de clientes**

Escenario: Eliminar un cliente
    
    Será un soft-delete porque tiene datos relevantes asociados como cuentas y movimientos.

    - Cuando: se elimina un cliente
    - Entonces: su estado pasa a false sin borrar físicamente su registro
    - Y: todas sus cuentas deben quedar con estado false (inactivas)

Escenario: Reactivar un cliente

    - Dado: un cliente inactivo (estado false)
    - Cuando: se reactiva
    - Entonces: su estado pasa a true
    - Y: sus cuentas permanecen inactivas
    - Pero: ya podrían activarse

Escenario: Bloquear ediciones durante desactivación de cliente

    Durante un proceso de desactivación o activación de un cliente, no se permite modificarlo para no afectar la consistencia del proceso.

    - Dado: un proceso de activación o desactivación de cliente
    - Cuando: se intenta modificar un campo de un cliente
    - Entonces: se rechaza el cambio hasta terminar el proceso

Escenario: Calcular edad

    Almacenar la edad no es conveniente porque se desactualiza, es preferible guardar fecha de nacimiento.

    - Cuando: se requiere la edad de una persona 
    - Entonces: se calcula con la fecha de nacimiento y la fecha actual


**Feature: Gestión de cuentas**

Escenario: Prohibir activar cuentas de cliente inactivo

    Para este escenario y para otros como crear cuentas, se requiere una proyección del modelo en el microservicio de cuentas.

    - Dado: un cliente inactivo
    - Cuando: se intenta activar una de sus cuentas
    - Entonces: se rechaza la activación


Escenario: Crear cuentas

    - Cuando: se solicita crear una cuenta
    - Entonces: se require que el cliente esté activo, que el saldo inicial sea cero o positivo

Escenario: No permitir cambios de campos 

    - Cuando: se intenta actualizar el número, tipo, cliente o saldo inicial de una cuenta
    - Entonces: se rechaza el cambio


**Feature: Gestión de movimientos**

Escenario: Movimientos inmutables

    Los movimientos representan una operación que ya sucedió, por ello no tiene senido modificarlos.

    - Dado: un movimiento que está guardado
    - Cuando: se intenta actualizarlo
    - Entonces: se rechaza el cambio


### F2 - Registro de movimientos

**Feature: Registrar movimientos**

Escenario: Rechazar movimientos inválidos

    - Cuando: se recibe un movimiento con valor cero, depósito negativo o retiro positivo
    - Entonces: se rechaza el movimiento

Escenario: Guardar un movimiento válido

    - Dado: una cuenta activa
    - Cuando: se recibe un movimiento que no es inválido
    - Entonces: se guarda el movimiento
    - Y: se actualiza el saldo disponible de la cuenta

Escenario: Bloquear movimientos en cuentas inactivas

    - Dado: una cuenta inactiva
    - Cuando: se recibe un movimiento
    - Entonces: se rechaza y no se guarda


### F3 - Saldo insuficiente

**Feature: Rechazar movimiento en cuentas sin saldo disponible**

Escenario: Retiro mayor al saldo disponible

    - Dado: una cuenta con saldo insuficiente para el retiro
    - Cuando: se recibe un movimiento de retiro
    - Entonces: se rechaza el retiro


### F4 - Estado de cuenta

**Feature: Consultar estado de cuenta**

Escenario: Limitar rango de fechas

    Debería existir un límite en el rango, utilicé 3 meses pero depende mucho del contexto.

    - Cuando: se consulta el estado de cuenta con rango mayor a 3 meses 
    - Entonces: se rechaza la consulta

Escenario: Obtener el reporte

    - Cuando: se consulta el estado de cuenta en un rango válido
    - Entonces: se devuelve un Json con los movimientos del rango, incluyendo fecha inicial y final
    - Y: cada cuenta tendrá su lista ordenada de movimientos y saldo actual
    - Y: cada movimiento tendrá el valor del saldo después del movimiento
    - Y: se incluyen cuentas que no tengan movimientos en el rango


### F5 - Prueba unitaria


Se realizará una prueba unitaria utilizando xUnit para este escenario que está en F1:

    Escenario: Bloquear ediciones durante desactivación de cliente


### F6 - Prueba de integración

Se realizará un test de integración para lo que se describió en el dominio respecto a la desactivación un cliente y sus cuentas. Puesto que el modelo de clientes tiene una proyección también debería verificarse esta parte. El escenario es:

    Escenario: Eliminar un cliente

Para esta prueba se utilizará xUnit, WebApplicationFactory, y Postgresql RabbitMQ mediante Testcontainers.


### F7 - Contenedores

Para desplegar en Docker utilizaré Dockerfiles multietapa, el sdk para generar el release y con el runtime para ejectuar la api. También tendré 3 docker compose para efectos de despliegue de este ejercicio práctico. 

- Uno para crear (build) las imagenes de los microservicios
- Otro para ejecutar el script BaseDatos.sql (o resetear las bases de datos)
- Y otro para ejecutar postgresql rabbitmq y los microservicios.  




## Decisiones de Arquitectura

### ADR01 - Estructura de servicios

- Contexto: 
    - Se pide comunicación asíncrona y 2 microservicios
    - El propósito del ejercicio es demostrar habilidades y buenas prácticas de desarrollo.
- Decisión:
    - Para efectos del ejercicio no considero necesario un api gateway, cada microservicio se ejecutará en un puerto distinto.
    - Utilizaré clean architecture con capas domain, application, infrastructure y api, para efectos del ejercicio no utilizaré contracts de application para consumirlos en api. Se codificará siguiendo clean code, solid, dry, design patterns relevantes.
    - Minimal APIs simplifica la implementación y requiere menos código de infraestructura.
- Consecuencia:
    - Arquitectura de microsevicios mantenible y escalable.


### ADR02 - Persistencia

- Contexto: 
    - Se maneja en cada microservicio con bases de datos dinstintas.
    - El modelo Persona tiene esta descripción "Debe manera su clave primaria (PK)" parace indicar que se requiere un id.
    - Cliente también requiere un id y hereda de Persona.  
- Decisión:
    - Utilizar Postgresql y también Efcore.
    - Para el id de Persona no usaré su identificador porque es un dato del dominio.
    - Para la relación de herencia utilizaré TPT de Efcore.
    - Para el script BaseDatos.sql utilizaré el código creado por las migraciones de Efcore, pero manualmente se agregará la creación de cada base de datos.
- Consecuencia:
    - Los esquemas parten de las migraciones, el id será compatido entre Personas y Clientes pero con tablas distintas.


### ADR03 - Comunicación Asíncrona

- Contexto: 
    - En el microservicio de cuentas se requiere consultar el cliente para crearlas o activarlas, también se deben desactivar las cuentas cuando se desactiva un cliente.
- Decisión:
    - Utilizaré una proyección de clientes en el microservicio de cuentas, que evita comunicación síncrona, y una espera relativamente larga si se consulta de manera asíncrona.
    - Utilizaré RabbitMQ, y wolverine para sagas para la consitencia y transactional outbox, esto permitirá cubrir varios escenarios como caídas de RabbitMQ.
    - Se utlizará versionamiento para que una modificación antigua no altere lo que hizo una modificación reciente (esto podría suceder por cortes entre Rabbitmq y el publicador del outbox). La proyección solo tiene id, estado, versión, y solo se permitirá ejecutar la lógica de negocio cuando una versión sea mayor.
- Consecuencia:
    - Se asegura consistencia el la proyección del cliente y que se ejecuten las operaciones adecuadas en cuentas.
    - Se require una saga para crear clientes en su microservicio y en la proyección del microservicio de cuentas.


### ADR04 - Concurrencia

- Contexto: 
    - Pueden existir movimientos simultaneos que provoquen errores en los saldos.
    - Pueden crearse varias sagas al intentar cambiar el estado de un cliente simultaneamente. 
- Decisión:
    - Utilizaré optimistic concurrency para casos de cuentas/movimientos y clientes.
- Consecuencia:
    - Los saldos serán consistentes.
    - Se evitan crear varias sagas simultaneas. 


### ADR05 - Despliegue

- Contexto: 
    - Se necesitá construir las imagenes de los microservicios.
    - Se requiere ejecutar el script para crear las bases y sus tablas, etc.
    - Se requieren ejecutar varios servicios que trabajen en conjunto como postgresql, rabbitMQ y los microservicios.
- Decisión:
    - Utilizaré Dockerfiles para generar las imagenes. Para el build utilizaré docker compose.
    - Utilizaré docker compose para ejectuar todos los servicios de postgresql, rabbitmq, microservicios.
    - Utilizaré docker compose para ejecutar con psql scripts para las bases de datos (init y reset), este utilizará como base el servicio postgresql del otro compose.
    - No utilizaré algun pipeline CI/CD, queda fuera del alcance.
- Consecuencia:
    - Comandos sencillos para ejecutar docker compose para cada tarea separada.
    - Evitar doble configuración de postgresql.


### Otras deciciones técnicas

- Se utilizará paginación donde amerite excepto en el reporte donde se restringirá el rango a un valor determinado. 

- No utilizaré auto mappers porque no son necesarios al momento. Tampoco strongly typed ids porque añade configuración y necesito priorizar primero otras funcionalidades.

- No utilizaré un mecanismo de autenticación y autorización, podría solicitar contraseña para las acciones con cuentas y movimientos y usar hash fucntion para comprobar autorización pero considero que sale del alcance. 

- Utilizaré un handler global de excepciones para cada API. Y utilizaré excepciones con clases predefinidas como ReglaNegocioException que tiene codigo y mensaje. Esto se puede ampliar segun el contexto, pero es suficiente para el ejercicio. Utilizaré librerías Shared para no tender que duplicar código en los microservicios.

- Los microservicios manejarán todos fechas en UTC al igual que las bases de datos, y se controlará la conversión local/UTC0 para todas las entradas con fechas, las salidas se mantendrán con UTC (puesto que el consumidor debe transformarlas a su zona horaria local).

- No utilizaré códigos para idempotencia en los movimientos, porque añade complejidad innecesaria para el ejercicio y no se conoce todo el contexto. Estos códigos permitirían por ejemplo reintentar desde el frontend que un movimiento se concrete (cuando hubo un error) sin que se genere uno nuevo.

- Para el endpoint del reporte las fechas deben estar separadas con una coma, y el cliente simplemente su id. No se incluirá en la respuesta el nombre del cliente pues el consumidor o frontend obtuvo su id de otra consulta previa donde seguramente ya tiene el nombre del cliente.

