# 📚 Guia Completo de Conceitos - API Catálogo de Produtos

Este documento explica **TODOS** os conceitos utilizados no projeto usando **aprendizado ativo**: começando pelo problema, mostrando a solução e ensinando como implementar passo a passo.

---

## 🎯 Índice

1. [Injeção de Dependências (DI)](#1-injeção-de-dependências-di)
2. [Padrão Repository](#2-padrão-repository)
3. [Padrão Service Layer](#3-padrão-service-layer)
4. [DTOs (Data Transfer Objects)](#4-dtos-data-transfer-objects)
5. [AutoMapper](#5-automapper)
6. [Entity Framework Core](#6-entity-framework-core)
7. [Arquitetura em Camadas](#7-arquitetura-em-camadas)
8. [Async/Await](#8-asyncawait)
9. [Cache Híbrido (L1 + L2)](#9-cache-híbrido-l1--l2)
10. [Middleware](#10-middleware)
11. [FluentValidation](#11-fluentvalidation)
12. [Data Annotations](#12-data-annotations)
13. [Swagger/OpenAPI](#13-swaggeropenapi)
14. [Routing e HTTP Verbs](#14-routing-e-http-verbs)
15. [Expression Trees](#15-expression-trees)
16. [Generics](#16-generics)

---

## 1. Injeção de Dependências (DI)

### 🎭 Para que serve? (Analogia Prática)

Imagine que para usar um **Carro**, você tivesse que **fabricar o Motor dentro dele**. Se quiser trocar o motor, tem que reconstruir o carro inteiro. 

Com **Injeção de Dependências**, o Carro apenas diz: *"Eu preciso de um motor"*, e alguém (o .NET) entrega ele pronto no momento da criação. Se quiser trocar o motor, só troca a configuração - o carro continua funcionando!

### ❌ O Problema (Refinamento Progressivo)

**Sem DI (código ruim):**
```csharp
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _service;
    
    public CategoriaController()
    {
        // ❌ PROBLEMA: Controller cria sua própria dependência
        _service = new CategoriaService(
            new CategoriaRepository(new AppDbContext(...)),
            new Mapper(...),
            new HybridCacheService(...)
        );
    }
}
```

**Por que isso é ruim?**
- 🔴 Se `CategoriaService` mudar, o Controller quebra
- 🔴 Não dá para testar (não consegue injetar um mock)
- 🔴 Se `CategoriaService` precisar de mais dependências, tem que mudar o Controller
- 🔴 Código espalhado em 50 lugares diferentes

### ✅ A Solução Recomendada

**Com DI (código bom):**
```csharp
// Linha 14 do CategoriaController.cs
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _service;
    
    // ✅ SOLUÇÃO: Pede a interface no construtor
    public CategoriaController(ICategoriaService service)
    {
        _service = service;  // .NET entrega automaticamente!
    }
}
```

**Por que isso é bom?**
- ✅ Controller não sabe como criar o Service
- ✅ Fácil testar (injeta um mock)
- ✅ Se Service mudar, Controller não precisa mudar
- ✅ Configuração centralizada no `Program.cs`

### 🔄 Fluxo Lógico: Como o Dado Viaja

```
1. Cliente faz requisição HTTP
   ↓
2. ASP.NET Core recebe a requisição
   ↓
3. Precisa criar CategoriaController
   ↓
4. Vê que Controller precisa de ICategoriaService
   ↓
5. Olha no Program.cs: "Ah! ICategoriaService = CategoriaService"
   ↓
6. Cria CategoriaService (que precisa de Repository, Mapper, Cache...)
   ↓
7. Cria todas as dependências em cascata
   ↓
8. Entrega tudo pronto para o Controller
   ↓
9. Controller usa _service normalmente
```

**Visualização:**
```
Request → ASP.NET Core → Program.cs (configuração) → Cria dependências → Controller pronto
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Defina a Interface (Contrato)
```csharp
// Catalogo.Application/Services/ICategoriaService.cs
public interface ICategoriaService
{
    Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync();
}
```

**💡 Lembre-se:** Interface é como um contrato. É uma classe que só tem assinaturas de métodos, sem implementação. Você já conhece isso de POO básico!

#### Passo 2: Implemente a Classe
```csharp
// Catalogo.Application/Services/CategoriaService.cs
public class CategoriaService : ICategoriaService
{
    // Implementa os métodos da interface
    public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
    {
        // Lógica aqui
    }
}
```

#### Passo 3: O Pulo do Gato - Registre no Program.cs
```csharp
// Program.cs (linha 46)
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
```

**Tradução:** *"Quando alguém pedir `ICategoriaService`, entregue uma instância de `CategoriaService`"*

#### Passo 4: Use no Controller
```csharp
public CategoriaController(ICategoriaService service)  // .NET injeta automaticamente!
{
    _service = service;
}
```

### 🎯 Tipos de Lifetime (Quando a Instância é Criada)

**`AddScoped`** (Mais comum):
- Uma instância **por requisição HTTP**
- Se 10 usuários fazem requisição ao mesmo tempo = 10 instâncias diferentes
- Quando a requisição termina, a instância é descartada

**`AddTransient`**:
- Nova instância **toda vez** que é solicitada
- Mais pesado, use só quando necessário

**`AddSingleton`**:
- Uma única instância para **toda a aplicação**
- Cuidado! Se guardar estado, pode causar bugs

### 🔗 Hierarquia de Injeção no Projeto

```
CategoriaController
    └── ICategoriaService (injetado via construtor)
            └── CategoriaService
                    ├── ICategoriaRepository (injetado)
                    ├── IMapper (injetado - AutoMapper)
                    └── IHybridCacheService (injetado)
```

**Cada nível recebe suas dependências via construtor!** O .NET resolve tudo automaticamente.

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection

---

## 2. Padrão Repository

### 🎭 Para que serve? (Analogia Prática)

Imagine que você tem uma **biblioteca** e precisa buscar livros. Sem o Repository, você teria que:
- Ir até a estante
- Saber como organizar os livros
- Saber onde cada livro está guardado
- Se mudar de biblioteca, aprender tudo de novo

Com o **Repository**, você apenas pede: *"Me traga todos os livros"*. O Repository sabe como buscar, onde buscar, e se você mudar de biblioteca (SQL Server → MongoDB), só muda o Repository - o resto do código continua igual!

### ❌ O Problema (Refinamento Progressivo)

**Sem Repository (código ruim):**
```csharp
public class CategoriaService
{
    // ❌ PROBLEMA: Código de banco espalhado no Service
    public async Task<List<Categoria>> GetCategorias()
    {
        using var context = new AppDbContext();
        return await context.Categorias.ToListAsync();
    }
    
    public async Task<Categoria> GetById(int id)
    {
        using var context = new AppDbContext();
        return await context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);
    }
    
    // Repetir isso em 50 lugares diferentes...
}
```

**Por que isso é ruim?**
- 🔴 Se mudar de SQL Server para MongoDB, tem que mexer em 50 arquivos
- 🔴 Código de acesso a dados misturado com lógica de negócio
- 🔴 Difícil testar (precisa de banco de dados real)
- 🔴 Se precisar mudar como busca categorias, tem que mudar em vários lugares

### ✅ A Solução Recomendada

**Com Repository (código bom):**
```csharp
public class CategoriaService
{
    private readonly ICategoriaRepository _repository;
    
    public CategoriaService(ICategoriaRepository repository)
    {
        _repository = repository;  // Repository injetado!
    }
    
    public async Task<List<Categoria>> GetCategorias()
    {
        return await _repository.GetAllAsync();  // Simples e limpo!
    }
}
```

**Por que isso é bom?**
- ✅ Código de banco centralizado em um lugar
- ✅ Se mudar de banco, só muda o Repository
- ✅ Fácil testar (injeta um mock do Repository)
- ✅ Service foca na lógica de negócio

### 🔄 Fluxo Lógico: Como o Dado Viaja

```
1. Service precisa buscar categorias
   ↓
2. Service chama _repository.GetAllAsync()
   ↓
3. Repository recebe a chamada
   ↓
4. Repository usa _context.Set<Categoria>() para acessar o banco
   ↓
5. EF Core traduz para SQL: SELECT * FROM Categorias
   ↓
6. Banco retorna dados
   ↓
7. EF Core converte para objetos Categoria
   ↓
8. Repository retorna para o Service
   ↓
9. Service retorna para o Controller
```

**Visualização:**
```
Controller → Service → Repository → DbContext → EF Core → Banco de Dados
                                                              ↓
Controller ← Service ← Repository ← DbContext ← EF Core ← Dados
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Crie a Interface no Core
```csharp
// Catalogo.Core/Interfaces/IRepository.cs
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}
```

**💡 Lembre-se:** `where T : class` é uma constraint genérica. É como dizer: "T pode ser qualquer tipo, MAS tem que ser uma classe". Você já conhece isso de Generics básico!

#### Passo 2: Crie Interface Específica (Opcional)
```csharp
// Catalogo.Core/Interfaces/ICategoriaRepository.cs
public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<IEnumerable<Categoria>> GetCategoriasProdutosAsync();  // Método específico
}
```

#### Passo 3: Implemente a Classe Base na Infrastructure
```csharp
// Catalogo.Infrastructure/Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    
    public Repository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }
    
    // Implementa outros métodos CRUD...
}
```

**💡 Lembre-se:** `_context.Set<T>()` é como dizer ao EF Core: "Me dê acesso à tabela do tipo T". É como um `DbSet<T>` genérico!

**`AsNoTracking()`**: Otimização importante! Diz ao EF Core: "Não precisa rastrear mudanças, só leia os dados". Mais rápido para operações de leitura.

#### Passo 4: Implemente Repository Específico
```csharp
// Catalogo.Infrastructure/Repositories/CategoriaRepository.cs
public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context) { }
    
    // Método específico de Categoria
    public async Task<IEnumerable<Categoria>> GetCategoriasProdutosAsync()
    {
        return await _context.Categorias
            .Include(p => p.Produtos)  // Eager Loading - carrega produtos também
            .AsNoTracking()
            .ToListAsync();
    }
}
```

**💡 Lembre-se:** `Include(p => p.Produtos)` é **Eager Loading**. É como dizer: "Quando buscar categorias, traga os produtos também em uma única query". Sem isso, teria que fazer 2 queries (uma para categorias, outra para produtos).

#### Passo 5: Registre no Program.cs
```csharp
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
```

### 🎯 Conceitos Importantes

**Generics (`<T>`):**
- Permite criar código reutilizável
- `Repository<Categoria>` e `Repository<Produto>` usam o mesmo código base
- Você já conhece isso de `List<T>` ou `Dictionary<TKey, TValue>`

**Herança:**
- `CategoriaRepository : Repository<Categoria>` herda todos os métodos CRUD
- Só precisa implementar métodos específicos de Categoria

**Expression Trees (`Expression<Func<T, bool>>`):**
- Permite passar queries type-safe
- `c => c.CategoriaId == id` é traduzido para SQL pelo EF Core
- Se `CategoriaId` não existir, erro em tempo de compilação!

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager

---

## 3. Padrão Service Layer

### 🎭 Para que serve? (Analogia Prática)

Imagine um **restaurante**:
- **Controller** = Garçom (recebe pedido do cliente)
- **Service** = Cozinheiro (prepara o prato, aplica receitas, coordena ingredientes)
- **Repository** = Estoque (busca ingredientes)

O garçom não sabe cozinhar! Ele só entrega o pedido ao cozinheiro. O cozinheiro sabe:
- Qual receita usar
- Como combinar ingredientes
- Quando usar cache (pratos prontos)
- Como transformar ingredientes em prato final

### ❌ O Problema (Refinamento Progressivo)

**Sem Service (código ruim):**
```csharp
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaRepository _repository;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // ❌ PROBLEMA: Lógica de negócio no Controller
        var categorias = await _repository.GetAllAsync();
        
        // Aplica cache manualmente
        // Converte Entity para DTO manualmente
        // Aplica regras de negócio aqui
        // Invalida cache manualmente
        
        return Ok(categorias);
    }
}
```

**Por que isso é ruim?**
- 🔴 Controller fica enorme (deveria ser fino)
- 🔴 Lógica de negócio espalhada
- 🔴 Se precisar usar em outro lugar, tem que copiar código
- 🔴 Difícil testar lógica de negócio isoladamente

### ✅ A Solução Recomendada

**Com Service (código bom):**
```csharp
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _service;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // ✅ SOLUÇÃO: Controller só delega para o Service
        var categoriasDTO = await _service.GetCategoriasAsync();
        return Ok(categoriasDTO);
    }
}
```

**Por que isso é bom?**
- ✅ Controller fino e focado (só recebe requisição e retorna resposta)
- ✅ Lógica de negócio centralizada no Service
- ✅ Pode reutilizar Service em outros lugares
- ✅ Fácil testar lógica de negócio

### 🔄 Fluxo Lógico: Como o Dado Viaja

```
1. Cliente faz GET /Categoria
   ↓
2. Controller recebe requisição
   ↓
3. Controller chama _service.GetCategoriasAsync()
   ↓
4. Service verifica cache (L1 → L2 → Banco)
   ↓
5. Service busca no Repository (se não tiver em cache)
   ↓
6. Service converte Entity → DTO (via AutoMapper)
   ↓
7. Service aplica regras de negócio
   ↓
8. Service retorna DTO para Controller
   ↓
9. Controller retorna JSON para cliente
```

**Visualização:**
```
Cliente → Controller → Service → Cache? → Repository → Banco
                                              ↓
Cliente ← Controller ← Service ← DTO ← Entity ← Banco
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Crie a Interface no Application
```csharp
// Catalogo.Application/Services/ICategoriaService.cs
public interface ICategoriaService
{
    Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync();
    Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id);
    Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto);
}
```

#### Passo 2: Implemente a Classe
```csharp
// Catalogo.Application/Services/CategoriaService.cs
public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repository;
    private readonly IMapper _mapper;
    private readonly IHybridCacheService _cache;
    
    // Injeção de dependências
    public CategoriaService(
        ICategoriaRepository repository, 
        IMapper mapper, 
        IHybridCacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }
    
    public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
    {
        // 1. Verifica cache
        // 2. Se não tiver, busca no Repository
        // 3. Converte Entity → DTO
        // 4. Retorna
    }
}
```

#### Passo 3: Registre no Program.cs
```csharp
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
```

### 🎯 Responsabilidades do Service

1. ✅ **Coordena acesso ao Repository** - Decide quando buscar dados
2. ✅ **Aplica cache (L1 + L2)** - Otimiza performance
3. ✅ **Converte Entity ↔ DTO** - Via AutoMapper
4. ✅ **Invalida cache quando necessário** - Quando cria/atualiza/deleta
5. ✅ **Aplica regras de negócio** - Validações, cálculos, transformações

**💡 Lembre-se:** Service é o "cérebro" da aplicação. É onde a mágica acontece!

---

## 4. DTOs (Data Transfer Objects)

### 🎭 Para que serve? (Analogia Prática)

Imagine que você tem uma **casa** (Entity) com:
- Quartos privados (campos internos do banco)
- Documentos pessoais (relacionamentos complexos)
- Itens valiosos (dados sensíveis)

Quando alguém visita sua casa, você não mostra **tudo**. Você mostra apenas:
- Sala de estar (dados públicos)
- Cozinha (dados necessários)
- Jardim (dados visíveis)

**DTO é como uma "versão pública" da sua casa** - mostra apenas o que o visitante precisa ver!

### ❌ O Problema (Refinamento Progressivo)

**Sem DTO (código ruim):**
```csharp
public class Categoria  // Entity do banco
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
    public ICollection<Produto>? Produtos { get; set; }  // ❌ Expõe relacionamento interno
    public DateTime DataCriacao { get; set; }  // ❌ Dado interno
    public string? UsuarioCriacao { get; set; }  // ❌ Dado sensível
}

// Controller retorna Entity diretamente
[HttpGet]
public async Task<Categoria> Get(int id)
{
    return await _repository.GetByIdAsync(id);  // ❌ Expõe tudo!
}
```

**Por que isso é ruim?**
- 🔴 Expõe dados internos do banco
- 🔴 Expõe relacionamentos complexos (pode causar loops infinitos no JSON)
- 🔴 Expõe dados sensíveis
- 🔴 Se mudar a Entity, quebra a API

### ✅ A Solução Recomendada

**Com DTO (código bom):**
```csharp
// Entity (privada, interna)
public class Categoria
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    // ... campos internos
}

// DTO (pública, para API)
public class CategoriaResponseDTO
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
    // Apenas o que o cliente precisa ver!
}

// Controller retorna DTO
[HttpGet]
public async Task<CategoriaResponseDTO> Get(int id)
{
    var categoria = await _repository.GetByIdAsync(id);
    return _mapper.Map<CategoriaResponseDTO>(categoria);  // ✅ Converte Entity → DTO
}
```

**Por que isso é bom?**
- ✅ Expõe apenas dados necessários
- ✅ Não expõe relacionamentos complexos (a menos que necessário)
- ✅ Não expõe dados sensíveis
- ✅ Se mudar Entity, só muda o mapeamento

### 🔄 Fluxo Lógico: Como o Dado Viaja

```
1. Cliente envia POST /Categoria com JSON
   ↓
2. JSON é deserializado para CategoriaCreateDTO
   ↓
3. Controller recebe CategoriaCreateDTO
   ↓
4. Controller passa para Service
   ↓
5. Service converte DTO → Entity (via AutoMapper)
   ↓
6. Service salva Entity no banco
   ↓
7. Service converte Entity → CategoriaResponseDTO
   ↓
8. Controller retorna CategoriaResponseDTO
   ↓
9. DTO é serializado para JSON
   ↓
10. Cliente recebe JSON limpo
```

**Visualização:**
```
Cliente (JSON) → DTO → Controller → Service → Entity → Banco
                                                      ↓
Cliente (JSON) ← DTO ← Controller ← Service ← Entity ← Banco
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Crie DTO de Criação (Entrada)
```csharp
// Catalogo.Application/DTOs/CategoriaCreateDTO.cs
public class CategoriaCreateDTO
{
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
    // ❌ NÃO tem CategoriaId (é gerado pelo banco)
}
```

**💡 Lembre-se:** DTO de criação é como um formulário. Você não preenche o ID - o banco gera!

#### Passo 2: Crie DTO de Resposta (Saída)
```csharp
// Catalogo.Application/DTOs/CategoriaResponseDTO.cs
public class CategoriaResponseDTO
{
    public int CategoriaId { get; set; }  // ✅ Tem ID (foi criado)
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
}
```

**💡 Lembre-se:** DTO de resposta é como um recibo. Mostra o que foi criado, incluindo o ID!

#### Passo 3: Crie DTO com Relacionamento (Quando Necessário)
```csharp
// Catalogo.Application/DTOs/CategoriaResponseProdutosDTO.cs
public class CategoriaResponseProdutosDTO
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public ICollection<ProdutoResponseDTO>? Produtos { get; set; }  // ✅ Inclui produtos relacionados
}
```

**💡 Lembre-se:** Use este DTO quando precisar retornar categoria COM seus produtos!

#### Passo 4: Configure AutoMapper (veja seção 5)
```csharp
CreateMap<Categoria, CategoriaResponseDTO>();
CreateMap<CategoriaCreateDTO, Categoria>();
```

### 🎯 Tipos de DTOs no Projeto

**CategoriaCreateDTO:**
- Usado em `POST /Categoria`
- Não tem ID (banco gera)
- Apenas dados de entrada

**CategoriaResponseDTO:**
- Usado em `GET /Categoria` e `GET /Categoria/{id}`
- Tem ID (foi criado)
- Dados de saída simples

**CategoriaResponseProdutosDTO:**
- Usado em `GET /Categoria/produtos`
- Inclui produtos relacionados
- Dados de saída complexos

### 💡 Lembre-se de POO Básico

**DTO é apenas uma classe simples:**
- Tem propriedades (`public string? Nome { get; set; }`)
- Não tem métodos complexos
- Não tem lógica de negócio
- É como uma "caixa" para transportar dados

**É diferente de Entity:**
- Entity tem relacionamentos complexos
- Entity pode ter métodos de negócio
- Entity representa o banco de dados

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types

---

## 5. AutoMapper

### 🎭 Para que serve? (Analogia Prática)

Imagine que você tem um **tradutor automático**:
- Você fala em Português (Entity)
- Ele traduz para Inglês (DTO)
- Você não precisa traduzir palavra por palavra manualmente!

**AutoMapper é esse tradutor** - converte automaticamente de um tipo para outro, desde que os nomes das propriedades sejam iguais!

### ❌ O Problema (Refinamento Progressivo)

**Sem AutoMapper (código ruim):**
```csharp
public CategoriaResponseDTO ConvertToDTO(Categoria categoria)
{
    // ❌ PROBLEMA: Código repetitivo e chato
    return new CategoriaResponseDTO
    {
        CategoriaId = categoria.CategoriaId,
        Nome = categoria.Nome,
        ImagemUrl = categoria.ImagemUrl
        // ... repetir para cada propriedade
    };
}

// E fazer isso em 50 lugares diferentes!
```

**Por que isso é ruim?**
- 🔴 Código repetitivo e chato
- 🔴 Se adicionar propriedade, tem que atualizar em vários lugares
- 🔴 Fácil esquecer de mapear alguma propriedade
- 🔴 Código fica enorme

### ✅ A Solução Recomendada

**Com AutoMapper (código bom):**
```csharp
// Configuração uma vez só
CreateMap<Categoria, CategoriaResponseDTO>();

// Uso simples
var dto = _mapper.Map<CategoriaResponseDTO>(categoria);  // ✅ Uma linha!
```

**Por que isso é bom?**
- ✅ Código limpo e curto
- ✅ Configuração centralizada
- ✅ Se propriedades tiverem mesmo nome, mapeia automaticamente
- ✅ Menos bugs

### 🔄 Fluxo Lógico: Como o Mapeamento Funciona

```
1. Você tem uma Entity (Categoria)
   ↓
2. Chama _mapper.Map<CategoriaResponseDTO>(categoria)
   ↓
3. AutoMapper olha no MappingProfile
   ↓
4. Encontra CreateMap<Categoria, CategoriaResponseDTO>()
   ↓
5. Compara propriedades:
   - Categoria.CategoriaId → CategoriaResponseDTO.CategoriaId ✅
   - Categoria.Nome → CategoriaResponseDTO.Nome ✅
   - Categoria.ImagemUrl → CategoriaResponseDTO.ImagemUrl ✅
   ↓
6. Cria novo CategoriaResponseDTO com valores copiados
   ↓
7. Retorna o DTO pronto
```

**Visualização:**
```
Entity (Categoria)
  ├── CategoriaId: 1
  ├── Nome: "Eletrônicos"
  └── ImagemUrl: "img.jpg"
         ↓ AutoMapper
DTO (CategoriaResponseDTO)
  ├── CategoriaId: 1
  ├── Nome: "Eletrônicos"
  └── ImagemUrl: "img.jpg"
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Instale o Pacote NuGet
```bash
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

#### Passo 2: Crie o MappingProfile
```csharp
// Catalogo.Application/Mappings/MappingProfile.cs
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO (saída - quando retorna dados)
        CreateMap<Categoria, CategoriaResponseDTO>();
        CreateMap<Produto, ProdutoResponseDTO>();
        
        // DTO → Entity (entrada - quando recebe dados)
        CreateMap<CategoriaCreateDTO, Categoria>();
        CreateMap<ProdutoCreateDTO, Produto>();
    }
}
```

**💡 Lembre-se:** `Profile` é uma classe do AutoMapper. É como um "caderno de traduções" - você escreve todas as traduções aqui!

#### Passo 3: Registre no Program.cs
```csharp
// Program.cs (linha 35)
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

**Tradução:** *"Registra AutoMapper e usa o MappingProfile para configurações"*

#### Passo 4: Injete no Service
```csharp
public class CategoriaService
{
    private readonly IMapper _mapper;
    
    public CategoriaService(IMapper mapper)  // Injetado automaticamente!
    {
        _mapper = mapper;
    }
}
```

#### Passo 5: Use no Código
```csharp
// Entity → DTO (quando retorna dados)
var categoriasDtos = _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);

// DTO → Entity (quando recebe dados)
var categoria = _mapper.Map<Categoria>(categoriaDto);

// Atualização (mapeia propriedades para entidade existente)
var categoriaExistente = await _repository.GetByIdAsync(id);
_mapper.Map(categoriaDto, categoriaExistente);  // Atualiza propriedades
```

### 🎯 Casos de Uso no Projeto

**1. Converter Entity → DTO (Retornar dados):**
```csharp
var categorias = await _repository.GetAllAsync();
var categoriasDTO = _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
return categoriasDTO;
```

**2. Converter DTO → Entity (Criar dados):**
```csharp
var categoriaDto = // ... recebido do cliente
var categoria = _mapper.Map<Categoria>(categoriaDto);
await _repository.CreateAsync(categoria);
```

**3. Atualizar Entity existente:**
```csharp
var categoriaExistente = await _repository.GetByIdAsync(id);
_mapper.Map(categoriaDto, categoriaExistente);  // Copia propriedades
await _repository.UpdateAsync(categoriaExistente);
```

### 💡 Regra de Ouro do AutoMapper

**Se os nomes das propriedades forem iguais, mapeia automaticamente!**

```csharp
// Entity
public class Categoria
{
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
}

// DTO
public class CategoriaResponseDTO
{
    public int CategoriaId { get; set; }  // ✅ Mesmo nome = mapeia automaticamente
    public string? Nome { get; set; }      // ✅ Mesmo nome = mapeia automaticamente
}
```

**Se os nomes forem diferentes, precisa configurar:**
```csharp
CreateMap<Categoria, CategoriaResponseDTO>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoriaId));
```

### 📚 Documentação AutoMapper
https://docs.automapper.org/en/stable/

---

## 6. Entity Framework Core

### 🎭 Para que serve? (Analogia Prática)

Imagine que você precisa **falar com alguém que só entende inglês**, mas você só fala português. Você precisa de um **tradutor**!

**EF Core é esse tradutor** - você escreve código C# (português), e ele traduz para SQL (inglês) automaticamente!

**Sem EF Core:** Você escreve SQL manualmente (chato, propenso a erros)
**Com EF Core:** Você escreve C# e ele traduz para SQL (fácil, type-safe)

### ❌ O Problema (Refinamento Progressivo)

**Sem EF Core (código ruim):**
```csharp
// ❌ PROBLEMA: SQL manual, propenso a erros
public List<Categoria> GetCategorias()
{
    var sql = "SELECT CategoriaId, Nome, ImagemUrl FROM Categorias";
    // Conectar ao banco manualmente
    // Executar SQL manualmente
    // Converter resultado manualmente para objetos
    // Tratar erros manualmente
}
```

**Por que isso é ruim?**
- 🔴 SQL como string (erros só aparecem em runtime)
- 🔴 Fácil fazer SQL Injection (se não sanitizar)
- 🔴 Difícil manter (mudanças no banco quebram código)
- 🔴 Muito código repetitivo

### ✅ A Solução Recomendada

**Com EF Core (código bom):**
```csharp
// ✅ SOLUÇÃO: Código C# type-safe
public async Task<List<Categoria>> GetCategorias()
{
    return await _context.Categorias.ToListAsync();  // Simples e seguro!
}
```

**Por que isso é bom?**
- ✅ Código C# type-safe (erros em tempo de compilação)
- ✅ Protegido contra SQL Injection automaticamente
- ✅ Fácil manter (mudanças no banco geram erros de compilação)
- ✅ Código limpo e expressivo

### 🔄 Fluxo Lógico: Como Funciona

```
1. Você escreve código C#
   var categorias = await _context.Categorias.ToListAsync();
   ↓
2. EF Core analisa o código
   ↓
3. EF Core traduz para SQL
   SELECT * FROM Categorias
   ↓
4. EF Core executa SQL no banco
   ↓
5. Banco retorna dados
   ↓
6. EF Core converte dados para objetos C#
   ↓
7. Você recebe List<Categoria>
```

**Visualização:**
```
C# Code → EF Core → SQL → Banco → Dados → EF Core → C# Objects
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Instale o Pacote NuGet
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

#### Passo 2: Crie o DbContext
```csharp
// Catalogo.Infrastructure/Data/Context/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    // Cada DbSet representa uma tabela no banco
    public DbSet<Categoria>? Categorias { get; set; }
    public DbSet<Produto>? Produtos { get; set; }
    public DbSet<Fornecedor>? Fornecedores { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configura relacionamentos complexos
        modelBuilder.Entity<Produto>()
            .HasOne(p => p.Fornecedor)           // Produto tem um Fornecedor
            .WithMany(f => f.Produtos)            // Fornecedor tem muitos Produtos
            .HasForeignKey(p => p.FornecedorId);  // Chave estrangeira
    }
}
```

**💡 Lembre-se:** 
- `DbSet<T>` = "Tabela no banco de dados"
- `OnModelCreating` = "Configurações que não cabem em Data Annotations"

#### Passo 3: Configure no Program.cs
```csharp
// Program.cs (linhas 38-40)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString,
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
```

**Tradução:**
- `UseSqlite` = "Usa SQLite como banco" (pode trocar para `UseSqlServer`, `UseNpgsql`, etc.)
- `MigrationsAssembly` = "Onde ficam as migrations"

#### Passo 4: Crie a Primeira Migration
```bash
dotnet ef migrations add InitialCreate --project Catalogo.Infrastructure --startup-project CatalogoProdutos.API
```

**O que isso faz?**
- Analisa suas Entities
- Compara com o banco atual
- Gera arquivos de migration (SQL para criar/alterar tabelas)

#### Passo 5: Aplique a Migration
```bash
dotnet ef database update --project Catalogo.Infrastructure --startup-project CatalogoProdutos.API
```

**O que isso faz?**
- Executa as migrations pendentes
- Cria/altera tabelas no banco

### 🎯 Operações CRUD com EF Core

**CREATE (Criar):**
```csharp
var categoria = new Categoria { Nome = "Eletrônicos", ImagemUrl = "img.jpg" };
_context.Categorias.Add(categoria);
await _context.SaveChangesAsync();  // Persiste no banco
```

**READ (Ler):**
```csharp
// Todos
var categorias = await _context.Categorias.ToListAsync();

// Por ID
var categoria = await _context.Categorias
    .FirstOrDefaultAsync(c => c.CategoriaId == id);

// Com filtro
var categorias = await _context.Categorias
    .Where(c => c.Nome.Contains("Eletr"))
    .ToListAsync();
```

**UPDATE (Atualizar):**
```csharp
var categoria = await _context.Categorias.FindAsync(id);
categoria.Nome = "Novo Nome";
_context.Entry(categoria).State = EntityState.Modified;
await _context.SaveChangesAsync();
```

**DELETE (Deletar):**
```csharp
var categoria = await _context.Categorias.FindAsync(id);
_context.Categorias.Remove(categoria);
await _context.SaveChangesAsync();
```

### 💡 Conceitos Importantes

**DbContext:**
- Representa uma sessão com o banco de dados
- Rastreia mudanças nas entidades
- Salva mudanças com `SaveChangesAsync()`

**DbSet<T>:**
- Representa uma tabela no banco
- Permite fazer queries LINQ
- `_context.Categorias` = acesso à tabela Categorias

**Migrations:**
- Versionamento do schema do banco
- Permite evoluir o banco sem perder dados
- Cada migration é um "checkpoint" do banco

**LINQ:**
- Sintaxe C# para queries
- Type-safe (erros em tempo de compilação)
- Traduzido para SQL pelo EF Core

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/ef/core/

---

## 7. Arquitetura em Camadas

### 📍 Estrutura do projeto:
```
CatalogoSolucao/
├── Catalogo.Core/              # Camada de Domínio
│   ├── Entities/              # Entidades de negócio
│   └── Interfaces/            # Contratos (interfaces)
│
├── Catalogo.Application/       # Camada de Aplicação
│   ├── DTOs/                  # Objetos de transferência
│   ├── Services/              # Lógica de negócio
│   ├── Mappings/              # AutoMapper
│   └── Validators/            # FluentValidation
│
├── Catalogo.Infrastructure/   # Camada de Infraestrutura
│   ├── Data/                  # EF Core, DbContext
│   └── Repositories/          # Implementação dos repositórios
│
└── CatalogoProdutos.API/      # Camada de Apresentação
    ├── Controllers/           # Endpoints HTTP
    ├── Middleware/            # Middlewares customizados
    └── Program.cs             # Configuração da aplicação
```

### 🔍 O que é?
**Arquitetura em Camadas** separa o código em camadas com responsabilidades específicas, seguindo o princípio de **Separation of Concerns**.

### ❓ Por que usar?
1. **Organização**: Código organizado e fácil de encontrar
2. **Manutenibilidade**: Mudanças isoladas em uma camada
3. **Testabilidade**: Cada camada pode ser testada independentemente
4. **Escalabilidade**: Fácil adicionar novas funcionalidades

### 🎯 Responsabilidades de cada camada:

#### 7.1. **Core** (Domínio):
- ✅ Entidades de negócio (`Categoria`, `Produto`, `Fornecedor`)
- ✅ Interfaces (contratos)
- ✅ **Não depende de nenhuma outra camada**

#### 7.2. **Application** (Aplicação):
- ✅ Lógica de negócio (Services)
- ✅ DTOs
- ✅ Validações
- ✅ **Depende apenas de Core**

#### 7.3. **Infrastructure** (Infraestrutura):
- ✅ Acesso a dados (EF Core)
- ✅ Implementação de repositórios
- ✅ **Depende de Core e Application**

#### 7.4. **API** (Apresentação):
- ✅ Controllers (endpoints HTTP)
- ✅ Middlewares
- ✅ Configuração (Program.cs)
- ✅ **Depende de todas as outras camadas**

**Regra de dependência:** Camadas internas **NÃO** dependem de camadas externas!

---

## 8. Async/Await

### 🎭 Para que serve? (Analogia Prática)

Imagine que você está **cozinhando**:
- **Síncrono (ruim)**: Você fica parado esperando a água ferver. Não pode fazer mais nada!
- **Assíncrono (bom)**: Você coloca a água para ferver e enquanto espera, corta os legumes. Quando a água ferve, você volta para ela!

**Async/Await é isso** - enquanto espera o banco de dados responder, o servidor pode atender outras requisições!

### ❌ O Problema (Refinamento Progressivo)

**Sem Async/Await (código ruim):**
```csharp
public List<Categoria> GetCategorias()
{
    // ❌ PROBLEMA: Thread fica BLOQUEADA esperando o banco
    var categorias = _context.Categorias.ToList();  // Espera aqui...
    return categorias;  // Só continua quando terminar
}
```

**Por que isso é ruim?**
- 🔴 Thread fica **parada** esperando o banco (que pode levar 100ms, 500ms, 1s...)
- 🔴 Se 100 usuários fazem requisição, precisa de 100 threads (muito pesado!)
- 🔴 Servidor não consegue atender muitas requisições simultâneas
- 🔴 Aplicação fica lenta e travada

### ✅ A Solução Recomendada

**Com Async/Await (código bom):**
```csharp
public async Task<List<Categoria>> GetCategoriasAsync()
{
    // ✅ SOLUÇÃO: Thread NÃO fica bloqueada
    var categorias = await _context.Categorias.ToListAsync();  // Libera thread enquanto espera
    return categorias;
}
```

**Por que isso é bom?**
- ✅ Thread **não fica bloqueada** - pode atender outras requisições
- ✅ Se 100 usuários fazem requisição, precisa de poucas threads (eficiente!)
- ✅ Servidor consegue atender **muitas** requisições simultâneas
- ✅ Aplicação fica rápida e responsiva

### 🔄 Fluxo Lógico: Como Funciona

**Síncrono (ruim):**
```
Thread 1: [Esperando banco...] [Esperando banco...] [Esperando banco...] [Resposta]
          ↑ Bloqueada por 500ms - não pode fazer mais nada!
```

**Assíncrono (bom):**
```
Thread 1: [Inicia busca] [Libera thread] [Outras requisições] [Volta quando banco responde] [Resposta]
          ↑ Liberada após iniciar - pode atender outras requisições!
```

**Visualização:**
```
Requisição 1 → Thread 1 inicia busca → Libera thread → Thread atende Requisição 2
                                                              ↓
Requisição 1 ← Thread 1 recebe resposta ← Banco responde ← Requisição 2 processando
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Marque o Método como `async`
```csharp
public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
//     ^^^^^                    ^^^^
//     marca como assíncrono    retorna Task
{
    // ...
}
```

**💡 Lembre-se:** 
- `async` = "Este método é assíncrono"
- `Task<T>` = "Vai retornar um T, mas não agora (é uma promessa)"

#### Passo 2: Use `await` em Operações Assíncronas
```csharp
public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
{
    var categorias = await _repository.GetAllAsync();  // await = "Espere, mas não bloqueie"
    return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
}
```

**💡 Lembre-se:** `await` = "Espere esta operação terminar, mas libere a thread enquanto espera"

#### Passo 3: Use Métodos Assíncronos do EF Core
```csharp
// ❌ Ruim (síncrono)
_context.Categorias.ToList()

// ✅ Bom (assíncrono)
await _context.Categorias.ToListAsync()
```

**Regra:** Métodos do EF Core terminam com `Async` quando são assíncronos!

#### Passo 4: Controller Também Assíncrono
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> Get()
//     ^^^^^                    ^^^^
{
    var categoriasDTO = await _service.GetCategoriasAsync();
    return Ok(categoriasDTO);
}
```

### 🎯 Padrão no Projeto

**TODOS os métodos que acessam banco são assíncronos:**

```csharp
// Repository
public async Task<IEnumerable<T>> GetAllAsync() { ... }
public async Task<T?> GetByIdAsync(...) { ... }
public async Task<T> CreateAsync(T entity) { ... }

// Service
public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync() { ... }

// Controller
public async Task<ActionResult> Get() { ... }
```

### 💡 Conceitos Importantes

**`async`:**
- Marca método como assíncrono
- Permite usar `await` dentro do método
- Método sempre retorna `Task` ou `Task<T>`

**`await`:**
- Aguarda operação assíncrona terminar
- **NÃO bloqueia** a thread
- Thread pode fazer outras coisas enquanto espera

**`Task<T>`:**
- Representa uma operação assíncrona que retorna T
- É como uma "promessa" de que vai retornar T no futuro
- Pode aguardar com `await`

### 📊 Comparação de Performance

**Síncrono:**
- 100 requisições = 100 threads bloqueadas
- Servidor fica lento
- Não escala bem

**Assíncrono:**
- 100 requisições = poucas threads (reutilizadas)
- Servidor fica rápido
- Escala muito bem!

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/

---

## 9. Cache Híbrido (L1 + L2)

### 🎭 Para que serve? (Analogia Prática)

Imagine que você precisa de um **livro**:
1. **Primeiro**, você olha na sua **mesa** (L1 - Memory Cache) - **super rápido**, mas espaço limitado
2. Se não tiver, você vai na **biblioteca da sua casa** (L2 - Redis) - **rápido**, compartilhado com família
3. Se não tiver, você vai na **biblioteca pública** (Banco de Dados) - **lento**, mas tem tudo

**Cache Híbrido é isso** - tenta primeiro no lugar mais rápido (L1), depois no médio (L2), e só vai no banco se não encontrar!

### ❌ O Problema (Refinamento Progressivo)

**Sem Cache (código ruim):**
```csharp
public async Task<List<Categoria>> GetCategorias()
{
    // ❌ PROBLEMA: Sempre vai no banco, mesmo que os dados não mudaram
    return await _context.Categorias.ToListAsync();  // Lento! 100-500ms
}
```

**Por que isso é ruim?**
- 🔴 **Sempre** vai no banco, mesmo que dados não mudaram
- 🔴 Banco fica sobrecarregado com queries repetidas
- 🔴 Resposta lenta para o usuário
- 🔴 Se 1000 usuários pedirem categorias, faz 1000 queries no banco!

### ✅ A Solução Recomendada

**Com Cache Híbrido (código bom):**
```csharp
public async Task<List<Categoria>> GetCategorias()
{
    // ✅ SOLUÇÃO: Verifica cache primeiro
    return await _cache.GetOrCreateAsync(
        cacheKeyL1: "categorias",
        cacheKeyL2: "categorias",
        factory: async () => await _context.Categorias.ToListAsync(),  // Só vai no banco se não tiver em cache
        absoluteExpirationL1: TimeSpan.FromMinutes(5),
        absoluteExpirationL2: TimeSpan.FromMinutes(30)
    );
}
```

**Por que isso é bom?**
- ✅ **Raramente** vai no banco (só quando cache expira)
- ✅ Banco fica livre para outras operações
- ✅ Resposta **super rápida** (1-5ms do cache vs 100-500ms do banco)
- ✅ Se 1000 usuários pedirem categorias, faz **1 query** no banco (os outros pegam do cache)!

### 🔄 Fluxo Lógico: Estratégia Cache-Aside

```
1. Cliente pede categorias
   ↓
2. Service verifica L1 (Memory Cache)
   ├─ ✅ Tem? → Retorna imediatamente (1ms) 🚀
   └─ ❌ Não tem? → Continua
       ↓
3. Service verifica L2 (Redis)
   ├─ ✅ Tem? → Retorna e popula L1 (5ms) ⚡
   └─ ❌ Não tem? → Continua
       ↓
4. Service busca no banco (factory)
   ↓
5. Service salva em L2 e L1
   ↓
6. Service retorna dados (100ms) 🐢
```

**Visualização:**
```
Request → L1? → [SIM] → Resposta (1ms) 🚀
           ↓
          [NÃO] → L2? → [SIM] → Popula L1 → Resposta (5ms) ⚡
                      ↓
                     [NÃO] → Banco → Salva L2+L1 → Resposta (100ms) 🐢
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Configure Memory Cache (L1)
```csharp
// Program.cs
builder.Services.AddMemoryCache();
```

**💡 Lembre-se:** Memory Cache é como uma "gaveta rápida" - super rápido, mas só existe enquanto a aplicação está rodando.

#### Passo 2: Configure Distributed Cache (L2) - Redis
```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";  // Endereço do Redis
    options.InstanceName = "CatalogoAPI_";     // Prefixo das chaves
});
```

**💡 Lembre-se:** Redis é como uma "biblioteca compartilhada" - pode ser acessada por várias instâncias da aplicação.

#### Passo 3: Crie o HybridCacheService
```csharp
// Catalogo.Application/Services/HybridCacheService.cs
public class HybridCacheService : IHybridCacheService
{
    private readonly IMemoryCache _memoryCache;      // L1
    private readonly IDistributedCache _distributedCache;  // L2
    
    public HybridCacheService(IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }
    
    public async Task<T> GetOrCreateAsync<T>(...)
    {
        // 1. Tenta L1
        if (_memoryCache.TryGetValue(cacheKeyL1, out T? resultL1) && resultL1 != null)
            return resultL1;
        
        // 2. Tenta L2
        // 3. Se não tiver, busca no banco (factory)
        // 4. Salva em L2 e L1
    }
}
```

#### Passo 4: Registre no Program.cs
```csharp
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();
```

#### Passo 5: Use no Service
```csharp
public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
{
    return await _cache.GetOrCreateAsync(
        cacheKeyL1: CacheKeys.CATEGORIAS_KEY,
        cacheKeyL2: CacheKeys.CATEGORIAS_KEY,
        factory: async () =>
        {
            // Só executa se não tiver em cache!
            var categorias = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
        },
        absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,  // 5 minutos
        absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2   // 30 minutos
    );
}
```

### 🎯 Diferenças entre L1 e L2

**L1 (Memory Cache):**
- ✅ **Super rápido** (1-2ms)
- ✅ Na memória da aplicação
- ❌ **Não compartilhado** (cada instância tem seu próprio)
- ❌ **Limitado** (depende da RAM)
- ⏱️ Expira rápido (5 minutos)

**L2 (Redis):**
- ✅ **Rápido** (5-10ms)
- ✅ **Compartilhado** (todas as instâncias veem o mesmo cache)
- ✅ **Escalável** (pode ter muito espaço)
- ⏱️ Expira mais tarde (30 minutos)

### 💡 Quando Invalidar Cache?

**Invalidar quando criar/atualizar/deletar:**
```csharp
public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO dto)
{
    var categoria = _mapper.Map<Categoria>(dto);
    await _repository.CreateAsync(categoria);
    
    // ✅ IMPORTANTE: Invalida cache porque dados mudaram!
    await _cache.RemoveAsync(CacheKeys.CATEGORIAS_KEY, CacheKeys.CATEGORIAS_KEY);
    
    return _mapper.Map<CategoriaResponseDTO>(categoria);
}
```

**💡 Lembre-se:** Se você cria/atualiza/deleta dados, o cache fica desatualizado. Precisa invalidar!

### 📊 Comparação de Performance

**Sem Cache:**
- 1000 requisições = 1000 queries no banco
- Tempo médio: 200ms por requisição
- Banco sobrecarregado

**Com Cache:**
- 1000 requisições = 1 query no banco (999 do cache)
- Tempo médio: 2ms por requisição (100x mais rápido!)
- Banco livre

### 📚 Documentação Microsoft
- Memory Cache: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory
- Distributed Cache: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed

---

## 10. Middleware

### 🎭 Para que serve? (Analogia Prática)

Imagine um **checkpoint de segurança**:
- Todas as pessoas (requisições) passam por ele antes de entrar no prédio (controller)
- Ele verifica documentos (autenticação)
- Ele registra quem entrou (logging)
- Ele trata problemas (exceções)

**Middleware é esse checkpoint** - intercepta TODAS as requisições antes de chegar no controller!

### ❌ O Problema (Refinamento Progressivo)

**Sem Middleware (código ruim):**
```csharp
public class CategoriaController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // ❌ PROBLEMA: Tratamento de erro em CADA método
            var categorias = await _service.GetCategoriasAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            // Código repetido em 50 lugares diferentes!
            _logger.LogError(ex, "Erro...");
            return StatusCode(500, "Erro interno");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(...)
    {
        try
        {
            // Mesmo código de tratamento de erro aqui também!
        }
        catch (Exception ex)
        {
            // Repetição...
        }
    }
}
```

**Por que isso é ruim?**
- 🔴 Código repetido em **todos** os controllers
- 🔴 Se mudar como tratar erros, tem que mudar em 50 lugares
- 🔴 Fácil esquecer de tratar erro em algum lugar
- 🔴 Controller fica poluído com lógica de infraestrutura

### ✅ A Solução Recomendada

**Com Middleware (código bom):**
```csharp
// Middleware trata erros UMA VEZ para TODAS as requisições
public class GlobalExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);  // Passa para o próximo (controller)
        }
        catch (Exception ex)
        {
            // ✅ Tratamento centralizado!
            // Todas as requisições passam por aqui
        }
    }
}

