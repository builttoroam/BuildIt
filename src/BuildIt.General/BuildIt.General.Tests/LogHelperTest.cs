// <copyright file="LogHelperTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.Logging;
using BuildIt.Logging.Filters;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof(LogHelper))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class LogHelperTest
    {

        [PexMethod]
        public void LogException(
            Exception ex,
            string message,
            string caller
        )
        {
            LogHelper.LogException(ex, message, caller: caller);
            // TODO: add assertions to method LogHelperTest.LogException(Exception, String, String)
        }

        public class TestPersonEntity
        {
            public string Name { get; set; }
        }


        [PexGenericArguments(typeof(int))]
        [PexGenericArguments(typeof(TestPersonEntity))]
        [PexMethod]
        public void Log<TEntity>(TEntity entity, string caller)
        {
            LogHelper.LogEntity<TEntity>(entity, caller);
            // TODO: add assertions to method LogHelperTest.Log(!!0, String)
        }

        [TestMethod]
        public void CustomLogServiceTest()
        {
            var service = LogHelper.LogService;
            var newService = new TestLogService();
            LogHelper.LogService = newService;
            var service2 = LogHelper.LogService;
            Assert.IsNotNull(service2);
            Assert.AreNotEqual(service, service2);
            Assert.AreEqual(newService, service2);
        }

        public class TestLogService : ILoggerService
        {
            public ILogFilter Filter { get; set; }
            public Task Log(ILogEntry logItem)
            {
                Debug.Write(logItem);
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public async Task SimpleLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    Debug.WriteLine(entry);
                    return Task.CompletedTask;
                }
            };
            "Test".Log();
            "Test2".Log();
            await Task.Delay(50); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(2, count);
        }

        private class SimpleEntityToLog
        {
            public int Item1 { get; set; }
            public string Item2 { get; set; }
        }

        [TestMethod]
        public async Task SimpleEntityLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    return Task.CompletedTask;
                }
            };

            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            entity.LogEntity();
            await Task.Delay(50); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task TypeFilterLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    var tentry = entry as ITypedLogEntry<SimpleEntityToLog>;
                    Assert.IsNotNull(tentry);
                    Assert.IsNotNull(tentry.Entity);
                    return Task.CompletedTask;
                },
                Filter = new EntityTypeLogFilter<SimpleEntityToLog>()
            };

            "Test".Log();

            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            entity.LogEntity();
            await Task.Delay(50); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(1, count);
        }


        [TestMethod]
        public async Task NotFilterLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    return Task.CompletedTask;
                },
                Filter = new NotLogFilter{ FilterToInvert = new EntityTypeLogFilter<SimpleEntityToLog>() }
            };

            "Test".Log();

            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            entity.LogEntity();
            await Task.Delay(50); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task MessageContainsLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    return Task.CompletedTask;
                },
                Filter = new MessageContainsLogFilter { MessageContains = "es"}
            };
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            var filter = LogHelper.LogService.Filter as MessageContainsLogFilter;

            // Case insensitive filter, lowercase
            "TESt".Log();
            entity.LogEntity("Test");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(2, count);

            // Case insensitive filter, uppercase
            filter.MessageContains = "ES";
            "TESt".Log();
            entity.LogEntity("Test");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(4, count);

            // Case insensitive filter, mix
            filter.MessageContains = "Es";
            "TESt".Log();
            entity.LogEntity("Test");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(6, count);

            // filter.MessageContainsCaseSensitive = true; // NB: This doesn't work, since the filter function has already been created
            filter = new MessageContainsLogFilter {MessageContains = "es", MessageContainsCaseSensitive = true};
            LogHelper.LogService.Filter = filter;

            // Case sensitive filter, lowercase
            filter.MessageContains = "es";
            "TESt".Log();
            entity.LogEntity("Test");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(7, count);

            // Case sensitive filter, uppercase
            filter.MessageContains = "ES";
            "TESt".Log();
            entity.LogEntity("Test");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(8, count);

            // Case sensitive filter, mix
            filter.MessageContains = "Es";
            "TEst".Log();
            entity.LogEntity("TeSt");
            "Not Valid".Log();
            entity.LogEntity("Not Valid");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(9, count);

        }


        [TestMethod]
        public async Task AndFilterLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    return Task.CompletedTask;
                },
                Filter = new AndLogFilter(new EntityTypeLogFilter<SimpleEntityToLog>(), new MessageContainsLogFilter{MessageContains = "Test"}) 
            };

            "Test".Log();
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            entity.LogEntity();
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(0, count);

            entity.LogEntity("Test");
            await Task.Delay(500); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task OrFilterLoggingTest()
        {
            var count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    return Task.CompletedTask;
                },
                Filter = new OrLogFilter(new EntityTypeLogFilter<SimpleEntityToLog>(), new MessageContainsLogFilter { MessageContains = "Test" })
            };

            "Test".Log();
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };
            entity.LogEntity();
            entity.LogEntity("Test");
            "Not Valid".Log();
            await Task.Delay(50); // There's no way to flush the log writer, so just wait for a bit for it to clear
            Assert.AreEqual(3, count);
        }
    }
}
