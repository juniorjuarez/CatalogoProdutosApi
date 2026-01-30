namespace Catalogo.Application.Constants
{
    public static class CacheKeys
    {
        public const string CATEGORIAS_KEY = "TodasCategorias";
        public const string CATEGORIAS_PRODUTOS_KEY = "TodasCategoriasProdutos";
        public const string PRODUTOS_KEY = "TodosProdutos";
        public const string CategoriaPrefix = "Categoria_";
        public const string ProdutoPrefix = "Produto_";
        public const string Fornecedores_Key = "TodosFornecedores";

        public const string FORNECEDORES_PRODUTOS_KEY = "TodosFornecedoresProduto";

        public const string FornecedorPrefix = "Fornecedor_";

        public static readonly TimeSpan ABSOLUTE_EXPIRATION_L1 = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan ABSOLUTE_EXPIRATION_L2 = TimeSpan.FromMinutes(30);


    }
}