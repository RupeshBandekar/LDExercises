using EventStoreSaleExercise.Tests.Helper;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EventStoreSaleExercise.Tests
{
    using Xunit;
   
    public class ViewerTests
    {
        [Fact]
        public void can_return_welcome_inventory_message()
        {
            IViewer viewer = new MockViewer();
            var message = EventStoreSaleExercise.Program.PerformAction("2", viewer);
            Assert.Equal("You have entered as a Inventory Manager", message);
        }
        [Fact]
        public void can_return_welcome_director_message()
        {
            IViewer viewer = new MockViewer();
            var message = EventStoreSaleExercise.Program.PerformAction("3", viewer);
            Assert.Equal("You have entered as a Director", message);
        }

        [Fact]
        public void can_write_on_console()
        {
        }
    }
}