// Controller limpo!
public class CategoriaController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // ✅ Não precisa tratar erro - Middleware faz isso!
        var categorias = await _service.GetCategoriasAsync();
        return Ok(categorias);
    }
}
```

**Por que isso é bom?**
- ✅ Código de tratamento de erro em **um lugar só**
- ✅ Se mudar como tratar erros, muda em 1 lugar
- ✅ **Impossível** esquecer de tratar erro
- ✅ Controller fica limpo e focado

### 🔄 Fluxo Lógico: Pipeline de Execução

```
1. Cliente faz requisição HTTP
   ↓
2. ASP.NET Core recebe
   ↓
3. Middleware 1 (Logging) → Registra requisição
   ↓
4. Middleware 2 (Autenticação) → Verifica token
   ↓
5. Middleware 3 (Exception Handler) → Envolve tudo em try-catch
   ↓
6. Controller → Processa requisição
   ↓
7. Se der erro → Middleware 3 captura
   ↓
8. Middleware 3 retorna resposta de erro padronizada
   ↓
9. Cliente recebe resposta
```

**Visualização:**
```
Request → Middleware 1 → Middleware 2 → Middleware 3 → Controller
                                                              ↓
Response ← Middleware 1 ← Middleware 2 ← Middleware 3 ← [Erro? Trata aqui]
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Crie a Classe do Middleware
```csharp
// CatalogoProdutos.API/Middleware/GlobalExceptionMiddleware.cs
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);  // Passa para o próximo middleware/controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
            
            // Retorna resposta padronizada
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno no servidor",
                Detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
            };
            
            var responseJson = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(responseJson);
        }
    }
}
```

