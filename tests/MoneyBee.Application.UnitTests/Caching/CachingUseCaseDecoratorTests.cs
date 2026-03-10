using FluentAssertions;
using MoneyBee.Shared.Application.Caching.Abstractions;
using MoneyBee.Shared.Application.Caching.Attributes;
using MoneyBee.Shared.Application.Caching.Implementations;
using MoneyBee.Shared.Application.UseCase;
using Moq;

namespace MoneyBee.Application.UnitTests.Caching
{
    public  class CachingUseCaseDecoratorTests
    {
        private sealed class TestInput
        {
            public Guid Id { get; set; }
        }

        private sealed class TestOutput
        {
            public string Value { get; set; }
        }

        private sealed class CachedTestUseCase : BaseUseCase<TestInput, TestOutput>
        {
            private readonly Func<TestOutput> _factory;

            public int ExecuteInternalCallCount { get; private set; }

            public CachedTestUseCase(Func<TestOutput> factory)
            {
                _factory = factory;
            }

            public override void Validate()
            {
            }

            [Caching(60, Prefix = "test-usecase", VaryByProperties = [nameof(TestInput.Id)])]
            public override Task<TestOutput> Execute(CancellationToken cancellationToken = default)
            {
                return base.Execute(cancellationToken);
            }

            protected override Task<TestOutput> ExecuteInternal(CancellationToken cancellationToken)
            {
                ExecuteInternalCallCount++;
                return Task.FromResult(_factory());
            }
        }

        private sealed class NonCachedTestUseCase : BaseUseCase<TestInput, TestOutput>
        {
            private readonly Func<TestOutput> _factory;

            public int ExecuteInternalCallCount { get; private set; }

            public NonCachedTestUseCase(Func<TestOutput> factory)
            {
                _factory = factory;
            }

            public override void Validate()
            {
            }

            protected override Task<TestOutput> ExecuteInternal(CancellationToken cancellationToken)
            {
                ExecuteInternalCallCount++;
                return Task.FromResult(_factory());
            }
        }

        [Fact]
        public async Task Execute_Should_ReturnCachedValue_When_KeyExists()
        {
            var input = new TestInput
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };

            var cachedOutput = new TestOutput
            {
                Value = "from-cache"
            };

            var inner = new CachedTestUseCase(() => new TestOutput
            {
                Value = "from-inner"
            });

            inner.SetInput(input);

            var cacheMock = new Mock<IKeyValueStore>();
            cacheMock.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            cacheMock.Setup(x => x.GetAsync<TestOutput>(It.IsAny<string>(), null!)).ReturnsAsync(cachedOutput);

            var keyGeneratorMock = new Mock<ICacheKeyGenerator>();
            keyGeneratorMock.Setup(x => x.Generate(typeof(CachedTestUseCase), input, It.IsAny<CachingAttribute>())).Returns("cache-key-1");

            var sut = new CachingUseCaseDecorator<TestInput, TestOutput>(inner, cacheMock.Object, keyGeneratorMock.Object);

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().Be("from-cache");
            inner.ExecuteInternalCallCount.Should().Be(0);

            cacheMock.Verify(x => x.ExistsAsync("cache-key-1"), Times.Once);
            cacheMock.Verify(x => x.GetAsync<TestOutput>("cache-key-1", default), Times.Once);
            cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        [Fact]
        public async Task Execute_Should_CallInnerAndCacheResult_When_KeyDoesNotExist()
        {
            var input = new TestInput
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };

            var innerOutput = new TestOutput
            {
                Value = "from-inner"
            };

            var inner = new CachedTestUseCase(() => innerOutput);
            inner.SetInput(input);

            var cacheMock = new Mock<IKeyValueStore>();
            cacheMock.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            cacheMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>())).ReturnsAsync(true);

            var keyGeneratorMock = new Mock<ICacheKeyGenerator>();
            keyGeneratorMock.Setup(x => x.Generate(typeof(CachedTestUseCase), input, It.IsAny<CachingAttribute>())).Returns("cache-key-2");

            var sut = new CachingUseCaseDecorator<TestInput, TestOutput>(inner, cacheMock.Object, keyGeneratorMock.Object);

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().Be("from-inner");
            inner.ExecuteInternalCallCount.Should().Be(1);

            cacheMock.Verify(x => x.ExistsAsync("cache-key-2"), Times.Once);
            cacheMock.Verify(x => x.SetAsync("cache-key-2", innerOutput, It.Is<TimeSpan?>(t => t == TimeSpan.FromSeconds(60))), Times.Once);
        }

        [Fact]
        public async Task Execute_Should_BypassCache_When_AttributeDoesNotExist()
        {
            var input = new TestInput
            {
                Id = Guid.NewGuid()
            };

            var inner = new NonCachedTestUseCase(() => new TestOutput
            {
                Value = "no-cache"
            });

            inner.SetInput(input);

            var cacheMock = new Mock<IKeyValueStore>();
            var keyGeneratorMock = new Mock<ICacheKeyGenerator>();

            var sut = new CachingUseCaseDecorator<TestInput, TestOutput>(inner, cacheMock.Object, keyGeneratorMock.Object);

            var result = await sut.Execute(CancellationToken.None);

            result.Should().NotBeNull();
            result.Value.Should().Be("no-cache");
            inner.ExecuteInternalCallCount.Should().Be(1);

            cacheMock.Verify(x => x.ExistsAsync(It.IsAny<string>()), Times.Never);
            cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()), Times.Never);
            keyGeneratorMock.Verify(x => x.Generate(It.IsAny<Type>(), It.IsAny<TestInput>(), It.IsAny<CachingAttribute>()), Times.Never);
        }
    }
}