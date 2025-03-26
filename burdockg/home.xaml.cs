using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Npgsql;

namespace burdockg
{
    public partial class home : Window
    {
        private string _connectionString = "Host=localhost;Port=5432;Database=лопух;Username=postgres;Password=00000000;";
        private List<Product> _products = new List<Product>();
        private string _searchText = "";
        private string _selectedProductType = "Все типы";
        private string _sortOption = "Наименование (А-Я)";

        public home()
        {
            InitializeComponent();
            
            // Initialize sort options
            sortComboBox.Items.Add("Наименование (А-Я)");
            sortComboBox.Items.Add("Наименование (Я-А)");
            sortComboBox.Items.Add("Стоимость (возр.)");
            sortComboBox.Items.Add("Стоимость (убыв.)");
            sortComboBox.SelectedIndex = 0;
            
            // Load product types for filtering
            LoadProductTypes();
            
            // Load products
            LoadProducts();
        }

        private void LoadProducts(bool resetPage = true)
        {
            try
            {
                _products.Clear();
                productsStackPanel.Children.Clear();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Build the query with filtering and sorting
                    string query = @"
                        SELECT p.""ID"", p.""Title"", p.""ArticleNumber"", p.""MinCostForAgent"", p.""Image"", 
                               pt.""Title"" as ProductType
                        FROM ""Product"" p
                        JOIN ""ProductType"" pt ON p.""ProductTypeID"" = pt.""ID""
                        WHERE 1=1";
                    
                    // Add search filter
                    if (!string.IsNullOrWhiteSpace(_searchText))
                    {
                        query += @" AND (p.""Title"" ILIKE @searchText OR p.""ArticleNumber"" ILIKE @searchText)";
                    }
                    
                    // Add product type filter
                    if (_selectedProductType != "Все типы")
                    {
                        query += @" AND pt.""Title"" = @productType";
                    }
                    
                    // Add sorting
                    switch (_sortOption)
                    {
                        case "Наименование (А-Я)":
                            query += @" ORDER BY p.""Title"" ASC";
                            break;
                        case "Наименование (Я-А)":
                            query += @" ORDER BY p.""Title"" DESC";
                            break;
                        case "Стоимость (возр.)":
                            query += @" ORDER BY p.""MinCostForAgent"" ASC";
                            break;
                        case "Стоимость (убыв.)":
                            query += @" ORDER BY p.""MinCostForAgent"" DESC";
                            break;
                        default:
                            query += @" ORDER BY p.""Title"" ASC";
                            break;
                    }
                    
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Add parameters
                        if (!string.IsNullOrWhiteSpace(_searchText))
                        {
                            cmd.Parameters.AddWithValue("@searchText", $"%{_searchText}%");
                        }
                        
                        if (_selectedProductType != "Все типы")
                        {
                            cmd.Parameters.AddWithValue("@productType", _selectedProductType);
                        }
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Title = reader["Title"].ToString(),
                                    ArticleNumber = reader["ArticleNumber"].ToString(),
                                    MinCostForAgent = Convert.ToDecimal(reader["MinCostForAgent"]),
                                    ProductType = reader["ProductType"].ToString()
                                };

                                // Fix the image data handling
                                if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                                {
                                    try
                                    {
                                        // Try to get as byte array directly
                                        product.ImageData = (byte[])reader["Image"];
                                    }
                                    catch
                                    {
                                        // If that fails, try to convert from string path to image
                                        string imagePath = reader["Image"].ToString();
                                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                                        {
                                            product.ImageData = File.ReadAllBytes(imagePath);
                                        }
                                    }
                                }

                                _products.Add(product);
                                
                                // Create UI element for this product
                                productsStackPanel.Children.Add(CreateProductElement(product));
                            }
                        }
                    }
                    
                    // Update product count
                    productCountTextBlock.Text = $"Показано {_products.Count} из {GetTotalProductCount(conn)} продуктов";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetTotalProductCount(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Product\"", conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void LoadProductTypes()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT \"Title\" FROM \"ProductType\" ORDER BY \"Title\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            productTypeComboBox.Items.Clear();
                            productTypeComboBox.Items.Add("Все типы");
                            
                            while (reader.Read())
                            {
                                productTypeComboBox.Items.Add(reader["Title"].ToString());
                            }
                            
                            productTypeComboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private UIElement CreateProductElement(Product product)
        {
            // Create border container
            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Create main grid
            Grid grid = new Grid { Height = 120 };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });

            // Product Image
            Border imageBorder = new Border
            {
                Margin = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = System.Windows.Media.Brushes.LightGray
            };

            Image image = new Image { Stretch = System.Windows.Media.Stretch.Uniform };
            
            // Set image source
            if (product.ImageData != null && product.ImageData.Length > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(product.ImageData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                image.Source = bitmap;
            }
            else
            {
                image.Source = new BitmapImage(new Uri("/images/logo.png", UriKind.Relative));
            }

            imageBorder.Child = image;
            Grid.SetColumn(imageBorder, 0);
            grid.Children.Add(imageBorder);

            // Product Details
            StackPanel detailsPanel = new StackPanel
            {
                Margin = new Thickness(10, 10, 0, 10),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Product Type and Name
            TextBlock typeText = new TextBlock
            {
                Text = $"{product.ProductType} | {product.Title}",
                FontSize = 16,
                FontFamily = new System.Windows.Media.FontFamily("Gabriola")
            };
            detailsPanel.Children.Add(typeText);

            // Article Number
            TextBlock articleText = new TextBlock
            {
                Text = $"Артикул: {product.ArticleNumber}",
                Margin = new Thickness(0, 5, 0, 5),
                FontSize = 14,
                FontFamily = new System.Windows.Media.FontFamily("Gabriola")
            };
            detailsPanel.Children.Add(articleText);

            Grid.SetColumn(detailsPanel, 1);
            grid.Children.Add(detailsPanel);

            // Price and Actions
            StackPanel priceActionsPanel = new StackPanel
            {
                Margin = new Thickness(0, 10, 10, 10)
            };

            TextBlock priceText = new TextBlock
            {
                Text = $"{product.MinCostForAgent:C}",
                HorizontalAlignment = HorizontalAlignment.Right,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                FontFamily = new System.Windows.Media.FontFamily("Gabriola")
            };
            priceActionsPanel.Children.Add(priceText);

            // Edit and Delete buttons
            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button editButton = new Button
            {
                Content = "Изменить",
                Width = 70,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                Tag = product.Id
            };
            editButton.Click += EditButton_Click;

            Button deleteButton = new Button
            {
                Content = "Удалить",
                Width = 70,
                Height = 30,
                Tag = product.Id
            };
            deleteButton.Click += DeleteButton_Click;

            buttonsPanel.Children.Add(editButton);
            buttonsPanel.Children.Add(deleteButton);
            priceActionsPanel.Children.Add(buttonsPanel);

            Grid.SetColumn(priceActionsPanel, 2);
            grid.Children.Add(priceActionsPanel);

            border.Child = grid;
            return border;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddProduct window
            AddProduct addProductWindow = new AddProduct();
            if (addProductWindow.ShowDialog() == true)
            {
                // Reload products when returning from add window
                LoadProducts();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to menu
            menu menuWindow = new menu();
            menuWindow.Show();
            this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the clicked button's tag
            if (sender is Button button && button.Tag is int productId)
            {
                // Navigate to edit product window
                EditProduct editProductWindow = new EditProduct(productId);
                if (editProductWindow.ShowDialog() == true)
                {
                    // Reload products when returning from edit window
                    LoadProducts();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the clicked button's tag
            if (sender is Button button && button.Tag is int productId)
            {
                // Show confirmation dialog
                MessageBoxResult result = MessageBox.Show(
                    "Вы уверены, что хотите удалить этот продукт?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool deleted = DeleteProductUsingFunction(productId);
                        
                        if (deleted)
                        {
                            MessageBox.Show("Продукт успешно удален!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                            // Reload products
                            LoadProducts();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool DeleteProductUsingFunction(int productId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                
                using (var cmd = new NpgsqlCommand("SELECT delete_product(@productId)", conn))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    return (bool)cmd.ExecuteScalar();
                }
            }
        }
        
        // Move these methods inside the home class
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = searchTextBox.Text;
            LoadProducts();
        }
        
        private void ProductTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productTypeComboBox.SelectedItem != null)
            {
                _selectedProductType = productTypeComboBox.SelectedItem.ToString();
                LoadProducts();
            }
        }
        
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sortComboBox.SelectedItem != null)
            {
                _sortOption = sortComboBox.SelectedItem.ToString();
                LoadProducts();
            }
        }
    } // End of home class

    // Product class to store data
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArticleNumber { get; set; }
        public decimal MinCostForAgent { get; set; }
        public string ProductType { get; set; }
        public byte[] ImageData { get; set; }
    }
}
