using Microsoft.Win32;
using Npgsql;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace burdockg
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        private string _connectionString = "Host=localhost;Port=5432;Database=лопух;Username=postgres;Password=00000000;";
        private string _selectedImagePath = null;
        
        public AddProduct()
        {
            InitializeComponent();
            LoadProductTypes();
            LoadMaterials();
        }

        private void LoadProductTypes()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM \"ProductType\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            productTypeComboBox.Items.Clear();
                            while (reader.Read())
                            {
                                string typeName = reader["Title"].ToString();
                                productTypeComboBox.Items.Add(typeName);
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

        private void LoadMaterials()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM \"Material\"", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            materialsComboBox.Items.Clear();
                            while (reader.Read())
                            {
                                string materialName = reader["Title"].ToString();
                                materialsComboBox.Items.Add(materialName);
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

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Выберите изображение продукта"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                productImage.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(productNameTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, введите наименование продукта", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if product name is too long
                if (productNameTextBox.Text.Length > 100)
                {
                    MessageBox.Show("Наименование продукта не должно превышать 100 символов", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (productTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите тип продукта", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (materialsComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите материал", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(costTextBox.Text, out decimal cost))
                {
                    MessageBox.Show("Пожалуйста, введите корректную стоимость", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] imageData = null;
                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    imageData = File.ReadAllBytes(_selectedImagePath);
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Use a transaction to ensure data consistency
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get ProductTypeID
                            int productTypeId;
                            using (var cmd = new NpgsqlCommand("SELECT \"ID\" FROM \"ProductType\" WHERE \"Title\" = @title", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", productTypeComboBox.SelectedItem.ToString());
                                productTypeId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Get MaterialID
                            int materialId;
                            using (var cmd = new NpgsqlCommand("SELECT \"ID\" FROM \"Material\" WHERE \"Title\" = @title", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", materialsComboBox.SelectedItem.ToString());
                                materialId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Check if article number already exists
                            string articleNumber;
                            bool isUnique = false;
                            int attempts = 0;
                            
                            do {
                                articleNumber = GenerateArticleNumber();
                                
                                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Product\" WHERE \"ArticleNumber\" = @articleNumber", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@articleNumber", articleNumber);
                                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                                    isUnique = (count == 0);
                                }
                                
                                attempts++;
                            } while (!isUnique && attempts < 10);
                            
                            if (!isUnique)
                            {
                                throw new Exception("Не удалось создать уникальный артикул для продукта. Пожалуйста, попробуйте еще раз.");
                            }

                            // Generate a unique ID for the new product
                            int newProductId;
                            using (var cmd = new NpgsqlCommand("SELECT COALESCE(MAX(\"ID\"), 0) + 1 FROM \"Product\"", conn, transaction))
                            {
                                newProductId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Insert new product with the generated ID
                            using (var cmd = new NpgsqlCommand(
                                "INSERT INTO \"Product\" (\"ID\", \"Title\", \"ProductTypeID\", \"ArticleNumber\", \"Description\", \"Image\", \"ProductionPersonCount\", \"ProductionWorkshopNumber\", \"MinCostForAgent\") " +
                                "VALUES (@id, @title, @productTypeId, @articleNumber, @description, @image, @personCount, @workshopNumber, @minCost)", conn, transaction))
                            {
                                // Ensure title doesn't exceed database column length
                                string title = productNameTextBox.Text;
                                if (title.Length > 100)
                                {
                                    title = title.Substring(0, 100);
                                }
                                
                                cmd.Parameters.AddWithValue("@id", newProductId);
                                cmd.Parameters.AddWithValue("@title", title);
                                cmd.Parameters.AddWithValue("@productTypeId", productTypeId);
                                cmd.Parameters.AddWithValue("@articleNumber", articleNumber);
                                cmd.Parameters.AddWithValue("@description", DBNull.Value);
                                
                                // In the SaveButton_Click method, modify the image handling section:
                                
                                // When handling the image path
                                if (_selectedImagePath != null)
                                {
                                    // Ensure the path doesn't exceed database column length
                                    if (_selectedImagePath.Length > 100)
                                    {
                                        // Get just the filename instead of the full path
                                        string fileName = Path.GetFileName(_selectedImagePath);
                                        
                                        // If the filename is still too long, truncate it
                                        if (fileName.Length > 100)
                                        {
                                            fileName = fileName.Substring(0, 97) + "...";
                                        }
                                        
                                        // Store just the filename or a relative path
                                        cmd.Parameters.AddWithValue("@image", "\\products\\" + fileName);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@image", _selectedImagePath);
                                    }
                                }
                                else if (imageData != null)
                                {
                                    // If we have binary image data
                                    cmd.Parameters.AddWithValue("@image", imageData);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@image", DBNull.Value);
                                }
                                
                                cmd.Parameters.AddWithValue("@personCount", 1); 
                                cmd.Parameters.AddWithValue("@workshopNumber", 1);
                                cmd.Parameters.AddWithValue("@minCost", cost);

                                cmd.ExecuteNonQuery();
                            }

                            // Insert product material relationship
                            using (var materialCmd = new NpgsqlCommand(
                                "INSERT INTO \"ProductMaterial\" (\"ProductID\", \"MaterialID\", \"Count\") VALUES (@productId, @materialId, @count)", conn, transaction))
                            {
                                materialCmd.Parameters.AddWithValue("@productId", newProductId);
                                materialCmd.Parameters.AddWithValue("@materialId", materialId);
                                materialCmd.Parameters.AddWithValue("@count", 1);
                                materialCmd.ExecuteNonQuery();
                            }
                            
                            // Commit the transaction
                            transaction.Commit();
                            
                            MessageBox.Show("Продукт успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                            Close();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if any error occurs
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при сохранении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateArticleNumber()
        {
            // Generate a unique article number with timestamp and GUID to ensure uniqueness
            Random random = new Random();
            string timestamp = DateTime.Now.ToString("MMddHHmmss");
            string guid = Guid.NewGuid().ToString().Substring(0, 4);
            return $"A{timestamp}{guid}".Substring(0, 10); // Ensure it's not longer than 10 chars
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
