using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using avalonia_application.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Windows;

public partial class ProductsListWindow : Window
{
    private AvaloniaContext context = new AvaloniaContext();

    public ProductsListWindow()
    {
        InitializeComponent();
        LoadData();
    }

    public void addButton_Click(object sender, RoutedEventArgs e)
    {
        ProductViewWindow pvw = new ProductViewWindow();
        pvw.Closed += (sender, e) =>
        {
            this.Show();
            LoadData();
        };
        this.Hide();
        pvw.Show();
    }

    public void editButton_Click(object sender, RoutedEventArgs e)
    {
        StackPanel si = products_listBox.SelectedItem as StackPanel;
        int id = int.Parse(si.Name);
        ProductViewWindow pvw = new ProductViewWindow(id);
        pvw.Closed += (sender, e) =>
        {
            this.Show();
            LoadData();
        };
        this.Hide();
        pvw.Show();
    }

    public void deleteButton_Click(object sender, RoutedEventArgs e)
    {
        StackPanel si = products_listBox.SelectedItem as StackPanel;
        if (si != null)
        {
            int id = int.Parse(si.Name);
            context.Products.Where(p => p.Id == id).ExecuteDelete();
            context.SaveChanges();
            LoadData();
        }
    }

    public void createPDF_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = System.IO.Path.Combine(documentsPath, $"Products_List_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf");

            var products = context.Products.Include(p => p.CategoryNavigation).ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Список товаров")
                        .SemiBold().FontSize(20)
                        .AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            foreach (var product in products)
                            {
                                column.Item().Column(item =>
                                {
                                    item.Item().Text($"Название: {product.Title}").FontSize(14).SemiBold();
                                    item.Item().Text($"Цена: {product.Price}₽");
                                    item.Item().Text($"Категория: {product.CategoryNavigation.Title}");
                                    item.Item().Text($"Описание: {product.Description}");
                                    item.Item().Text(" ");
                                    item.Spacing(5);
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Страница ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(fileName);

            var messageBox = new Window()
            {
                Width = 200,
                Height = 100,
                Content = new TextBlock()
                {
                    Text = $"PDF файл создан:\n{fileName}",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    TextAlignment = Avalonia.Media.TextAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };
            messageBox.Show();
        }
        catch (Exception ex)
        {
            var messageBox = new Window()
            {
                Width = 300,
                Height = 150,
                Content = new TextBlock()
                {
                    Text = $"Ошибка при создании PDF:\n{ex.Message}",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    TextAlignment = Avalonia.Media.TextAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };
            messageBox.Show();
        }
    }

    private void LoadData()
    {
        products_listBox.ItemsSource = null;

        context.ChangeTracker.Clear();

        List<Product> products = context.Products.Include(p => p.CategoryNavigation).ToList();
        List<StackPanel> list = new List<StackPanel>();
        foreach (var p in products)
        {
            StackPanel sp = new StackPanel();
            sp.Name = p.Id.ToString();
            TextBlock tb = new TextBlock();
            tb.FontSize = 20;
            tb.Text = @$"Название: {p.Title}
Цена: {p.Price}₽
Категория: {p.CategoryNavigation.Title}
Описание: {p.Description}";
            sp.Children.Add(tb);
            list.Add(sp);
        }
        products_listBox.ItemsSource = list;
    }
}
