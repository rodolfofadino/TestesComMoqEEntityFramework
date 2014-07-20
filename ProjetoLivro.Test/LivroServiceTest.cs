using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjetoLivro.BL;
using System.Linq.Expressions;
using System.Threading;

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


        ////Query test
        [TestMethod]
        public async Task RetornaSomenteLivrosAtivosAsync()
        {
            var data = new List<Livro>
            {
            new Livro { Nome = "Livro A", Status=true },
            new Livro { Nome = "Livro B", Status=false },
            new Livro { Nome = "Livro C", Status=true },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Livro>>();
            mockSet.As<IDbAsyncEnumerable<Livro>>()
            .Setup(m => m.GetAsyncEnumerator())
            .Returns(new TestDbAsyncEnumerator<Livro>(data.GetEnumerator()));

            mockSet.As<IQueryable<Livro>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<Livro>(data.Provider));

            mockSet.As<IQueryable<Livro>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Livro>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<LivroContext>();
            mockContext.Setup(c => c.Livros).Returns(mockSet.Object);

            var service = new LivroService(mockContext.Object);
            var livros = await service.GetLivrosAtivosAsync();

            Assert.AreEqual(2, livros.Count);
            Assert.AreEqual("Livro A", livros[0].Nome);
            Assert.AreEqual("Livro C", livros[1].Nome);
        }
    }
    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }

    }
    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }
    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}
