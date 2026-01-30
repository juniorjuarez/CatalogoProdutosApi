# 📚 LIVRO INTERATIVO DE ENGENHARIA REVERSA

## Projeto: CatalogoProdutosApi

---

## 🎯 OBJETIVO

Este livro foi criado para você dominar completamente o código do projeto `CatalogoProdutosApi` através de engenharia reversa e aprendizado ativo.

---

## 📋 SUMÁRIO - ORDEM LÓGICA DE APRENDIZADO

### **PARTE 1: FUNDAMENTOS DE C# (Base Necessária)**

#### **Lição 1: Classes, Propriedades e Namespaces**

- O que são classes e como representam "coisas" do mundo real
- Propriedades: getters e setters automáticos
- Namespaces: organização de código
- **No seu projeto:** `Categoria.cs` e `Produto.cs` (entidades)
- **Conceitos:** `public`, `private`, `?` (nullable), `get; set;`

---

#### **Lição 2: Data Annotations e Atributos**

- O que são atributos `[Key]`, `[Required]`, `[StringLength]`
- Como o Entity Framework usa esses atributos
- **No seu projeto:** Linhas 7-17 de `Categoria.cs` e `Produto.cs`
- **Conceitos:** Annotations, validação de dados, mapeamento de tabelas

---

#### **Lição 3: Collections e Relacionamentos (ICollection)**

- O que é `ICollection<T>` e por que usar
- Relacionamentos 1:N (Uma Categoria tem muitos Produtos)
- **No seu projeto:** Linha 20 de `Categoria.cs` (`ICollection<Produto>?`)
- **Conceitos:** Relacionamentos, navegação entre entidades

---

### **PARTE 2: INTERFACES E CONTRATOS**

#### **Lição 4: Interfaces - Contratos que Classes Assumem**

- O que é uma interface e por que usar
- Diferença entre interface e classe
- **No seu projeto:** `IRepository<T>`, `ICategoriaService`, `ICategoriaRepository`
- **Conceitos:** Polimorfismo, desacoplamento, testabilidade

---

#### **Lição 5: Generics (`<T>`) - Código Reutilizável**

- O que são Generics e como funcionam
- Por que `IRepository<T>` funciona para qualquer entidade
- **No seu projeto:** `IRepository<T>` (linha 6) e `Repository<T>` (linha 8)
- **Conceitos:** Type parameters, reutilização de código, type safety

---

### **PARTE 3: PROGRAMação ASSÍNCRONA**

#### **Lição 6: Tasks e Async/Await - Não Bloquear o Servidor**

- Por que usar `async` e `await`
- O que é `Task<T>` e `Task`
- Diferença entre código síncrono e assíncrono
- **No seu projeto:** Todos os métodos em `Repository.cs` e `CategoriaService.cs`
- **Conceitos:** Threading, I/O assíncrono, performance

---

### **PARTE 4: ENTITY FRAMEWORK CORE**

#### **Lição 7: DbContext - A Ponte com o Banco de Dados**

- O que é `DbContext` e para que serve
- `DbSet<T>`: representação de tabelas
- **No seu projeto:** `AppDbContext.cs` (linhas 6-15)
- **Conceitos:** ORM, mapeamento objeto-relacional, migrations

---

#### **Lição 8: Repository Pattern - Abstraindo o Acesso a Dados**

- Por que separar lógica de acesso a dados
- Como o Repository esconde detalhes do EF Core
- **No seu projeto:** `Repository<T>.cs` (linhas 8-48)
- **Conceitos:** Padrão de projeto, separação de responsabilidades, testabilidade

---

#### **Lição 9: LINQ e Expression Trees**

- O que é LINQ e como usar
- `Expression<Func<T, bool>>`: filtros dinâmicos
- `AsNoTracking()`, `Include()`, `FirstOrDefaultAsync()`
- **No seu projeto:** Linhas 19, 22-24, 19 de `Repository.cs` e `CategoriaRepository.cs`
- **Conceitos:** Query expressions, deferred execution, performance

---

### **PARTE 5: ARQUITETURA EM CAMADAS**

#### **Lição 10: DTOs (Data Transfer Objects) - Separando o que Vai e Vem**

- Por que não expor entidades diretamente
- DTOs de entrada (`CreateDTO`) vs saída (`ResponseDTO`)
- **No seu projeto:** `CategoriaCreateDTO.cs` vs `CategoriaResponseDTO.cs`
- **Conceitos:** Segurança, versionamento de API, separação de camadas

---

#### **Lição 11: AutoMapper - Transformando Objetos Automaticamente**

- Por que mapear manualmente é chato
- Como o AutoMapper faz isso automaticamente
- **No seu projeto:** `MappingProfile.cs` (linhas 8-23)
- **Conceitos:** Object mapping, convenções, configuração

---

#### **Lição 12: Services - A Camada de Lógica de Negócio**

- Por que Services existem entre Controllers e Repositories
- Responsabilidades de cada camada
- **No seu projeto:** `CategoriaService.cs` (linhas 10-179)
- **Conceitos:** Business logic, regras de negócio, orquestração

---

### **PARTE 6: INJEÇÃO DE DEPENDÊNCIA**

#### **Lição 13: Dependency Injection (DI) - O Coração do ASP.NET Core**