**💡 Lembre-se:** 
- `RequestDelegate _next` = "Próximo middleware na fila"
- `await _next(context)` = "Passa a requisição adiante"

#### Passo 2: Registre no Program.cs
```csharp
// Program.cs (linha 76)
app.UseMiddleware<GlobalExceptionMiddleware>();
```

**⚠️ IMPORTANTE:** Ordem importa! Middlewares executam na ordem que são registrados.

**Ordem recomendada:**
```csharp
app.UseHttpsRedirection();           // 1. Redireciona HTTP para HTTPS
app.UseMiddleware<GlobalExceptionMiddleware>();  // 2. Trata erros (deve vir ANTES dos controllers)
app.MapControllers();                // 3. Mapeia controllers
```

### 🎯 Casos de Uso Comuns

**1. Tratamento de Exceções (como no projeto):**
- Captura erros não tratados
- Retorna resposta padronizada
- Loga erros

**2. Autenticação:**
- Verifica token JWT
- Valida permissões
- Rejeita requisições não autenticadas

**3. Logging:**
- Registra todas as requisições
- Mede tempo de resposta
- Rastreia erros

**4. CORS:**
- Permite requisições de outros domínios
- Configura headers de segurança

### 💡 Conceitos Importantes

**Pipeline:**
- Sequência de middlewares que processam a requisição
- Cada middleware pode modificar a requisição ou resposta
- Executam na ordem de registro

