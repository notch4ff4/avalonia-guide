using System;
using System.Collections.Generic;

namespace avalonia_application.Entities;

public partial class ProductCategory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
