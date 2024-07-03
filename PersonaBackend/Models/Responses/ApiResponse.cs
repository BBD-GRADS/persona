using PersonaBackend.Interfaces;

namespace PersonaBackend.Models.Responses
{
    public class ApiResponse<T> : IApiResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }

        public static implicit operator ApiResponse<T>(ApiResponse<List<Persona.Persona>> v)
        {
            throw new NotImplementedException();
        }
    }
}