**RequestDelegate:**
- Representa o próximo middleware
- `await _next(context)` passa a requisição adiante
- Se não chamar `_next`, a pipeline para (útil para autenticação que rejeita)

**HttpContext:**
- Contém tudo sobre a requisição/resposta
- `context.Request` = dados da requisição
- `context.Response` = dados da resposta

### 📚 Documentação Microsoft
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/

---

## 11. FluentValidation

### 🎭 Para que serve? (Analogia Prática)

Imagine um **porteiro de prédio**:
- Ele verifica se você tem documento (campo obrigatório)
- Ele verifica se o documento está válido (formato correto)
- Ele verifica se você tem permissão (regras de negócio)
- Se algo estiver errado, ele **não deixa entrar**

**FluentValidation é esse porteiro** - valida os dados antes de chegar no controller!

### ❌ O Problema (Refinamento Progressivo)

**Sem FluentValidation (código ruim):**
```csharp
[HttpPost]
public async Task<IActionResult> Post(CategoriaCreateDTO dto)
{
    // ❌ PROBLEMA: Validação manual e repetitiva
    if (string.IsNullOrEmpty(dto.Nome))
    {
        return BadRequest("Nome é obrigatório");
    }
    
    if (dto.Nome.Length < 3 || dto.Nome.Length > 10)
    {
        return BadRequest("Nome deve ter entre 3 e 10 caracteres");
    }
    
    // Repetir isso em TODOS os controllers...
}
```

