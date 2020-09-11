using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Tests;
using Xunit;

namespace HotChocolate.Data.Sorting
{

    public class QueryableSortVisitorBooleanTests
        : IClassFixture<SchemaCache>
    {
        private static readonly Foo[] _fooEntities = new[]{
            new Foo { Bar = true },
            new Foo { Bar = false }};

        private static readonly FooNullable[] _fooNullableEntities = new[]{
            new FooNullable { Bar = true },
            new FooNullable { Bar = null },
            new FooNullable { Bar = false }};

        private readonly SchemaCache _cache;

        public QueryableSortVisitorBooleanTests(
            SchemaCache cache)
        {
            _cache = cache;
        }

        [Fact]
        public async Task Create_Boolean_OrderBy()
        {
            // arrange
            IRequestExecutor tester = _cache.CreateSchema<Foo, FooSortType>(_fooEntities);

            // act
            // assert
            IExecutionResult res1 = await tester.ExecuteAsync(
                QueryRequestBuilder.New()
                    .SetQuery("{ root(order: { bar: ASC}){ bar}}")
                    .Create());

            res1.MatchSqlSnapshot("ASC");

            IExecutionResult res2 = await tester.ExecuteAsync(
                QueryRequestBuilder.New()
                    .SetQuery("{ root(order: { bar: DESC}){ bar}}")
                    .Create());

            res2.MatchSqlSnapshot("DESC");
        }

        [Fact]
        public async Task Create_Boolean_OrderBy_Nullable()
        {
            // arrange
            IRequestExecutor tester = _cache.CreateSchema<FooNullable, FooNullableSortType>(
                _fooNullableEntities);

            // act
            // assert
            IExecutionResult res1 = await tester.ExecuteAsync(
                QueryRequestBuilder.New()
                    .SetQuery("{ root(order: { bar: ASC}){ bar}}")
                    .Create());

            res1.MatchSqlSnapshot("ASC");

            IExecutionResult res2 = await tester.ExecuteAsync(
                QueryRequestBuilder.New()
                    .SetQuery("{ root(order: { bar: DESC}){ bar}}")
                    .Create());

            res2.MatchSqlSnapshot("DESC");
        }

        public class Foo
        {
            public int Id { get; set; }

            public bool Bar { get; set; }
        }

        public class FooNullable
        {
            public int Id { get; set; }

            public bool? Bar { get; set; }
        }

        public class FooSortType
            : SortInputType<Foo>
        {
        }

        public class FooNullableSortType
            : SortInputType<FooNullable>
        {
        }
    }
}