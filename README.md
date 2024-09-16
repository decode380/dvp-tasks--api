
# NET Core Web API para Double V Partners

La API se encuentra desarrollada en la versión de .Net SDK 7.0.100. Consta de una WebAPI principal que cuenta con swagger para la documentación y las librerias de clases como proyectos para las otras capas de la arquitectura.

Los endpoints de consulta cuentan con paginación.

## Arquitectura

La API implementa `Onion Architecture` para mejorar la escalabilidad, mantenimiento y gestión de dependencias. Por ello se pueden ver las capas de Dominio, Presentación, Aplicación e Infraestructura.


## Patrones y dependencias

- Implementa los patrones `CQRS` y `Mediator`  con la libería MediatR para la separación de responsabilidades y facilidad en el mantenimiento.
- Cuenta con la dependencia `FluentValidation` para la verificación de los campos que se envian en los Commandos y Queries desde Mediator.
- Utiliza la librería `BCrypt-Net` para generar el hash de contraseñas y verificarlos al momento del login.
- Implementa la librería `IdentityModel.Tokens.Jwt` para la validación de los tokens firmados desde la misma aplicación, protegiendo las rutas que se indican en los controladores.
- Utiliza `Entity Framework` con el enfoque `Database First` que permite generar las entidades de dominio dentro de la aplicación automáticamente con el comando `dotnet ef dbcontext scaffold "Data Source=localhost,1433;Database=DvpTasks;Integrated Security=false;User ID=dvp_tasks;Password=DvpT4sk5;Encrypt=true;TrustServerCertificate=true;" Microsoft.EntityFrameworkCore.SqlServer -o ../Domain/Entities -n Domain.Entities -c ApplicationContext --context-dir ../Infrastructure/Contexts --context-namespace Infrastructure.Contexts --project Domain -f --no-onconfiguring`
- `Swagger` y `Swashbuckle` para la documentación en formato XML desde los controladores.

## Tener en cuenta

El servicio de swagger implementado en la aplicación no puede recibir el Bearer JWT Token que valida todas las rutas excepto Login, Por tanto los endpoint se deben consumir desde otro cliente y pasar el header de autorización por el prefijo Bearer.