**Por que isso é ruim?**
- 🔴 Código de validação **espalhado** em todos os controllers
- 🔴 Fácil esquecer de validar algum campo
- 🔴 Mensagens de erro inconsistentes
- 🔴 Difícil testar validações isoladamente

### ✅ A Solução Recomendada

**Com FluentValidation (código bom):**
```csharp
// Validador separado (uma vez só)
public class CategoryCreateDTOValidator : AbstractValidator<CategoriaCreateDTO>
{
    public CategoryCreateDTOValidator()
    {
        RuleFor(c => c.Nome)
            .NotEmpty()
            .WithMessage("O nome da categoria é obrigatório.")
            .Length(3, 10)
            .WithMessage("O nome deve ter entre 3 e 10 caracteres.");
    }
}

// Controller limpo!
[HttpPost]
public async Task<IActionResult> Post(CategoriaCreateDTO dto)
{
    // ✅ Validação automática! Se inválido, retorna 400 antes mesmo de entrar aqui
    var categoria = await _service.CreateCategoriaAsync(dto);
    return Ok(categoria);
}
```

**Por que isso é bom?**
- ✅ Validação **centralizada** em classes separadas
- ✅ **Impossível** esquecer de validar (é automático)
- ✅ Mensagens de erro consistentes
- ✅ Fácil testar validações isoladamente

