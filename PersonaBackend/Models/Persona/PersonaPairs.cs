using Newtonsoft.Json;

namespace PersonaBackend.Models.Persona
{
    public class PersonaPairs
    {
        /// <example>
        /// <![CDATA[
        /// [
        ///     [1, 2],
        ///     [3, 7],
        ///     [4, 8]
        /// ]
        /// ]]>
        /// </example>

        public List<long[]> Pairs { get; set; } = new List<long[]>();
    }
}