﻿#if UNO
using Microsoft.Practices.ServiceLocation;
#else

using CommonServiceLocator;

#endif

using System;
using System.Collections.Generic;

namespace ServiceLocation.Tests.Mocks
{
    internal class MockServiceLocator : IServiceLocator
    {
        public object GetInstance(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}