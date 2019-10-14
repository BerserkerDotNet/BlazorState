using System;
using System.Collections.Generic;
using BlazorState.Redux.Interfaces;
using BlazorState.Redux.Tests.Types;
using BlazorState.Redux.Tests.Types.Actions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BlazorState.Redux.Tests
{
    [TestFixture]
    public class AggregateReducerTests
    {
        [Test]
        public void ShouldHandleEmptyReducersList()
        {
            var reducer = new AggregateReducer<EmptyState>(new Dictionary<string, object>());
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
        }

        [Test]
        public void ShouldHandleNullReducersList()
        {
            var reducer = new AggregateReducer<EmptyState>(null);
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
        }

        [Test]
        public void ShouldCallReducerForEveryProperty()
        {
            const string expectedName = "Foo";
            const string expectedCompany = "Bla Inc.";
            const string expectedTitle = "Principal";
            const int expectedAge = 34;
            const float expectedBonus = 24.52f;

            var mapping = new Dictionary<string, object>
            {
                { nameof(MultiPropertyState.Name), GetSimpleReducer(expectedName) },
                { nameof(MultiPropertyState.Company), GetSimpleReducer(expectedCompany) },
                { nameof(MultiPropertyState.Title), GetSimpleReducer(expectedTitle) },
                { nameof(MultiPropertyState.Age), GetSimpleReducer(expectedAge) },
                { nameof(MultiPropertyState.Bonus), GetSimpleReducer(expectedBonus) }
            };

            var reducer = new AggregateReducer<MultiPropertyState>(mapping);
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
            newState.Name.Should().Be(expectedName);
            newState.Title.Should().Be(expectedTitle);
            newState.Company.Should().Be(expectedCompany);
            newState.Age.Should().Be(expectedAge);
            newState.Bonus.Should().Be(expectedBonus);
        }

        [Test]
        public void ShouldOnlyVisitPublicProperties()
        {
            const string expectedName = "Foo";
            const string expectedCompany = "Bla Inc.";
            const string expectedTitle = "Principal";
            const int expectedAge = 34;

            var mapping = new Dictionary<string, object>
            {
                { nameof(NonPublicPropertiesState.Name), GetSimpleReducer(expectedName) },
                { nameof(NonPublicPropertiesState.Company), GetSimpleReducer(expectedCompany) },
                { "Title", GetSimpleReducer(expectedTitle) },
                { "Age", GetSimpleReducer(expectedAge) },
                { nameof(NonPublicPropertiesState.StaticProp), GetSimpleReducer("Fooo") }
            };

            var reducer = new AggregateReducer<NonPublicPropertiesState>(mapping);
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
            newState.Name.Should().Be(expectedName);
            newState.GetTitle().Should().BeNullOrEmpty();
            newState.Company.Should().BeNullOrEmpty();
            newState.GetAge().Should().Be(0);
            NonPublicPropertiesState.StaticProp.Should().BeNullOrEmpty();
        }

        [Test]
        public void ShouldSetPropertiesWithoutPublicSetter()
        {
            const string expectedName = "Foo";

            var mapping = new Dictionary<string, object>
            {
                { nameof(PrivateSetterState.Name), GetSimpleReducer(expectedName) }
            };

            var reducer = new AggregateReducer<PrivateSetterState>(mapping);
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
            newState.Name.Should().Be(expectedName);
        }

        [Test]
        public void ShouldSkipReadOnlyProperties()
        {
            const string expectedName = "Foo";
            const string expectedTitle = "Senior";
            const int expectedAge = 5;

            var mapping = new Dictionary<string, object>
            {
                { nameof(ReadOnlyProperties.Name), GetSimpleReducer(expectedName) },
                { nameof(ReadOnlyProperties.Title), GetSimpleReducer(expectedTitle) },
                { nameof(ReadOnlyProperties.Age), GetSimpleReducer(expectedAge) }
            };

            var reducer = new AggregateReducer<ReadOnlyProperties>(mapping);
            var newState = reducer.Reduce(null, new EmptyAction());

            newState.Should().NotBeNull();
            newState.Name.Should().BeNullOrEmpty();
            newState.Title.Should().Be("Principal");
            newState.Age.Should().Be(expectedAge);
        }

        [Test]
        public void ShouldThrowExceptionIfNotReducerTypeInMapping()
        {
            var mapping = new Dictionary<string, object>
            {
                { nameof(PrivateSetterState.Name), new object() }
            };

            var reducer = new AggregateReducer<PrivateSetterState>(mapping);
            reducer.Invoking(r => r.Reduce(null, new EmptyAction()))
                .Should().Throw<ArgumentException>();
        }

        [Test]
        public void ShouldCreateNewStateInstanceEverytime()
        {
            var initialState = new EmptyState();
            var reducer = new AggregateReducer<EmptyState>(new Dictionary<string, object>());
            var newState = reducer.Reduce(initialState, new EmptyAction());

            newState.Should().NotBeNull();
            newState.Should().NotBeSameAs(initialState);
        }

        private IReducer<T> GetSimpleReducer<T>(T value)
        {
            var moq = new Mock<IReducer<T>>();
            moq.Setup(r => r.Reduce(It.IsAny<T>(), It.IsAny<IAction>()))
                .Returns(value);
            return moq.Object;
        }
    }
}