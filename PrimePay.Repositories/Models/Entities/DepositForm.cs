using System;
using System.Collections.Generic;

namespace Tgs.Payment.PrimePay.Repositories.Entities
{
    public partial class DepositForm
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public Guid BrandId { get; set; }
        public decimal Amount { get; set; }
        public Guid BrandPaymentId { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string ApplyInfo { get; set; }
        public string PaymentReturn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
