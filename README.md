# Product Catalog API (.NET 8)

[![.NET 8](https://img.shields.io/badge/.NET-8-blueviolet?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Cache](https://img.shields.io/badge/Cache-Redis%20%26%20In--Memory-red?style=for-the-badge&logo=redis)](https://redis.io/)
[![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=for-the-badge)]()

A robust REST API for catalog and inventory management (Products and Categories), built with .NET 8, Clean Architecture, and a focus on performance and best practices.

## 🚀 Live Demo

### Category Management (CRUD)
*(Flow: `GET` (empty) -> `POST` (create) -> `GET` (with data))*

![Category Demo](docs/assets/demo-categoria.gif)

### Product Management (CRUD)
*(Flow: `GET` (empty) -> `POST` (create) -> `GET` (with data))*

![Product Demo](docs/assets/demo-produto.gif)

---

## ✨ Key Features & Concepts Applied

This project is not just a CRUD, but a showcase for advanced architecture and performance concepts:

* **Clean Architecture:** The project is segregated into 4 layers (Core, Application, Infrastructure, API) following the **Dependency Rule**, ensuring low coupling and high testability.
* **SOLID Principles:**
    * **S (Single Responsibility):** Each Service, Repository, and Controller has a single responsibility.
    * **O (Open/Closed):** Using Dependency Injection and Interfaces (`IRepository`, `ICacheService`) allows extending behavior (e.g., adding a new database or cache) without modifying existing code.
    * **L (Liskov Substitution):** The use of the generic `Repository<T>` is a clear example.
    * **I (Interface Segregation):** Specific interfaces like `IProdutoRepository` segregate contracts that the generic `IRepository<T>` doesn't cover.
    * **D (Dependency Inversion):** The `Application` layer depends on abstractions (Core's `Interfaces`) and not on implementations (`Infrastructure`).
* **Hybrid Cache (L1/L2):**
    * **L1 (In-Memory):** In-memory cache (`IMemoryCache`) for ultra-fast access to frequently read data.
    * **L2 (Distributed):** Distributed cache with **Redis** (`IDistributedCache`) to ensure consistency across multiple application instances.
    * **Active Invalidation:** The cache is automatically invalidated (removed) on write operations (Create, Update, Delete) to prevent stale data.
* **Generic Repository Pattern:** Abstracts data access, allowing the data source to be swapped (e.g., from EF Core to Dapper) without impacting business logic.
* **Global Exception Handling Middleware:** A centralized middleware catches all unhandled exceptions, formatting a standardized error response (`ProblemDetails`) for the API.
* **Validation (FluentValidation):** Declarative and robust validation of input DTOs, keeping controllers clean.
* **Dependency Injection (DI):** Fully configured natively by .NET to decouple all components.
* **Asynchronous Programming (Async/Await):** Extensive use of `async/await` from controllers down to the database to ensure performance and scalability.

---

## 📂 Solution Structure (Clean Architecture)

The project structure reflects the separation of concerns:

```
├── 📁 Catalogo.Core (Domain)
│   ├── 📁 Entities (Ex: Produto, Categoria)
│   └── 📁 Interfaces (Ex: IRepository<T>, IProdutoRepository)
│
├── 📁 Catalogo.Application (Application)
│   ├── 📁 DTOs (Ex: ProdutoResponseDTO, CategoriaCreateDTO)
│   ├── 📁 Interfaces (Ex: IProdutoService, ICacheService)
│   ├── 📁 Mappings (AutoMapper)
│   ├── 📁 Services (Business Logic)
│   └── 📁 Validators (FluentValidation)
│
├── 📁 Catalogo.Infrastructure (Infrastructure)
│   ├── 📁 Data (DbContext, Migrations)
│   └── 📁 Repositories (Implementations of Core Interfaces)
│
├── 📁 CatalogoProdutos.API (Presentation)
│   ├── 📁 Controllers (API Endpoints)
│   └── 📁 Middleware (Ex: GlobalExceptionMiddleware)
│
└── 📄 CatalogoSolucao.sln
```

---

## 💻 Tech Stack

* **.NET 8**
* **ASP.NET Core 8** (for the REST API)
* **Entity Framework Core 8** (ORM for data persistence)
* **SQLite** (Local Database)
* **Redis** (Distributed Cache)
* **AutoMapper** (DTO Mapping)
* **FluentValidation** (DTO Validation)
* **Swagger/OpenAPI** (API Documentation)

---

## 🚀 How to Run The Project

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* A [Redis](https://redis.io/docs/getting-started/installation/) server (or [Redis running via Docker](https://hub.docker.com/_/redis))

### 1. Clone the Repository

```bash
git clone [https://github.com/juniorjuarez/CatalogoProdutosApi.git](https://github.com/juniorjuarez/CatalogoProdutosApi.git)
cd CatalogoProdutosApi
```

### 2. Configure Connections

Open the `CatalogoProdutos.API/appsettings.Development.json` file and configure your database and Redis connection strings:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=catalogo.db", // (Already set for SQLite)
  "Redis": "localhost:6379" // (Confirm your Redis is on this port)
}
```

### 3. Restore Dependencies and Run Migrations

```bash
# Restore NuGet packages
dotnet restore

# Navigate to the API project (where appsettings is)
cd CatalogoProdutos.API

# Apply migrations (creates the database)
# (The --project flag points to where the DbContext lives)
dotnet ef database update --project ../Catalogo.Infrastructure
```

### 4. Run the Application

```bash
# Still inside the CatalogoProdutos.API folder
dotnet run
```

Access the Swagger documentation and test the endpoints at: `http://localhost:5000/swagger` (or the port shown in the terminal).

---

## 🗺️ API Endpoints (Swagger)

Full documentation can be accessed via Swagger (`/swagger`) when the application is running.

#### Category Endpoints

| Verb | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/Categoria` | Lists all categories. |
| `GET` | `/api/v1/Categoria/{id}` | Gets a category by ID. |
| `GET` | `/api/v1/Categoria/produtos` | Lists all categories with their products. |
| `POST` | `/api/v1/Categoria` | Creates a new category. |
| `PUT` | `/api/v1/Categoria/{id}` | Updates an existing category. |
| `DELETE` | `/api/v1/Categoria/{id}` | Deletes a category. |

#### Product Endpoints

| Verb | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/Produtos` | Lists all products. |
| `GET` | `/api/v1/Produtos/{id}` | Gets a product by ID. |
| `POST` | `/api/v1/Produtos` | Creates a new product. |
| `PUT` | `/api/v1/Produtos/{id}` | Updates an existing product. |
| `DELETE` | `/api/v1/Produtos/{id}` | Deletes a product. |

---

## 🛣️ Roadmap (Next Steps)

* [ ] Implement **Authentication and Authorization** (JWT).
* [ ] Implement **Pagination** on list (`GET`) endpoints.
* [ ] Add **Unit Tests** (xUnit/NUnit).
* [ ] Implement the **Unit of Work** pattern for atomic transactions.
