using System;

namespace Orders.DTO;

public record OrderDto(int OrderId, string Status, int? DeliveryId, DateTime? PaymentDate, int ProductId,
    int PaymentRetries);