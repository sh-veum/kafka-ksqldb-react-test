namespace KafkaAuction.Utilities;

public static class TypeMapper
{
    public static string GetKSqlType(Type type)
    {
        // Check if the type is nullable and get the underlying type
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type switch
        {
            var t when t == typeof(int) => "INT",
            var t when t == typeof(short) => "INT",  // Map Int16 to INT
            var t when t == typeof(long) => "BIGINT",
            var t when t == typeof(string) => "VARCHAR",
            var t when t == typeof(decimal) => "DECIMAL(18,2)",
            var t when t == typeof(bool) => "BOOLEAN",
            var t when t == typeof(double) => "DOUBLE",
            var t when t == typeof(float) => "DOUBLE",  // Map float to DOUBLE
            var t when t == typeof(byte) => "INT",  // Map byte to INT
            var t when t == typeof(uint) => "BIGINT",  // Map unsigned int to BIGINT
            var t when t == typeof(ushort) => "BIGINT",  // Map unsigned short to BIGINT
            var t when t == typeof(ulong) => "BIGINT",  // Map unsigned long to BIGINT
            var t when t == typeof(char) => "VARCHAR",  // Map char to VARCHAR
            var t when t == typeof(DateTime) => "VARCHAR",
            _ => throw new NotSupportedException($"Type {type.Name} is not supported.")
        };
    }
}