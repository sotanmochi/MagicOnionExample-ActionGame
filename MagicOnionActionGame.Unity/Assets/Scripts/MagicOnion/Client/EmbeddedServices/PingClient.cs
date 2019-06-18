﻿using Grpc.Core;
using MagicOnion.Server.EmbeddedServices;
using MessagePack;

#if !NON_UNITY

namespace MagicOnion.Server.EmbeddedServices
{
    public interface IMagicOnionEmbeddedPing : IService<IMagicOnionEmbeddedPing>
    {
        UnaryResult<Nil> Ping();
    }
}

#endif

namespace MagicOnion.Client.EmbeddedServices
{
    [Ignore]
    public class PingClient : MagicOnionClientBase<IMagicOnionEmbeddedPing>, IMagicOnionEmbeddedPing
    {
        static readonly Method<byte[], byte[]> Method;

        static PingClient()
        {
            Method = new Method<byte[], byte[]>(MethodType.Unary, "IMagicOnionEmbeddedPing", "Ping", MagicOnionMarshallers.ThroughMarshaller, MagicOnionMarshallers.ThroughMarshaller);
        }

        PingClient()
        {
        }

        public PingClient(Channel channel)
            : this(new DefaultCallInvoker(channel))
        {

        }

        public PingClient(CallInvoker callInvoker)
            : base(callInvoker, null)
        {
        }

        protected override MagicOnionClientBase<IMagicOnionEmbeddedPing> Clone()
        {
            var clone = new PingClient(this.callInvoker);
            clone.host = this.host;
            clone.option = this.option;
            clone.callInvoker = this.callInvoker;
            clone.resolver = this.resolver;
            return clone;
        }

        public UnaryResult<Nil> Ping()
        {
            var __callResult = callInvoker.AsyncUnaryCall<byte[], byte[]>(Method, base.host, base.option, MagicOnionMarshallers.UnsafeNilBytes);
            return new UnaryResult<Nil>(__callResult, MessagePack.Resolvers.BuiltinResolver.Instance);
        }
    }
}