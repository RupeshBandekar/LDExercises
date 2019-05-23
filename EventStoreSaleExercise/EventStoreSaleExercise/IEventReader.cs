namespace EventStoreSaleExercise
{
    public interface IEventReader
    {
        void SubscribeEventStream(long start);
    }
}
