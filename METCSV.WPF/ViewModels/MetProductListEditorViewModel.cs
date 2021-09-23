using Prism.Mvvm;

namespace METCSV.WPF.ViewModels
{
    public class MetProductListEditorViewModel : ProductBrowserBaseViewModel
    {
        public MetProductListEditorViewModel() : base()
        {
            IsGridReadOnly = false;
        }
    }
}
