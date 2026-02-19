using AutoMapper;
using Catalogo.Application.Interfaces;
using Catalogo.Application.Services;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalogo.Application.Tests
{
    public class CategoriaServiceTests
    {

        private readonly Mock<ICategoriaRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHybridCacheService> _mockCache;
        private readonly CategoriaService _service;


        public CategoriaServiceTests()
        {
            _mockRepo = new Mock<ICategoriaRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCache = new Mock<IHybridCacheService>();
            _service = new CategoriaService(_mockRepo.Object, _mockMapper.Object, _mockCache.Object);



            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Categoria>);
        }

        [Fact]
        public async Task GetAllAsync_QuandoNaoExistiremCategorias_DeveRetornarListaVazia()
        {
            // Act
            var result = await _service.GetCategoriasAsync();
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);


        }
    }
}