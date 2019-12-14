using System;
using BlazorState.Redux.Hooks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace BlazorState.Redux.Tests
{
    [TestFixture]
    public class HooksServiceTests
    {
        [Test]
        public void ReturnInitialState()
        {
            const int expectedS1 = 0;
            const int expectedS2 = 99;

            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            var (s1, _) = service.UseState(expectedS1, fakeComponent);
            var (s2, _) = service.UseState(expectedS2, fakeComponent);

            s1.Should().Be(expectedS1);
            s2.Should().Be(expectedS2);
        }

        [Test]
        public void HandleDifferentStateType()
        {
            const int expectedS1 = 0;
            const string expectedS2 = "Foo";

            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            var (s1, _) = service.UseState(expectedS1, fakeComponent);
            var (s2, _) = service.UseState(expectedS2, fakeComponent);

            s1.Should().Be(expectedS1);
            s2.Should().Be(expectedS2);
        }

        [Test]
        public void ReturnSameStateForEveryRender()
        {
            const int expectedS3 = 45;
            const int expectedS4 = -56;

            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            var (s1, setS1) = service.UseState(0, fakeComponent);
            var (s2, setS2) = service.UseState(99, fakeComponent);
            service.ComponentRendered(fakeComponent);

            setS1(expectedS3);
            setS2(expectedS4);

            var (s3, _) = service.UseState(0, fakeComponent);
            var (s4, _) = service.UseState(99, fakeComponent);

            s3.Should().Be(expectedS3);
            s4.Should().Be(expectedS4);
        }

        [Test]
        public void FailIfCalledNotInSameOrder()
        {
            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            service.UseState(0, fakeComponent);
            service.UseState("Foo", fakeComponent);
            service.UseState(99, fakeComponent);
            service.ComponentRendered(fakeComponent);

            service.UseState(0, fakeComponent);
            service.Invoking(s => s.UseState(99, fakeComponent))
                .Should().Throw<InvalidCastException>();
        }

        [Test]
        public void FailIfCalledNotSameNumberOfTimes()
        {
            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            service.UseState(0, fakeComponent);
            service.UseState("Foo", fakeComponent);
            service.ComponentRendered(fakeComponent);

            service.UseState(0, fakeComponent);
            service.UseState("Foo", fakeComponent);
            service.Invoking(s => s.UseState(99, fakeComponent))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void RemoveStateIfComponentDisposed()
        {
            var fakeComponent = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            var (state, setState) = service.UseState(0, fakeComponent);
            service.ComponentRendered(fakeComponent);
            setState(478);

            service.ComponentDisposed(fakeComponent);

            var (stateNew, setStateNew) = service.UseState(0, fakeComponent);

            stateNew.Should().Be(0);
        }

        [Test]
        public void HandleStateForMultipleComponents()
        {
            var fakeComponent1 = Moq.Mock.Of<IComponent>();
            var fakeComponent2 = Moq.Mock.Of<IComponent>();
            var service = new HooksService();

            var (_, setAge) = service.UseState(12, fakeComponent1);
            var (_, setHeight) = service.UseState(197, fakeComponent2);
            var (_, setSubscribe) = service.UseState(true, fakeComponent1);
            var (_, setIsAdmin) = service.UseState(false, fakeComponent2);

            service.ComponentRendered(fakeComponent1);
            service.ComponentRendered(fakeComponent2);

            setAge(21);
            setHeight(175);

            var (age, _) = service.UseState(12, fakeComponent1);
            var (subscribe, _) = service.UseState(true, fakeComponent1);

            var (height, _) = service.UseState(197, fakeComponent2);
            var (isAdmin, _) = service.UseState(false, fakeComponent2);

            age.Should().Be(21);
            subscribe.Should().BeTrue();
            height.Should().Be(175);
            isAdmin.Should().BeFalse();
        }
    }
}
