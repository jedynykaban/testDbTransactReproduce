﻿using System.Linq;
using DbTransactProblem.Implementation;
using DbTransactProblem.Interfaces;
using LightInject;

namespace DbTransactProblem
{
    class Program
    {
        static void Main()
        {
            var container = RegisterDi();

            container
                .GetAllInstances<IHttpHandler>()
                .ToList()
                .ForEach(h => h.Register());
        }

        private static ServiceContainer RegisterDi()
        {
            var container = new ServiceContainer();

            container.Register<IHttpHandler, TestHttpHandler>(nameof(TestHttpHandler));
            container.Register<ITestRepository, TestRepository>();
            container.Register<IDbSelectionUtil, ScDbSelectionUtil>(new PerContainerLifetime());
            container.Register<IDbDataManipulationUtil, ScDbDataManipulationUtil>(new PerContainerLifetime());
            var blobReaderWriter = new BlobReaderWriter();
            container.Register<IBlobReader>(factory => blobReaderWriter, new PerContainerLifetime());
            container.Register<IBlobWriter>(factory => blobReaderWriter, new PerContainerLifetime());
            container.Register<IBlobReaderWriter>(factory => blobReaderWriter, new PerContainerLifetime());

            return container;
        }
    }
}