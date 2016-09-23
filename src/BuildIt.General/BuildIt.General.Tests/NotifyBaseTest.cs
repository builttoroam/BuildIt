using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.Tests
{
    [TestClass]
    [PexClass(typeof (NotifyBase))]
    [PexAllowedExceptionFromTypeUnderTest(typeof (ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof (InvalidOperationException))]
    public partial class NotifyBaseTest
    {

        [PexMethod]
        public void ExpressionTest(int val)
        {
            var original = val;
            var target = new TestNotifyClass();
            target.PropertyChanged += (s,e) => val++;
            target.ExpressionVariable = val;
            Assert.AreEqual(target.ExpressionVariable+(original == 0 ?0:1),val);
        }


        [PexMethod]
        public void InvalidExpressionTest(int val)
        {
            var original = val;
            var target = new TestNotifyClass();
            target.InvalidExpressionVariable = val;
            Assert.AreEqual(target.InvalidExpressionVariable , val);
        }

        [PexMethod]
        public void ExpressionNoHandlerTest(int val)
        {
            var original = val;
            var target = new TestNotifyClass();
            target.ExpressionVariable = val;
            Assert.AreEqual(target.ExpressionVariable, val);
        }

        [PexMethod]
        public void CallerMemberTest(string val)
        {
            var original = val;
            var target = new TestNotifyClass();
            target.PropertyChanged += (s, e) => val += "test";
            target.CallermemberVariable = val;
            Assert.AreEqual(target.CallermemberVariable+(original==null?null: "test"), val+null);
        }



        [PexMethod]
        public void CallerMemberNoHandlerTest(string val)
        {
            var original = val;
            var target = new TestNotifyClass();
            target.CallermemberVariable = val;
            Assert.AreEqual(target.CallermemberVariable , val );
        }
    }

    public class TestNotifyClass:NotifyBase
    {
        private int expressionVariable;

        public int ExpressionVariable
        {
            get { return expressionVariable; }
            set
            {
                if (ExpressionVariable == value) return;
                expressionVariable = value;
                OnPropertyChanged(() => ExpressionVariable);
            }
        }

        public int InvalidExpressionVariable
        {
            get { return expressionVariable; }
            set
            {
                if (InvalidExpressionVariable == value) return;
                expressionVariable = value;
                OnPropertyChanged(() => InvalidExpressionVariable+ExpressionVariable);
            }
        }

        private string _CallermemberVariable;

        public string CallermemberVariable
        {
            get { return _CallermemberVariable; }
            set
            {
                if (CallermemberVariable == value) return;
                _CallermemberVariable = value;
                OnPropertyChanged();
            }
        }


    }
}
