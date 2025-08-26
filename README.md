# Importante: Configuração do appsettings.Development.json

Para rodar a API corretamente, é necessário criar o arquivo `appsettings.Development.json` na raiz do projeto. Sem esse arquivo, a aplicação **não funcionará**.

### Exemplo de `appsettings.Development.json`:

```json
{
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_AQUI",
    "Issuer": "HardWorkAPI",
    "Audience": "HardWorkAPIUsers",
    "ExpireHours": 24
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=HardWorkDB;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;"
  }
}
```

- Substitua os valores conforme seu ambiente (principalmente a connection string e a chave JWT).
- O arquivo **não deve ser versionado**.

> **Dica:** Copie o conteúdo acima, ajuste para seu ambiente e salve como `appsettings.Development.json` na raiz do projeto.

# HardWorkAPI - API Routes

## AuthController

| Method | Route              | Auth         | Description                |
|--------|--------------------|--------------|----------------------------|
| POST   | /api/auth/register | None         | Register a new user        |
| POST   | /api/auth/login    | None         | Login and get JWT token    |

## UsersController

| Method | Route           | Auth (Role) | Description                |
|--------|-----------------|-------------|----------------------------|
| GET    | /api/users      | Admin       | List all users             |
| GET    | /api/users/me   | Any         | Get current user info      |
| GET    | /api/users/{id} | Admin       | Get user by id             |
| POST   | /api/users      | Admin       | Create user                |
| PUT    | /api/users/{id} | Admin       | Update user                |
| DELETE | /api/users/{id} | Admin       | Delete user                |

## ExercisesController

| Method | Route                | Auth (Role) | Description                |
|--------|----------------------|-------------|----------------------------|
| GET    | /api/exercises       | None        | List all exercises         |
| GET    | /api/exercises/{id}  | None        | Get exercise by id         |
| POST   | /api/exercises       | Admin       | Create exercise            |
| PUT    | /api/exercises/{id}  | Admin       | Update exercise            |
| DELETE | /api/exercises/{id}  | Admin       | Delete exercise            |

## WorkoutsController

| Method | Route                                      | Auth (Role)      | Description                                 |
|--------|--------------------------------------------|------------------|---------------------------------------------|
| GET    | /api/workouts/all                          | Admin            | List all workouts                           |
| GET    | /api/workouts                              | Trainer, Admin   | List workouts created by logged-in trainer  |
| GET    | /api/workouts/{id}                         | Trainer, Admin   | Get workout by id (owner or admin only)     |
| POST   | /api/workouts                              | Trainer, Admin   | Create workout                              |
| PUT    | /api/workouts/{id}                         | Trainer, Admin   | Update workout (owner or admin only)        |
| DELETE | /api/workouts/{id}                         | Trainer, Admin   | Delete workout (owner or admin only)        |
| POST   | /api/workouts/{id}/exercises               | Trainer, Admin   | Add exercise to workout (owner/admin only)  |
| DELETE | /api/workouts/{workoutId}/exercises/{exerciseId} | Trainer, Admin   | Remove exercise from workout (owner/admin only) |


> **Note:**
> - "Admin" = Only users with Admin role.
> - "Trainer" = Only users with Trainer role.
> - "Any" = Any authenticated user.
> - "None" = No authentication required.
> - "owner" = Only the trainer who created the workout.