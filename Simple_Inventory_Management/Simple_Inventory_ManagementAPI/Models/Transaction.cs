﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Simple_Inventory_ManagementAPI.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; }

    public virtual Product Product { get; set; }
}