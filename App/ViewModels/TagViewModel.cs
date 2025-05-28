using CommunityToolkit.Mvvm.ComponentModel;

namespace App.ViewModels;

public partial class TagViewModel : ObservableObject
{
    public int Id { get; set; }
    public string Name { get; set; }

    [ObservableProperty]
    bool isSelected;

    public TagViewModel(string name)
    {
        Name = name;
    }
}
