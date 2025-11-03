# API de Gerenciamento de Catálogo e Estoque (.NET)

![Status: Em Construção](https://img.shields.io/badge/status-em_construção-yellow)
![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![Cache](https://img.shields.io/badge/Cache-Redis_&_InMemory-red)
![Arquitetura](https://img.shields.io/badge/Arquitetura-Clean_Architecture-blue)

## 🎯 Sobre o Projeto

Este projeto é o back-end (API REST) para um sistema de gerenciamento de inventário. O objetivo é construir uma aplicação robusta e escalável para controlar produtos, categorias, kits e o ciclo de vida do estoque (entradas, saídas e ajustes).

A aplicação está sendo desenvolvida com foco em performance e nas melhores práticas de mercado, utilizando **Clean Architecture** para garantir um código desacoplado e manutenível.

## ✨ Features Técnicas Principais

* **Arquitetura Limpa:** Projeto dividido em 4 camadas (Core, Application, Infrastructure, API).
* **Padrão Repository Genérico:** Abstração do acesso a dados com `IRepository<T>`.
* **Cache Híbrido (L1/L2):**
    * **L1:** Cache em memória (`IMemoryCache`) para acesso ultrarrápido.
    * **L2:** Cache distribuído com **Redis** (`IDistributedCache`) para dados compartilhados.
* **Invalidação de Cache:** Estratégia de remoção de chaves em operações de escrita (CUD) para evitar dados obsoletos.
* **Otimização de EF Core:** Uso de `AsNoTracking()` em leituras e `Include()` para carregar dados relacionados (evitando N+1).
* **Injeção de Dependência:** Todo o projeto é configurado via DI.
* **Padrão DTO e AutoMapper:** Separação total entre modelos de domínio e modelos de API.

## 📂 Arquitetura do Projeto

A solução é dividida nas seguintes camadas:

* **`Catalogo.Core` (Domínio):** Contém as entidades de negócio (ex: `Produto`, `Categoria`) e as interfaces de contrato (ex: `IRepository`, `IHybridCacheService`). Não depende de nenhuma outra camada.
* **`Catalogo.Application` (Aplicação):** Contém a lógica de negócio (Services), DTOs, Mapeamentos (AutoMapper) e constantes. Orquestra as operações.
* **`Catalogo.Infrastructure` (Infraestrutura):** Implementa os contratos da Core. É responsável pela persistência de dados (EF Core, Repositórios) e serviços externos (como o Cache).
* **`CatalogoProdutos.API` (Apresentação):** Expõe a lógica da camada de Aplicação como endpoints REST (Controllers).

## 🚀 Como Rodar (Em Breve)

*(Seção a ser preenchida com instruções de build, setup do banco de dados e execução do projeto).*

## 🛣️ Roadmap (Próximos Passos)

O projeto ainda está em desenvolvimento. As próximas features planejadas são:

* [ ] Implementação da lógica de **Movimentação de Estoque** (entrada, saída, ajuste).
* [ ] Implementação do cadastro de **Kits** (conjunto de produtos).
* [ ] Adição de **Paginação** nos endpoints de listagem.
* [ ] Implementação de **Validação** de DTOs (FluentValidation).
* [ ] Criação de um **Middleware Global de Exceções**.
* [ ] Implementação do padrão **Unit of Work** para transações atômicas.
* [ ] Adição de **Autenticação e Autorização** (JWT).
