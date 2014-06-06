using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjetoLivro.BL;

namespace ProjetoLivro.Test
{
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
    }
}
