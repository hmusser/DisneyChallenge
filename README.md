# DisneyChallenge
Esta es mi primer versión del Challenge de C# con .NET Core de Alkemy en Visual Studio.

Para hacer pruebas realizar una primera migracion para que se creen las tablas en el localdb de sql de manera que se carguen también algunos datos de ejemplo.

Para autenticarse y obtener un token válido, utilizar las siguientes credenciales:

email: musserhoracio@gmail.com   
Contraseña: hH123456$

Queda pendiente agregar el envío de correos electronicos al usuario cuando se crea una nueva cuenta, esto quedó aun pendiente pues Sendgrid desactivó mi cuenta por usar el servicio desde un repositorio público de GitHub continúo investigando una allternativa. 
Tampoco se realizó alguna implementación de pruebas unitarias sugeridas en el challenge.

Se recomienda realizar pruebas desde Postman para probar los endpoints desarrollados.

Update 03_05_2022:
Se agrega el envío de un email de bienvenida al registrar un nuevo usuario utilizando una cuenta de Gmail de pruebas
Fuente desde la que adapté la solución:

https://codewithmukesh.com/blog/send-emails-with-aspnet-core/
