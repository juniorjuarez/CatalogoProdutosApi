### 📖 README.md Draft (English)


# Catalog and Inventory Management API (.NET)

![Status: Under Construction](https://img.shields.io/badge/status-under_construction-yellow)
![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![Cache](https://img.shields.io/badge/Cache-Redis_&_InMemory-red)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue)

## 🎯 About The Project

This project is the back-end (REST API) for an inventory management system. The goal is to build a robust and scalable application to manage products, categories, kits, and the complete inventory lifecycle (stock-in, stock-out, and adjustments).

The application is being developed with a strong focus on performance and market best practices, using **Clean Architecture** to ensure a decoupled and maintainable codebase.

## ✨ Key Technical Features

* **Clean Architecture:** Project segregated into 4 layers (Core, Application, Infrastructure, API).
* **Generic Repository Pattern:** Data access abstraction using `IRepository<T>`.
* **Hybrid Cache (L1/L2):**
    * **L1:** In-memory cache (`IMemoryCache`) for ultra-fast access.
    * **L2:** Distributed cache with **Redis** (`IDistributedCache`) for shared data.
* **Cache Invalidation:** Active cache key removal on write operations (CUD) to prevent stale data.
* **EF Core Optimizations:** Use of `AsNoTracking()` in read queries and `Include()` for eager loading (avoiding N+1).
* **Dependency Injection:** The entire project is wired up using DI.
* **DTO Pattern & AutoMapper:** Complete separation between domain models and API models.

## 📂 Project Architecture

The solution is divided into the following layers:

* **`Catalogo.Core` (Domain):** Contains business entities (e.g., `Produto`, `Categoria`) and contract interfaces (e.g., `IRepository`, `IHybridCacheService`). It has no dependencies on other layers.
* **`Catalogo.Application` (Application):** Contains business logic (Services), DTOs, Mappings (AutoMapper), and constants. It orchestrates all operations.
* **`Catalogo.Infrastructure` (Infrastructure):** Implements the Core contracts. It is responsible for data persistence (EF Core, Repositories) and external services (like the Cache).
* **`CatalogoProdutos.API` (Presentation):** Exposes the Application layer's logic as REST endpoints (Controllers).

## 🚀 How To Run (Coming Soon)

*(Section to be filled in with instructions for building, database setup, and running the project).*

## 🛣️ Roadmap (Next Steps)

This project is still under active development. The next planned features are:

* [ ] Implement **Stock Movement** logic (stock-in, stock-out, adjustment).
* [ ] Implement **Kits** management (bundles of products).
* [ ] Add **Pagination** to all list endpoints.
* [ ] Implement DTO **Validation** (FluentValidation).
* [ ] Create a **Global Exception Handling Middleware**.
* [ ] Implement the **Unit of Work** pattern for atomic transactions.
* [ ] Add **Authentication and Authorization** (JWT).
