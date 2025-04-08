using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using Npgsql;

namespace burdockg
{
    public partial class EditProduct : Window
    {
        // Add this field at the top of the class
        private string _connectionString = "Host=localhost;Port=5432;Database=лопух;Username=postgres;Password=00000000;";
        private byte[] selectedImageData = null;
        private string selectedImagePath = null; // Add this line to store the image path
        private int productId; // To store the ID of the product being edited
        private bool _dialogResult = false;

        public EditProduct()
        {
            InitializeComponent();
            LoadProductTypes();
            LoadMaterials(); // Added materials loading
        }

        // Constructor that accepts a product ID
        public EditProduct(int id)
        {
            InitializeComponent();
            productId = id;
            LoadProductTypes();
            LoadMaterials(); // Added materials loading
            LoadProductData(id);
        }

        private void LoadProductTypes()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT \"ID\", \"Title\" FROM \"ProductType\" ORDER BY \"Title\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            productTypeComboBox.Items.Clear();
                            
                            while (reader.Read())
                            {
                                ComboBoxItem item = new ComboBoxItem
                                {
                                    Content = reader["Title"].ToString(),
                                    Tag = Convert.ToInt32(reader["ID"])
                                };
                                productTypeComboBox.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Added method to load materials
        private void LoadMaterials()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT \"ID\", \"Title\" FROM \"Material\" ORDER BY \"Title\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            materialsComboBox.Items.Clear();
                            
                            while (reader.Read())
                            {
                                ComboBoxItem item = new ComboBoxItem
                                {
                                    Content = reader["Title"].ToString(),
                                    Tag = Convert.ToInt32(reader["ID"])
                                };
                                materialsComboBox.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке материалов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to load product data based on ID
        private void LoadProductData(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Load product details
                    using (var cmd = new NpgsqlCommand("SELECT p.\"Title\", p.\"ProductTypeID\", p.\"Image\", p.\"MinCostForAgent\" " +
                                                      "FROM \"Product\" p WHERE p.\"ID\" = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Set product name
                                productNameTextBox.Text = reader["Title"].ToString();
                                
                                // Set product type
                                int productTypeId = Convert.ToInt32(reader["ProductTypeID"]);
                                foreach (ComboBoxItem item in productTypeComboBox.Items)
                                {
                                    if ((int)item.Tag == productTypeId)
                                    {
                                        productTypeComboBox.SelectedItem = item;
                                        break;
                                    }
                                }
                                
                                // Set cost
                                costTextBox.Text = reader["MinCostForAgent"].ToString();
                                
                                // Set image if available
                                if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                                {
                                    // Fix: Properly handle the image data based on its actual type
                                    object imageData = reader["Image"];
                                    if (imageData is byte[])
                                    {
                                        // Handle byte array images (existing approach)
                                        selectedImageData = (byte[])imageData;
                                        using (MemoryStream ms = new MemoryStream(selectedImageData))
                                        {
                                            BitmapImage image = new BitmapImage();
                                            image.BeginInit();
                                            image.CacheOption = BitmapCacheOption.OnLoad;
                                            image.StreamSource = ms;
                                            image.EndInit();
                                            productImage.Source = image;
                                        }
                                    }
                                    // In the LoadProductData method, modify the image handling section:
                                    else if (imageData is string)
                                    {
                                        // Handle image path stored as string
                                        string imagePath = imageData.ToString();
                                        
                                        // Get just the filename from the path
                                        string fileName = Path.GetFileName(imagePath);
                                        
                                        // Try multiple possible locations for the image file
                                        string[] possiblePaths = new string[]
                                        {
                                            // Direct path as stored in DB
                                            imagePath,
                                            
                                            // Path relative to application directory
                                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products", fileName),
                                            
                                            // Explicit path to your project folder
                                            Path.Combine(@"E:\burdockg\burdockg\products", fileName),
                                            
                                            // Try with different casing
                                            Path.Combine(@"E:\burdockg\burdockg\Products", fileName),
                                            
                                            // Try with different extensions
                                            Path.Combine(@"E:\burdockg\burdockg\products", Path.GetFileNameWithoutExtension(fileName) + ".jpg"),
                                            Path.Combine(@"E:\burdockg\burdockg\products", Path.GetFileNameWithoutExtension(fileName) + ".jpeg"),
                                            Path.Combine(@"E:\burdockg\burdockg\products", Path.GetFileNameWithoutExtension(fileName) + ".png")
                                        };
                                        
                                        // Try each path until we find one that exists
                                        bool imageFound = false;
                                        foreach (string path in possiblePaths)
                                        {
                                            if (File.Exists(path))
                                            {
                                                try
                                                {
                                                    BitmapImage image = new BitmapImage();
                                                    image.BeginInit();
                                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                                    image.UriSource = new Uri(path);
                                                    image.EndInit();
                                                    productImage.Source = image;
                                                    
                                                    // Store the path for later use
                                                    selectedImagePath = path;
                                                    imageFound = true;
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine($"Error loading image from {path}: {ex.Message}");
                                                    // Continue trying other paths
                                                }
                                            }
                                        }
                                        
                                        if (!imageFound)
                                        {
                                            // Log that the image wasn't found
                                            Console.WriteLine($"Image not found for product {id}: {imagePath}");
                                            MessageBox.Show($"Файл изображения не найден по пути: {imagePath}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Изображение имеет неподдерживаемый формат: {imageData.GetType().Name}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                            }
                        }
                    }
                    
                    // Load product material
                    using (var cmd = new NpgsqlCommand("SELECT pm.\"MaterialID\" FROM \"ProductMaterial\" pm WHERE pm.\"ProductID\" = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int materialId = Convert.ToInt32(reader["MaterialID"]);
                                
                                // Debug message to check the material ID
                                Console.WriteLine($"Looking for material ID: {materialId}");
                                
                                bool materialFound = false;
                                foreach (ComboBoxItem item in materialsComboBox.Items)
                                {
                                    // Debug each item's tag
                                    Console.WriteLine($"Checking item with tag: {item.Tag}");
                                    
                                    if (item.Tag != null && (int)item.Tag == materialId)
                                    {
                                        materialsComboBox.SelectedItem = item;
                                        materialFound = true;
                                        break;
                                    }
                                }
                                
                                if (!materialFound)
                                {
                                    MessageBox.Show($"Материал с ID {materialId} не найден в списке материалов.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = _dialogResult;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(productNameTextBox.Text) ||
                productTypeComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(costTextBox.Text) ||
                materialsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Try to parse cost as a number
            if (!decimal.TryParse(costTextBox.Text, out decimal cost))
            {
                MessageBox.Show("Стоимость должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Begin transaction to ensure both product update and material relationship update succeed
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = @"
                                UPDATE ""Product"" 
                                SET ""Title"" = @title, 
                                    ""MinCostForAgent"" = @cost, 
                                    ""ProductTypeID"" = @productTypeId";
                            
                            // Add image update if we have a new image
                            if (selectedImageData != null)
                            {
                                query += @", ""Image"" = @image";
                            }
                            else if (!string.IsNullOrEmpty(selectedImagePath))
                            {
                                query += @", ""Image"" = @imagePath";
                            }
                            
                            query += @" WHERE ""ID"" = @productId";
                            
                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", productNameTextBox.Text);
                                cmd.Parameters.AddWithValue("@cost", cost);
                                cmd.Parameters.AddWithValue("@productId", productId);
                                
                                // Get the selected product type ID
                                ComboBoxItem selectedItem = (ComboBoxItem)productTypeComboBox.SelectedItem;
                                cmd.Parameters.AddWithValue("@productTypeId", (int)selectedItem.Tag);
                                
                                // Add image parameter if we have a new image
                                if (selectedImageData != null)
                                {
                                    cmd.Parameters.AddWithValue("@image", selectedImageData);
                                }
                                else if (!string.IsNullOrEmpty(selectedImagePath))
                                {
                                    cmd.Parameters.AddWithValue("@imagePath", selectedImagePath);
                                }
                                
                                cmd.ExecuteNonQuery();
                            }
                            
                            // Update the product material relationship
                            // First, delete existing relationships
                            using (var cmd = new NpgsqlCommand("DELETE FROM \"ProductMaterial\" WHERE \"ProductID\" = @productId", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@productId", productId);
                                cmd.ExecuteNonQuery();
                            }
                            
                            // Then, insert the new relationship
                            using (var cmd = new NpgsqlCommand(
                                "INSERT INTO \"ProductMaterial\" (\"ProductID\", \"MaterialID\", \"Count\") VALUES (@productId, @materialId, @count)", 
                                conn, transaction))
                            {
                                ComboBoxItem selectedMaterial = (ComboBoxItem)materialsComboBox.SelectedItem;
                                cmd.Parameters.AddWithValue("@productId", productId);
                                cmd.Parameters.AddWithValue("@materialId", (int)selectedMaterial.Tag);
                                cmd.Parameters.AddWithValue("@count", 1); // Default count
                                cmd.ExecuteNonQuery();
                            }
                            
                            transaction.Commit();
                            MessageBox.Show("Изменения сохранены успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                            _dialogResult = true;
                            this.DialogResult = true;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex; // Re-throw to be caught by the outer catch
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
            openFileDialog.Title = "Выберите изображение продукта";
        
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string imagePath = openFileDialog.FileName;
                    
                    // Ask user if they want to store the image as a path or as binary data
                    MessageBoxResult result = MessageBox.Show(
                        "Хотите сохранить путь к изображению вместо самого изображения?",
                        "Выбор способа хранения",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                        
                    if (result == MessageBoxResult.Yes)
                    {
                        // Check if the image is within the project directory
                        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        if (imagePath.StartsWith(appDirectory, StringComparison.OrdinalIgnoreCase))
                        {
                            // Convert to a relative path
                            string relativePath = imagePath.Substring(appDirectory.Length);
                            
                            // Ask if user wants to store as relative path
                            MessageBoxResult pathResult = MessageBox.Show(
                                "Сохранить относительный путь вместо абсолютного?",
                                "Тип пути",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);
                                
                            if (pathResult == MessageBoxResult.Yes)
                            {
                                selectedImagePath = relativePath;
                            }
                            else
                            {
                                selectedImagePath = imagePath;
                            }
                        }
                        else
                        {
                            selectedImagePath = imagePath;
                        }
                        
                        selectedImageData = null; // Clear any previous image data
                    }
                    else
                    {
                        // Store as binary data (existing approach)
                        selectedImageData = File.ReadAllBytes(imagePath);
                        selectedImagePath = null; // Clear any previous path
                    }
                    
                    // Display the image
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.EndInit();
                    productImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        // Add this method to handle the delete button click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Ask for confirmation before deleting
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот продукт?", 
                "Подтверждение удаления", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        conn.Open();
                        
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // First delete from ProductMaterial table (foreign key constraint)
                                using (var cmd = new NpgsqlCommand(
                                    "DELETE FROM \"ProductMaterial\" WHERE \"ProductID\" = @productId", 
                                    conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@productId", productId);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // Then delete the product
                                using (var cmd = new NpgsqlCommand(
                                    "DELETE FROM \"Product\" WHERE \"ID\" = @productId", 
                                    conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@productId", productId);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                transaction.Commit();
                                MessageBox.Show("Продукт успешно удален!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                                _dialogResult = true;
                                this.DialogResult = true;
                                this.Close();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
