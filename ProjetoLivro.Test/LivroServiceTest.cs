using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjetoLivro.BL;

namespace ProjetoLivro.Test
{
    [TestClass]
   public class LivroServiceTest
    {
        ////noQuery test
        [TestMethod]
        public void CriarLivroSalvaOContext()
        {
            var mockSet = new Mock<DbSet<Livro>>();

            var mockContext = new Mock<LivroContext>();
            mockContext.Setup(m => m.Livros).Returns(mockSet.Object);

            var service = new LivroService(mockContext.Object);
            service.Salvar(new Livro() { Nome = "Livro de teste", Status = true });

            mockSet.Verify(m => m.Add(It.IsAny<Livro>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        ////Query test
        [TestMethod]
        public void RetornaSomenteLivrosAtivos()
        {
            var data = new List<Livro>
            {
                new Livro { Nome = "Livro A", Status=true },
                new Livro { Nome = "Livro B", Status=false },
                new Livro { Nome = "Livro C", Status=true },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Livro>>();
            mockSet.As<IQueryable<Livro>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<LivroContext>();
            mockContext.Setup(c => c.Livros).Returns(mockSet.Object);

            var service = new LivroService(mockContext.Object);
            var livros = service.GetLivrosAtivos();

            Assert.AreEqual(2, livros.Count);
            Assert.AreEqual("Livro A", livros[0].Nome);
            Assert.AreEqual("Livro C", livros[1].Nome);

        }
    }
}
