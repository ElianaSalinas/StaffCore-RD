==================================================
  STAFFCORE RD — Sistema de Gestión de Personal
  ISW-311 Práctica Final
==================================================

DESARROLLADO POR
-----------------
Nombre    : Eliana Salinas
Matrícula : 2023-3777
Docente   : Prof. Ivan Zorrilla

REPOSITORIO GITHUB
------------------
https://github.com/ElianaSalinas/StaffCore-RD

INSTRUCCIONES PARA EJECUTAR
-----------------------------
1. Asegúrate de tener .NET 8 SDK y SQL Server LocalDB instalados.
2. Abre el proyecto en Visual Studio 2022.
3. Ejecuta la migración si la BD no existe:
      dotnet ef database update
4. Presiona F5 o clic en "Iniciar depuración".
5. Regístrate con el primer usuario → se asignará rol Administrador.

CREDENCIALES DEL ADMINISTRADOR DE PRUEBA
------------------------------------------
Email    : admin@staffcore.rd
Contraseña : Admin1234!

NOTA: Si la base de datos está vacía, registra este usuario primero.
El primer usuario registrado en el sistema recibe automáticamente
el rol de Administrador.

STACK TECNOLÓGICO
------------------
- ASP.NET Core 8 MVC (Code First)
- Entity Framework Core 8.0.15
- ASP.NET Core Identity 8.0.15
- SQL Server LocalDB → (localdb)\mssqllocaldb, BD: StaffCoreDB
- Bootstrap 5 + Bootstrap Icons

==================================================
