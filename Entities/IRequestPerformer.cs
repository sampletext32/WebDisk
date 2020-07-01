namespace Entities
{
    public interface IRequestPerformer
    {
        byte[] PerformRequest(byte[] data);
    }
}
