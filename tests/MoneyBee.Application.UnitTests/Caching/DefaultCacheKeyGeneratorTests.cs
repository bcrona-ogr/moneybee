using FluentAssertions;
using MoneyBee.Shared.Application.Caching.Attributes;
using MoneyBee.Shared.Application.Caching.Implementations;

namespace MoneyBee.Application.UnitTests.Caching
{
    public  class DefaultCacheKeyGeneratorTests
    {
        private sealed class SampleInput
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private sealed class DummyUseCase
        {
        }

        [Fact]
        public void Generate_Should_ReturnSameKey_When_InputIsSame()
        {
            var generator = new DefaultCacheKeyGenerator();

            var input = new SampleInput
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Ali"
            };

            var attribute = new CachingAttribute(60)
            {
                Prefix = "sample",
                VaryByProperties = [nameof(SampleInput.Id), nameof(SampleInput.Name)]
            };

            var key1 = generator.Generate(typeof(DummyUseCase), input, attribute);
            var key2 = generator.Generate(typeof(DummyUseCase), input, attribute);

            key1.Should().Be(key2);
        }

        [Fact]
        public void Generate_Should_ReturnDifferentKey_When_VaryByPropertyChanges()
        {
            var generator = new DefaultCacheKeyGenerator();

            var attribute = new CachingAttribute(60)
            {
                Prefix = "sample",
                VaryByProperties = [nameof(SampleInput.Id)]
            };

            var input1 = new SampleInput
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Ali"
            };

            var input2 = new SampleInput
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Ali"
            };

            var key1 = generator.Generate(typeof(DummyUseCase), input1, attribute);
            var key2 = generator.Generate(typeof(DummyUseCase), input2, attribute);

            key1.Should().NotBe(key2);
        }

        [Fact]
        public void Generate_Should_IgnoreNonVaryingProperties()
        {
            var generator = new DefaultCacheKeyGenerator();

            var attribute = new CachingAttribute(60)
            {
                Prefix = "sample",
                VaryByProperties = [nameof(SampleInput.Id)]
            };

            var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var input1 = new SampleInput
            {
                Id = id,
                Name = "Ali"
            };

            var input2 = new SampleInput
            {
                Id = id,
                Name = "Veli"
            };

            var key1 = generator.Generate(typeof(DummyUseCase), input1, attribute);
            var key2 = generator.Generate(typeof(DummyUseCase), input2, attribute);

            key1.Should().Be(key2);
        }
    }
}