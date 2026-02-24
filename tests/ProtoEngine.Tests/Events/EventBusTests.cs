using ProtoEngine.Events;

namespace ProtoEngine.Tests.Events;

public class EventBusTests
{
    private record TestEvent(string Message) : IGameEvent;
    private record OtherEvent(int Value) : IGameEvent;

    [Fact]
    public void Publish_WithSubscriber_InvokesHandler()
    {
        var bus = new EventBus();
        TestEvent? captured = null;
        bus.Subscribe<TestEvent>(e => captured = e);

        bus.Publish(new TestEvent("hello"));

        Assert.NotNull(captured);
        Assert.Equal("hello", captured.Message);
    }

    [Fact]
    public void Publish_NoSubscribers_DoesNotThrow()
    {
        var bus = new EventBus();

        bus.Publish(new TestEvent("hello"));
    }

    [Fact]
    public void Publish_MultipleSubscribers_InvokesAll()
    {
        var bus = new EventBus();
        var count = 0;
        bus.Subscribe<TestEvent>(_ => count++);
        bus.Subscribe<TestEvent>(_ => count++);

        bus.Publish(new TestEvent("hello"));

        Assert.Equal(2, count);
    }

    [Fact]
    public void Publish_DifferentEventType_DoesNotInvokeWrongHandler()
    {
        var bus = new EventBus();
        var called = false;
        bus.Subscribe<OtherEvent>(_ => called = true);

        bus.Publish(new TestEvent("hello"));

        Assert.False(called);
    }
}
