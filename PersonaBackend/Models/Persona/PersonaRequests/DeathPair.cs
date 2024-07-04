namespace PersonaBackend.Models.Persona.PersonaRequests
{
    public class DeathPair
    {
        public long deceased { get; set; }
        public long? next_of_kin { get; set; }
    }
}