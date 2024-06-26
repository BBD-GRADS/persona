namespace PersonaBackend.Interfaces
{
    public interface IApiResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}