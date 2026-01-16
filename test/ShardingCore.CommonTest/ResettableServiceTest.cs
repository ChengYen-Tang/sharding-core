using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShardingCore.EFCores;
using ShardingCore.Sharding;
using Xunit;

namespace ShardingCore.CommonTest
{
    public class ResettableServiceTest
    {
        [Fact]
        public void ResetState_ClearsExecutorFlag()
        {
            var options = new DbContextOptionsBuilder<TestResettableDbContext>().Options;
            using var context = new TestResettableDbContext(options);
            var executorFlag = typeof(AbstractShardingDbContext)
                .GetField("_createExecutor", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(executorFlag);
            executorFlag!.SetValue(context, true);

            var resettableService = new ShardingDbContextResettableService(new TestCurrentDbContext(context));
            resettableService.ResetState();

            Assert.False((bool)executorFlag!.GetValue(context)!);
        }

        [Fact]
        public async Task ResetStateAsync_ClearsExecutorFlag()
        {
            var options = new DbContextOptionsBuilder<TestResettableDbContext>().Options;
            using var context = new TestResettableDbContext(options);
            var executorFlag = typeof(AbstractShardingDbContext)
                .GetField("_createExecutor", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(executorFlag);
            executorFlag!.SetValue(context, true);

            var resettableService = new ShardingDbContextResettableService(new TestCurrentDbContext(context));
            await resettableService.ResetStateAsync();

            Assert.False((bool)executorFlag!.GetValue(context)!);
        }

        private sealed class TestResettableDbContext : AbstractShardingDbContext
        {
            public TestResettableDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        private sealed class TestCurrentDbContext : ICurrentDbContext
        {
            public TestCurrentDbContext(DbContext context)
            {
                Context = context;
            }

            public DbContext Context { get; }
        }
    }
}
