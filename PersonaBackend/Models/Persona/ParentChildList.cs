namespace PersonaBackend.Models.Persona
{
    public class ParentChildList
    {
        public List<long> Parents { get; set; }
        public Dictionary<long, List<long>> Children { get; set; }
    }
}