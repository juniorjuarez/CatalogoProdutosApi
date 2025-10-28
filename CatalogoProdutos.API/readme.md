# 📦 Catálogo de Produtos API

## 📚 Sobre o Projeto

A **Catálogo de Produtos API** é uma aplicação desenvolvida com **ASP.NET Core 8**, com o objetivo de gerenciar produtos e suas respectivas categorias de forma simples e eficiente.

Essa API RESTful permite realizar operações CRUD (Criar, Ler, Atualizar e Deletar) tanto para **produtos** quanto para **categorias**. O projeto foi criado como parte de um estudo pessoal para aprimorar conhecimentos em desenvolvimento de APIs utilizando as tecnologias mais recentes do ecossistema .NET.

---

## 🚀 Tecnologias Utilizadas

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 9**
- **SQLite** (banco de dados leve e prático)
- **Swagger / OpenAPI** (para documentação e testes de endpoints)

---

## ✅ Funcionalidades

### 🛍️ Produtos

- Listar todos os produtos
- Buscar produto por ID
- Criar novo produto
- Atualizar produto existente
- Remover produto

### 🗂️ Categorias

- Listar todas as categorias
- Buscar categoria por ID
- Listar categorias com seus respectivos produtos
- Criar nova categoria
- Atualizar categoria existente
- Remover categoria

---

## 🧱 Estrutura dos Dados

### 📌 Produto

| Campo          | Tipo            | Restrições        |
| -------------- | --------------- | ----------------- |
| `ProdutoId`    | `int`           | Chave Primária    |
| `Nome`         | `string(80)`    | Obrigatório       |
| `Descricao`    | `string(300)`   | Obrigatório       |
| `Preco`        | `decimal(10,2)` | Obrigatório       |
| `ImagemUrl`    | `string(300)`   | Obrigatório       |
| `Estoque`      | `float`         | Opcional          |
| `DataCadastro` | `DateTime`      | Opcional          |
| `CategoriaId`  | `int`           | Chave Estrangeira |

### 📌 Categoria

| Campo         | Tipo          | Restrições     |
| ------------- | ------------- | -------------- |
| `CategoriaId` | `int`         | Chave Primária |
| `Nome`        | `string(80)`  | Obrigatório    |
| `ImagemUrl`   | `string(300)` | Obrigatório    |

---

## 🛠️ Como Executar o Projeto

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Editor de código (Visual Studio, VS Code, etc.)

### 2. Clonar o Repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd <NOME_DA_PASTA>
```

### 3. Configurar o Banco de Dados

- A aplicação utiliza SQLite por padrão.
- A string de conexão pode ser editada no arquivo `appsettings.json`.

#### Criar a base de dados:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add CriacaoInicial
dotnet ef database update
```

> ℹ️ As dependências `Microsoft.EntityFrameworkCore.Design` e `Microsoft.EntityFrameworkCore.Tools` já estão incluídas no projeto.

### 4. Executar a Aplicação

```bash
dotnet run
```

### 5. Acessar a Documentação Swagger

Após iniciar, acesse a interface do Swagger via:

```
https://localhost:<PORTA>/swagger/index.html
```

---

## 🌐 Endpoints Disponíveis

### Produtos

| Método | Rota             | Descrição               |
| ------ | ---------------- | ----------------------- |
| GET    | `/Produtos`      | Lista todos os produtos |
| GET    | `/Produtos/{id}` | Retorna produto por ID  |
| POST   | `/Produtos`      | Cria um novo produto    |
| PUT    | `/Produtos/{id}` | Atualiza um produto     |
| DELETE | `/Produtos/{id}` | Remove um produto       |

### Categorias

| Método | Rota                  | Descrição                               |
| ------ | --------------------- | --------------------------------------- |
| GET    | `/Categoria`          | Lista todas as categorias               |
| GET    | `/Categoria/produtos` | Categorias com seus produtos associados |
| GET    | `/Categoria/{id}`     | Retorna categoria por ID                |
| POST   | `/Categoria`          | Cria uma nova categoria                 |
| PUT    | `/Categoria/{id}`     | Atualiza uma categoria existente        |
| DELETE | `/Categoria/{id}`     | Remove uma categoria                    |

---

## 🔧 Melhorias Futuras

- [ ] Tratamento de erros mais completo
- [ ] Validações de dados com Data Annotations
- [ ] Camada de serviço para separação de responsabilidades
- [ ] Paginação nas listagens
- [ ] Autenticação e autorização
- [ ] Testes unitários e de integração
