using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PetStoreProject.Models;

[Table("DiscountUse")]
public partial class DiscountUse
{
    [Key]
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? DiscountId { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("DiscountUses")]
    public virtual Discount? Discount { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("DiscountUses")]
    public virtual Order? Order { get; set; }
}
