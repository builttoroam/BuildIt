// <copyright file="LogHelperTest.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>

using BuildIt.Logging;
using BuildIt.Logging.Filters;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
            // ReSharper disable once ExplicitCallerInfoArgument
            ex.LogError(message, caller: caller);
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
            entity.LogEntity(caller);
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
                    "Test".LogMessage();
                    "Test2".LogMessage();
                }
            );
        }

        private class SimpleEntityToLog
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local - Used by reflection
            public int Item1 { get; set; }

            public string Item2 { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

        [TestMethod]
        public void SimpleEntityLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing",
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
                Item2 = "Testing",
            };

            BaseLogFilterTest(
                () => new EntityTypeLogFilter<SimpleEntityToLog>(),
                1,
                () =>
                {
                    "Test".LogMessage();
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
                Item2 = "Testing",
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
                    "Test".LogMessage();
                }
            );
        }

        [TestMethod]
        public void MessageContainsLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing",
            };

            // Case insensitive filter, lowercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "es" },
                2,
                () =>
                {
                    "Not Valid".LogMessage();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "Test".LogMessage();
                    entity.LogEntity("Test");
                }
            );

            // Case insensitive filter, uppercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "ES" },
                2,
                () =>
                {
                    "Not Valid".LogMessage();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "Test".LogMessage();
                    entity.LogEntity("TESt");
                }
            );

            // Case insensitive filter, mix
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "Es" },
                2,
                () =>
                {
                    "Not Valid".LogMessage();
                    entity.LogEntity("Not Valid");
                },
                () =>
                {
                    "Test".LogMessage();
                    entity.LogEntity("Test");
                }
            );

            // Case sensitive filter, lowercase
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "es", MessageContainsCaseSensitive = true },
                1,
                () =>
                {
                    "Not Valid".LogMessage();
                    entity.LogEntity("Not Valid");
                    "TESt".LogMessage();
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
                    "Not Valid".LogMessage();
                    entity.LogEntity("Valid Not");
                    entity.LogEntity("Test");
                },
                () =>
                {
                    "TESt".LogMessage();
                }
            );

            // Case sensitive filter, mix
            BaseLogFilterTest(
                () => new MessageContainsLogFilter { MessageContains = "Es", MessageContainsCaseSensitive = true },
                1,
                () =>
                {
                    "Not Valid".LogMessage();
                    entity.LogEntity("Not Valid");
                    entity.LogEntity("TeSt");
                },
                () =>
                {
                    "TEst".LogMessage();
                }
            );
        }

        [TestMethod]
        public void AndFilterLoggingTest()
        {
            var entity = new SimpleEntityToLog
            {
                Item1 = 5,
                Item2 = "Testing",
            };

            BaseLogFilterTest(
                () => new AndLogFilter(new EntityTypeLogFilter<SimpleEntityToLog>(), new MessageContainsLogFilter { MessageContains = "Test" }),
                1,
                () =>
                {
                    "Test".LogMessage();
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
                        "Not Valid".LogMessage();
                    },
                () =>
                    {
                        "Test".LogMessage();
                        var entity = new SimpleEntityToLog
                        {
                            Item1 = 5,
                            Item2 = "Testing",
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
                Filter = filterCreator(),
            };

            logInvalidItems?.Invoke();
            logItems?.Invoke();
            if (!reset.WaitOne(500000))  // There's no way to flush the log writer, so just wait for a bit for it to clear
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
                    "Test".LogMessage();
                },
                () =>
                {
                    "Test".LogMessage(assembly: GetType().Assembly);
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
                    "Test".LogMessage(level: LogLevel.None);
                    "Test".LogMessage(level: LogLevel.Verbose);
                },
                () =>
                {
                    "Test".LogMessage();
                    "Test".LogMessage(level: LogLevel.Error);
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
                    "Test".LogMessage();
                    "Test".LogMessage(new[] { "invalid" });
                    "Test".LogMessage(new[] { "Cat1" });
                    "Test".LogMessage(new[] { "Cat2" });
                },
                () =>
                {
                    "Test".LogMessage(new[] { "Cat1", "Cat2" });
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
                    "Test".LogMessage();
                    "Test".LogMessage(metadata: new Dictionary<string, string>());
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K1", "v1" } });
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K1", null }, { "K2", null } });
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K1", null }, { "K2", "xx" } });
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K2", null } });
                },
                () =>
                {
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K1", "v1" }, { "K2", "xx" } });
                    "Test".LogMessage(metadata: new Dictionary<string, string> { { "K1", "v1" }, { "K2", null } });
                }
            );
        }
    }
}