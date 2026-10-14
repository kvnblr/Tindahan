namespace Api.Entities.ExceptionModel;

public sealed class OffsetArgumentOutOfRangeException()
    : BadRequestException("The requested page exceeds the supported offset.")
{ }
