using Newtonsoft.Json;

namespace PersonaBackend.Models.Persona
{
    public class PersonaIdList
    {
        /// <example>[1, 2, 3, 7, 8]</example>

        public List<long> PersonaIds { get; set; } = new List<long>();
    }
}