### 🔄 Fluxo Lógico: Como Funciona

```
1. Cliente envia POST /Categoria com JSON
   ↓
2. ASP.NET Core deserializa JSON para CategoriaCreateDTO
   ↓
3. FluentValidation intercepta ANTES do controller
   ↓
4. Valida CategoriaCreateDTO usando CategoryCreateDTOValidator
   ├─ ✅ Válido? → Passa para o controller
   └─ ❌ Inválido? → Retorna 400 Bad Request com mensagens de erro
   ↓
5. Controller só recebe se estiver válido!
```

**Visualização:**
```
Request → Deserialização → FluentValidation → [Válido?] → Controller
                                      ↓
                                 [Inválido?] → 400 Bad Request
```

### 📝 Receita de Bolo (Como Implementar)

#### Passo 1: Instale o Pacote NuGet
```bash
dotnet add package FluentValidation.AspNetCore
```

#### Passo 2: Crie o Validador
```csharp
// Catalogo.Application/Validators/CategoryCreateDTOValidator.cs
using FluentValidation;

public class CategoryCreateDTOValidator : AbstractValidator<CategoriaCreateDTO>
{
    public CategoryCreateDTOValidator()
    {
        RuleFor(c => c.Nome)
            .NotEmpty()
            .WithMessage("O nome da categoria é obrigatório.")
            .Length(3, 10)
            .WithMessage("O nome deve ter entre 3 e 10 caracteres.");
    }
}
```

