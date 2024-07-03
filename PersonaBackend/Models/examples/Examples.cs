using PersonaBackend.Models.Persona.PersonaRequests;
using PersonaBackend.Models.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace PersonaBackend.Models.examples
{
    public class ApiResponsePersonaIdListExample : IExamplesProvider<ApiResponse<PersonaIdList>>
    {
        public ApiResponse<PersonaIdList> GetExamples()
        {
            return new ApiResponse<PersonaIdList>
            {
                Data = new PersonaIdList
                {
                    PersonaIds = new List<long> { 1, 2, 3, 7, 8 }
                },
                Message = "Successfully retrieved persona IDs",
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

    public class ApiResponseMarriedPairExample : IExamplesProvider<ApiResponse<PersonaMarriageList>>
    {
        public ApiResponse<PersonaMarriageList> GetExamples()
        {
            return new ApiResponse<PersonaMarriageList>
            {
                Data = new PersonaMarriageList
                {
                    MarriagePairs = new List<PersonaMarriagePair>
                    {
                        new PersonaMarriagePair { FirstPerson = 1, SecondPerson = 2 },
                        new PersonaMarriagePair { FirstPerson = 3, SecondPerson = 7 },
                        new PersonaMarriagePair { FirstPerson = 4, SecondPerson = 8 }
                    }
                },
                Message = "Successfully married the personas",
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
                     patentChildIds = [1, 2, 3, 4, 5, 6, 7]
                },
                Message = "Successfully retrieved parent-child relationships",
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
                Message = "Operation completed successfully",
                Success = true
            };
        }
    }
}
