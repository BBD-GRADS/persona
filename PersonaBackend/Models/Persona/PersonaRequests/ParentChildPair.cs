using Newtonsoft.Json;

namespace PersonaBackend.Models.Persona.PersonaRequests
{
    public class ParentChildPair
    {
        public long ParentId { get; set; }
        public long ChildId { get; set; }
    }
}