**💡 Lembre-se:** 
- `AbstractValidator<T>` = "Validador para o tipo T"
- `RuleFor(c => c.Nome)` = "Regra para a propriedade Nome"
- `.NotEmpty()` = "Não pode ser vazio"
- `.Length(3, 10)` = "Deve ter entre 3 e 10 caracteres"

#### Passo 3: Registre no Program.cs
```csharp
// Program.cs (linhas 31-33)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreateDTOValidator>();
```

**Tradução:**
- `AddFluentValidationAutoValidation()` = "Habilita validação automática"
- `AddValidatorsFromAssemblyContaining` = "Registra todos os validators do assembly"

#### Passo 4: Use no Controller (Automático!)
```csharp
[HttpPost]
public async Task<IActionResult> Post(CategoriaCreateDTO dto)
{
    // ✅ Se dto for inválido, retorna 400 ANTES de entrar aqui!
    var categoria = await _service.CreateCategoriaAsync(dto);
    return Ok(categoria);
}
```

**Não precisa fazer nada!** A validação é automática!

### 🎯 Regras Comuns do FluentValidation

**Validações Básicas:**
```csharp
RuleFor(x => x.Nome)
    .NotEmpty()                    // Não pode ser vazio
    .NotNull()                     // Não pode ser null
    .MaximumLength(100)            // Máximo 100 caracteres
    .MinimumLength(3)              // Mínimo 3 caracteres
    .Length(3, 100)                // Entre 3 e 100 caracteres
    .EmailAddress()                // Formato de email válido
    .Matches(@"^[0-9]+$")          // Regex (apenas números)
```

**Validações Customizadas:**
```csharp
RuleFor(x => x.Preco)
    .GreaterThan(0)               // Maior que 0
    .LessThan(10000)              // Menor que 10000
    .InclusiveBetween(1, 9999)    // Entre 1 e 9999 (inclusive)
```

**Validações Condicionais:**
```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .When(x => x.ReceberNotificacoes)  // Só valida se ReceberNotificacoes for true
    .WithMessage("Email é obrigatório quando deseja receber notificações");
```

### 💡 Exemplo de Resposta de Erro

**Se enviar dados inválidos:**
```json
POST /Categoria
{
  "nome": "AB"  // Muito curto!
}
```

**Resposta automática (400 Bad Request):**
```json
{
  "errors": {
    "Nome": [
      "O nome deve ter entre 3 e 10 caracteres."
    ]
  },
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

### 📚 Documentação FluentValidation
https://docs.fluentvalidation.net/

---

## 12. Data Annotations

### 📍 Onde está sendo usado:
- `Catalogo.Core.Entities.Categoria`
- `Catalogo.Core.Entities.Produto`

### 🔍 O que é?
**Data Annotations** são atributos que definem regras e metadados para propriedades de classes, usados pelo EF Core para mapear entidades para o banco de dados.

### ❓ Por que usar?
1. **Declarativo**: Regras definidas diretamente nas propriedades
2. **EF Core**: Usado para gerar schema do banco
3. **Validação**: Pode ser usado para validação também

### 🎯 Como funciona no projeto:

#### 12.1. Exemplo em `Categoria`:
```csharp
[Table("Categorias")]  // Nome da tabela no banco
public class Categoria
{
    [Key]  // Chave primária
    public int CategoriaId { get; set; }
    
    [Required]  // NOT NULL
    [StringLength(80)]  // VARCHAR(80)
    public string? Nome { get; set; }
    
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]  // Ignora na serialização JSON
    public ICollection<Produto>? Produtos { get; set; }
}
```

**Atributos comuns:**
- `[Key]`: Chave primária
- `[Required]`: Campo obrigatório (NOT NULL)
- `[StringLength(n)]`: Tamanho máximo da string
- `[Table("NomeTabela")]`: Nome da tabela
- `[Column(TypeName = "decimal(10,2)")]`: Tipo específico no banco
- `[JsonIgnore]`: Ignora na serialização JSON

**Documentação Microsoft:** https://learn.microsoft.com/en-us/ef/core/modeling/entity-types

---

## 13. Swagger/OpenAPI

### 📍 Onde está sendo usado:
- `Program.cs` (linhas 62-73)

### 🔍 O que é?
**Swagger/OpenAPI** gera documentação interativa da API automaticamente, permitindo testar endpoints diretamente no navegador.

### ❓ Por que usar?
1. **Documentação automática**: Sempre atualizada com o código
2. **Testes**: Testa API sem Postman/Insomnia
3. **Colaboração**: Frontend sabe exatamente como usar a API

### 🎯 Como funciona no projeto:

#### 13.1. Configuração no `Program.cs`:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

**`AddSwaggerGen()`**: Gera documentação OpenAPI.

**`UseSwagger()`**: Expõe endpoint `/swagger/v1/swagger.json`.

**`UseSwaggerUI()`**: Expõe interface web em `/swagger`.

#### 13.2. Acesso:
- **JSON:** `https://localhost:5001/swagger/v1/swagger.json`
- **UI:** `https://localhost:5001/swagger`

**Documentação Microsoft:** https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger

---

## 14. Routing e HTTP Verbs

### 📍 Onde está sendo usado:
- `CategoriaController.cs` (linhas 10, 24, 40, 57, 69, 81, 93)

### 🔍 O que é?
**Routing** mapeia URLs para actions dos controllers. **HTTP Verbs** (GET, POST, PUT, DELETE) definem a ação a ser executada.

### ❓ Por que usar?
1. **RESTful**: Segue padrão REST
2. **Semântica**: Verbos HTTP têm significado claro
3. **Padrão**: Convenção amplamente aceita

### 🎯 Como funciona no projeto:

#### 14.1. Atributos de Roteamento:
```csharp
[Route("[controller]")]  // Rota base: /Categoria
[ApiController]
public class CategoriaController : ControllerBase
{
    [HttpGet]  // GET /Categoria
    public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> Get()
    {
        // ...
    }
    
    [HttpGet("{id:int}", Name = "ObterCategoria")]  // GET /Categoria/5
    public async Task<ActionResult<CategoriaResponseDTO>> Get(int id)
    {
        // ...
    }
    
    [HttpGet("produtos")]  // GET /Categoria/produtos
    public async Task<ActionResult<IEnumerable<CategoriaResponseProdutosDTO>>> GetCategoriasProdutos()
    {
        // ...
    }
    
    [HttpPost]  // POST /Categoria
    public async Task<ActionResult> Post(CategoriaCreateDTO categoriaDTO)
    {
        // ...
    }
    
    [HttpPut("{id:int}")]  // PUT /Categoria/5
    public async Task<ActionResult> Put(int id, CategoriaCreateDTO categoriaDTO)
    {
        // ...
    }
    
    [HttpDelete("{id:int}")]  // DELETE /Categoria/5
    public async Task<ActionResult> Delete(int id)
    {
        // ...
    }
}
```

**`[Route("[controller]")]`**: Usa o nome do controller (sem "Controller") como rota base.

**`[ApiController]`**: Habilita comportamentos de API (validação automática, binding de parâmetros, etc.).

**`{id:int}`**: Constraint de rota - só aceita inteiros.

**`Name = "ObterCategoria"`**: Nome da rota (usado em `CreatedAtRouteResult`).

#### 14.2. Tipos de Retorno:
```csharp
// Retorna 200 OK com dados
return Ok(categoriasDTO);

// Retorna 404 Not Found
return NotFound("Nenhuma categoria encontrado!");

// Retorna 201 Created com Location header
return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaResponseDTO.CategoriaId }, categoriaResponseDTO);
```

**Documentação Microsoft:** https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing

---

## 15. Expression Trees

### 📍 Onde está sendo usado:
- `Catalogo.Core.Interfaces.IRepository<T>` (linha 9)
- `Catalogo.Infrastructure.Repositories.Repository<T>` (linha 22)

### 🔍 O que é?
**Expression Trees** são estruturas de dados que representam código como dados. Permitem criar queries dinâmicas e type-safe.

