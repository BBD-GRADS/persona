namespace PersonaBackend.Models.examples
{
    using PersonaBackend.Models.Persona;
    using PersonaBackend.Models.Responses;
    using Swashbuckle.AspNetCore.Filters;

    public class ApiResponsePersonaIdListExample : IExamplesProvider<ApiResponse<PersonaIdList>>
    {
        public ApiResponse<PersonaIdList> GetExamples()
        {
            return new ApiResponse<PersonaIdList>
            {
                Data = new PersonaIdList
                {
                    PersonaIds = [1, 2, 3, 7, 8]
                },
                Message = "Successfully retrieved blah blah blah",
                Success = true
            };
        }
    }

    public class ApiResponsePersonaIdListEmptyExample : IExamplesProvider<ApiResponse<PersonaIdList>>
    {
        public ApiResponse<PersonaIdList> GetExamples()
        {
            return new ApiResponse<PersonaIdList>
            {
                Data = new PersonaIdList
                {
                    PersonaIds = new List<long>()
                },
                Message = "No records found",
                Success = false
            };
        }
    }

    public class ApiResponseMarriedPairExample : IExamplesProvider<ApiResponse<PersonaPairs>>
    {
        public ApiResponse<PersonaPairs> GetExamples()
        {
            return new ApiResponse<PersonaPairs>
            {
                Data = new PersonaPairs
                {
                    Pairs = [[1, 2], [3, 7], [4, 8]]
                },
                Message = "They be married",
                Success = true
            };
        }
    }

    public class ApiResponseChildPairExample : IExamplesProvider<ApiResponse<ParentChildList>>
    {
        public ApiResponse<ParentChildList> GetExamples()
        {
            return new ApiResponse<ParentChildList>
            {
                Data = new ParentChildList
                {
                    Parents = [1, 4, 8],
                    Children = new Dictionary<long, List<long>>
                            {
                        { 1, [22, 32] },
                        {4,[38] },
                        {8,[9] }
                            }
                },
                Message = "They had a good time",
                Success = true
            };
        }
    }

    public class ApiResponseStringExample : IExamplesProvider<ApiResponse<string>>
    {
        public ApiResponse<string> GetExamples()
        {
            return new ApiResponse<string>
            {
                Data = string.Empty,
                Message = "No records found",
                Success = false
            };
        }
    }

    public class ApiResponseBoolExample : IExamplesProvider<ApiResponse<bool>>
    {
        public ApiResponse<bool> GetExamples()
        {
            return new ApiResponse<bool>
            {
                Data = true,
                Message = "This call was successfull or something along those lines",
                Success = true
            };
        }
    }
}