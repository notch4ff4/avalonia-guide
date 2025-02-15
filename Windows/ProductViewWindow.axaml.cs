using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using avalonia_application.Entities;

namespace Windows;

public partial class ProductViewWindow : Window
{
    private AvaloniaContext context = new AvaloniaContext();
    public int id = 0;
    public ProductViewWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public ProductViewWindow(int _id)
    {
        InitializeComponent();
        id = _id;
        LoadData();
        Product product = context.Products.FirstOrDefault(p => p.Id == id);
        title_textBox.Text = product.Title;
        price_textBox.Text = product.Price.ToString();
        description_textBox.Text = product.Description;
        foreach (ComboBoxItem item in category_comboBox.Items)
        {
            if (item.Name == product.Category.ToString())
            {
                category_comboBox.SelectedItem = item;
                break;
            }
        }
    }

    private void LoadData()
    {
        List<ProductCategory> categories = new List<ProductCategory>();
        categories = context.ProductCategories.ToList();
        foreach (var category in categories)
        {
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Name = category.Id.ToString();
            cbi.Content = category.Title;
            category_comboBox.Items.Add(cbi);
        }
    }

    public void save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string Title = title_textBox.Text;
            float Price = float.Parse(price_textBox.Text);
            string Description = description_textBox.Text;
            int Category = int.Parse((category_comboBox.SelectedItem as ComboBoxItem).Name);
            if (id != 0)
            {
                Product product = context.Products.FirstOrDefault(p => p.Id == id);
                product.Title = Title;
                product.Price = Price;
                product.Description = Description;
                product.Category = Category;
            }
            else {
                Product product = new Product() {
                    Title = Title,
                    Price = Price,
                    Description = Description,
                    Category = Category
                };
                context.Products.Add(product);
            }
            context.SaveChanges();
            this.Close();
        } catch (Exception error) {
            Trace.TraceError(error.ToString());
        }

    }
}
