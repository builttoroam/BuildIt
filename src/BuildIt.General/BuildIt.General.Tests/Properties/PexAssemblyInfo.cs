// <copyright file="PexAssemblyInfo.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Suppression;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("BuildIt.General")]
[assembly: PexInstrumentAssembly("System.Reflection.Extensions")]
[assembly: PexInstrumentAssembly("System.Linq")]
[assembly: PexInstrumentAssembly("System.Xml.XDocument")]
[assembly: PexInstrumentAssembly("System.Collections")]
[assembly: PexInstrumentAssembly("System.Threading")]
[assembly: PexInstrumentAssembly("System.Text.Encoding")]
[assembly: PexInstrumentAssembly("Newtonsoft.Json")]
[assembly: PexInstrumentAssembly("Microsoft.Practices.ServiceLocation")]
[assembly: PexInstrumentAssembly("System.Diagnostics.Debug")]
[assembly: PexInstrumentAssembly("System.Linq.Expressions")]
[assembly: PexInstrumentAssembly("System.Runtime")]
[assembly: PexInstrumentAssembly("System.Resources.ResourceManager")]
[assembly: PexInstrumentAssembly("System.IO")]
[assembly: PexInstrumentAssembly("System.Runtime.Serialization.Json")]
[assembly: PexInstrumentAssembly("System.Reflection")]
[assembly: PexInstrumentAssembly("System.ObjectModel")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Reflection.Extensions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Linq")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Xml.XDocument")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Collections")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Threading")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Text.Encoding")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Newtonsoft.Json")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.Practices.ServiceLocation")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Diagnostics.Debug")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Linq.Expressions")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Resources.ResourceManager")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.IO")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Runtime.Serialization.Json")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Reflection")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.ObjectModel")]
[assembly: PexInstrumentType(typeof(Buffer))]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(Buffer))]
[assembly: PexSuppressUninstrumentedMethodFromType("System.Xml.XmlTextEncoder")]
[assembly: PexSuppressUninstrumentedMethodFromType("System.Runtime.Serialization.TypeHandleRefEqualityComparer")]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(ConfigurationElement))]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(ConfigurationElementCollection))]
[assembly: PexSuppressUninstrumentedMethodFromType("System.Configuration.Internal.ConfigurationManagerInternal")]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(Enum))]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(TextWriter))]
[assembly: PexSuppressUninstrumentedMethodFromType("System.Xml.XmlCharType")]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(UnicodeEncoding))]
[assembly: PexSuppressStaticFieldStore(typeof(StringWriter), "m_encoding")]
[assembly: PexUseType(typeof(TypeDelegator))]
[assembly: PexUseType(typeof(GC), "System.RuntimeType")]
[assembly: PexUseType(typeof(GC), "System.ReflectionOnlyType")]
[assembly: PexUseType(typeof(GC), "System.Reflection.Emit.SymbolType")]
[assembly: PexUseType(typeof(TypeBuilder))]
[assembly: PexUseType(typeof(GC), "System.Reflection.Emit.TypeBuilderInstantiation")]
[assembly: PexUseType(typeof(GenericTypeParameterBuilder))]
[assembly: PexUseType(typeof(EnumBuilder))]
[assembly: PexSuppressExplorableEvent("Microsoft.ExtendedReflection.Metadata.Impl.__ArrayHelper`1")]
[assembly: PexSuppressUninstrumentedMethodFromType("System.DateTimeParse")]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(DateTimeFormatInfo))]
[assembly: PexSuppressUninstrumentedMethodFromType(typeof(TextInfo))]
[assembly: PexSuppressStaticFieldStore("BuildIt.Tests.UtilitiesTest+<>c", "<>9__74_0")]
[assembly: PexSuppressStaticFieldStore(typeof(TraceSource), "s_LastCollectionCount")]
[assembly: PexSuppressStaticFieldStore(typeof(Switch), "s_LastCollectionCount")]
[assembly: PexSuppressStaticFieldStore("BuildIt.Utilities+<>c__32`2", "<>9__32_0")]
[assembly: PexSuppressStaticFieldStore("BuildIt.Utilities+<>c__32`2", "<>9__32_1")]
[assembly: PexSuppressExplorableEvent("System.Collections.Generic.Dictionary`2+Entry")]
[assembly: PexInstrumentType("mscorlib", "System.Globalization.CodePageDataItem")]