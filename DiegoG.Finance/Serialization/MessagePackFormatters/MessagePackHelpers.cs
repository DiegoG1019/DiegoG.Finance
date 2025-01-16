using MessagePack;
using MessagePack.Resolvers;

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public static class MessagePackHelpers
{
    public static MessagePackSerializerOptions WithFinanceFormatters(this MessagePackSerializerOptions options)
        => options.WithResolver(CompositeResolver.Create(FinanceResolver.Instance, options.Resolver));
}