- O que é DI e por que é fundamental
- `AddScoped`, `AddSingleton`, `AddTransient`: quando usar cada um
- **No seu projeto:** `Program.cs` (linhas 36-45)
- **Conceitos:** IoC (Inversion of Control), desacoplamento, testabilidade

---

#### **Lição 14: Constructor Injection - Recebendo Dependências**

- Como classes recebem suas dependências
- Por que usar construtores e não `new`
- **No seu projeto:** Construtores em `CategoriaService.cs` (linha 20), `Repository.cs` (linha 12)
- **Conceitos:** Dependency injection, lifecycle management

---

### **PARTE 7: CACHING (MEMÓRIA E REDIS)**

#### **Lição 15: Caching - Acelerando Respostas**

- Por que cache existe (performance)
- Memory Cache vs Distributed Cache (Redis)
- **No seu projeto:** `HybridCacheService.cs` (linhas 10-97)
- **Conceitos:** Cache L1 (memória) vs L2 (Redis), invalidação de cache

---

#### **Lição 16: Func<T> e Delegates - Passando Código como Parâmetro**

- O que são delegates e `Func<T>`
- Como passar "fábricas" de dados para o cache
- **No seu projeto:** `HybridCacheService.GetOrCreateAsync` (linha 27: `Func<Task<T>> factory`)
- **Conceitos:** Higher-order functions, lazy evaluation, factory pattern

---

### **PARTE 8: ASP.NET CORE WEB API**

#### **Lição 17: Controllers - O Ponto de Entrada da API**

- O que são Controllers e Actions
- Atributos `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- **No seu projeto:** `CategoriaController.cs` (linhas 9-152)
- **Conceitos:** REST, HTTP verbs, routing

---

#### **Lição 18: ActionResult e Status Codes HTTP**

- O que cada status code significa (200, 201, 404, 500)
- `Ok()`, `NotFound()`, `CreatedAtRoute()`, `BadRequest()`
- **No seu projeto:** Métodos em `CategoriaController.cs`
- **Conceitos:** HTTP protocol, RESTful APIs, convenções

---

#### **Lição 19: Program.cs - Configurando a Aplicação**

- O que acontece quando a aplicação inicia
- `builder.Services`: registrando serviços
- `app.Use*`: middleware pipeline
- **No seu projeto:** `Program.cs` (linhas 16-72)
- **Conceitos:** Startup, middleware, pipeline, configuração

---

#### **Lição 20: Swagger/OpenAPI - Documentação Automática**

- Por que documentar APIs
- Como o Swagger gera documentação automaticamente
- **No seu projeto:** `Program.cs` (linhas 55-56, 64-65)
- **Conceitos:** API documentation, OpenAPI spec, UI interativa

---

### **PARTE 9: CONCEITOS AVANÇADOS**

#### **Lição 21: Nullable Reference Types (`?`)**

- O que significa `string?` vs `string`
- Por que o C# 8+ tem isso
- **No seu projeto:** Propriedades nullable em entidades e DTOs
- **Conceitos:** Null safety, nullable context, prevenção de NullReferenceException

---

#### **Lição 22: Herança e Polimorfismo no Repository**

- Como `CategoriaRepository` herda de `Repository<Categoria>`
- `base(context)`: chamando construtor da classe pai
- **No seu projeto:** `CategoriaRepository.cs` (linha 11-15)
- **Conceitos:** Herança, polimorfismo, reutilização de código

---

#### **Lição 23: JSON Serialization e ReferenceHandler**

- Por que evitar ciclos de referência (Categoria → Produtos → Categoria)
- `JsonIgnore` e `ReferenceHandler.IgnoreCycles`
- **No seu projeto:** `Categoria.cs` (linha 19), `Produto.cs` (linha 28), `Program.cs` (linha 25)
- **Conceitos:** Serialization, circular references, JSON

---

#### **Lição 24: Migrations - Versionando o Banco de Dados**

- O que são migrations e por que usar
- Como o EF Core cria e aplica migrations
- **No seu projeto:** Pasta `Migrations/` em `Catalogo.Infrastructure`
- **Conceitos:** Database versioning, schema evolution, rollback

---

#### **Lição 25: Estrutura da Solução - Organizando Projetos**

- Por que separar em camadas (Core, Application, Infrastructure, API)
- Dependências entre projetos
- **No seu projeto:** Estrutura de 4 projetos na solução
- **Conceitos:** Clean Architecture, Separation of Concerns, SOLID

---

## 🎓 METODOLOGIA DE CADA LIÇÃO

Cada lição seguirá esta estrutura:

1. **O Conceito** - Explicação simples e didática
2. **No Seu Projeto** - Onde está implementado (arquivo + linhas)
3. **A "Receita de Bolo"** - Passo a passo genérico
4. **O "Porquê"** - Justificativa técnica
5. **Desafio Prático** - Exercício hands-on
6. **Feedback** - Correção e melhorias

---

## ✅ PRÓXIMO PASSO

Aguarde seu **"OK"** para iniciarmos a **Lição 1: Classes, Propriedades e Namespaces**.

---

**Total de Lições:** 25
**Nível:** Do básico ao avançado
**Foco:** Aprendizado prático baseado no seu código real.
