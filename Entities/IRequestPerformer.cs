namespace Entities
{
    // интерфейс выполнитель запросов
    public interface IRequestPerformer
    {
        // выполнение запроса - ответ в байтах на запрос в байтах
        byte[] PerformRequest(byte[] data);
    }
}
