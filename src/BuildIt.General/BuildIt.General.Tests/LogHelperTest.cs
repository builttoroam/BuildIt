// <copyright file="LogHelperTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        public void SimpleLoggingTest()
        {
            BaseLogFilterTest(
                () => null,
                2,
                () =>
                {
                },
                () =>
                {
                    "Test".Log();
                    "Test2".Log();
                }
            );
        }

        private class SimpleEntityToLog
        {
            public int Item1 { get; set; }
            public string Item2 { get; set; }
        }

        [TestMethod]
        public void SimpleEntityLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };

            BaseLogFilterTest(
                () => null,
                1,
                () =>
                {
                },
                () =>
                {
                    entity.LogEntity();
                }
            );
        }

        [TestMethod]
        public void TypeFilterLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };

            BaseLogFilterTest(
                () => new EntityTypeLogFilter<SimpleEntityToLog>(),
                1,
                () =>
                {
                    "Test".Log();
                },
                () =>
                {
                    entity.LogEntity();
                }
            );
        }

        [TestMethod]
        public void NotFilterLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };

            BaseLogFilterTest(
                () => new NotLogFilter { FilterToInvert = new EntityTypeLogFilter<SimpleEntityToLog>() },
                1,
                () =>
                {
                    entity.LogEntity();
                },
                () =>
                {
                    "Test".Log();
                }
            );
        }

        [TestMethod]
        public void MessageContainsLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };

            // Case insensitive filter, lowercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "es" },
                2,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "TESt".Log();
                    entity.LogEntity("Test");
                }
            );

            // Case insensitive filter, uppercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "ES" },
                2,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "Test".Log();
                    entity.LogEntity("TESt");
                }
            );

            // Case insensitive filter, mix
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "Es" },
                2,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "TESt".Log();
                    entity.LogEntity("Test");
                }
            );

            // Case sensitive filter, lowercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "es", MessageContainsCaseSensitive = true },
                1,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Not Valid");
                    "TESt".Log();
                },
                () =>
                {
                    entity.LogEntity("Test");
                }
            );

            // Case sensitive filter, uppercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "ES", MessageContainsCaseSensitive = true },
                1,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Valid Not");
                    entity.LogEntity("Test");
                },
                () =>
                {
                    "TESt".Log();
                }
            );

            // Case sensitive filter, mix
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "Es", MessageContainsCaseSensitive = true },
                1,
                () =>
                {
                    "Not Valid".Log();
                    entity.LogEntity("Not Valid");
                    entity.LogEntity("TeSt");
                },
                () =>
                {
                    "TEst".Log();
                }
            );
        }

        [TestMethod]
        public void AndFilterLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing"
            };

            BaseLogFilterTest(
                () => new AndLogFilter(new EntityTypeLogFilter<SimpleEntityToLog>(), new MessageContainsLogFilter { MessageContains = "Test" }),
                1,
                () =>
                {
                    "Test".Log();
                    entity.LogEntity();
                },
                () =>
                {
                    entity.LogEntity("Test");
                }
            );
        }

        [TestMethod]
        public void OrFilterLoggingTest()
        {
            BaseLogFilterTest(
                () => new OrLogFilter(new EntityTypeLogFilter<SimpleEntityToLog>(), new MessageContainsLogFilter { MessageContains = "Test" }),
                3,
                () =>
                    {
                        "Not Valid".Log();
                    },
                () =>
                    {
                        "Test".Log();
                        var entity = new SimpleEntityToLog
                        {
                            Item1 = 5,
                            Item2 = "Testing"
                        };
                        entity.LogEntity();
                        entity.LogEntity("Test");
                    }
                );
        }

        public void BaseLogFilterTest(
            Func<ILogFilter> filterCreator,
            int expectedNumberToLog,
            Action logInvalidItems,
            Action logItems,
            Action<ILogEntry> logItemValidator = null
            )
        {
            var count = 0;
            var reset = new ManualResetEvent(false);
            var target = expectedNumberToLog;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    Assert.IsNotNull(entry);
                    logItemValidator?.Invoke(entry);
                    if (count >= target)
                    {
                        reset.Set();
                    }
                    return Task.CompletedTask;
                },
                Filter = filterCreator()
            };

            logInvalidItems?.Invoke();
            logItems?.Invoke();
            if (!reset.WaitOne(5000))  // There's no way to flush the log writer, so just wait for a bit for it to clear
            {
                Assert.Fail("Timeout exceeded");
            }
            Assert.AreEqual(target, count);
        }

        [TestMethod]
        public void AssemblyNameLoggingTest()
        {
            BaseLogFilterTest(
                () => new AssemblyNameLogFilter { AssemblyName = "BuildIt.General.Tests" },
                1,
                () =>
                {
                    "Test".Log();
                },
                () =>
                {
                    "Test".Log(assembly: GetType().Assembly);
                }
            );
        }

        [TestMethod]
        public void MinimumLogLevelLoggingTest()
        {
            BaseLogFilterTest(
                () => new MinimumLogLevelLogFilter { MinimumLogLevel = LogLevel.Information },
                2,
                () =>
                {
                    "Test".Log(level: LogLevel.None);
                    "Test".Log(level: LogLevel.Verbose);
                },
                () =>
                {
                    "Test".Log();
                    "Test".Log(level: LogLevel.Error);
                }
            );
        }

        [TestMethod]
        public void RequiredCategoriesLoggingTest()
        {
            BaseLogFilterTest(
                () => new RequiredCategoriesLogFilter { RequiredCategories = new[] { "Cat2", "Cat1" } },
                1,
                () =>
                {
                    "Test".Log();
                    "Test".Log(new[] { "invalid" });
                    "Test".Log(new[] { "Cat1" });
                    "Test".Log(new[] { "Cat2" });
                },
                () =>
                {
                    "Test".Log(new[] { "Cat1", "Cat2" });
                }
            );
        }

        [TestMethod]
        public void RequiredMetadataLoggingTest()
        {
            BaseLogFilterTest(
                () => new RequiredMetadataLogFilter { RequiredMetadata = new Dictionary<string, string> { { "K1", "v1" }, { "K2", null } } },
                2,
                () =>
                {
                    "Test".Log();
                    "Test".Log(metadata: new Dictionary<string, string>());
                    "Test".Log(metadata: new Dictionary<string, string> { { "K1", "v1" } });
                    "Test".Log(metadata: new Dictionary<string, string> { { "K1", null }, { "K2", null } });
                    "Test".Log(metadata: new Dictionary<string, string> { { "K1", null }, { "K2", "xx" } });
                    "Test".Log(metadata: new Dictionary<string, string> { { "K2", null } });
                },
                () =>
                {
                    "Test".Log(metadata: new Dictionary<string, string> { { "K1", "v1" }, { "K2", "xx" } });
                    "Test".Log(metadata: new Dictionary<string, string> { { "K1", "v1" }, { "K2", null } });
                }
            );
        }
    }
}
