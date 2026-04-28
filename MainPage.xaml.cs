using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq; // Обязательно для суммы баланса

namespace BudgetCalculatorApp; // Проверь, чтобы это имя совпадало с твоим проектом!

public partial class MainPage : ContentPage
{
    // Список транзакций, который видит экран
    public ObservableCollection<Transaction> Transactions { get; set; } = new();
    private double balance = 0;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadTransactionsAsync();
    }

    // Логика кнопки "Доход"
    private void OnAddIncomeClicked(object sender, EventArgs e)
    {
        if (double.TryParse(AmountEntry.Text, out double amount))
        {
            Transactions.Add(new Transaction { Description = DescriptionEntry.Text, Amount = amount });
            UpdateBalance();
            ClearInputs();
        }
    }

    // Логика кнопки "Расход"
    private void OnAddExpenseClicked(object sender, EventArgs e)
    {
        if (double.TryParse(AmountEntry.Text, out double amount))
        {
            Transactions.Add(new Transaction { Description = DescriptionEntry.Text, Amount = -amount });
            UpdateBalance();
            ClearInputs();
        }
    }

    // Сохранение в JSON
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string json = JsonSerializer.Serialize(Transactions);
        string path = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");
        await File.WriteAllTextAsync(path, json);
        await DisplayAlert("Готово", "Данные сохранены", "OK");
    }

    // Загрузка из JSON при старте
    private async void LoadTransactionsAsync()
    {
        try
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path);
                var loaded = JsonSerializer.Deserialize<List<Transaction>>(json);
                if (loaded != null)
                {
                    Transactions.Clear();
                    foreach (var item in loaded) Transactions.Add(item);
                    UpdateBalance();
                }
            }
        }
        catch { }
    }

    private void UpdateBalance()
    {
        balance = Transactions.Sum(t => t.Amount);
        BalanceLabel.Text = $"Баланс: {balance}";
    }

    private void ClearInputs()
    {
        AmountEntry.Text = string.Empty;
        DescriptionEntry.Text = string.Empty;
    }
}

// Класс данных
public class Transaction
{
    public string Description { get; set; }
    public double Amount { get; set; }
}
