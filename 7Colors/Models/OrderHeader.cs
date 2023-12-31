﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _7Colors.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserNameIdentifier{ get; set; }
        [ForeignKey("UserNameIdentifier")]
        public virtual User? User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime ShippingDate { get; set; }

        public float OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        public virtual IEnumerable<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
    }
}
