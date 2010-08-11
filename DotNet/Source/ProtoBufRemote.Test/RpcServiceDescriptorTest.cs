using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class RpcServiceDescriptorTest
    {
        interface ISimpleService
        {
            int DoStuff(int x, string y, MultiplyInput multiplyInput);
        }

        [Test]
        public void ServiceNameTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(ISimpleService));
            Assert.That(descriptor.Name, Is.EqualTo("ISimpleService"));
        }

        [RpcService(Name="OverriddenName")]
        interface ISimpleServiceNameOverride
        {
            int DoStuff(int x);
        }

        [Test]
        public void ServiceNameOverrideTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(ISimpleServiceNameOverride));
            Assert.That(descriptor.Name, Is.EqualTo("OverriddenName"));
        }

        interface ISimpleService2 : ISimpleService
        {
            int DoStuff2(int x);
        }

        [Test]
        public void ServiceNameDerivedTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(ISimpleService2));
            Assert.That(descriptor.Name, Is.EqualTo("ISimpleService2"));
            Assert.That(descriptor.Methods.Count(), Is.EqualTo(2));
        }

        [RpcService]
        interface ISimpleServiceWithAttr
        {
            int DoStuff(int x);
        }
        interface ISimpleServiceWithAttr2 : ISimpleServiceWithAttr
        {
            int DoStuff2(int x);
        }

        [Test]
        public void ServiceNameBaseAttrTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(ISimpleServiceWithAttr2));
            Assert.That(descriptor.Name, Is.EqualTo("ISimpleServiceWithAttr"));

            //the type with the attribute defines the service, methods from the derived type should be ignored
            Assert.That(descriptor.Methods.Count(), Is.EqualTo(1));
            Assert.That(descriptor.Methods.First().Name, Is.EqualTo("DoStuff"));
        }

        interface IEmptyService { }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void EmptyServiceTest()
        {
            new RpcServiceDescriptor(typeof(IEmptyService));
        }

        [RpcService(IsAttributedMethodsOnly = true)]
        interface IAttrMethodsOnlyService
        {
            [RpcMethod]
            int DoStuff(int x);

            int DoStuff2(int x);
        }

        [Test]
        public void AttrMethodsOnlyTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(IAttrMethodsOnlyService));
            Assert.That(descriptor.Methods.Count(), Is.EqualTo(1));
        }

        class BadServiceWithoutAttributes
        {
            public int DoStuff(int x) { return x + 1; }
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadServiceWithoutAttributesTest()
        {
            //class types must be attributed when used as a service
            new RpcServiceDescriptor(typeof(BadServiceWithoutAttributes));
        }

        [RpcService]
        class BadServiceWithoutAttributesOnlyFlag
        {
            public int DoStuff(int x) { return x + 1; }
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadServiceWithoutAttributesOnlyFlagTest()
        {
            //class types must have all methods explicitly marked when used as a service
            new RpcServiceDescriptor(typeof(BadServiceWithoutAttributesOnlyFlag));
        }

        interface IBadAsyncService
        {
            IAsyncResult BeginStuff(int x, AsyncCallback callback, object state);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadAsyncServiceTest()
        {
            new RpcServiceDescriptor(typeof(IBadAsyncService));
        }

        [Test]
        public void SimpleMethodTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(ISimpleService));
            Assert.That(descriptor.Methods.Count(), Is.EqualTo(1));

            RpcMethodDescriptor methodDescriptor = descriptor.Methods.First();
            Assert.That(methodDescriptor.Name, Is.EqualTo("DoStuff"));
            Assert.That(methodDescriptor.SyncCallMethod, Is.EqualTo(typeof(ISimpleService).GetMethod("DoStuff")));
            Assert.That(methodDescriptor.HasAsyncDeclarations, Is.False);
            Assert.That(methodDescriptor.ParameterTypes,
                Is.EquivalentTo(new[] { typeof(int), typeof(string), typeof(MultiplyInput) }));
            Assert.That(methodDescriptor.ReturnType, Is.EqualTo(typeof(int)));
        }

        public class NotAProtoClass { }
        interface IBadParameterService
        {
            void DoStuff(int x, NotAProtoClass badParameter);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadParameterTest()
        {
            new RpcServiceDescriptor(typeof(IBadParameterService));
        }

        interface IBadReturnService
        {
            NotAProtoClass DoStuff(int x);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadReturnTest()
        {
            new RpcServiceDescriptor(typeof(IBadReturnService));
        }

        interface IMethodNameOverrideService
        {
            [RpcMethod(Name = "DoOtherStuff")]
            int DoStuff(int x);
        }

        [Test]
        public void MethodNameOverrideTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(IMethodNameOverrideService));
            RpcMethodDescriptor methodDescriptor = descriptor.Methods.First();
            Assert.That(methodDescriptor.Name, Is.EqualTo("DoOtherStuff"));
            Assert.That(methodDescriptor.SyncCallMethod,
                Is.EqualTo(typeof(IMethodNameOverrideService).GetMethod("DoStuff")));
        }

        interface IAsyncService
        {
            int DoStuff(int x, string y);
            IAsyncResult BeginDoStuff(int x, string y, AsyncCallback callback, object state);
            int EndDoStuff(IAsyncResult asyncResult);
        }

        [Test]
        public void AsyncServiceTest()
        {
            var descriptor = new RpcServiceDescriptor(typeof(IAsyncService));
            Assert.That(descriptor.Methods.Count(), Is.EqualTo(1));

            RpcMethodDescriptor methodDescriptor = descriptor.Methods.First();
            Assert.That(methodDescriptor.HasAsyncDeclarations, Is.True);
            Assert.That(methodDescriptor.Name, Is.EqualTo("DoStuff"));
            Assert.That(methodDescriptor.SyncCallMethod, Is.EqualTo(typeof(IAsyncService).GetMethod("DoStuff")));
            Assert.That(methodDescriptor.AsyncBeginCallMethod,
                Is.EqualTo(typeof(IAsyncService).GetMethod("BeginDoStuff")));
            Assert.That(methodDescriptor.AsyncEndCallMethod,
                Is.EqualTo(typeof(IAsyncService).GetMethod("EndDoStuff")));
        }

        interface IAsyncBadReturnService
        {
            int DoStuff(int x, string y);
            IAsyncResult BeginDoStuff(int x, string y, AsyncCallback callback, object state);
            string EndDoStuff(IAsyncResult asyncResult);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AsyncBadReturnTest()
        {
            new RpcServiceDescriptor(typeof(IAsyncBadReturnService));
        }

        interface IAsyncBadParameterService
        {
            int DoStuff(int x, int y);
            IAsyncResult BeginDoStuff(int x, string y, AsyncCallback callback, object state);
            int EndDoStuff(IAsyncResult asyncResult);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void AsyncBadParameterTest()
        {
            new RpcServiceDescriptor(typeof(IAsyncBadParameterService));
        }

        interface IBadMultipleSyncService
        {
            int DoStuff(int x, string y);
            [RpcMethod(Name = "DoStuff")]
            int DoStuff2(int x, string y);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadMultipleSyncTest()
        {
            new RpcServiceDescriptor(typeof(IBadMultipleSyncService));
        }

        interface IBadMultipleBeginAsyncService
        {
            IAsyncResult BeginDoStuff(int x, string y, AsyncCallback callback, object state);
            int EndDoStuff(IAsyncResult asyncResult);
            [RpcMethod(Name = "DoStuff")]
            IAsyncResult BeginDoStuff2(int x, string y, AsyncCallback callback, object state);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadMultipleBeginAsyncTest()
        {
            new RpcServiceDescriptor(typeof(IBadMultipleBeginAsyncService));
        }

        interface IBadMultipleEndAsyncService
        {
            IAsyncResult BeginDoStuff(int x, string y, AsyncCallback callback, object state);
            int EndDoStuff(IAsyncResult asyncResult);
            [RpcMethod(Name = "DoStuff")]
            int EndDoStuff2(IAsyncResult asyncResult);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadMultipleEndAsyncTest()
        {
            new RpcServiceDescriptor(typeof(IBadMultipleEndAsyncService));
        }
    }
}
