namespace EventStoreSaleExercise.Tests
{
    using Xunit;
    using EventStoreSaleExercise.Tests.Helper;

    public class ActionServiceTests
    {
        [Fact]
        public void can_return_welcome_salesman_message()
        {
            IViewer viewer = new MockViewer();
            var message = ActionService.PerformAction("1", viewer);
            Assert.Equal("You have entered as a Salesman", message);
        }

        [Fact]
        public void can_return_welcome_inventory_message()
        {
            IViewer viewer = new MockViewer();
            var message = ActionService.PerformAction("2", viewer);
            Assert.Equal("You have entered as a Inventory Manager", message);
        }
        [Fact]
        public void can_return_welcome_director_message()
        {
            IViewer viewer = new MockViewer();
            var message = ActionService.PerformAction("3", viewer);
            Assert.Equal("You have entered as a Director", message);
        }

        [Fact]
        public void can_return_same_message_written_on_console()
        {
            var retMessage = Viewer.ConsoleWrite("test message");
            Assert.Equal("test message", retMessage);
        }
    }
}
