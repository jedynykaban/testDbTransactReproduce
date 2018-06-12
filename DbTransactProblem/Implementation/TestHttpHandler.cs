using System;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class TestHttpHandler : IHttpHandler
    {
        private readonly ITestRepository _testRepository;

        public TestHttpHandler(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public void Register()
        {
            Handle.GET("/dbget", GetTest);
            Handle.PUT("/dbputdi", (Request request) => PutTest(request));
            Handle.PUT("/dbputdirect", (Request request) => PutTestDirect(request));
        }

        private Response GetTest() => new Response
        {
            Headers = {["Access-Control-Allow-Origin"] = "*", ["Content-Type"] = "application/json"},
            Body = _testRepository.GetFirst()?.ToJson(),
            StatusCode = (ushort)System.Net.HttpStatusCode.OK
        };

        private Response PutTest(Request request)
        {
            var typedJson = new TestViewModel();
            typedJson.PopulateFromJson(request.Body);

            try
            {
                return new Response
                {
                    Headers = { ["Access-Control-Allow-Origin"] = "*", ["Content-Type"] = "application/json" },
                    Body = _testRepository.AddOrUpdate(typedJson)?.ToJson(),
                    StatusCode = (ushort)System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Headers = { ["Access-Control-Allow-Origin"] = "*", ["Content-Type"] = "text/html" },
                    Body = ex.Message,
                    StatusCode = (ushort)System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        private Response PutTestDirect(Request request)
        {
            var typedJson = new TestViewModel();
            typedJson.PopulateFromJson(request.Body);
            
            return new Response
            {
                Headers = {["Access-Control-Allow-Origin"] = "*", ["Content-Type"] = "application/json"},
                Body = new TestRepository(new ScDbSelectionUtil(), null, new BlobReaderWriter())
                    .AddOrUpdateDbTransactExplicitlyCalled(typedJson)
                    ?.ToJson(),
                StatusCode = (ushort)System.Net.HttpStatusCode.OK
            };
        }
    }
}
