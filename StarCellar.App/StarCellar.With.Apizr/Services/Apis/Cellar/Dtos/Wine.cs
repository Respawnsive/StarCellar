namespace StarCellar.With.Apizr.Services.Apis.Cellar.Dtos
{
    public partial class Wine : ObservableObject
    {
        [ObservableProperty] public Guid _id;
        [ObservableProperty] public string _name;
        [ObservableProperty] public string _description;
        [ObservableProperty] public string _imageUrl;
        [ObservableProperty] public int _stock;
        [ObservableProperty] public int _score;
        [ObservableProperty] public Guid _ownerId;
    }
}