### ❓ Por que usar?
1. **Type-safe**: Erros detectados em tempo de compilação
2. **Flexibilidade**: Queries dinâmicas
3. **EF Core**: Usado pelo EF Core para traduzir LINQ em SQL

### 🎯 Como funciona no projeto:

#### 15.1. Uso no Repository:
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
}

// Implementação
public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate)
{
    return await _context.Set<T>().FirstOrDefaultAsync(predicate);
}

// Uso
var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);
```

**`Expression<Func<T, bool>>`**: Representa uma expressão lambda que pode ser analisada e traduzida.

**`c => c.CategoriaId == id`**: Expressão lambda que o EF Core traduz para SQL:
```sql
SELECT * FROM Categorias WHERE CategoriaId = @id
```

**Vantagem:** Type-safe! Se `CategoriaId` não existir, erro em tempo de compilação.

**Documentação Microsoft:** https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/

---

## 16. Generics

### 📍 Onde está sendo usado:
- `IRepository<T>`
- `Repository<T>`
- `CategoriaRepository : Repository<Categoria>`

### 🔍 O que é?
**Generics** permitem criar classes, interfaces e métodos que trabalham com tipos definidos posteriormente, aumentando reutilização e type-safety.

### ❓ Por que usar?
1. **Reutilização**: Mesmo código para diferentes tipos
2. **Type-safety**: Erros detectados em tempo de compilação
3. **Performance**: Sem boxing/unboxing

### 🎯 Como funciona no projeto:

#### 16.1. Interface Genérica:
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}
```

**`T`**: Tipo genérico (placeholder).

**`where T : class`**: Constraint - T deve ser uma classe.

#### 16.2. Implementação Genérica:
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
}
```

**`Repository<T>`**: Classe genérica que funciona para qualquer tipo T.

**`_context.Set<T>()`**: Método do EF Core que retorna DbSet para o tipo T.

#### 16.3. Especialização:
```csharp
public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context) { }
    
    // Métodos específicos de Categoria
    public async Task<IEnumerable<Categoria>> GetCategoriasProdutosAsync()
    {
        return await _context.Categorias
            .Include(p => p.Produtos)
            .ToListAsync();
    }
}
```

**`Repository<Categoria>`**: Especializa o Repository genérico para Categoria.

**Benefício:** Código comum (CRUD) no Repository genérico, código específico no CategoriaRepository!

**Documentação Microsoft:** https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics

---

## 🎓 Resumo dos Conceitos por Camada

### **API (Apresentação)**
- ✅ Controllers (ASP.NET Core) - Recebem requisições HTTP
- ✅ Routing e HTTP Verbs - Mapeiam URLs para actions
- ✅ Middleware - Interceptam requisições (erros, autenticação, logging)
- ✅ Swagger/OpenAPI - Documentação automática
- ✅ Async/Await - Operações assíncronas

### **Application (Aplicação)**
- ✅ Service Layer - Lógica de negócio
- ✅ DTOs - Objetos de transferência de dados
- ✅ AutoMapper - Conversão automática Entity ↔ DTO
- ✅ FluentValidation - Validação de dados
- ✅ Cache Híbrido - Performance (L1 + L2)

### **Infrastructure (Infraestrutura)**
- ✅ Entity Framework Core - ORM (acesso a dados)
- ✅ Repository Pattern - Abstração de acesso a dados
- ✅ DbContext - Conexão com banco
- ✅ Migrations - Versionamento de schema

### **Core (Domínio)**
- ✅ Entities - Entidades de negócio
- ✅ Interfaces - Contratos (abstrações)
- ✅ Data Annotations - Metadados para EF Core
- ✅ Expression Trees - Queries type-safe
- ✅ Generics - Código reutilizável

---

## ✅ Checklist de Implementação (Receita de Bolo Completa)

### Quando criar uma nova funcionalidade (ex: Fornecedor):

#### 1️⃣ **Core (Domínio)**
- [ ] Criar Entity (`Fornecedor.cs`) com Data Annotations
- [ ] Criar Interface (`IFornecedorRepository.cs`) herdando de `IRepository<Fornecedor>`

#### 2️⃣ **Application (Aplicação)**
- [ ] Criar DTOs:
  - [ ] `FornecedorCreateDTO.cs` (entrada)
  - [ ] `FornecedorResponseDTO.cs` (saída)
  - [ ] `FornecedorResponseProdutoDTO.cs` (com relacionamento, se necessário)
- [ ] Criar Interface do Service (`IFornecedorService.cs`)
- [ ] Criar Service (`FornecedorService.cs`) com:
  - [ ] Injeção de Repository, Mapper e Cache
  - [ ] Métodos assíncronos (`async Task<T>`)
  - [ ] Cache nos métodos Get
  - [ ] Invalidação de cache nos métodos Create/Update/Delete
- [ ] Criar Validador (`FornecedorCreateDTOValidator.cs`)
- [ ] Configurar AutoMapper (`MappingProfile.cs`):
  - [ ] `CreateMap<Fornecedor, FornecedorResponseDTO>()`
  - [ ] `CreateMap<FornecedorCreateDTO, Fornecedor>()`

#### 3️⃣ **Infrastructure (Infraestrutura)**
- [ ] Criar Repository (`FornecedorRepository.cs`) herdando de `Repository<Fornecedor>`
- [ ] Adicionar `DbSet<Fornecedor>` no `AppDbContext`
- [ ] Criar Migration: `dotnet ef migrations add AddFornecedor`
- [ ] Aplicar Migration: `dotnet ef database update`

#### 4️⃣ **API (Apresentação)**
- [ ] Criar Controller (`FornecedorController.cs`) com:
  - [ ] `[Route("[controller]")]` e `[ApiController]`
  - [ ] Injeção de `IFornecedorService`
  - [ ] Métodos assíncronos:
    - [ ] `[HttpGet]` - Listar todos
    - [ ] `[HttpGet("{id:int}")]` - Buscar por ID
    - [ ] `[HttpPost]` - Criar
    - [ ] `[HttpPut("{id:int}")]` - Atualizar
    - [ ] `[HttpDelete("{id:int}")]` - Deletar

#### 5️⃣ **Program.cs (Registros)**
- [ ] Registrar Repository: `builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>()`
- [ ] Registrar Service: `builder.Services.AddScoped<IFornecedorService, FornecedorService>()`
- [ ] Validador já registrado automaticamente (se usar `AddValidatorsFromAssemblyContaining`)

### 🎯 Ordem de Implementação Recomendada:

```
1. Core (Entity + Interface)
   ↓
2. Infrastructure (Repository + DbContext)
   ↓
3. Application (DTOs + Service + Validator + Mapping)
   ↓
4. API (Controller)
   ↓
5. Program.cs (Registros)
```

---

## 🔄 Fluxo Completo: Requisição HTTP Completa

```
1. Cliente faz POST /Categoria
   ↓
2. Middleware (Exception Handler) envolve tudo
   ↓
3. ASP.NET Core deserializa JSON → CategoriaCreateDTO
   ↓
4. FluentValidation valida CategoriaCreateDTO
   ├─ ❌ Inválido? → 400 Bad Request
   └─ ✅ Válido? → Continua
   ↓
5. Controller recebe CategoriaCreateDTO
   ↓
6. Controller chama _service.CreateCategoriaAsync(dto)
   ↓
7. Service converte DTO → Entity (AutoMapper)
   ↓
8. Service chama _repository.CreateAsync(entity)
   ↓
9. Repository salva no banco (EF Core)
   ↓
10. Service invalida cache
   ↓
11. Service converte Entity → DTO (AutoMapper)
   ↓
12. Service retorna CategoriaResponseDTO
   ↓
13. Controller retorna 201 Created com DTO
   ↓
14. Middleware serializa DTO → JSON
   ↓
15. Cliente recebe resposta
```

---

## 📖 Referências Úteis

- **ASP.NET Core:** https://learn.microsoft.com/en-us/aspnet/core/
- **Entity Framework Core:** https://learn.microsoft.com/en-us/ef/core/
- **Dependency Injection:** https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **AutoMapper:** https://docs.automapper.org/
- **FluentValidation:** https://docs.fluentvalidation.net/
- **C# Async/Await:** https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
- **Redis:** https://redis.io/

---

## 💡 Dicas Finais para Relembrar

1. **Sempre use Async/Await** em métodos que acessam banco/API/arquivos
2. **Sempre injete dependências** via construtor (nunca `new` dentro da classe)
3. **Sempre use DTOs** para comunicação com API (nunca retorne Entities diretamente)
4. **Sempre configure AutoMapper** quando converter Entity ↔ DTO
5. **Sempre valide dados** com FluentValidation antes de processar
6. **Sempre use cache** em operações de leitura frequentes
7. **Sempre invalide cache** quando criar/atualizar/deletar dados
8. **Sempre trate erros** com Middleware (não em cada controller)

---

**Criado para relembrar conceitos após 3 meses sem trabalhar no projeto! 🚀**

**Metodologia aplicada:** Refinamento Progressivo + Analogias Práticas + Receitas de Bolo + Fluxos Lógicos
