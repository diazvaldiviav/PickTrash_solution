## PickTrash API Documentation

# Overview
PickTrash es una API REST que gestiona un sistema de recolección de basura, permitiendo la interacción entre clientes y conductores. La API maneja autenticación mediante JWT (JSON Web Tokens) y soporta diferentes roles de usuario.

# Base URL 
pendiente


# Autenticacion
La API utiliza JWT para la autenticación. El token debe ser incluido en el header de las peticiones:

# Autenticacion
Registra un nuevo usuario en el sistema.

POST /auth/register
Content-Type: application/json

# Request Body
{
    "email": "usuario@ejemplo.com",
    "userName": "usuario123",
    "password": "Contraseña123!",
    "phoneNumber": "+1234567890",
    "role": 0  // 0 = Cliente, 1 = Driver
}

# Response(201 Created)
{
    "userId": 1,
    "email": "usuario@ejemplo.com",
    "userName": "usuario123",
    "role": 0,
    "token": "eyJhbGciOiJIUzI1NiIs..."
}

# Login
POST /auth/login
Content-Type: application/json



# Request Body
{
    "userName": "usuario123",
    "password": "Contraseña123!",
    "role": 0  // 0 = Cliente, 1 = Driver
}

# Response
{
    "userId": 1,
    "email": "usuario@ejemplo.com",
    "userName": "usuario123",
    "role": 0,
    "token": "eyJhbGciOiJIUzI1NiIs..."
}

# Codigos de Estados

- `200 OK`: Petición exitosa
- `201 Created`: Recurso creado exitosamente
- 400 Bad Request: Error en la petición
- 401 Unauthorized: No autorizado
- 403 Forbidden: Acceso prohibido
- 404 Not Found: Recurso no
- `500 Internal Server Error`: Error del servidor
  
# Roles de Usuario

- `Cliente (0)`: Usuario que solicita servicios de recolección

- `Driver (1)`: Conductor que realiza los servicios de recolección
  
- `Admin (2)`: Administrador del sistema
