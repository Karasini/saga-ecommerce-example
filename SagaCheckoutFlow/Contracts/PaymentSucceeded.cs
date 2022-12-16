using System;

namespace Contracts;

public record PaymentSucceeded(int OrderId, DateTime PaymentDate);